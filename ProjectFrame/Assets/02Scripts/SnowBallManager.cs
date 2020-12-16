using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class SnowBallManager : MonoBehaviour
{
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
    public static bool m_snowIncrease = true;

    [SerializeField] float m_snowBallIncreaseCount = 0.005f;
    [SerializeField] float m_snowBallPoxZIncreaseCount = 0.0035f;
    [SerializeField] bool m_upgradeSnowBall = false;

    Sequence snowBallSeq;


    private void Start()
    {
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
                            m_snowBallIncreaseCount = 0.01f;
                            m_snowBallPoxZIncreaseCount = 0.04f;

                            if (this.transform.localScale.x >= 5)
                            {
                                m_upgradeSnowBall = true;
                                UpgradeSnowBall(eSnowBallSize.One);
                            }
                            break;

                        case eSnowBallSize.Five:
                            m_snowBallIncreaseCount = 0.005f;
                            m_snowBallPoxZIncreaseCount = 0.01f;

                            if (this.transform.localScale.x >= 10)
                            {
                                m_upgradeSnowBall = true;
                                UpgradeSnowBall(eSnowBallSize.Five);
                            }
                            break;

                        case eSnowBallSize.Ten:
                            m_snowBallIncreaseCount = 0.0025f;
                            m_snowBallPoxZIncreaseCount = 0.005f;

                            if (this.transform.localScale.x >= 15)
                            {
                                m_upgradeSnowBall = true;
                                UpgradeSnowBall(eSnowBallSize.Ten);
                            }
                            break;

                        case eSnowBallSize.Fifteen:
                            m_snowBallIncreaseCount = 0.001f;
                            m_snowBallPoxZIncreaseCount = 0.001f;

                            if (this.transform.localScale.x >= 20)
                            {
                                m_upgradeSnowBall = true;
                                UpgradeSnowBall(eSnowBallSize.Fifteen);
                            }
                            break;

                        case eSnowBallSize.Twenty:
                            m_snowBallIncreaseCount = 0.0005f;

                            break;
                    }


                    // === 눈덩이 크기 증가 === //
                    if (m_snowIncrease == true)
                    {
                        this.transform.localPosition = new Vector3(0, 0, this.transform.localPosition.z + m_snowBallPoxZIncreaseCount);
                        this.transform.localScale += new Vector3(m_snowBallIncreaseCount, m_snowBallIncreaseCount, m_snowBallIncreaseCount);
                    }
                    else
                    {
                        this.transform.localPosition = new Vector3(0, 0, this.transform.localPosition.z - m_snowBallPoxZIncreaseCount);
                        this.transform.localScale -= new Vector3(m_snowBallIncreaseCount, m_snowBallIncreaseCount, m_snowBallIncreaseCount);
                    }
                    // === 눈덩이 크기 증가 === //
                }


                break;

            case InGameManager.eGameStage.End:
                break;
        }
    }


    private void UpgradeSnowBall(eSnowBallSize a_snowBallSize)
    {
        switch(a_snowBallSize)
        {
            case eSnowBallSize.One:
                // === 카메라 줌아웃 및 팡!이펙트 === //
                snowBallSeq = DOTween.Sequence()
                                    .AppendInterval(1.5f)
                                    .AppendCallback(() =>
                                    {
                                        CameraManager.uniqueInstance.SetFollowOffset();
                                    })
                                    .OnComplete(() =>
                                    {
                                        m_upgradeSnowBall = false;

                                        PlayerManager.uniqueInstance.m_moveSpeed = 70;
                                        PlayerManager.uniqueInstance.SetPushAnimSpeedUp();

                                        m_curSnowBallSize = eSnowBallSize.Five;
                                    });
                // === 카메라 줌아웃 및 팡!이펙트 === //
                break;

            case eSnowBallSize.Five:
                snowBallSeq = DOTween.Sequence()
                                    .AppendInterval(1.5f)
                                    .AppendCallback(() =>
                                    {
                                        CameraManager.uniqueInstance.SetFollowOffset();
                                    })
                                    .OnComplete(() =>
                                    {
                                        m_upgradeSnowBall = false;

                                        PlayerManager.uniqueInstance.m_moveSpeed = 90;
                                        PlayerManager.uniqueInstance.SetPushAnimSpeedUp();

                                        m_curSnowBallSize = eSnowBallSize.Ten;
                                    });
                break;

            case eSnowBallSize.Ten:
                snowBallSeq = DOTween.Sequence()
                                    .AppendInterval(1.5f)
                                    .AppendCallback(() =>
                                    {
                                        CameraManager.uniqueInstance.SetFollowOffset();
                                    })
                                    .OnComplete(() =>
                                    {
                                        m_upgradeSnowBall = false;

                                        PlayerManager.uniqueInstance.m_moveSpeed = 120;
                                        PlayerManager.uniqueInstance.SetPushAnimSpeedUp();

                                        m_curSnowBallSize = eSnowBallSize.Fifteen;
                                    });
                break;

            case eSnowBallSize.Fifteen:
                snowBallSeq = DOTween.Sequence()
                                    .AppendInterval(1.5f)
                                    .AppendCallback(() =>
                                    {
                                        CameraManager.uniqueInstance.SetFollowOffset();
                                    })
                                    .OnComplete(() =>
                                    {
                                        m_upgradeSnowBall = false;

                                        PlayerManager.uniqueInstance.m_moveSpeed = 150;
                                        PlayerManager.uniqueInstance.SetPushAnimSpeedUp();

                                        m_curSnowBallSize = eSnowBallSize.Twenty;
                                    });
                break;

            case eSnowBallSize.Twenty:

                break;
        }
    }
}
