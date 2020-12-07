using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager uniqueInstance;

    [Header("캐릭터 회전 DOTween Ease")]
    [SerializeField] Ease m_rotateEase;

    Animator m_animCtrl;
    Sequence playerSeq;

    private void Awake()
    {
        m_animCtrl = GetComponent<Animator>();
    }

    private void Start()
    {
        uniqueInstance = this;
    }


    #region (Left, Right)버튼 누를시 호출될 함수
    public void Turning(bool a_isLeft = false)
    {
        float a_y = this.transform.eulerAngles.y;

        if(a_isLeft == true)
        {// -90 방향
            playerSeq = DOTween.Sequence()
                               .OnStart(() =>
                               {
                                   m_animCtrl.SetTrigger("Crawling");
                               })
                               .Append(this.transform.DORotate(new Vector3(0, a_y - 90, 0), 1.5f).SetEase(m_rotateEase))
                               .OnComplete(() =>
                               {
                                   m_animCtrl.SetTrigger("Idle");

                                   this.transform.eulerAngles = new Vector3(0, a_y - 90, 0);
                               });
        }
        else
        {// +90 방향
            playerSeq = DOTween.Sequence()
                               .OnStart(() =>
                               {
                                   m_animCtrl.SetTrigger("Crawling");
                               })
                               .Append(this.transform.DORotate(new Vector3(0, a_y + 90, 0), 1.5f).SetEase(m_rotateEase))
                               .OnComplete(() =>
                               {
                                   m_animCtrl.SetTrigger("Idle");

                                   this.transform.eulerAngles = new Vector3(0, a_y + 90, 0);

                               });
        }
    }

    #endregion
}
