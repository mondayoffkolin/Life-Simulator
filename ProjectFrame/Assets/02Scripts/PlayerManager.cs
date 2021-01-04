using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.VFX;

public enum ePlayerState
{
    Idle,
    Run,
    Happy,
    Death,

    None
}

public class PlayerManager : MonoBehaviour
{
    [Header("플레이어 상태")]
    public ePlayerState m_playerState = ePlayerState.None;


    [Header("플레이어 스탯")]
    public float m_playerMovSpd = 1f;                                               // 플레이어 움직임 속도
    [SerializeField] protected float m_originPlayerMovSpd = 1f;                     // 플레이어 움직임 속도
    [SerializeField] protected SkinnedMeshRenderer[] m_characterMeshRdr = null;       // 플레이어 렌더


    [Header("스노우볼 Obj")]
    [SerializeField] protected GameObject m_snowBallObj = null;                     // 스노우볼 오브젝트
    [SerializeField] public SnowBallManager m_snowBallMgr = null;                   // 스노우볼 매니저
    [SerializeField] protected MeshRenderer m_snowBallMeshRdr = null;               // 스노우볼 렌더러
    protected Transform m_playerParentTf = null;                                    // 플레이어 부모 오브젝트Tf


    [Header("플레이어 이펙트")]
    [SerializeField] private ParticleSystem m_playerRunSplashEffect = null;         // 플레이어 달릴때 이펙트
    [SerializeField] private ParticleSystem m_snowBallRunSplashEffect = null;       // 플레이어 달릴때 이펙트(조절용)
    [SerializeField] private ParticleSystem m_camSplashEffect = null;               // 플레이어 달릴때 카메라에 나올 이펙트

    [SerializeField] protected ParticleSystem m_crashHitEffect = null;              // 일반(장애물)에 부딪힐때 나오는 이펙트
    [SerializeField] protected ParticleSystem m_crashRockEffect = null;             // 바위(장애물)에 부딪힐때 나오는 이펙트


    [Header("클리어시 Tf")]
    [SerializeField] protected Transform m_clearTf = null;
    [SerializeField] protected Vector3 m_playerStartTf = Vector3.zero;


    protected PlayerManager m_plyMgr = null;
    protected Animator m_animCtrl = null;
    protected Rigidbody m_playerRigid = null;


    protected Sequence playerFastSeq;             // (FastZone) 밟았을 때 DoTween
    protected Sequence playerClearSeq;
    protected Sequence playerGameOverSeq;


    protected virtual void Awake()
    {
        m_plyMgr = this.GetComponent<PlayerManager>();
        m_animCtrl = this.GetComponent<Animator>();
        m_playerRigid = this.GetComponent<Rigidbody>();


        m_playerParentTf = this.transform.parent;
    }


    // Start is called before the first frame update
    protected virtual void Start()
    {
        m_originPlayerMovSpd = m_playerMovSpd;

        //SetAnim_Idle();
    }


    #region (GameState = Ready) 플레이어 시작위치 찾기
    /// <summary>
    /// (GameState = Ready)플레이어 시작 위치 Transform 찾는 함수
    /// </summary>
    public virtual void SetPlayerStartPos()
    {
        m_playerStartTf = this.transform.position;
    }


    /// <summary>
    /// (GameState = Play/End)플레이어 시작 위치로 초기화 함수
    /// </summary>
    protected virtual void SetPlayerToStartPos()
    {
        SetAnimSpeedOrigin();

        this.transform.position = this.transform.position - new Vector3(0, 0, 100);
        this.transform.rotation = Quaternion.Euler(0, 0, 0);

        m_playerState = ePlayerState.Idle;
    }
    #endregion


