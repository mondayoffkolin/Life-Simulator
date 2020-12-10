using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class InGameManager : MonoBehaviour
{
    public static InGameManager uniqueInstance;

    [Header("UI 버튼 관련")]
    [SerializeField] Button m_leftButton = null;
    [SerializeField] Button m_rightButton = null;

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
    }

}
