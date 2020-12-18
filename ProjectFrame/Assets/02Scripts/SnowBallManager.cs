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


    [Header("눈덩이 오브젝트")]
    [SerializeField] GameObject m_snowBallObj = null;                    // 눈덩이 오브젝트
    public Vector3 m_snowBallIncreaseVec = new Vector3(.5f, .5f, .5f);  // 눈덩이 크지는 크기 벡터

    [Header("장애물과 눈덩이 관련")]
    [SerializeField] Transform[] m_obstaclePosTf = null;
    [SerializeField] Transform m_obstacleParentTf = null;                         // 장애물이 눈덩이에 속할 부모Tf


    Sequence snowBallIncreaseSeq;           // 스노우볼 커지는 DoTween
    Sequence snowBallDecreaseSeq;           // 스노우볼 작아지는 DoTween


    private void Start()
    {
        m_curSnowBallSize = eSnowBallSize.One;
    }


    #region (GameState = Play) 스노우볼 크기 관련
    /// <summary>
    /// (GameState = Play)스노우볼 크기 증가/감소 함수
    /// </summary>
    /// <param name="m_isUp"></param>
    public void SetSnowBallSize(bool m_isUp)
    {
        if(m_isUp == true)
        {
            #region 눈덩이 크기 증가 & 카메라 줌아웃
            snowBallIncreaseSeq = DOTween.Sequence()
                                    .Append(this.transform.DOScale(this.transform.localScale + m_snowBallIncreaseVec, .1f).SetEase(Ease.Linear))
                                    .Join(this.transform.DOLocalMove(new Vector3(0, 0, this.transform.localPosition.z + .17f), .1f).SetEase(Ease.Linear))
                                    .AppendCallback(() =>
                                    {
                                        switch (m_curSnowBallSize)
                                        {
                                            case eSnowBallSize.One:
                                                if (this.transform.localScale.x >= 35)
                                                {
                                                    InGameManager.m_camMgr.SetFollowOffset(true);
                                                    m_snowBallIncreaseVec = new Vector3(1f, 1f, 1f);
                                                    m_curSnowBallSize = eSnowBallSize.ThirtyFive;
                                                }
                                                break;

                                            case eSnowBallSize.ThirtyFive:
                                                if (this.transform.localScale.x >= 90)
                                                {
                                                    InGameManager.m_camMgr.SetFollowOffset(true);
                                                    m_snowBallIncreaseVec = new Vector3(1.4f, 1.4f, 1.4f);
                                                    m_curSnowBallSize = eSnowBallSize.Ninety;
                                                }
                                                break;

                                            case eSnowBallSize.Ninety:
                                                if (this.transform.localScale.x >= 150)
                                                {
                                                    InGameManager.m_camMgr.SetFollowOffset(true);
                                                    m_snowBallIncreaseVec = new Vector3(2f, 2f, 2f);
                                                    m_curSnowBallSize = eSnowBallSize.HundredFIf;
                                                }
                                                break;
                                        }
                                    })
                                    .SetLoops(-1, LoopType.Incremental);
            #endregion
        }
        else
        {
            #region 눈덩이 크기 감소 & 카메라 줌인
            switch (m_curSnowBallSize)
            {
                case eSnowBallSize.One:
                    InGameManager.m_snowGround.StopGroundMoving();
                    InGameManager.m_plyMgr.SetAnim_GameOver();          
                    break;

                case eSnowBallSize.ThirtyFive:
                    InGameManager.m_camMgr.SetFollowOffset(false);

                    snowBallDecreaseSeq = DOTween.Sequence()
                                                 .Append(this.transform.DOScale(new Vector3(10f, 10f, 10f), .3f))
                                                 .Join(this.transform.DOLocalMove(new Vector3(0, 0, 8.8f), .3f))
                                                 .OnComplete(() =>
                                                 {
                                                     m_curSnowBallSize = eSnowBallSize.One;

                                                     SetSnowBallSize(true);

                                                     m_snowBallIncreaseVec = new Vector3(.5f, .5f, .5f);
                                                 });

                    break;

                case eSnowBallSize.Ninety:
                    InGameManager.m_camMgr.SetFollowOffset(false);

                    snowBallDecreaseSeq = DOTween.Sequence()
                                                 .Append(this.transform.DOScale(new Vector3(50f, 50f, 50f), .3f))
                                                 .Join(this.transform.DOLocalMove(new Vector3(0, 0, 22.5f), .3f))
                                                 .OnComplete(() =>
                                                 {
                                                     m_curSnowBallSize = eSnowBallSize.ThirtyFive;

                                                     SetSnowBallSize(true);

                                                     m_snowBallIncreaseVec = new Vector3(1f, 1f, 1f);
                                                 });

                    break;

                case eSnowBallSize.HundredFIf:
                    InGameManager.m_camMgr.SetFollowOffset(false);

                    snowBallDecreaseSeq = DOTween.Sequence()
                                                 .Append(this.transform.DOScale(new Vector3(100f, 100f, 100f), .3f))
                                                 .Join(this.transform.DOLocalMove(new Vector3(0, 0, 32f), .3f))
                                                 .OnComplete(() =>
                                                 {
                                                     m_curSnowBallSize = eSnowBallSize.Ninety;

                                                     SetSnowBallSize(true);

                                                     m_snowBallIncreaseVec = new Vector3(1.4f, 1.4f, 1.4f);
                                                 }); 

                    break;
            }
            #endregion
        }
    }

    /// <summary>
    /// (GameState = Play)스노우볼 (장애물에 의해/AI 흡수) 작아지는 순간 호출되는 함수
    /// </summary>
    public void StopIncreaseSnowBall(bool a_isClear = false)
    {
        snowBallIncreaseSeq.Kill();

        if(a_isClear == false)
            SetSnowBallSize(false);
    }

    /// <summary>
    /// (GameState = Play) 장애물이 눈속에 달라붙는 함숭
    /// </summary>
    public void AttachToSnowBall(GameObject a_obstacle)
    {
        int a_rnd = Random.Range(0, m_obstaclePosTf.Length);
        a_obstacle.transform.SetParent(m_obstacleParentTf.transform);
        a_obstacle.transform.position = m_obstaclePosTf[a_rnd].position;
        a_obstacle.transform.rotation = m_obstaclePosTf[a_rnd].rotation;
        a_obstacle.transform.localScale = m_obstaclePosTf[a_rnd].localScale;
    }
    #endregion



    #region 눈덩이 굴러가는 연출
    /// <summary>
    /// (GameState = Play)눈덩이 굴러가는 DoTween함수
    /// </summary>
    public void RotateSnowBall()
    {
        m_snowBallObj.transform.DORotate(Vector3.right * 100, .2f).SetLoops(-1, LoopType.Incremental);
    }

    /// <summary>
    /// (GameState = End)눈덩이 굴러가는거 멈추는 DoTween함수 & 눈덩이에 박힌 장애물들 폭발
    /// </summary>
    public void StopRotateSnowBall()
    {
        m_snowBallObj.transform.DOKill();

        ObstacleManager[] a_snowBall = new ObstacleManager[m_obstacleParentTf.childCount];
        for (int n = 0; n < m_obstacleParentTf.childCount; n++)
        {
            a_snowBall[n] = m_obstacleParentTf.GetChild(n).GetComponent<ObstacleManager>();
            a_snowBall[n].AddExplosion();
        }
    }
    #endregion


    #region (GameState = Clear) 스노우볼 굴러가기
    /// <summary>
    /// Clear시 스노우볼 굴러가는 함b
    /// </summary>
    public void RollSnowBall()
    {
        
    }
    #endregion

}
