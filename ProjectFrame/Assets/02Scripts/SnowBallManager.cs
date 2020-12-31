using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class SnowBallManager : MonoBehaviour
{
    public enum eSnowBallSize
    {
        One,
        ThirtyFive,
        Ninety,
        HundredFIf,
        TwoHdTen,
        TwoHdEighty,
        ThreeHdFIfFive,

        End,
        None
    }


    [Header("현재 눈덩이 크기레벨")]
    public eSnowBallSize m_curSnowBallSize;
    [SerializeField] protected PlayerManager m_myCharacter = null;
    [SerializeField] private bool m_isPlayer = false;


    [Header("눈덩이 오브젝트")]
    [SerializeField] private GameObject m_snowBallObj = null;                 // 눈덩이 오브젝트
    [SerializeField] private SphereCollider m_snowBallCollider = null;                // 눈덩이 콜라이더
    [SerializeField] private Vector3 m_originSnowBallSizeVec = Vector3.zero;          // 원래 눈덩이 크기
    [SerializeField] private Vector3 m_originSnowBallPositionVec = Vector3.zero;      // 원래 눈덩이 거리
    public Vector3 m_snowBallIncreaseVec = Vector3.zero;        // 눈덩이 커지는 크기 벡터
    public Vector3 m_snowBallDecreaseVec = Vector3.zero;        // 눈덩이 작아지는 크기 벡터
    [SerializeField] private float m_increasePosZ = 0;
    [SerializeField] private float m_decreasePosZ = 0;
    public Vector3 m_originSnowBallIncreaseVec = Vector3.zero;                // 초기 눈덩이 증가량
    public Transform m_parentTf = null;


    [Header("눈덩이 이펙트 관련")]
    [SerializeField] Transform[] m_snowEffectTf = null;                       // 눈덩이 이펙트 Scale를 위한 (왼/오)
    [SerializeField] ParticleSystem[] m_snowTrailEffect = null;               // 눈덩이 이펙트 (왼/오)
    public FastZoneTrailManager m_fastZoneTrailMgr = null;                    // 눈덩이 지나간 흔적


    [Header("카메라 이펙트 관련")]
    [SerializeField] ParticleSystem m_camEffect = null;                      // Camera Splah 이펙


    [Header("장애물과 눈덩이 관련")]
    [SerializeField] private Transform[] m_obstaclePosTf = null;
    [SerializeField] private Transform m_obstacleParentTf = null;            // 장애물이 눈덩이에 속할 부모Tf
    [SerializeField] private CharacterInSnowBall[] m_characterObj = null;


    private float m_snowBallSizeX = 0;
    private float m_snowBallISizeTmp = 0;

    private List<Tweener> m_tweener;
    private List<Tweener> m_tweenerD;


    private void Start()
    {
        //m_curSnowBallSize = eSnowBallSize.One;
        
        m_originSnowBallIncreaseVec = m_snowBallIncreaseVec;

        m_tweener = new List<Tweener>();
        m_tweenerD = new List<Tweener>();


        if (m_myCharacter == null)
            m_myCharacter = this.transform.parent.GetComponent<PlayerManager>();

        if (m_parentTf == null)
            m_parentTf = this.transform.parent;
    }


    private void Update()
    {
        if(m_myCharacter.m_playerState == ePlayerState.Run)
        {
            // === 스노우볼 사이즈 측정 === //
            switch (m_curSnowBallSize)
            {
                case eSnowBallSize.One:
                    if (m_snowBallISizeTmp - m_snowBallSizeX >= 4)
                    {
                        m_snowBallSizeX = m_snowBallISizeTmp;

                        if (m_isPlayer == true)
                        {
                            InGameManager.m_camMgr.SetFollowOffset(true);
                            InGameManager.m_plyMgr.SetAnimSpeedUp();
                        }
                        else
                        {
                            m_parentTf.GetComponent<AIManager>().SetAnimSpeedUp();
                        }
                    }
                    else
                    {
                        m_snowBallISizeTmp = this.transform.localScale.x;
                    }
                    break;
            }
            // === 스노우볼 사이즈 측정 === //


            // === 스노우볼 x축 회전 === //
            m_snowBallObj.transform.Rotate(Vector3.right * 500 * Time.deltaTime, Space.Self);
            // === 스노우볼 x축 회전 === //
        }
    }


    #region (GameState = Play) 스노우볼 크기 관련
    /// <summary>
    /// (GameState = Play)스노우볼 크기 증가/감소 함수
    /// </summary>
    /// <param name="m_isUp"></param>
    public void SetSnowBallSize(bool m_isUp, bool m_isPlayer = true)
    {
        if(m_isUp == true)
        {
            m_myCharacter.SetAnim_Push();     // 플레이어 Push Anim 실행


            SetSnowBallTrailEffect(true);     // SnowBall(왼/오) 이펙트 켜기
            if(m_isPlayer == true)
                SetCamEffect(true);           // 카메라 Splsh 이펙트 켜기


            #region 눈덩이 크기 증가
            m_tweener.Add(this.transform.DOScale(this.transform.localScale + m_snowBallIncreaseVec, .1f)
                    .SetEase(Ease.Linear)
                    .SetLoops(-1, LoopType.Incremental));
            m_tweener.Add(this.transform.DOLocalMove(new Vector3(0, -1.6f, this.transform.localPosition.z + m_increasePosZ), .1f)
                    .SetEase(Ease.Linear)
                    .SetLoops(-1, LoopType.Incremental));
            #endregion
        }
        else
        {
            if (m_isPlayer == true)
                SetCamEffect(false);           // 카메라 Splsh 이펙트 끄기

            #region 눈덩이 크기 감소
            m_tweener.Add(this.transform.DOScale(this.transform.localScale - m_snowBallDecreaseVec, .1f)
                    .SetEase(Ease.Linear)
                    .SetLoops(-1, LoopType.Incremental));
            m_tweener.Add(this.transform.DOLocalMove(new Vector3(0, -1.6f, this.transform.localPosition.z - m_decreasePosZ), .1f)
                    .SetEase(Ease.Linear)
                    .SetLoops(-1, LoopType.Incremental));
            #endregion



            //#region 눈덩이 크기 감소 & 카메라 줌인
            //switch (m_curSnowBallSize)
            //{
            //    case eSnowBallSize.One:
            //        StopCoroutine(m_fastZoneTrailMgr.SetTrailVec(false));
            //        StartCoroutine(m_fastZoneTrailMgr.SetTrailVec(true));           // 눈덩이 흔적 초기s

            //        StopCoroutine(SetSnowBallEffectScale(false));
            //        StartCoroutine(SetSnowBallEffectScale(true));                         // 눈덩이 튀기는 이펙트 초기화

            //        if (m_isPlayer == true)
            //            InGameManager.m_plyMgr.SetAnim_GameOver();
            //        else
            //            m_parentTf.GetComponent<AIManager>().SetAiAnim_GameOver();

            //        break;


            //    case eSnowBallSize.ThirtyFive:
            //        InGameManager.m_camMgr.SetFollowOffset(false);

            //        m_tweenerD.Add(this.transform.DOScale(new Vector3(15f, 15f, 15f), .3f));
            //        m_tweenerD.Add(this.transform.DOLocalMove(new Vector3(0, 0, 14f), .3f)
            //                  .OnComplete(() =>
            //                  {
            //                      m_curSnowBallSize = eSnowBallSize.One;

            //                      m_tweenerD.Clear();

            //                      SetSphereCollider(true);

            //                      if (m_isPlayer == true)
            //                          SetSnowBallSize(true);
            //                      else
            //                          SetSnowBallSize(true, false);

            //                  }));
            //        break;


            //    case eSnowBallSize.Ninety:
            //        InGameManager.m_camMgr.SetFollowOffset(false);


            //        m_tweenerD.Add(this.transform.DOScale(new Vector3(35f, 35f, 35f), .3f));
            //        m_tweenerD.Add(this.transform.DOLocalMove(new Vector3(0, 0, 22f), .3f)
            //                  .OnComplete(() =>
            //                  {
            //                      m_curSnowBallSize = eSnowBallSize.ThirtyFive;

            //                      m_tweenerD.Clear();

            //                      SetSphereCollider(true);


            //                      if (m_isPlayer == true)
            //                          SetSnowBallSize(true);
            //                      else
            //                          SetSnowBallSize(true, false);
            //                  }));
            //        break;

            //    case eSnowBallSize.HundredFIf:
            //        InGameManager.m_camMgr.SetFollowOffset(false);


            //        m_tweenerD.Add(this.transform.DOScale(new Vector3(80f, 80f, 80f), .3f));
            //        m_tweenerD.Add(this.transform.DOLocalMove(new Vector3(0, 0, 44f), .3f)
            //                  .OnComplete(() =>
            //                  {
            //                      m_curSnowBallSize = eSnowBallSize.Ninety;

            //                      m_tweenerD.Clear();

            //                      SetSphereCollider(true);

            //                      if(m_isPlayer == true)
            //                          SetSnowBallSize(true);
            //                      else
            //                          SetSnowBallSize(true, false);
            //                  }));
            //        break;
            //}
            //#endregion
        }
    }


    /// <summary>
    /// (GameState = Play)스노우볼 (장애물에 의해/AI 흡수) 작아지는 순간 호출되는 함수
    /// 눈덩이 크기증가 Stop
    /// </summary>
    public void StopIncreaseSnowBall(PlayerManager a_obj, bool a_isOver = false)
    {
        m_myCharacter.m_playerState = ePlayerState.Death;

        // === 트위너 초기화 === //
        ResetTweener();
        // === 트위너 초기화 === //

        // === 콜라이더 끄기 === //
        SetSphereCollider(false);
        // === 콜라이더 끄기 === //

        if (a_isOver == false)
        {
            SetSnowBallSize(false);
        }
        else
        {
            //InGameManager.m_fastZoneTrail.m_isTrailOn = false;

            a_obj.SetAnim_GameOver();
        }
    }
    public void StopIncreaseAiSnowBall(AIManager a_obj, bool a_isOver = false)
    {
        m_myCharacter.m_playerState = ePlayerState.Death;

        // === 트위너 초기화 === //
        ResetTweener();
        // === 트위너 초기화 === //

        // === 콜라이더 끄기 === //
        SetSphereCollider(false);
        // === 콜라이더 끄기 === //

        if (a_isOver == false)
        {
            SetSnowBallSize(false, false);
        }
        else
        {
            a_obj.SetAiAnim_GameOver();
        }
    }


    /// <summary>
    /// SnowBall 콜라이더 여부
    /// </summary>
    /// <param name="a_setSphere"></param>
    public void SetSphereCollider(bool a_setSphere)
    {
        if (a_setSphere == true)
            m_snowBallCollider.enabled = true;
        else
            m_snowBallCollider.enabled = false;
    }


    /// <summary>
    /// 눈덩이 크기 초기화 함수
    /// </summary>
    public void SetLocalScale()
    {
        m_curSnowBallSize = eSnowBallSize.One;

        m_snowBallIncreaseVec = m_originSnowBallIncreaseVec;

        this.transform.localScale = m_originSnowBallSizeVec;
        this.transform.localPosition = m_originSnowBallPositionVec;
    }


    /// <summary>
    /// (GameState = Play) 장애물이 눈속에 달라붙는 함수
    /// </summary>
    public void AttachToSnowBall(eObstacleLevel a_obsLevel, GameObject a_obstacle)
    {
        int a_rnd = Random.Range(0, m_obstaclePosTf.Length);
        a_obstacle.transform.SetParent(m_obstacleParentTf.transform);
        a_obstacle.transform.position = m_obstaclePosTf[a_rnd].position;
        a_obstacle.transform.rotation = m_obstaclePosTf[a_rnd].rotation;


        switch(a_obsLevel)
        {
            case eObstacleLevel.Small:
                a_obstacle.transform.localScale = new Vector3(.4f, .35f, .4f);
                break;

            case eObstacleLevel.Normal:
                a_obstacle.transform.localScale = new Vector3(.5f, .45f, .5f);
                break;

            case eObstacleLevel.Middle:
                a_obstacle.transform.localScale = new Vector3(.44f, .44f, .44f);
                break;
        }
    }
    #endregion

    
    #region 눈덩이 굴러가는 연출
    /// <summary>
    /// (GameState = End)눈덩이 굴러가는거 멈추는 DoTween함수 & 눈덩이에 박힌 장애물들 폭발
    /// </summary>
    public void StopRotateSnowBall()
    {
        // === 스노우볼에 뭍혀있던 장애물들 폭발 부분 === //
        ObstacleManager[] a_snowBall = new ObstacleManager[m_obstacleParentTf.childCount];
        if(a_snowBall.Length != 0)
        {
            for (int n = 0; n < m_obstacleParentTf.childCount; n++)
            {
                a_snowBall[n] = m_obstacleParentTf.GetChild(n).GetComponent<ObstacleManager>();
                a_snowBall[n].AddExplosion();
            }
        }
        // === 스노우볼에 뭍혀있던 장애물들 폭발 부분 === //


        // === 스노우볼에 붙혀있던 Character 폭발 부분 === //
        for(int n = 0; n < m_characterObj.Length; n++)
        {
            if (m_characterObj[n].gameObject.activeSelf == true)
            {
                m_characterObj[n].CharacterAddForce();
            }
        }
        // === 스노우볼에 붙혀있던 Character 폭발 부분 === //
    }
    #endregion


    #region 눈덩이 이펙트 관련
    public void SetSnowEffect(bool a_isStart)
    {
        if(a_isStart == true)
        {
            // === 눈바닥자취 크기 증감 코루틴 === //
            StartCoroutine(m_fastZoneTrailMgr.SetTrailVec()); ;
            // === 눈바닥자취 크기 증감 코루틴 === //


            // === SnowBall(왼/오) 나오는 이펙트 크기 증감 코루틴 === //
            StartCoroutine(SetSnowBallEffectScale());
            // === SnowBall(왼/오) 나오는 이펙트 크기 증감 코루틴 === //


            // === 눈덩이 자취 흔적 코루틴 === //
            StartCoroutine(m_fastZoneTrailMgr.DequeueTrailOnSnowEffect());
            // === 눈덩이 자취 흔적 코루틴 === //
        }
        else
        {
            StopCoroutine(m_fastZoneTrailMgr.SetTrailVec());
            StopCoroutine(SetSnowBallEffectScale());
            StopCoroutine(m_fastZoneTrailMgr.DequeueTrailOnSnowEffect());
        }
    }

    /// <summary>
    /// 눈덩이(왼/오)에서 나오는 이펙트 크기 증감 코루m_isSBEffectIncrease
    /// </summary>
    WaitForSeconds m_delayTime = new WaitForSeconds(.8f);
    public bool m_isSBEffectIncrease = true;
    private IEnumerator SetSnowBallEffectScale()
    {
        if (m_isSBEffectIncrease == false)
        {
            for (int n = 0; n < m_snowEffectTf.Length; n++)
                m_snowEffectTf[n].localScale -= new Vector3(0.1f, 0.1f, 0.1f);
        }
        else
        {
            for (int n = 0; n < m_snowEffectTf.Length; n++)
                m_snowEffectTf[n].localScale += new Vector3(0.1f, 0.1f, 0.1f);
        }
        
        yield return m_delayTime;

        SetSnowBallEffectScale();
    }


    /// <summary>
    /// SnowBall 좌/우에 생성되는 이펙트 on/off 여부
    /// </summary>
    /// <param name="a_isStart"></param>
    public void SetSnowBallTrailEffect(bool a_isStart)
    {
        if (a_isStart == true)
            for (int n = 0; n < m_snowTrailEffect.Length; n++)
                m_snowTrailEffect[n].Play();
        else
            for (int n = 0; n < m_snowTrailEffect.Length; n++)
                m_snowTrailEffect[n].Stop();
    }


    public void SetCamEffect(bool a_isStart)
    {
        if (a_isStart == true)
                m_camEffect.Play();
        else
                m_camEffect.Stop();
    }
    #endregion



    #region 눈덩이에 뭍히는 연출 & 눈덩이 크기 증가
    public void CharacterInSnowBall(eSnowBallSize a_snowBallSize)
    {
        switch(a_snowBallSize)
        {
            case eSnowBallSize.One:
                // === 눈덩이 크기 증가 === //
                this.transform.DOScale(this.transform.localScale + new Vector3(8, 8, 8), .1f)
                              .SetEase(Ease.Linear);
                this.transform.DOLocalMove(new Vector3(0, 0, this.transform.localPosition.z + 2.5f), .1f)
                              .SetEase(Ease.Linear)
                              .OnComplete(() =>
                              {
                                  SetSnowBallSize(true);
                              });
                // === 눈덩이 크기 증가 === //
                break;

            case eSnowBallSize.ThirtyFive:
                this.transform.DOScale(this.transform.localScale + new Vector3(35, 35, 35), .1f)
                              .SetEase(Ease.Linear);
                this.transform.DOLocalMove(new Vector3(0, 0, this.transform.localPosition.z + 8f), .1f)
                              .SetEase(Ease.Linear)
                              .OnComplete(() =>
                              {
                                  SetSnowBallSize(true);
                              });
                break;

            case eSnowBallSize.Ninety:
                this.transform.DOScale(this.transform.localScale + new Vector3(80, 80, 80), .1f)
                             .SetEase(Ease.Linear);
                this.transform.DOLocalMove(new Vector3(0, 0, this.transform.localPosition.z + 30f), .1f)
                              .SetEase(Ease.Linear)
                              .OnComplete(() =>
                              {
                                  SetSnowBallSize(true);
                              });
                break;

            case eSnowBallSize.HundredFIf:
                this.transform.DOScale(this.transform.localScale + new Vector3(160, 160, 160), .1f)
                             .SetEase(Ease.Linear);
                this.transform.DOLocalMove(new Vector3(0, 0, this.transform.localPosition.z + 50f), .1f)
                              .SetEase(Ease.Linear)
                              .OnComplete(() =>
                              {
                                  SetSnowBallSize(true);
                              });
                break;

            case eSnowBallSize.End:
                this.transform.DOScale(this.transform.localScale + new Vector3(160, 160, 160), .1f)
                         .SetEase(Ease.Linear);
                this.transform.DOLocalMove(new Vector3(0, 0, this.transform.localPosition.z + 50f), .1f)
                              .SetEase(Ease.Linear)
                              .OnComplete(() =>
                              {
                                  SetSnowBallSize(true);
                              });

                break;
        }


        // === 눈덩이에 박히는 사람 On === //
        while (true)
        {
            int a_charNum = Random.Range(0, m_characterObj.Length);
            if(m_characterObj[a_charNum].gameObject.activeSelf == false)
            {
                m_characterObj[a_charNum].gameObject.SetActive(true);
                break;
            }
        }
        // === 눈덩이에 박히는 사람 On === //
    }
    #endregion


    /// <summary>
    /// 트위너 리셋 함수
    /// </summary>
    public void ResetTweener()
    {
        for (int n = 0; n < m_tweener.Count; n++)
            m_tweener[n].Kill();

        for (int n = 0; n < m_tweenerD.Count; n++)
            m_tweenerD[n].Kill();

        m_tweener.Clear();
    }
}
