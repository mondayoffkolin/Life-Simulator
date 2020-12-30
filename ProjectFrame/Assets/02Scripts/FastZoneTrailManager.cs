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
    public void SetTrailVec(bool a_isReset)
    {
        if(a_isReset == true)
            m_trailVec = new Vector3(11, 1, 6);
        else
            m_trailVec += new Vector3(m_trailVec.x + 0.001f, 1, m_trailVec.z + 0.001f);
    }
}
