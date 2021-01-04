using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class SnowBallTrigger : MonoBehaviour
{
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
    

    private Vector3 BounceVec()
    {
        Vector3 a_bounceVec = Vector3.zero;
        float a_x = Random.Range(-100, 100);
        float a_z = Random.Range(-100, 100);

        a_bounceVec = new Vector3(a_x, 0, a_z);

        return a_bounceVec;
    }


    private Vector3 a_myBounce = Vector3.zero;
    private Vector3 a_otherBounce = Vector3.zero;
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("SnowBall"))
        {
            // === SnowBall 크기 비교 === //
            if (m_snowBallTf.transform.localScale.x < other.transform.parent.localScale.x) 
            {
                // === 튕겨나가기 === //
                m_manager.PlayCrashEffect(true);
                other.GetComponent<SnowBallTrigger>().m_manager.PlayCrashEffect(true);

                a_myBounce = BounceVec();
                a_otherBounce = BounceVec();

                m_manager.transform.DOMove(m_manager.transform.position + a_myBounce, .5f);
                other.GetComponent<SnowBallTrigger>().m_manager.transform.DOMove(other.GetComponent<SnowBallTrigger>().m_manager.transform.position + a_otherBounce, .5f);            
                // === 튕겨나가기 === //
            }
            // === SnowBall 크기 비교 === //
        }
        else if(other.gameObject.layer == LayerMask.NameToLayer("SnowMan"))
        {
            // === 눈사람 먹으면 눈덩이 커짐 === //
            other.gameObject.SetActive(false);
            m_snowBallMgr.ResetTweener();
            m_snowBallMgr.GainSnowMan();
            // === 눈사람 먹으면 눈덩이 커짐 === //
        }
    }
}
