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
    [SerializeField] protected SkinnedMeshRenderer m_characterMeshRdr = null;              // 플레이어 렌더


    [Header("스노우볼 Obj")]
    [SerializeField] protected GameObject m_snowBallObj = null;                     // 스노우볼 오브젝트
    [SerializeField] public SnowBallManager m_snowBallMgr = null;                   // 스노우볼 매니저
    [SerializeField] protected MeshRenderer m_snowBallMeshRdr = null;               // 스노우볼 렌더러
    protected Transform m_playerParentTf = null;                                    // 플레이어 부모 오브젝트Tf


    [Header("플레이어 이펙트")]
    [SerializeField] protected GameObject m_splashEffect = null;                    //  플레이어 달릴때 이펙트
    [SerializeField] protected VisualEffect m_playerRunSplashEffect = null;         // 플레이어 달릴때 이펙트(조절용)
    [SerializeField] private ParticleSystem m_camSplashEffect = null;               // 플레이어 달릴때 카메라에 나올 이펙트


    [SerializeField] protected ParticleSystem m_crashHitEffect = null;              // 일반(장애물)에 부딪힐때 나오는 이펙트
    [SerializeField] protected ParticleSystem m_crashRockEffect = null;             // 바위(장애물)에 부딪힐때 나오는 이펙 


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

        this.transform.position = m_playerStartTf;
        this.transform.rotation = Quaternion.Euler(0, 0, 0);

        //m_animCtrl.SetTrigger("Idle");

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
        m_splashEffect.SetActive(true);

        m_playerState = ePlayerState.Run;
    }
    /// <summary>
    /// (GameState = Play)플레이어 Clear 애니메이션 실행함수
    /// </summary>
    public virtual void SetAnim_Clear()
    {
        InGameManager.uniqueInstance.m_curGameState = InGameManager.eGameState.Clear;

        // === ClearPos로 이동 === //
        playerClearSeq = DOTween.Sequence()
                                .Append(this.transform.DOMove(m_clearTf.position, 1.5f))
                                .Join(this.transform.DORotate(m_clearTf.eulerAngles, 1.5f))
                                .OnComplete(() =>
                                {
                                    m_playerState = ePlayerState.Happy;

                                    m_animCtrl.SetTrigger("Clear");
                                    m_splashEffect.SetActive(false);

                                    // === 눈덩이 Player Obj에서 떼어낸 후 => 굴러가게 === //
                                    m_snowBallObj.transform.SetParent(m_playerParentTf.transform);
                                    m_snowBallObj.transform.DORotate(Vector3.zero, 1f);
                                    // === 눈덩이 Player Obj에서 떼어낸 후 => 굴러가게 === //
                                });
        // === ClearPos로 이동 === //
    }


    /// <summary>
    /// (GameState = End)플레이어 GameOver 애니메이션 실행함수
    /// </summary>
    public void SetAnim_GameOver()
    {
        //InGameManager.uniqueInstance.m_curGameState = InGameManager.eGameState.End;
        m_animCtrl.ResetTrigger("Push");
        m_animCtrl.SetTrigger("GameOver");                                // 플레이어 Die 애니메이션 실행


        m_snowBallMeshRdr.enabled = false;                                  // 눈덩이 메쉬렌더러 끄기
        //m_characterMeshRdr.enabled = false;                               // 캐릭터 렌더러 끄기
        m_splashEffect.SetActive(false);                                   // 플레이어 달리는 이펙트 끄기
        m_camSplashEffect.Stop();                                         // 카메라에 붙어있는 이펙트 끄기



        StopPlayerMoving();                                               // 플레이어 움직임 Stop

        //StartCoroutine(testt());
        playerGameOverSeq = DOTween.Sequence()
                                   .AppendInterval(2.5f)
                                   //.Append(Camera.main.transform.DOLocalMove(new Vector3(0, 20f, -36f), 1f).SetEase(Ease.Linear))
                                   //.AppendInterval(1f)
                                   .AppendCallback(() =>
                                   {
                                       InGameManager.m_camMgr.SetCamParent();           // 카메라 오브젝트 초기부모 오브젝트로
                                   })
                                   .AppendInterval(1.5f)
                                   .AppendCallback(() =>
                                   {
                                       this.transform.DOKill();

                                       SetPlayerToStartPos();                                                // 플레이어 초기 시작 위치로
                                       SetAnim_Idle();

                                       m_snowBallMgr.SetLocalScale();                                   // 스노우볼 크기 초기화
                                       m_snowBallMeshRdr.enabled = true;                            // 스노우볼 렌더링 켜기
                                       m_characterMeshRdr.enabled = true;                           // 캐릭터 렌더러 끄기
                                   })
                                   .AppendInterval(1f)
                                   .OnComplete(() =>
                                   {
                                       InGameManager.m_camMgr.CamToPlayerProduction();          // 카메라 연출
                                   });
    }


    /// <summary>
    /// 눈덩이에 파뭍혔을 때 실행함수
    /// </summary>
    public virtual void SetWhenInTheSnowBall()
    {
        m_playerState = ePlayerState.Death;


        m_snowBallMeshRdr.enabled = false;                                 // 스노우볼 렌더링 끄기
        m_characterMeshRdr.enabled = false;                                // 캐릭터 렌더러 끄기
        

        m_splashEffect.SetActive(false);                                  // 플레이어 달리는 이펙트 끄기


        StopPlayerMoving();                                               // 플레이어 움직임 Stop


        playerGameOverSeq = DOTween.Sequence()
                                   .AppendInterval(1.5f)
                                   //.Append(Camera.main.transform.DOLocalMove(new Vector3(0, 20f, -36f), 1f).SetEase(Ease.Linear))
                                   //.AppendInterval(1f)
                                   .AppendCallback(() =>
                                   {
                                       InGameManager.m_camMgr.SetCamParent();           // 카메라 오브젝트 초기부모 오브젝트로
                                   })
                                   .AppendInterval(1f)
                                   .AppendCallback(() =>
                                   {
                                       SetPlayerToStartPos();                                  // 플레이어 초기 시작 위치로

                                       m_snowBallMgr.SetLocalScale();                     // 스노우볼 크기 초기화
                                       m_snowBallMeshRdr.enabled = true;              // 스노우볼 렌더링 켜기
                                       m_characterMeshRdr.enabled = true;             // 스노우볼 렌더링 켜기
                                   })
                                   .AppendInterval(1f)
                                   .OnComplete(() =>
                                   {
                                       InGameManager.m_camMgr.CamToPlayerProduction();          // 카메라 연출
                                   });
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

            StartCoroutine(PlayerMoving());
        }
    }
    /// <summary>
    /// (GameState = End) 플레이어 게임 Clear/GameOver시 호출되는 함수
    /// </summary>
    public virtual void StopPlayerMoving()
    {
        m_snowBallMgr.ResetTweener();
        m_snowBallMgr.SetSphereCollider(false);

        m_isMoving = false;
        m_playerRigid.isKinematic = true;
        SetAnimSpeedOrigin();                       // 플레이어 스피드 1, 기본스피드로 초기화
        StopCoroutine(PlayerMoving());              // 플레이어 움직임 중단
    }
    // 0 0 -763

    /// <summary>
    /// (GameState = Play)눈덩이 커질때마다 호출되는 함수 (증가)
    /// </summary>
    public virtual void SetAnimSpeedUp()
    {
        m_animCtrl.speed += 1.1f;
        m_playerMovSpd += 1.1f;
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


    #region 플레이어 충돌 이펙트
    public virtual void PlayCrashEffect(int a_num)
    {
        if (a_num == 0)
            m_crashHitEffect.Play();
        else if (a_num == 1)
            m_crashRockEffect.Play();
    }
    #endregion


    protected virtual void OnTriggerEnter(Collider other)
    {
        if(m_playerState == ePlayerState.Run)
        {
            if (other.gameObject.layer == LayerMask.NameToLayer("FastZone"))
            {
                playerFastSeq = DOTween.Sequence()
                                       .AppendCallback(() =>
                                       {
                                           InGameManager.m_fastZoneTrail.m_isTrailOn = true;

                                           if(m_camSplashEffect != null)
                                               m_camSplashEffect.Play();

                                           m_playerRunSplashEffect.SetFloat("Speed", 70);
                                           m_playerMovSpd += 1;
                                       })
                                       .AppendInterval(2f)
                                       .OnComplete(() =>
                                       {
                                           InGameManager.m_fastZoneTrail.m_isTrailOn = false;

                                           if(m_camSplashEffect != null)
                                               m_camSplashEffect.Stop();

                                           m_playerRunSplashEffect.SetFloat("Speed", 18);
                                           m_playerMovSpd -= 1;
                                       });
            }
            else if(other.gameObject.layer == LayerMask.NameToLayer("GoalLine"))
            {
                //InGameManager.m_snowGround.StopGroundMoving();
                StopPlayerMoving();
                SetAnim_Clear();
            }
        }
    }



}