    #region 플레이어 애니메이션 실행
    public virtual void SetAnim_Idle()
    {
        m_animCtrl.ResetTrigger("GameOver");
        m_animCtrl.ResetTrigger("Push");
        m_animCtrl.SetTrigger("Idle");

        m_playerState = ePlayerState.Idle;
    }
    /// <summary>
    /// (GameState = Play)플레이어 Push 애니메이션 실행함수
    /// </summary>
    public virtual void SetAnim_Push()
    {
        m_animCtrl.ResetTrigger("Idle");
        m_animCtrl.SetTrigger("Push");

        m_playerState = ePlayerState.Run;
    }
    /// <summary>
    /// (GameState = Play)플레이어 Clear 애니메이션 실행함수
    /// </summary>
    public virtual void SetAnim_Clear()
    {
        m_animCtrl.SetTrigger("Clear");

        StopPlayerMoving();
        PlayBoosterEffect(false);
    }


    /// <summary>
    /// (GameState = End)플레이어 GameOver 애니메이션 실행함수
    /// </summary>
    public void SetAnim_GameOver()
    {
        //InGameManager.uniqueInstance.m_curGameState = InGameManager.eGameState.End;
        m_animCtrl.ResetTrigger("Push");
        m_animCtrl.SetTrigger("GameOver");                        // 플레이어 Die 애니메이션 실행


        m_snowBallMeshRdr.enabled = false;                        // 눈덩이 메쉬렌더러 끄기


        StopPlayerMoving();                                       // 플레이어 움직임 Stop

        if(m_camSplashEffect != null)
            m_camSplashEffect.Stop();                             // 카메라에 붙어있는 이펙트 끄기

        m_snowBallMgr.SetSnowBallTrailEffect(false);              // 눈덩이 이펙트 끄기
        m_snowBallMgr.SetCamEffect(false);                        // 카메라 이펙트 끄기

    }
    #endregion


    #region 플레이어 이동관련
    /// <summary>
    /// (GameState = Play) 플레이어 앞으로 움직이는 코루틴함수
    /// </summary>
    public bool m_isMoving = false;
    public virtual IEnumerator PlayerMoving()
    {
        if(m_isMoving == true)
        {
            this.transform.Translate(Vector3.forward * m_playerMovSpd, Space.Self);

            yield return null;

            StartCoroutine("PlayerMoving");
        }
    }
    /// <summary>
    /// (GameState = End) 플레이어 게임 Clear/GameOver시 호출되는 함수
    /// </summary>
    public virtual void StopPlayerMoving()
    {
        m_snowBallMeshRdr.enabled = false;                                 // 스노우볼 렌더링 끄기

        m_snowBallMgr.ResetTweener();
        m_snowBallMgr.SetSphereCollider(false);

        //m_isMoving = false;
        m_playerRigid.isKinematic = true;
        SetAnimSpeedOrigin();                       // 플레이어 스피드 1, 기본스피드로 초기화
        StopCoroutine("PlayerMoving");              // 플레이어 움직임 중단
    }


    /// <summary>
    /// (GameState = Play)눈덩이 커질때마다 호출되는 함수 (증가)
    /// </summary>
    public virtual void SetAnimSpeedUp()
    {
        m_animCtrl.speed += 0.2f;
        m_playerMovSpd += 0.2f;
    }
    public virtual void SetAnimSpeedDown()
    {
        m_animCtrl.speed -= 0.2f;
        m_playerMovSpd -= 0.2f;
    }
    /// <summary>
    /// (GameState = Clear/End)골인/플레이어죽음 때 호출되는 함수
    /// </summary>
    public virtual void SetAnimSpeedOrigin()
    {
        m_animCtrl.speed = 1;
        m_playerMovSpd = m_originPlayerMovSpd;
    }
    #endregion


    #region 플레이어 이펙트
    public virtual void PlayCrashEffect(bool a_isPlay)
    {
        if (a_isPlay == true)
            m_crashHitEffect.Play();
        else
            m_crashRockEffect.Play();
    }

    public virtual void PlayBoosterEffect(bool a_isPlay)
    {
        if(a_isPlay == true)
        {
            if (m_snowBallRunSplashEffect != null)
                m_snowBallRunSplashEffect.Play();

            if (m_playerRunSplashEffect != null)
                m_playerRunSplashEffect.Play();

            if(m_camSplashEffect != null)
                m_camSplashEffect.Play();
        }
        else
        {
            if (m_snowBallRunSplashEffect != null)
                m_snowBallRunSplashEffect.Stop();

            if (m_playerRunSplashEffect != null)
                m_playerRunSplashEffect.Stop();

            if(m_camSplashEffect != null)
                m_camSplashEffect.Stop();                                 // 카메라에 붙어있는 이펙트 끄기
        }
    }
    #endregion


