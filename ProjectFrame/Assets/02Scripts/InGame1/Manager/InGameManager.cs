using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class InGameManager : MonoBehaviour
{
    public static InGameManager uniqueInstance;

    [Header("InGameScene UI 버튼 관련")]
    [SerializeField] Button m_leftButton = null;
    [SerializeField] Button m_rightButton = null;

    [Header("InGameScene3 UI d버튼 관련")]
    [SerializeField] Button m_startElevatorButton = null;
    [SerializeField] Button m_stopElevatorButton = null;
    [SerializeField] Button m_closeElevatorButton = null;
    [SerializeField] DoorProduction[] m_doorProduction = null;

    [SerializeField] TextMeshProUGUI[] m_floorNumText = null;

    [SerializeField] Light m_light = null;

    private static int m_floorNum = 1;

    Sequence ctrlElvSeq;
    Sequence ctrlLightSeq;

    // Start is called before the first frame update
    void Start()
    {
        uniqueInstance = this;

        #region 버튼 관련
        if(m_leftButton != null)
        {
            m_leftButton.onClick.AddListener(() =>
            {
                PlayerManager.uniqueInstance.Turning(true);
            }); 
        }

        if(m_rightButton != null)
        {
            m_rightButton.onClick.AddListener(() =>
            {
                PlayerManager.uniqueInstance.Turning();
            });
        }
        #endregion


        if(m_startElevatorButton != null)
        {
            m_startElevatorButton.onClick.AddListener(() =>
            {
                if (DoorProduction.m_isOpen == false)
                {
                    m_startElevatorButton.enabled = false;
                    m_stopElevatorButton.enabled = true;

                    InvokeRepeating("FloorUp", 2f, 1.8f);
                }
            });
        }
        if(m_stopElevatorButton != null)
        {
            m_stopElevatorButton.onClick.AddListener(() =>
            {
                CameraManager.uniqueInstance.StopShaking();
                ctrlLightSeq.Kill();

                ctrlElvSeq = DOTween.Sequence()
                            .OnStart(() =>
                            {
                                m_stopElevatorButton.enabled = false;

                                CancelInvoke("FloorUp");
                            })
                            .AppendInterval(2f)
                            .OnComplete(() =>
                            {
                                m_startElevatorButton.enabled = true;

                                m_doorProduction[0].DoorOpen();
                                m_doorProduction[1].DoorOpen();
                            });
            });
        }
        if (m_closeElevatorButton != null)
        {
            m_closeElevatorButton.onClick.AddListener(() =>
            {
                m_doorProduction[0].DoorClose();
                m_doorProduction[1].DoorClose();
            });
        }
    }

    private void FloorUp()
    {
        m_floorNum++;
        m_floorNumText[0].text = m_floorNum.ToString();
        m_floorNumText[1].text = m_floorNum.ToString();

        CameraManager.uniqueInstance.CamShaking();


        ctrlLightSeq = DOTween.Sequence()
                              .Append(m_light.transform.DORotate(new Vector3(270, 180, 0), 1f))
                              .Append(m_light.transform.DORotate(new Vector3(50, 180, 0), 1f))
                              .SetLoops(-1);
    }

}
