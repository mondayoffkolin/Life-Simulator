using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class InGameManager : MonoBehaviour
{
    public static InGameManager uniqueInstance;


    public enum eGameState
    {
        Ready,
        Start,
        Play,
        Clear,
        End,

        None
    }

    [Header("현재 게임 상태")]
    public eGameState m_curGameState;
    [SerializeField] bool m_isPlay = true;


    [Header("객체 인스턴스")]
    public static PlayerManager m_plyMgr = null;
    public static SnowBallManager m_snowBallMgr = null;
    public static CameraManager m_camMgr = null;
    public static Joystick m_joystick = null;
    public static P_BombArea m_bombArea = null;
    public static AIManager[] m_aiMgrs = null;
    public static AIPathManager m_aiPathMgr = null;

    public static SnowGroundScrolling m_snowGround = null;      // (★고민중)



    [Header("게임시작시 실행될 DoTween")]
    Sequence gameStartSeq;


    // Start is called before the first frame update
    void Start()
    {
        uniqueInstance = this;

        // === 플랫폼 초기 세팅 === //
        Application.targetFrameRate = 60;
        QualitySettings.vSyncCount = 0;
        // === 플랫폼 초기 세팅 === //

        if(m_isPlay == true)
            GameStartDoTween();
    }


    /// <summary>
    /// 메인 스크립트가 붙은 Obj인스턴스 찾기
    /// </summary>
    private void GameStartDoTween()
    {
        m_aiMgrs = FindObjectsOfType<AIManager>();
        m_aiPathMgr = FindObjectOfType<AIPathManager>();

        // === AI 초기화 관련 === //
        if (m_aiMgrs != null && m_aiPathMgr != null)
        {
            for (int n = 0; n < m_aiMgrs.Length; n++)
            {
                m_aiMgrs[n].SetPlayerStartPos();                                 // AI StartPos 초기화
                m_aiMgrs[n].m_snowBallMgr.m_fastZoneTrailMgr.SetTrailOnSnowPooling();  // AI SnowBall 흔적풀링 초기화
                m_aiMgrs[n].SetAIPath();                                         // AI StartPos 지나갈 자리 초기
            }
        }
        else
            print("AIManager나 AIPathManager 중 찾을 수 없습니다.");
        // === AI 초기화 관련 === //


        gameStartSeq = DOTween.Sequence()
                                   .OnStart(() =>
                                   {
                                       m_curGameState = eGameState.Ready;
                                   })
                                   .AppendInterval(1f)
                                   .AppendCallback(() =>
                                   {
                                       // === 메인 Objs인스턴스 찾는 함수 === //
                                       FindMainObjs();
                                       // === 메인 Objs인스턴스 찾는 함수 === //

                                       PlayAiGame();
                                   })
                                   .AppendInterval(1f)
                                   .AppendCallback(() =>
                                   {
                                       // === 게임시작시 카메라 => 플레이어 뒤로 이동 (연출) === //
                                       m_camMgr.CamToPlayerProduction(true);                    // GameState (Start)
                                       // === 게임시작시 카메라 => 플레이어 뒤로 이동 (연출) === //


                                       // === 플레이어 초기화 관련 === //
                                       m_plyMgr.SetPlayerStartPos();                                            // 플레이어 StartPos 초기화
                                       m_plyMgr.m_snowBallMgr.m_fastZoneTrailMgr.SetTrailOnSnowPooling();       // fastzone에서의 이펙트 풀링 초기화
                                       m_plyMgr.m_snowBallMgr.m_fastZoneTrailMgr.SetTrailOnSoilPooling();       // fastzone에서의 이펙트 풀링 초기화
                                       // === 플레이어 초기화 관련 === //


                                       m_curGameState = eGameState.Start;
                                   })
                                   .AppendInterval(4f)
                                   .OnComplete(() =>
                                   {
                                       // === 게임 시작! === //
                                       PlayPlayerGame();
                                       // === 게임 시작! === //
                                   });
    }


    /// <summary>
    /// 메인 Objs인스턴스 찾는 함수
    /// </summary>
    private void FindMainObjs()
    {
        // === 메인 Obj인스턴스 찾기 === //
        m_plyMgr = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerManager>();
        m_snowBallMgr = GameObject.FindGameObjectWithTag("SnowBall").GetComponent<SnowBallManager>();

        m_camMgr = FindObjectOfType<CameraManager>();
        m_joystick = FindObjectOfType<Joystick>();
        m_bombArea = FindObjectOfType<P_BombArea>();
        m_aiMgrs = FindObjectsOfType<AIManager>();


        m_snowGround = FindObjectOfType<SnowGroundScrolling>();     // (★고민중)

        if (m_camMgr != null && m_plyMgr != null && m_snowBallMgr != null &&
            m_joystick != null && m_snowGround != null && m_bombArea != null)
            print("Objs 찾기완료!");
        // === 메인 Obj인스턴스 찾기 === //
    }


    /// <summary>
    /// 플레이어 게임시작 함수
    /// </summary>
    public void PlayPlayerGame()
    {
        // === 플레이어 관련 === //
        m_plyMgr.m_isMoving = true;                           // 플레이어 움직임 여부
        m_plyMgr.m_snowBallMgr.SetSphereCollider(true);       // 스노우볼 콜라이더 켜기
        m_plyMgr.m_snowBallMgr.SetSnowBallSize(true);         // 스노우볼 사이즈업 함수
        m_plyMgr.m_snowBallMgr.SetSnowEffect(true);           // 이펙트 크기 증가 함수 (눈덩이 지나간 흔적, 눈덩이 튀기는 이펙트(왼/오))
        StartCoroutine(m_plyMgr.PlayerMoving());              // 플레이어 Forward 방향 코루틴
        // === 플레이어 관련 === //


        m_curGameState = eGameState.Play;            // GameState (Play)
    }



    /// <summary>
    /// AI 게임시작 함
    /// </summary>
    public void PlayAiGame()
    {
        // === AI 관련 === //
        for (int n = 0; n < m_aiMgrs.Length; n++)
        {
            m_aiMgrs[n].m_isMoving = true;
            m_aiMgrs[n].m_snowBallMgr.SetSphereCollider(true);
            m_aiMgrs[n].m_snowBallMgr.SetSnowBallSize(true, false);
            StartCoroutine(m_aiMgrs[n].PlayerMoving());
        }
        // === AI 관련 === //
    }
}
