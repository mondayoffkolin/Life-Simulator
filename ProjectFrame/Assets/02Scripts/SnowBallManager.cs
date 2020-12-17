using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class SnowBallManager : MonoBehaviour
{
    public static SnowBallManager uniqueInstance;

    public enum eSnowBallSize
    {
        One,
        Five,
        Ten,
        Fifteen,
        Twenty,
        TwentyFive,
        Thirty,
        ThirtyFive,

        None
    }

    public eSnowBallSize m_curSnowBallSize;
    //public static bool m_snowIncrease = true;

    [SerializeField] float m_snowBallIncreaseCount = 0.005f;        // 스노우볼 스케일 증가량
    [SerializeField] float m_snowBallPoxZIncreaseCount = 0.0035f;   // 눈덩이가 커짐에따른 눈덩이Z값위치
    public static bool m_upgradeSnowBall = false;                // 눈덩이 크기 업!여부

    [SerializeField] GameObject m_snowBallObj = null;               // 눈덩이 오브젝트
    public static float m_snowBallRotateSpeed = 150f;                // 눈덩이 회전

    Sequence snowBallSeq;


    private void Start()
    {
        uniqueInstance = this;

        m_curSnowBallSize = eSnowBallSize.One;
    }

    // Update is called once per frame
    void Update()
    {
        switch (InGameManager.m_curGameState)
        {
            case InGameManager.eGameStage.Play:

                if(m_upgradeSnowBall == false)
                {
                    switch(m_curSnowBallSize)
                    {
                        case eSnowBallSize.One:
                            m_snowBallIncreaseCount = 0.04f;
                            m_snowBallPoxZIncreaseCount = 0.15f;

                            if (this.transform.localScale.x >= 5)
                            {
                                //m_upgradeSnowBall = true;
                                UpgradeSnowBall(eSnowBallSize.One);
                            }
                            break;

                         case eSnowBallSize.Five:
                             m_snowBallIncreaseCount = 0.03f;
                             m_snowBallPoxZIncreaseCount = 0.1f;

                             if (this.transform.localScale.x >= 10)
                             {
                                 //m_upgradeSnowBall = true;
                                 UpgradeSnowBall(eSnowBallSize.Five);
                             }
                             break;

                         case eSnowBallSize.Ten:
                            m_snowBallIncreaseCount = 0.02f;
                            m_snowBallPoxZIncreaseCount = 0.05f;

                            if (this.transform.localScale.x >= 15)
                            {
                                //m_upgradeSnowBall = true;
                                UpgradeSnowBall(eSnowBallSize.Ten);
                            }
                            break;

                         case eSnowBallSize.Fifteen:
                             m_snowBallIncreaseCount = 0.01f;
                             m_snowBallPoxZIncreaseCount = 0.025f;
                         
                             if (this.transform.localScale.x >= 20)
                             {
                                 //m_upgradeSnowBall = true;
                                 UpgradeSnowBall(eSnowBallSize.Fifteen);
                             }
                             break;
                         
                         case eSnowBallSize.Twenty:
                             m_snowBallIncreaseCount = 0.005f;
                             m_snowBallPoxZIncreaseCount = 0.015f;
                         
                         
                             if (this.transform.localScale.x >= 25)
                             {
                                 m_upgradeSnowBall = true;
                                 UpgradeSnowBall(eSnowBallSize.Twenty);
                                }
                                break;
                            
                         case eSnowBallSize.TwentyFive:
                             m_snowBallIncreaseCount = 0.0025f;
                             m_snowBallPoxZIncreaseCount = 0.01f;


                             if (this.transform.localScale.x >= 30)
                             {
                                 m_upgradeSnowBall = true;
                                 UpgradeSnowBall(eSnowBallSize.TwentyFive);
                         }
                         break;

                        case eSnowBallSize.Thirty:
                            m_snowBallIncreaseCount = 0.00015f;
                            m_snowBallPoxZIncreaseCount = 0.005f;


                               if (this.transform.localScale.x >= 35)
                               {
                                   m_upgradeSnowBall = true;
                                   UpgradeSnowBall(eSnowBallSize.Thirty);
                               }
                               break;

                        case eSnowBallSize.ThirtyFive:
                            m_snowBallIncreaseCount = 0.0001f;
                            m_snowBallPoxZIncreaseCount = 0.001f;
                            break;
                    }


                    // === 눈덩이 크기 증가 === //
                    this.transform.localPosition = new Vector3(0, 0, this.transform.localPosition.z + m_snowBallPoxZIncreaseCount);
                    this.transform.localScale += new Vector3(m_snowBallIncreaseCount, m_snowBallIncreaseCount, m_snowBallIncreaseCount);
                    // === 눈덩이 크기 증가 === //
                }
                break;

            case InGameManager.eGameStage.End:
                break;
        }
    }

    /// <summary>
    /// 5배수로 눈덩이 커질때마다 호출되는 함수
    /// </summary>
    /// <param name="a_snowBallSize"></param>
    private void UpgradeSnowBall(eSnowBallSize a_snowBallSize)
    {
        switch(a_snowBallSize)
        {
            case eSnowBallSize.One:
                // === 카메라 줌아웃 및 팡!이펙트 === //
                CameraManager.uniqueInstance.SetFollowOffset(true);

                PlayerManager.uniqueInstance.m_moveSpeed += 20;
                PlayerManager.uniqueInstance.SetPushAnimSpeedUp();

                m_curSnowBallSize = eSnowBallSize.Five;
                // === 카메라 줌아웃 및 팡!이펙트 === //
                break;

            case eSnowBallSize.Five:
                CameraManager.uniqueInstance.SetFollowOffset();

                PlayerManager.uniqueInstance.m_moveSpeed += 20;
                PlayerManager.uniqueInstance.SetPushAnimSpeedUp();

                m_curSnowBallSize = eSnowBallSize.Ten;
                break;

            case eSnowBallSize.Ten:
                CameraManager.uniqueInstance.SetFollowOffset();

                PlayerManager.uniqueInstance.m_moveSpeed += 30;
                PlayerManager.uniqueInstance.SetPushAnimSpeedUp();

                m_curSnowBallSize = eSnowBallSize.Fifteen;
                break;

            case eSnowBallSize.Fifteen:
                CameraManager.uniqueInstance.SetFollowOffset();

                PlayerManager.uniqueInstance.m_moveSpeed += 40;
                PlayerManager.uniqueInstance.SetPushAnimSpeedUp();

                m_upgradeSnowBall = false;
                m_curSnowBallSize = eSnowBallSize.Twenty;
                break;

            case eSnowBallSize.Twenty:
                CameraManager.uniqueInstance.SetFollowOffset();

                PlayerManager.uniqueInstance.m_moveSpeed += 50;
                PlayerManager.uniqueInstance.SetPushAnimSpeedUp();

                m_curSnowBallSize = eSnowBallSize.TwentyFive;               
                break;

            case eSnowBallSize.TwentyFive:
                CameraManager.uniqueInstance.SetFollowOffset();

                PlayerManager.uniqueInstance.m_moveSpeed += 60;
                PlayerManager.uniqueInstance.SetPushAnimSpeedUp();

                m_curSnowBallSize = eSnowBallSize.Thirty;
                break;

            case eSnowBallSize.Thirty:
                CameraManager.uniqueInstance.SetFollowOffset();

                PlayerManager.uniqueInstance.m_moveSpeed += 70;
                PlayerManager.uniqueInstance.SetPushAnimSpeedUp();

                m_curSnowBallSize = eSnowBallSize.ThirtyFive;
                break;
        }
    }
    public void DegradeSnowBall(eSnowBallSize a_snowBallSize)
    {
        switch (a_snowBallSize)
        {
            case eSnowBallSize.One:
                PlayerManager.uniqueInstance.m_moveSpeed = 100;
                break;

            case eSnowBallSize.Five:
                PlayerManager.uniqueInstance.m_moveSpeed = 120;
                break;

            case eSnowBallSize.Ten:
                PlayerManager.uniqueInstance.m_moveSpeed = 140;
                break;

            case eSnowBallSize.Fifteen:
                PlayerManager.uniqueInstance.m_moveSpeed = 170;
                break;

            case eSnowBallSize.Twenty:
                PlayerManager.uniqueInstance.m_moveSpeed = 210;
                break;

            case eSnowBallSize.TwentyFive:
                PlayerManager.uniqueInstance.m_moveSpeed = 260;
                break;

            case eSnowBallSize.Thirty:
                PlayerManager.uniqueInstance.m_moveSpeed = 320;
                break;
        }
    }

    public void RotateSnowBall()
    {
        m_snowBallObj.transform.DORotate(Vector3.right * 100, .2f).SetLoops(-1, LoopType.Incremental);
    }
}
