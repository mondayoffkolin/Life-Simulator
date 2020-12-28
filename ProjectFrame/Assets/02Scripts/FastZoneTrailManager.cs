using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FastZoneTrailManager : MonoBehaviour
{
    [Header("FastZone에서의 이펙트")]
    [SerializeField] ParticleSystem[] m_fastZoneTrailEffect = null;
    [SerializeField] GameObject[] m_fastZoneTrailEffect2 = null;


    [Header("SnowBall 위치")]
    [SerializeField] private Transform m_snowballTf = null;       // 스노우볼 위치를 위한 변수


    private Queue<GameObject> m_trailQueue = new Queue<GameObject>();


    public bool m_isTrailOn = false;                     // 트레일 이펙트가 켜진상태인가 여부
    private Vector3 m_trailVec = new Vector3(11, 1, 6);


    // Start is called before the first frame update
    void Start()
    {
        
    }



    /// <summary>
    /// (GameState = Start) 트레일 이펙트 풀링 함수
    /// </summary>
    public void SetTrailPooling()
    {
        for(int n = 0; n < m_fastZoneTrailEffect2.Length; n++)
        {
            EnqueueTrailEffect(m_fastZoneTrailEffect2[n].gameObject);
        }
    }


    /// <summary>
    /// 지나간 자리Obj 다시 큐에 넣기
    /// </summary>
    /// <param name="a_trailObj"></param>
    public void EnqueueTrailEffect(GameObject a_trailObj)
    {
        a_trailObj.SetActive(false);
        m_trailQueue.Enqueue(a_trailObj);
    }

    /// <summary>
    /// 지나간 자리Obj 큐에서 빼기
    /// </summary>
    public void DequeueTrailEffect()
    {
        GameObject a_trailObj = m_trailQueue.Dequeue();
        //a_trailObj.transform.position = m_playerTf.position;
        a_trailObj.transform.position = m_snowballTf.position;
        //a_trailObj.transform.rotation = m_playerTf.rotation;
        a_trailObj.transform.rotation = m_snowballTf.rotation;
        a_trailObj.transform.localScale = m_trailVec;
        

        a_trailObj.SetActive(true);
    }
    public void SetTrailVec()
    {
        if ((int)m_snowballTf.localScale.x < 20)
            m_trailVec = new Vector3(11, 1, 6);
        else if ((int)m_snowballTf.localScale.x == 20)
            m_trailVec = new Vector3(14, 1, 10);
        else if ((int)m_snowballTf.localScale.x == 25)
            m_trailVec = new Vector3(17, 1, 12);
        else if ((int)m_snowballTf.localScale.x == 30)
            m_trailVec = new Vector3(20, 1, 14);
        else if ((int)m_snowballTf.localScale.x == 35)
            m_trailVec = new Vector3(22, 1, 16);
        else if ((int)m_snowballTf.localScale.x == 40)
            m_trailVec = new Vector3(24, 1, 18);
        else if ((int)m_snowballTf.localScale.x == 45)
            m_trailVec = new Vector3(26, 1, 20);
        else if ((int)m_snowballTf.localScale.x == 50)
            m_trailVec = new Vector3(28, 1, 22);
        else if ((int)m_snowballTf.localScale.x == 55)
            m_trailVec = new Vector3(30, 1, 24);
        else if ((int)m_snowballTf.localScale.x == 60)
            m_trailVec = new Vector3(32, 1, 26);
        else if ((int)m_snowballTf.localScale.x == 65)
            m_trailVec = new Vector3(34, 1, 28);
        else if ((int)m_snowballTf.localScale.x == 70)
            m_trailVec = new Vector3(36, 1, 30);
        else if ((int)m_snowballTf.localScale.x == 75)
            m_trailVec = new Vector3(38, 1, 32);
        else if ((int)m_snowballTf.localScale.x == 90)
            m_trailVec = new Vector3(40, 1, 34);
        else if ((int)m_snowballTf.localScale.x == 95)
            m_trailVec = new Vector3(42, 1, 36);
        else if ((int)m_snowballTf.localScale.x == 100)
            m_trailVec = new Vector3(44, 1, 38);
        else if ((int)m_snowballTf.localScale.x == 105)
            m_trailVec = new Vector3(46, 1, 40);
        else if ((int)m_snowballTf.localScale.x == 110)
            m_trailVec = new Vector3(48, 1, 42);
        else if ((int)m_snowballTf.localScale.x == 115)
            m_trailVec = new Vector3(50, 1, 44);
        else if ((int)m_snowballTf.localScale.x == 120)
            m_trailVec = new Vector3(52, 1, 46);
        else if ((int)m_snowballTf.localScale.x == 125)
            m_trailVec = new Vector3(54, 1, 48);
        else if ((int)m_snowballTf.localScale.x == 130)
            m_trailVec = new Vector3(56, 1, 50);
        else if ((int)m_snowballTf.localScale.x == 135)
            m_trailVec = new Vector3(58, 1, 52);
        else if ((int)m_snowballTf.localScale.x == 140)
            m_trailVec = new Vector3(60, 1, 54);
        else if ((int)m_snowballTf.localScale.x == 150)
            m_trailVec = new Vector3(62, 1, 56);
    }
}
