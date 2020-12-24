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
    [SerializeField] private GameObject m_snowBallObj = null;                    // 눈덩이 오브젝트
    [SerializeField] SphereCollider m_snowBallCollider = null;                      // 눈덩이 콜라이더
    public Vector3 m_snowBallIncreaseVec = new Vector3(.8f, .8f, .8f);          // 눈덩이 크지는 크기 벡터
    public Vector3 m_originSnowBallIncreaseVec = Vector3.zero;                // 초기 눈덩이 증가량
    public Transform m_parentTf = null;


    [Header("장애물과 눈덩이 관련")]
    [SerializeField] private Transform[] m_obstaclePosTf = null;
    [SerializeField] private Transform m_obstacleParentTf = null;                         // 장애물이 눈덩이에 속할 부모Tf
    [SerializeField] private CharacterInSnowBall[] m_characterObj = null;
    

    private List<Tweener> m_tweener;
    private List<Tweener> m_tweenerD;


    private void Start()
    {
        m_curSnowBallSize = eSnowBallSize.One;
        
        m_originSnowBallIncreaseVec = m_snowBallIncreaseVec;

        m_tweener = new List<Tweener>();
        m_tweenerD = new List<Tweener>();
    }


    private void Update()
    {
        //if(InGameManager.uniqueInstance.m_curGameState == InGameManager.eGameState.Play)
        if(m_myCharacter.m_playerState == ePlayerState.Run)
        {
            switch (m_curSnowBallSize)
            {
                case eSnowBallSize.One:
                    if (this.transform.localScale.x >= 35)
                    {
                        if (m_isPlayer == true)
                        {
                            InGameManager.m_camMgr.SetFollowOffset(true);
                            InGameManager.m_plyMgr.SetAnimSpeedUp();
                        }
                        else
                            m_parentTf.GetComponent<AIManager>().SetAnimSpeedUp();

                        m_snowBallIncreaseVec = new Vector3(1f, 1f, 1f);
                        m_curSnowBallSize = eSnowBallSize.ThirtyFive;
                        break;
                    }
                    break;

                case eSnowBallSize.ThirtyFive:
                    if (this.transform.localScale.x >= 90)
                    {
                        if (m_isPlayer == true)
                        {
                            InGameManager.m_camMgr.SetFollowOffset(true);
                            InGameManager.m_plyMgr.SetAnimSpeedUp();
                        }
                        else
                            m_parentTf.GetComponent<AIManager>().SetAnimSpeedUp();

                        m_snowBallIncreaseVec = new Vector3(1.4f, 1.4f, 1.4f);
                        m_curSnowBallSize = eSnowBallSize.Ninety;
                        break;
                    }
                    break;

                case eSnowBallSize.Ninety:
                    if (this.transform.localScale.x >= 150)
                    {
                        if (m_isPlayer == true)
                        {
                            InGameManager.m_camMgr.SetFollowOffset(true);
                            InGameManager.m_plyMgr.SetAnimSpeedUp();
                        }
                        else
                            m_parentTf.GetComponent<AIManager>().SetAnimSpeedUp();

                        m_snowBallIncreaseVec = new Vector3(2f, 2f, 2f);
                        m_curSnowBallSize = eSnowBallSize.HundredFIf;
                        break;
                    }
                    break;

                case eSnowBallSize.HundredFIf:
                    if (this.transform.localScale.x > 210)
                    {
                        if (m_isPlayer == true)
                        {
                            InGameManager.m_camMgr.SetFollowOffset(true);
                            InGameManager.m_plyMgr.SetAnimSpeedUp();
                        }
                        else
                            m_parentTf.GetComponent<AIManager>().SetAnimSpeedUp();

                        m_curSnowBallSize = eSnowBallSize.TwoHdTen;
                        break;
                    }
                    break;
                case eSnowBallSize.TwoHdTen:
                    if (this.transform.localScale.x > 280)
                    {
                        if (m_isPlayer == true)
                        {
                            InGameManager.m_camMgr.SetFollowOffset(true);
                            InGameManager.m_plyMgr.SetAnimSpeedUp();
                        }
                        else
                            m_parentTf.GetComponent<AIManager>().SetAnimSpeedUp();

                        m_curSnowBallSize = eSnowBallSize.TwoHdEighty;
                        break;
                    }
                    break;
                case eSnowBallSize.TwoHdEighty:
                    if (this.transform.localScale.x > 355)
                    {
                        if (m_isPlayer == true)
                        {
                            InGameManager.m_camMgr.SetFollowOffset(true);
                            InGameManager.m_plyMgr.SetAnimSpeedUp();
                        }
                        else
                            m_parentTf.GetComponent<AIManager>().SetAnimSpeedUp();

                        m_curSnowBallSize = eSnowBallSize.End;
                        break;
                    }
                    break;
            }

            m_snowBallObj.transform.Rotate(Vector3.right * 500 * Time.deltaTime, Space.World);
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
            m_myCharacter.SetAnim_Push();                        // 플레이어 Push Anim 실행

            #region 눈덩이 크기 증가 & 카메라 줌아웃
            m_tweener.Add(this.transform.DOScale(this.transform.localScale + m_snowBallIncreaseVec, .1f)
                    .SetEase(Ease.Linear)
                    .SetLoops(-1, LoopType.Incremental));
            m_tweener.Add(this.transform.DOLocalMove(new Vector3(0, 0, this.transform.localPosition.z + .25f), .1f)
                    .SetEase(Ease.Linear)
                    .SetLoops(-1, LoopType.Incremental));
            #endregion
        }
        else
        {
            if(m_isPlayer == true)
            {
                #region 눈덩이 크기 감소 & 카메라 줌인
                switch (m_curSnowBallSize)
                {
                    case eSnowBallSize.One:
                        //InGameManager.m_snowGround.StopGroundMoving();
                        InGameManager.m_plyMgr.SetAnim_GameOver();
                        break;

                    case eSnowBallSize.ThirtyFive:
                        InGameManager.m_camMgr.SetFollowOffset(false);

                        m_tweenerD.Add(this.transform.DOScale(new Vector3(10f, 10f, 10f), .3f));
                        m_tweenerD.Add(this.transform.DOLocalMove(new Vector3(0, 0, 8.8f), .3f)
                                  .OnComplete(() =>
                                  {
                                      m_curSnowBallSize = eSnowBallSize.One;

                                      SetSphereCollider(true);
                                      SetSnowBallSize(true);

                                      m_snowBallIncreaseVec = new Vector3(.8f, .8f, .8f);

                                      m_tweenerD.Clear();
                                  }));
                        break;

                    case eSnowBallSize.Ninety:
                        InGameManager.m_camMgr.SetFollowOffset(false);


                        m_tweenerD.Add(this.transform.DOScale(new Vector3(50f, 50f, 50f), .3f));
                        m_tweenerD.Add(this.transform.DOLocalMove(new Vector3(0, 0, 22.5f), .3f)
                                  .OnComplete(() =>
                                  {
                                      m_curSnowBallSize = eSnowBallSize.ThirtyFive;

                                      SetSphereCollider(true);
                                      SetSnowBallSize(true);

                                      m_snowBallIncreaseVec = new Vector3(1f, 1f, 1f);

                                      m_tweenerD.Clear();
                                  }));
                        break;

                    case eSnowBallSize.HundredFIf:
                        InGameManager.m_camMgr.SetFollowOffset(false);


                        m_tweenerD.Add(this.transform.DOScale(new Vector3(100f, 100f, 100f), .3f));
                        m_tweenerD.Add(this.transform.DOLocalMove(new Vector3(0, 0, 32f), .3f)
                                  .OnComplete(() =>
                                  {
                                      m_curSnowBallSize = eSnowBallSize.Ninety;

                                      SetSphereCollider(true);
                                      SetSnowBallSize(true);

                                      m_snowBallIncreaseVec = new Vector3(1.4f, 1.4f, 1.4f);

                                      m_tweenerD.Clear();
                                  }));
                        break;
                }
                #endregion
            }
            else
            {
                #region 눈덩이 크기 감소
                switch (m_curSnowBallSize)
                {
                    case eSnowBallSize.One:
                        //InGameManager.m_snowGround.StopGroundMoving();
                        m_parentTf.GetComponent<AIManager>().SetAiAnim_GameOver();
                        break;

                    case eSnowBallSize.ThirtyFive:
                        m_tweenerD.Add(this.transform.DOScale(new Vector3(10f, 10f, 10f), .3f));
                        m_tweenerD.Add(this.transform.DOLocalMove(new Vector3(0, 0, 8.8f), .3f)
                                  .OnComplete(() =>
                                  {
                                      m_curSnowBallSize = eSnowBallSize.One;

                                      SetSphereCollider(true);
                                      SetSnowBallSize(true, false);

                                      float a_increaseNum = Random.Range(.4f, 8f);
                                      m_snowBallIncreaseVec = new Vector3(a_increaseNum, a_increaseNum, a_increaseNum);

                                      m_tweenerD.Clear();
                                  }));

                        break;

                    case eSnowBallSize.Ninety:
                        m_tweenerD.Add(this.transform.DOScale(new Vector3(50f, 50f, 50f), .3f));
                        m_tweenerD.Add(this.transform.DOLocalMove(new Vector3(0, 0, 22.5f), .3f)
                                  .OnComplete(() =>
                                  {
                                      m_curSnowBallSize = eSnowBallSize.ThirtyFive;

                                      SetSphereCollider(true);
                                      SetSnowBallSize(true, false);

                                      float a_increaseNum = Random.Range(.7f, 1.1f);
                                      m_snowBallIncreaseVec = new Vector3(a_increaseNum, a_increaseNum, a_increaseNum);

                                      m_tweenerD.Clear();
                                  }));

                        break;

                    case eSnowBallSize.HundredFIf:
                        m_tweenerD.Add(this.transform.DOScale(new Vector3(100f, 100f, 100f), .3f));
                        m_tweenerD.Add(this.transform.DOLocalMove(new Vector3(0, 0, 32f), .3f)
                                  .OnComplete(() =>
                                  {
                                      m_curSnowBallSize = eSnowBallSize.Ninety;

                                      SetSphereCollider(true);
                                      SetSnowBallSize(true, false);

                                      float a_increaseNum = Random.Range(1.2f, 1.5f);
                                      m_snowBallIncreaseVec = new Vector3(a_increaseNum, a_increaseNum, a_increaseNum);

                                      m_tweenerD.Clear();
                                  }));
                        break;
                }
                #endregion
            }
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
            InGameManager.m_fastZoneTrail.m_isTrailOn = false;

            StopRotateSnowBall();
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
            StopRotateSnowBall();
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

        this.transform.localScale = new Vector3(1, 1, 1);
        this.transform.localPosition = new Vector3(0, 0, 8.8f);
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


    #region (GameState = Clear) 스노우볼 굴러가기
    /// <summary>
    /// Clear시 스노우볼 굴러가는 함수
    /// </summary>
    public void RollSnowBall()
    {
        
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
                this.transform.DOScale(this.transform.localScale + new Vector3(40, 40, 40), .1f)
                              .SetEase(Ease.Linear);
                this.transform.DOLocalMove(new Vector3(0, 0, this.transform.localPosition.z + 12.5f), .1f)
                              .SetEase(Ease.Linear)
                              .OnComplete(() =>
                              {
                                  SetSnowBallSize(true);
                              });
                break;

            case eSnowBallSize.Ninety:
                this.transform.DOScale(this.transform.localScale + new Vector3(96, 96, 96), .1f)
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
