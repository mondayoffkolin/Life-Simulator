using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnowBallPooingManager : MonoBehaviour
{
    public static SnowBallPooingManager uniqueInstance;

    [SerializeField] GameObject[] m_snowBall = null;            // 스노우볼 풀링 오브젝트
    [SerializeField] Transform m_snowBallRootPos = null;        // 스노우볼 초기 위치
    [SerializeField] Transform m_originSnowBallTf = null;       // 플레이어한테 붙어있는 스노우a_snowBallPos

    Queue<GameObject> m_snowBallQueue = new Queue<GameObject>();

    private void Start()
    {
        uniqueInstance = this;

        for(int n = 0; n < m_snowBall.Length; n++)
            EnqueueSnowBall(m_snowBall[n]);
    }

    public void EnqueueSnowBall(GameObject a_snowBall)
    {
        a_snowBall.SetActive(false);
        m_snowBallQueue.Enqueue(a_snowBall);
    }

    public GameObject DequeueSnowBall()
    {
        GameObject a_snowBall = null;
        if (m_snowBallQueue.Count != 0)
        {
            a_snowBall = m_snowBallQueue.Dequeue();
            a_snowBall.transform.position = m_snowBallRootPos.position;
            MouseUp(a_snowBall);
        }
        else
        {
            print("m_snowBallQueue 에 오브젝트가 들어있지 않습니다.");
        }

        return a_snowBall;
    }
    public void MouseUp(GameObject a_snowBallPos)
    {
        //a_snowBallPos.transform.position = m_originSnowBallTf.position;
        //a_snowBallPos.transform.rotation = m_originSnowBallTf.rotation;

        //SnowBallCopyManager.m_isAddForce = true;
        //SnowBallCopyManager.m_rigid.isKinematic = false;
    }
}
