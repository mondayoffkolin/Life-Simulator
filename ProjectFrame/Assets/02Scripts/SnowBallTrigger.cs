using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class SnowBallTrigger : MonoBehaviour
{
    [SerializeField] SnowBallManager.eSnowBallSize m_eSnowBallSize = SnowBallManager.eSnowBallSize.None;
    public Transform m_snowBallTf = null;                            // 내 SnowBall
    [SerializeField] PlayerManager m_manager = null;                 // 내 SnowBall 의 플레이어
    [SerializeField] SnowBallManager m_snowBallMgr = null;           // 내 SnowBallManager 스크립트


    private SphereCollider m_snowBallCollider = null;


    private void Start()
    {
        m_snowBallCollider = this.GetComponent<SphereCollider>();


        if(m_snowBallTf == null)
            m_snowBallTf = this.transform.parent.transform;


        if (m_manager == null)
            this.transform.parent.GetComponent<PlayerManager>();
    }
    

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("SnowBall") ||
            other.gameObject.layer == LayerMask.NameToLayer("SnowBall_E"))
        {
            // === SnowBall 크기 비교 === //
            if (m_snowBallTf.transform.localScale.x < other.transform.parent.localScale.x) 
            {// 상대 SnowBall에 흡수
                m_manager.m_playerState = ePlayerState.Death;


                m_snowBallCollider.enabled = false;


                m_manager.SetWhenInTheSnowBall();                                                              // 상대 SnowBall 안으로 흡수
                m_snowBallMgr.ResetTweener();                                                                  // 내 트위너 초기화


                m_eSnowBallSize = m_snowBallMgr.m_curSnowBallSize;                                             // 내 SnowBall 사이즈 상태
                other.transform.parent.GetComponent<SnowBallManager>().ResetTweener();                         // 상대 트위너 초기화
                other.transform.parent.GetComponent<SnowBallManager>().CharacterInSnowBall(m_eSnowBallSize);   // 상대 SnowBall 박힌 캐릭터 On & 눈덩이 크기 증가
            }
            // === SnowBall 크기 비교 === //
        }
    }
}
