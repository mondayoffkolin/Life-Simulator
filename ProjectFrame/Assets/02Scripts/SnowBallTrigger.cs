using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnowBallTrigger : MonoBehaviour
{
    [SerializeField] Transform m_snowBallTf = null;
    [SerializeField] PlayerManager m_manager = null;
    [SerializeField] SnowBallManager m_snowBallMgr = null;

    private void Start()
    {
        m_snowBallTf = this.transform.parent.transform;
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("SnowBall") ||
            other.gameObject.layer == LayerMask.NameToLayer("SnowBall_E"))
        {
            print(m_snowBallTf.transform.localScale.x);
            print(other.gameObject.transform.localScale.x);

            // === 스노우볼 크기 비교 === //
            if (m_snowBallTf.transform.localScale.x < other.gameObject.transform.localScale.x)
            {// 상대 SnowBall에 흡수
                m_manager.SetWhenInTheSnowBall();
                m_snowBallMgr.ResetTweener();                                   // 내 트위너 최기화
                other.GetComponent<SnowBallManager>().CharacterInSnowBall();    // 상대 눈덩이에 박힌 캐릭터 On
            }
            else
            {// 내 SnowBall에 흡
                m_manager.SetWhenInTheSnowBall();
                other.GetComponent<SnowBallManager>().ResetTweener();           // 상대 트위너 초기화
                m_snowBallMgr.CharacterInSnowBall();                            // 내 눈덩이에 박힌 캐릭터 On
            }
        }
    }
}
