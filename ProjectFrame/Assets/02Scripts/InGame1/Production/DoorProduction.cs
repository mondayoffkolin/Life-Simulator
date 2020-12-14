using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class DoorProduction : MonoBehaviour
{
    [Header("문 연출 관련")]
    public static bool m_isOpen = false;
    [SerializeField] Transform m_openTf = null;
    [SerializeField] Transform m_closeTf = null;

    Sequence doorSeq;

    // Start is called before the first frame update
    void Start()
    {

    }


    public void DoorOpen()
    {
        if(m_isOpen == false)
        {
            doorSeq = DOTween.Sequence()
                            .Append(this.transform.DOMove(m_openTf.position, 2f))
                            .OnComplete(() =>
                            {
                                m_isOpen = true;
                            });
        }
    }

    public void DoorClose()
    {
        if (m_isOpen == true)
        {
            doorSeq = DOTween.Sequence()
                            .Append(this.transform.DOMove(m_closeTf.position, 2f))
                            .OnComplete(() =>
                            {
                                m_isOpen = false;
                            });
        }
    }
}
