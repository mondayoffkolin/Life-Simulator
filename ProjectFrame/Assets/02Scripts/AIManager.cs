using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.VFX;

public enum eLevel
{
    Low,
    Middle,
    High,

    None
}

public class AIManager : PlayerManager
{
    [Header("MeshRender 관련")]
    [SerializeField] float m_appearTime = 0f;
    [SerializeField] ParticleSystem m_appearEffect = null;
    [SerializeField] SkinnedMeshRenderer[] m_aiSkinMeshRdr = null;
    [SerializeField] MeshRenderer m_meshRdr = null;


    [Header("Ai Level")]
    [SerializeField] private eLevel m_level = eLevel.None;


    [Header("Ai Path")]
    [SerializeField] private List<Vector3> m_aiPathTf;



    protected override void Awake()
    {
        base.Awake();

        m_aiPathTf = new List<Vector3>();
    }


    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
    }


    #region AI 등장 연출
    public void AppearProduction()
    {
        StartCoroutine("AppearPd");
    }

    private IEnumerator AppearPd()
    {
        yield return new WaitForSeconds(m_appearTime);

        m_appearEffect.Play();

        for (int n = 0; n < m_aiSkinMeshRdr.Length; n++)
            m_aiSkinMeshRdr[n].enabled = true;

        m_meshRdr.enabled = true;
    }
    #endregion


    #region (GameState = Ready) AI 시작위치 찾기
    /// <summary>
    /// (GameState = Ready)Ai 시작 위치 Transform 찾는 함수
    /// </summary>
    public override void SetPlayerStartPos()
    {
        m_playerStartTf = this.transform.position;
    }

    /// <summary>
    /// (GameState = Play/End)Ai 시작 위치로 초기화 함수
    /// </summary>
    protected override void SetPlayerToStartPos()
    {
        //base.SetPlayerToStartPos();


        this.transform.position = InGameManager.m_plyMgr.transform.position - new Vector3(0, 0, 100);
        this.transform.rotation = Quaternion.Euler(0, 0, 0);

        //m_animCtrl.SetTrigger("Idle");

        m_playerState = ePlayerState.Idle;
    }
    #endregion


    #region AI 애니메이션 실행
    public override void SetAnim_Idle()
    {
        base.SetAnim_Idle();
    }
    /// <summary>
    /// (GameState = Play)Ai Push 애니메이션 실행함수
    /// </summary>
    public override void SetAnim_Push()
    {
        base.SetAnim_Push();
    }

    /// <summary>
    /// (GameState = Play)Ai Clear 애니메이션 실행함수
    /// </summary>
    public override void SetAnim_Clear()
    {
        m_playerState = ePlayerState.Happy;

        m_animCtrl.SetTrigger("Clear");

        InGameManager.uniqueInstance.GameOver();

        this.gameObject.SetActive(false);
    }


    /// <summary>
    /// (GameState = End)Ai GameOver 애니메이션 실행함수
    /// </summary>
    public void SetAiAnim_GameOver()
    {
        m_animCtrl.ResetTrigger("Push");
        m_animCtrl.SetTrigger("GameOver");                          // 플레이어 Die 애니메이션 실행


        m_snowBallMeshRdr.enabled = false;                          // 눈덩이 메쉬렌더러 끄기


        StopPlayerMoving();                                         // 플레이어 움직임 Stop


        m_snowBallMgr.SetSnowBallTrailEffect(false);                // 눈덩이 이펙트 끄기
    }
    #endregion


    #region 플레이어 이동관련
    private int m_pathCount = 0;
    public void SetAIPath()
    {
        m_aiPathTf.Clear();
        //m_pathCount = 0;

        // === Ai Path 설정 === //
        Vector3[] a_aiPathTf;

        a_aiPathTf = InGameManager.m_aiPathMgr.SetPathRoot(m_level);

        for (int n = 0; n < a_aiPathTf.Length; n++)
            m_aiPathTf.Add(a_aiPathTf[n]);
        // === Ai Path 설정 === //
    }

    /// <summary>
    /// (GameState = Play) Ai 경로따라 움직이는 코루틴함수               고치는중~!~
    /// </summary>
    public override IEnumerator PlayerMoving()
    {
        if (m_aiPathTf.Count <= m_pathCount)
        {
            m_isMoving = false;
            StopCoroutine(PlayerMoving());
        }

        if (m_isMoving == true)
        {
            // === 지경 Path로 이동 === //
            float a_dis = (this.transform.position - m_aiPathTf[m_pathCount]).sqrMagnitude;
            if (a_dis < 50f)
                m_pathCount += 1;
            else
            {
                this.transform.Translate(Vector3.forward * m_playerMovSpd, Space.Self);
                this.transform.DOLookAt(m_aiPathTf[m_pathCount], 1.5f);
            }
            // === 지경 Path로 이동 === //

            yield return new WaitForSeconds(0.01f);

            StartCoroutine(PlayerMoving());
        }
    }

    /// <summary>
    /// (GameState = End) Ai 게임 Clear/GameOver시 호출되는 함수
    /// </summary>
    public override void StopPlayerMoving()
    {
        m_snowBallMeshRdr.enabled = false;                 // 스노우볼 렌더링 끄기

        m_snowBallMgr.ResetTweener();
        m_snowBallMgr.SetSphereCollider(false);

        m_isMoving = false;
        m_playerRigid.isKinematic = true;
        StopCoroutine(PlayerMoving());                   // 플레이어 움직임 중단
    }

    /// <summary>
    /// (GameState = Play)눈덩이 커질때마다 호출되는 함수 (증가)
    /// </summary>
    public override void SetAnimSpeedUp()
    {
        base.SetAnimSpeedUp();
    }
    public override void SetAnimSpeedDown()
    {
        base.SetAnimSpeedDown();
    }
    /// <summary>
    /// (GameState = Clear/End)골인/플레이어죽음 때 호출되는 함수
    /// </summary>
    public override void SetAnimSpeedOrigin()
    {
        base.SetAnimSpeedOrigin();
    }
    #endregion


    #region 플레이어 충돌 이펙트
    public override void PlayCrashEffect(bool a_isPlay)
    {
        base.PlayCrashEffect(a_isPlay);
    }
    #endregion


    protected override void OnTriggerEnter(Collider other)
    {
        if (m_playerState == ePlayerState.Run)
        {
            if (other.gameObject.layer == LayerMask.NameToLayer("SnowGround"))
            {
                m_playerMovSpd -= .5f;

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
                                           m_playerMovSpd += 1;
                                       })
                                       .AppendInterval(2f)
                                       .OnComplete(() =>
                                       {
                                           m_playerMovSpd -= 1;
                                       });
            }
            else if (other.gameObject.layer == LayerMask.NameToLayer("GoalLine"))
            {
                StopPlayerMoving();
                SetAnim_Clear();
            }
        }
    }


    protected override void OnTriggerExit(Collider other)
    {
        if (m_playerState == ePlayerState.Run)
        {
            if (other.gameObject.layer == LayerMask.NameToLayer("SnowGround"))
            {
                print("눈바닥 위 아님");
                m_playerMovSpd += .5f;

                m_snowBallMgr.ResetTweener();
                m_snowBallMgr.m_isSBEffectIncrease = false;                              // 눈 튀기는 이펙트 크기 감소
                m_snowBallMgr.m_fastZoneTrailMgr.m_isSnowTrailDequeue = false;           // false 눈바닥 흔적 Dequeue

                m_snowBallMgr.SetSnowBallSize(false);
            }
        }
    }
}