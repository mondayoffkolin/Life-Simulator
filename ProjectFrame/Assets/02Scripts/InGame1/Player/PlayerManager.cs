using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager uniqueInstance;

    [Header("캐릭터 회전 DOTween Ease")]
    [SerializeField] Ease m_rotateEase;

    [Header("플레이어가 바라볼 방향")]
    [SerializeField] Transform[] m_playerLookAt = null;
    [SerializeField] private int m_looAtCnt = 0;
    [SerializeField] private bool m_playerRotateEnd = false;

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
        playerSeq.Kill();

        float a_y = this.transform.eulerAngles.y;


        if(a_isLeft == true)
        {// 왼쪽 회전
            m_looAtCnt += 1;
            if (m_looAtCnt >= m_playerLookAt.Length)
                m_looAtCnt = 0;

            playerSeq = DOTween.Sequence()
                               .OnStart(() =>
                               {
                                   m_playerRotateEnd = false;
                                   m_animCtrl.SetTrigger("Crawling");
                               })
                               .Append(this.transform.DOLookAt(m_playerLookAt[m_looAtCnt].position, 1.5f).SetEase(m_rotateEase))
                               .OnComplete(() =>
                               {
                                   m_playerRotateEnd = true;
                                   m_animCtrl.SetTrigger("Idle");
                               });
        }
        else
        {// 오른쪽 회전
            m_looAtCnt -= 1;
            if (m_looAtCnt < 0)
                m_looAtCnt = m_playerLookAt.Length - 1;

            playerSeq = DOTween.Sequence()
                               .OnStart(() =>
                               {
                                   m_playerRotateEnd = false;
                                   m_animCtrl.SetTrigger("Crawling");
                               })
                               .Append(this.transform.DOLookAt(m_playerLookAt[m_looAtCnt].position, 1.5f).SetEase(m_rotateEase))
                               .OnComplete(() =>
                               {
                                   m_playerRotateEnd = true;
                                   m_animCtrl.SetTrigger("Idle");
                               });
        }
    }

    #endregion
}
