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

        None
    }


    [Header("현재 눈덩이 크기레벨")]
    public eSnowBallSize m_curSnowBallSize;
    [SerializeField] bool m_isPlayer = false;


    [Header("눈덩이 오브젝트")]
    [SerializeField] GameObject m_snowBallObj = null;                    // 눈덩이 오브젝트
    public Vector3 m_snowBallIncreaseVec = new Vector3(.8f, .8f, .8f);  // 눈덩이 크지는 크기 벡터
    public Vector3 m_originSnowBallIncreaseVec = Vector3.zero;
    public Transform m_parentTf = null;


    [Header("장애물과 눈덩이 관련")]
    [SerializeField] Transform[] m_obstaclePosTf = null;
    [SerializeField] Transform m_obstacleParentTf = null;                         // 장애물이 눈덩이에 속할 부모Tf
    [SerializeField] CharacterInSnowBall[] m_characterObj = null;


    List<Tweener> m_tweener;
    List<Tweener> m_tweenerD;


    private void Start()
    {
        m_curSnowBallSize = eSnowBallSize.One;


        m_originSnowBallIncreaseVec = m_snowBallIncreaseVec;


        m_tweener = new List<Tweener>();
        m_tweenerD = new List<Tweener>();
    }


    private void Update()
    {
        if(InGameManager.uniqueInstance.m_curGameState == InGameManager.eGameState.Play)
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
            }

            m_snowBallObj.transform.Rotate(Vector3.right * 200 * Time.deltaTime, Space.World);
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
            if(m_isPlayer == true)
            {
                #region 눈덩이 크기 증가 & 카메라 줌아웃
                m_tweener.Add(this.transform.DOScale(this.transform.localScale + m_snowBallIncreaseVec, .1f)
                        .SetEase(Ease.Linear)
                        .SetLoops(-1, LoopType.Incremental));
                m_tweener.Add(this.transform.DOLocalMove(new Vector3(0, 0, this.transform.localPosition.z + .25f), .1f)
                        .SetEase(Ease.Linear)
                        .SetLoops(-1, LoopType.Incremental));
                //m_tweener.Add(m_snowBallObj.transform.DOLocalRotateQuaternion(Quaternion.Euler(350, 0, 0), 2f)
                //        .SetEase(Ease.Linear)
                //        .SetLoops(-1));
                #endregion
            }
            else
            {
                #region 눈덩이 크기 증가
                m_tweener.Add(this.transform.DOScale(this.transform.localScale + m_snowBallIncreaseVec, .1f)
                       .SetEase(Ease.Linear)
                       .SetLoops(-1, LoopType.Incremental));
                m_tweener.Add(this.transform.DOLocalMove(new Vector3(0, 0, this.transform.localPosition.z + .25f), .1f)
                       .SetEase(Ease.Linear)
                       .SetLoops(-1, LoopType.Incremental));
                //m_tweener.Add(m_snowBallObj.transform.DORotate(Vector3.right * 50, .2f)
                //       .SetEase(Ease.Linear)
                //       .SetLoops(-1, LoopType.Incremental));
                #endregion
            }
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

                                      SetSnowBallSize(true, false);

                                      m_snowBallIncreaseVec = new Vector3(.8f, .8f, .8f);

                                      m_tweenerD.Clear();
                                  }));

                        break;

                    case eSnowBallSize.Ninety:
                        m_tweenerD.Add(this.transform.DOScale(new Vector3(50f, 50f, 50f), .3f));
                        m_tweenerD.Add(this.transform.DOLocalMove(new Vector3(0, 0, 22.5f), .3f)
                                  .OnComplete(() =>
                                  {
                                      m_curSnowBallSize = eSnowBallSize.ThirtyFive;

                                      SetSnowBallSize(true, false);

                                      m_snowBallIncreaseVec = new Vector3(1f, 1f, 1f);

                                      m_tweenerD.Clear();
                                  }));

                        break;

                    case eSnowBallSize.HundredFIf:
                        m_tweenerD.Add(this.transform.DOScale(new Vector3(100f, 100f, 100f), .3f));
                        m_tweenerD.Add(this.transform.DOLocalMove(new Vector3(0, 0, 32f), .3f)
                                  .OnComplete(() =>
                                  {
                                      m_curSnowBallSize = eSnowBallSize.Ninety;

                                      SetSnowBallSize(true, false);

                                      m_snowBallIncreaseVec = new Vector3(1.4f, 1.4f, 1.4f);

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
        ResetTweener();


        if (a_isOver == false)
        {
            SetSnowBallSize(false);
        }
        else
        {
            InGameManager.m_fastZoneTrail.m_isTrailOn = false;

            a_obj.SetAnim_GameOver();
            StopRotateSnowBall();
        }
    }
    public void StopIncreaseAiSnowBall(AIManager a_obj, bool a_isOver = false)
    {
        ResetTweener();


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
        //m_snowBallObj.transform.DOKill();


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


    #region 눈덩이에 뭍히는 연출
    public void CharacterInSnowBall()
    {
        while(true)
        {
            int a_charNum = Random.Range(0, m_characterObj.Length);
            if(m_characterObj[a_charNum].gameObject.activeSelf == false)
            {
                m_characterObj[a_charNum].gameObject.SetActive(true);
                break;
            }
        }
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


    //private void OnTriggerEnter(Collider other)
    //{
    //    if (other.gameObject.layer == LayerMask.NameToLayer("SnowBall") ||
    //        other.gameObject.layer == LayerMask.NameToLayer("SnowBall_E"))
    //    {
    //        // === 스노우볼 크기 비교 === //
    //        if (m_snowBallObj.transform.localScale.x < other.gameObject.transform.localScale.x)
    //        {// 상대 SnowBall에 흡수
    //            m_manager.SetWhenInTheSnowBall();
    //            ResetTweener();                                                 // 내 트위너 최기화
    //            other.GetComponent<SnowBallManager>().CharacterInSnowBall();    // 상대 눈덩이에 박힌 캐릭터 On
    //        }
    //        else
    //        {// 내 SnowBall에 흡
    //            m_manager.SetWhenInTheSnowBall();
    //            other.GetComponent<SnowBallManager>().ResetTweener();           // 상대 트위너 초기화
    //            CharacterInSnowBall();                                          // 내 눈덩이에 박힌 캐릭터 On
    //        }
    //    }
    //}
}
