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
    [SerializeField] private GameObject m_snowBallObj = null;                         // 눈덩이 오브젝트
    [SerializeField] private SphereCollider m_snowBallCollider = null;                // 눈덩이 콜라이더
    [SerializeField] private Vector3 m_originSnowBallSizeVec = Vector3.zero;          // 원래 눈덩이 크기
    [SerializeField] private Vector3 m_originSnowBallPositionVec = Vector3.zero;      // 원래 눈덩이 거리
    public Vector3 m_snowBallIncreaseVec = Vector3.zero;                              // 눈덩이 커지는 크기 벡터
    public Vector3 m_snowBallDecreaseVec = Vector3.zero;                              // 눈덩이 작아지는 크기 벡터
    [SerializeField] private float m_increasePosZ = 0;
    [SerializeField] private float m_decreasePosZ = 0;
    public Vector3 m_originSnowBallIncreaseVec = Vector3.zero;                // 초기 눈덩이 증가량
    public Transform m_parentTf = null;


    [Header("눈덩이 이펙트 관련")]
    [SerializeField] Transform[] m_snowEffectTf = null;                       // 눈덩이 이펙트 Scale를 위한 (왼/오)
    [SerializeField] ParticleSystem[] m_snowTrailEffect = null;               // 눈덩이 이펙트 (왼/오)
    public FastZoneTrailManager m_fastZoneTrailMgr = null;                    // 눈덩이 지나간 흔적


    [Header("카메라 이펙트 관련")]
    [SerializeField] ParticleSystem m_camEffect = null;                      // Camera Splah 이펙



    private int m_snowBallSizeX = 0;
    private int m_snowBallISizeTmp = 0;

    private List<Tweener> m_tweener;
    private List<Tweener> m_tweenerD;


    private void Start()
    {
        //m_curSnowBallSize = eSnowBallSize.One;
        
        m_originSnowBallIncreaseVec = m_snowBallIncreaseVec;
        m_snowBallISizeTmp = (int)this.transform.localScale.x;

        m_tweener = new List<Tweener>();
        m_tweenerD = new List<Tweener>();


        if (m_myCharacter == null)
            m_myCharacter = this.transform.parent.GetComponent<PlayerManager>();

        if (m_parentTf == null)
            m_parentTf = this.transform.parent;
    }


    private void Update()
    {
        //if(m_myCharacter.m_playerState == ePlayerState.Run)
        if(InGameManager.uniqueInstance.m_curGameState == InGameManager.eGameState.Play)
        {
            // === 눈덩이 사이즈가 0보다 작아진다면 === //
            if (m_snowBallISizeTmp <= 0)
            {
                ResetTweener();
                SetSnowEffect(false);
                m_myCharacter.SetAnim_GameOver();

                m_myCharacter.m_playerState = ePlayerState.Death;
                return;
            }
            // === 눈덩이 사이즈가 0보다 작아진다면 === //


            // === 눈덩이 사이즈 측정 === //
            //if (Mathf.Abs(m_snowBallISizeTmp - m_snowBallSizeX) >= 4)
            if (m_snowBallISizeTmp - m_snowBallSizeX >= 4)
            {
                m_snowBallSizeX = (int)m_snowBallISizeTmp;

                if (m_isPlayer == true)
                {
                    m_curSnowBallSize += 1;
                    InGameManager.m_camMgr.SetFollowOffset(true);               // 카메라 줌아웃
                    InGameManager.m_plyMgr.SetAnimSpeedUp();                    // 플레이어 스피스업 & 애니메이션 스피드업
                }
                else
                {
                    m_curSnowBallSize += 1;
                    m_parentTf.GetComponent<AIManager>().SetAnimSpeedUp();      // AI 스피드업
                }
            }
            else if(m_snowBallISizeTmp - m_snowBallSizeX < 0)
            {
                m_snowBallSizeX -= 4;

                if (m_isPlayer == true)
                {
                    m_curSnowBallSize -= 1;
                    InGameManager.m_camMgr.SetFollowOffset(false);               // 카메라 줌아웃
                    InGameManager.m_plyMgr.SetAnimSpeedDown();                   // 플레이어 스피스업 & 애니메이션 스피드업
                }
                else
                {
                    m_curSnowBallSize -= 1;
                    m_parentTf.GetComponent<AIManager>().SetAnimSpeedDown();      // AI 스피드업
                }
            }
            else
            {
                m_snowBallISizeTmp = (int)this.transform.localScale.x;
            }

            // === 스노우볼 사이즈 측정 === //


            // === 스노우볼 x축 회전 === //
            m_snowBallObj.transform.Rotate(Vector3.right * 500 * Time.deltaTime, Space.Self);
            // === 스노우볼 x축 회전 === //
        }
        else if(InGameManager.uniqueInstance.m_curGameState == InGameManager.eGameState.Clear)
        {
            // === 눈덩이 사이즈가 0보다 작아진다면 === //
            if (m_snowBallISizeTmp <= 0)
            {
                ResetTweener();

                m_myCharacter.SetAnim_Clear();
                InGameManager.m_camMgr.SetMotionBlur(false);

                SetSnowEffect(false);
                SetSnowBallTrailEffect(false);


                m_myCharacter.m_playerState = ePlayerState.Happy;
                return;
            }
            else
            {
                m_snowBallISizeTmp = (int)this.transform.localScale.x;

                // === 스노우볼 x축 회전 === //
                m_snowBallObj.transform.Rotate(Vector3.right * 500 * Time.deltaTime, Space.Self);
                // === 스노우볼 x축 회전 === //
            }
            // === 눈덩이 사이즈가 0보다 작아진다면 === //
        }
        else
        {
            ResetTweener();
        }
    }


    #region (GameState = Play) 스노우볼 크기 관련
    /// <summary>
    /// (GameState = Play)스노우볼 크기 증가/감소 함수
    /// </summary>
    /// <param name="m_isUp"></param>
    [Header("눈덩이 증감 여부")]
    [SerializeField] private bool m_sizeUp = true;
    public void SetSnowBallSize(bool m_isUp)
    {
        m_sizeUp = m_isUp;

        if(m_isUp == true)
        {
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
            #region 눈덩이 크기 감소
            m_tweener.Add(this.transform.DOScale(this.transform.localScale - m_snowBallDecreaseVec, .1f)
                    .SetEase(Ease.Linear)
                    .SetLoops(-1, LoopType.Incremental));
            m_tweener.Add(this.transform.DOLocalMove(new Vector3(0, -1.6f, this.transform.localPosition.z - m_decreasePosZ), .1f)
                    .SetEase(Ease.Linear)
                    .SetLoops(-1, LoopType.Incremental));
            #endregion
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
            SetSnowBallSize(false);
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
    #endregion


    #region 눈사람 먹었을때 크기증가
    /// <summary>
    /// 눈사람 먹어서 크기 커지는 함수
    /// </summary>
    public void GainSnowMan()
    {
        m_fastZoneTrailMgr.GainSnowMan();
        this.transform.DOScale(this.transform.localScale + new Vector3(.25f, .25f, .25f), .1f)
                            .SetEase(Ease.Linear);
        this.transform.DOLocalMove(new Vector3(0, -1.6f, this.transform.localPosition.z + .2f), .1f)
                      .SetEase(Ease.Linear)
                      .OnComplete(() =>
                      {
                          SetSnowBallSize(true);
                      });
    }
    #endregion


    #region 눈덩이 이펙트 관련
    public void SetSnowEffect(bool a_isStart)
    {
        if(a_isStart == true)
        {
            // === SnowBall(왼/오) 나오는 이펙트 크기 증감 코루틴 === //
            StartCoroutine("SetSnowBallEffectScale");
            // === SnowBall(왼/오) 나오는 이펙트 크기 증감 코루틴 === //


            // === 눈덩이 자취 흔적 코루틴 === //
            m_fastZoneTrailMgr.SetDequeueTrail(true);
            // === 눈덩이 자취 흔적 코루틴 === //
        }
        else
        {
            StopCoroutine("SetSnowBallEffectScale");
            m_fastZoneTrailMgr.SetDequeueTrail(false);
        }
    }


    /// <summary>
    /// 눈덩이(왼/오)에서 나오는 이펙트 크기 증감 코루틴
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

        StartCoroutine("SetSnowBallEffectScale");
    }


    /// <summary>
    /// SnowBall 좌/우에 생성되는 이펙트 on/off 여부
    /// </summary>
    /// <param name="a_isStart"></param>
    public void SetSnowBallTrailEffect(bool a_isStart)
    {
        if (a_isStart == true)
            for (int n = 0; n < m_snowTrailEffect.Length; n++)
            {
                if(m_snowTrailEffect[n] != null)
                    m_snowTrailEffect[n].Play();
            }
        else
            for (int n = 0; n < m_snowTrailEffect.Length; n++)
            {
                if (m_snowTrailEffect[n] != null)
                    m_snowTrailEffect[n].Stop();
            }
    }


    /// <summary>
    /// 카메라 Splash 이펙트 실행여부
    /// </summary>
    /// <param name="a_isStart"></param>
    public void SetCamEffect(bool a_isStart)
    {
        if (a_isStart == true)
            if(m_camEffect != null)
                m_camEffect.Play();
        else
            if (m_camEffect != null)
                m_camEffect.Stop();
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
