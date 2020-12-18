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
    

    [Header("객체 인스턴스")]
    public static CameraManager m_camMgr = null;
    public static PlayerManager m_plyMgr = null;
    public static SnowBallManager m_snowBallMgr = null;
    public static Joystick m_joystick = null;
    public static SnowGroundScrolling m_snowGround = null;      // (★고민중)
    public static P_BombArea m_bombArea = null;
    public static FastZoneTrailManager m_fastZoneTrail = null;


    [Header("게임시작시 실행될 DoTween")]
    Sequence gameStartSeq;


    // Start is called before the first frame update
    void Start()
    {
        uniqueInstance = this;
        
        GameStartDoTween();
    }

    /// <summary>
    /// 메인 스크립트가 붙은 Obj인스턴스 찾기
    /// </summary>
    private void GameStartDoTween()
    {
        gameStartSeq = DOTween.Sequence()
                                   .OnStart(() =>
                                   {
                                       m_curGameState = eGameState.Ready;
                                   })
                                   .AppendInterval(2f)
                                   .AppendCallback(() =>
                                   {
                                       // === 메인 Objs인스턴스 찾는 함수 === //
                                       FindMainObjs();
                                       // === 메인 Objs인스턴스 찾는 함수 === //
                                   })
                                   .AppendInterval(1f)
                                   .AppendCallback(() =>
                                   {
                                       // === 게임시작시 카메라 => 플레이어 뒤로 이동 (연출) === //
                                       m_camMgr.FirstCamProduction();       // GameState (Start)
                                       // === 게임시작시 카메라 => 플레이어 뒤로 이동 (연출) === //

                                       m_fastZoneTrail.SetTrailPooling();           // fastzone에서의 이펙트 풀링 초기화
                                   })
                                   .AppendInterval(4f)
                                   .AppendCallback(() =>
                                   {
                                       m_plyMgr.SetAnim_Push();                     // 플레이어 Push Anim 실행
                                       PlayerManager.m_isMoving = true;             // 플레이어 움직임 여부
                                       StartCoroutine(m_plyMgr.PlayerMoving());     // 플레이어 Forward 방향 코루틴


                                       m_snowBallMgr.RotateSnowBall();              // 스노우볼 굴러가는 함수
                                       m_snowBallMgr.SetSnowBallSize(true);         // 스노우볼 사이즈업 함수




                                       m_snowGround.GroundMoving();         // (★고민중)


                                       m_curGameState = eGameState.Play;   // GameState (Play)
                                       CameraManager.m_isPlay = true;
                                   });
    }

    /// <summary>
    /// 메인 Objs인스턴스 찾는 함수
    /// </summary>
    private void FindMainObjs()
    {
        // === 메인 Obj인스턴스 찾기 === //
        m_camMgr = FindObjectOfType<CameraManager>();
        m_plyMgr = FindObjectOfType<PlayerManager>();
        m_snowBallMgr = FindObjectOfType<SnowBallManager>();
        m_joystick = FindObjectOfType<Joystick>();
        m_snowGround = FindObjectOfType<SnowGroundScrolling>();     // (★고민중)
        m_bombArea = FindObjectOfType<P_BombArea>();
        m_fastZoneTrail = FindObjectOfType<FastZoneTrailManager>();

        if (m_camMgr != null && m_plyMgr != null && m_snowBallMgr != null &&
            m_joystick != null && m_snowGround != null && m_bombArea != null &&
            m_fastZoneTrail != null)
                print("Objs 찾기완료!");
        // === 메인 Obj인스턴스 찾기 === //
    }
    
}