    private Transform m_goalPoint = null;
    protected virtual void OnTriggerEnter(Collider other)
    {
        if (m_playerState == ePlayerState.Run)
        {
            if (other.gameObject.layer == LayerMask.NameToLayer("SnowGround"))
            {
                print("눈바닥");
                m_playerMovSpd -= .5f;

                m_snowBallMgr.SetCamEffect(false);                                      // 카메라 Splash 이펙트 끄기
                m_snowBallMgr.ResetTweener();
                m_snowBallMgr.m_isSBEffectIncrease = true;                              // 눈 튀기는 이펙트 크기 증가
                m_snowBallMgr.m_fastZoneTrailMgr.m_isSnowTrailDequeue = true;           // true 흙바닥 흔적 Dequeue

                m_snowBallMgr.SetSnowBallSize(true);
            }
            else if (other.gameObject.layer == LayerMask.NameToLayer("FastZone"))
            {
                playerFastSeq = DOTween.Sequence()
                                       .AppendCallback(() =>
                                       {
                                           if(m_camSplashEffect != null)
                                               m_camSplashEffect.Play();

                                           if (m_playerRunSplashEffect != null)
                                               m_playerRunSplashEffect.Play();

                                           if (m_snowBallRunSplashEffect != null)
                                               m_snowBallRunSplashEffect.Play();

                                           m_playerMovSpd += 1;

                                           InGameManager.m_camMgr.SetMotionBlur(true);
                                       })
                                       .AppendInterval(4f)
                                       .OnComplete(() =>
                                       {
                                           if(m_camSplashEffect != null)
                                               m_camSplashEffect.Stop();

                                           if(m_playerRunSplashEffect != null)
                                               m_playerRunSplashEffect.Stop();

                                           if (m_snowBallRunSplashEffect != null)
                                               m_snowBallRunSplashEffect.Stop();

                                           m_playerMovSpd -= 1;

                                           InGameManager.m_camMgr.SetMotionBlur(false);
                                       });
            }
            else if(other.gameObject.layer == LayerMask.NameToLayer("GoalLine"))
            {
                InGameManager.uniqueInstance.m_curGameState = InGameManager.eGameState.Clear;

                // === AI들 멈춤 === //
                InGameManager.uniqueInstance.GameClear();      
                // === AI들 멈춤 === //


                // === 골인 지점으로 === //
                m_goalPoint = GameObject.FindGameObjectWithTag("PlayerGoalPoint").transform;
                this.transform.DOMove(m_goalPoint.position, .1f);
                this.transform.DORotate(m_goalPoint.eulerAngles, .1f);
                // === 골인 지점으로 === //


                InGameManager.m_camMgr.SetMotionBlur(true);
                m_snowBallMgr.m_fastZoneTrailMgr.SetDequeueTrail(false);
                m_playerMovSpd = 5f;
                PlayBoosterEffect(true);


                m_snowBallMgr.SetSnowBallSize(false);
            }
        }
    }


    protected virtual void OnTriggerExit(Collider other)
    {
        if (m_playerState == ePlayerState.Run)
        {
            if (other.gameObject.layer == LayerMask.NameToLayer("SnowGround"))
            {
                print("눈바닥 위 아님");
                m_playerMovSpd += .5f;

                m_snowBallMgr.SetCamEffect(true);                                       // 카메라 Splash 이펙트 켜기
                m_snowBallMgr.ResetTweener();
                m_snowBallMgr.m_isSBEffectIncrease = false;                              // 눈 튀기는 이펙트 크기 감소
                m_snowBallMgr.m_fastZoneTrailMgr.m_isSnowTrailDequeue = false;           // false 눈바닥 흔적 Dequeue

                m_snowBallMgr.SetSnowBallSize(false);
            }
        }
    }
}
