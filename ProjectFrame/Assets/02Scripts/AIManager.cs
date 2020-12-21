using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.VFX;

public class AIManager : PlayerManager
{
    [Header("Ai Path")]
    [SerializeField] private List<Transform> m_aiPathTf;

    Sequence test;

    protected override void Awake()
    {
        base.Awake();

        m_aiPathTf = new List<Transform>();
    }


    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
    }


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
        SetAnimSpeedOrigin();

        this.transform.position = m_playerStartTf;
        this.transform.rotation = Quaternion.Euler(0, 0, 0);

        m_animCtrl.SetTrigger("Idle");
    }
    #endregion


    #region AI 애니메이션 실행
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
        // === ClearPos로 이동 === //
        //playerClearSeq = DOTween.Sequence()
        //                        .Append(this.transform.DOMove(m_clearTf.position, 1.5f))
        //                        .Join(this.transform.DORotate(m_clearTf.eulerAngles, 1.5f))
        //                        .OnComplete(() =>
        //                        {
        //                            m_animCtrl.SetTrigger("Clear");
        //                            m_splashEffect.SetActive(false);

        //                            // === 눈덩이 Player Obj에서 떼어낸 후 => 굴러가게 === //
        //                            m_snowBallObj.transform.SetParent(m_playerParentTf.transform);
        //                            m_snowBallObj.transform.DORotate(Vector3.zero, 1f);
        //                            // === 눈덩이 Player Obj에서 떼어낸 후 => 굴러가게 === //
        //                        });
        // === ClearPos로 이동 === //
    }

    /// <summary>
    /// (GameState = End)Ai GameOver 애니메이션 실행함수
    /// </summary>
    public void SetAiAnim_GameOver()
    {
        m_animCtrl.SetTrigger("GameOver");                                // 플레이어 Die 애니메이션 실행


        m_snowBallMeshRdr.enabled = false;                                // 눈덩이 메쉬렌더러 끄기


        m_splashEffect.SetActive(false);                                  // 플레이어 달리는 이펙트 끄기


        StopPlayerMoving();                                               // 플레이어 움직임 Stop


        test = DOTween.Sequence()
                       .AppendInterval(3.2f)
                       .AppendCallback(() =>
                       {
                           SetPlayerToStartPos();                         // 플레이어 초기 시작 위치로
                           SetAIPath();                                   // 경로 다시설/


                           m_snowBallMgr.SetLocalScale();                 // 스노우볼 크기 초기화
                           m_snowBallMeshRdr.enabled = true;              // 스노우볼 렌더링 켜기
                       })
                       .AppendInterval(3f)
                       .OnComplete(() =>
                       {
                           SetAnim_Push();
                           m_isMoving = true;
                           m_snowBallMgr.RotateSnowBall();
                           m_snowBallMgr.SetSnowBallSize(true, false);
                           StartCoroutine(PlayerMoving());
                       });
    }
    #endregion


    #region 플레이어 이동관련
    private int m_pathCount = 0;
    public void SetAIPath()
    {
        m_aiPathTf.Clear();
        m_pathCount = 0;

        m_aiPathTf = InGameManager.m_aiPathMgr.SetPathRoot();
    }

    /// <summary>
    /// (GameState = Play) Ai 경로따라 움직이는 코루틴함수               고치는중~!~
    /// </summary>
    public override IEnumerator PlayerMoving()
    {
        if (m_isMoving == true)
        {
            // === 지경 Path로 이동 === //
            float a_dis = (this.transform.position - m_aiPathTf[m_pathCount].position).sqrMagnitude;
            if (a_dis < 5f)
                m_pathCount += 1;
            else
            {
                this.transform.Translate(Vector3.forward * m_playerMovSpd, Space.Self);
                this.transform.DOLookAt(m_aiPathTf[m_pathCount].position, 2.5f);
            }
            // === 지경 Path로 이동 === //

            yield return null;

            StartCoroutine(PlayerMoving());
        }
    }

    /// <summary>
    /// (GameState = End) Ai 게임 Clear/GameOver시 호출되는 함수
    /// </summary>
    public override void StopPlayerMoving()
    {
        m_isMoving = false;
        m_playerRigid.isKinematic = true;
        SetAnimSpeedOrigin();                       // 플레이어 스피드 1, 기본스피드로 초기
        StopCoroutine(PlayerMoving());              // 플레이어 움직임 중단
    }

    /// <summary>
    /// (GameState = Play)눈덩이 커질때마다 호출되는 함수 (증가)
    /// </summary>
    public override void SetAnimSpeedUp()
    {
        float a_rnd = Random.Range(.7f, 1.3f);
        m_animCtrl.speed += a_rnd;
        m_playerMovSpd += a_rnd;
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
    public override void PlayCrashEffect(int a_num)
    {
        base.PlayCrashEffect(a_num);
    }
    #endregion


    protected override void OnTriggerEnter(Collider other)
    {

    }
}