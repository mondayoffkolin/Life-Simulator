using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FastZoneTrailManager : MonoBehaviour
{
    [Header("눈 지나간 흔적")]
    [SerializeField] GameObject[] m_onTheSnowTrail = null;          // 눈바닥 위에서의 흔적 Obj
    [SerializeField] GameObject[] m_onTheSoilTrail = null;          // 흙바닥 위에서의 흔적 Obj


    [Header("SnowBall 위치")]
    [SerializeField] private Transform m_snowballTf = null;       // 스노우볼 위치를 위한 변수


    [Header("눈덩이 흔적 true면 흙바닥, false면 눈바닥")]
    public bool m_isSnowTrailDequeue = true;


    [Header("눈덩이 흔적 증감 여부")]
    public bool m_isTrailVecIncrease = true;


    [HideInInspector] private Vector3 m_trailVec = new Vector3(5, 5, 5);


    private Queue<GameObject> m_trailOnSnowQueue = new Queue<GameObject>();
    private Queue<GameObject> m_trailOnSoilQueue = new Queue<GameObject>();


    // Start is called before the first frame update
    void Start()
    {
        
    }



    #region 눈바닥 위에서의 자취
    /// <summary>
    /// (GameState = Start) 트레일 이펙트 풀링 함수
    /// </summary>
    public void SetTrailOnSnowPooling()
    {
        for(int n = 0; n < m_onTheSnowTrail.Length; n++)
        {
            EnqueueTrailOnSnowEffect(m_onTheSnowTrail[n].gameObject);
        }
    }


    /// <summary>
    /// 지나간 자리Obj 다시 큐에 넣기
    /// </summary>
    /// <param name="a_trailObj"></param>
    public void EnqueueTrailOnSnowEffect(GameObject a_trailObj)
    {
        a_trailObj.SetActive(false);
        m_trailOnSnowQueue.Enqueue(a_trailObj);
    }

    /// <summary>
    /// 지나간 자리Obj 큐에서 빼기
    /// </summary>
    static float a_time = .1f;
    WaitForSeconds m_delayTime = new WaitForSeconds(a_time);
    public IEnumerator DequeueTrailOnSnowEffect()
    {
        if (m_isSnowTrailDequeue == true)
        {
            GameObject a_trailObj = m_trailOnSnowQueue.Dequeue();
            a_trailObj.transform.position = m_snowballTf.position;
            a_trailObj.transform.rotation = m_snowballTf.rotation;
            a_trailObj.transform.localScale = m_trailVec;

            a_trailObj.SetActive(true);


            yield return m_delayTime;
            a_time += .1f;
        }
        else
        {
            GameObject a_trailObj = m_trailOnSoilQueue.Dequeue();
            a_trailObj.transform.position = m_snowballTf.position;
            a_trailObj.transform.rotation = m_snowballTf.rotation;
            a_trailObj.transform.localScale = m_trailVec;

            a_trailObj.SetActive(true);

            yield return m_delayTime;
            a_time -= .1f;
        }

        StartCoroutine(DequeueTrailOnSnowEffect());
    }
    #endregion


    #region 흙바닥 위에서의 자취
    public void SetTrailOnSoilPooling()
    {
        for (int n = 0; n < m_onTheSoilTrail.Length; n++)
        {
            EnqueueTrailOnSoilEffect(m_onTheSoilTrail[n].gameObject);
        }
    }


    public void EnqueueTrailOnSoilEffect(GameObject a_trailObj)
    {
        a_trailObj.SetActive(false);
        m_trailOnSoilQueue.Enqueue(a_trailObj);
    }

    public IEnumerator DequeueTrailOnSoilEffect()
    {
        if (m_isSnowTrailDequeue == false)
        {
            GameObject a_trailObj = m_trailOnSoilQueue.Dequeue();
            a_trailObj.transform.position = m_snowballTf.position;
            a_trailObj.transform.rotation = m_snowballTf.rotation;
            a_trailObj.transform.localScale = m_trailVec;

            a_trailObj.SetActive(true);

            yield return m_delayTime;
            a_time -= .5f;

            StartCoroutine(DequeueTrailOnSoilEffect());
        }
        else
            StopCoroutine(DequeueTrailOnSoilEffect());
    }
    #endregion


    /// <summary>
    /// 지나간 자취 크기 증감 코루틴
    /// </summary>
    WaitForSeconds m_delayTime2 = new WaitForSeconds(.1f);
    public IEnumerator SetTrailVec()
    {
        if (m_isTrailVecIncrease == false)
            m_trailVec -= new Vector3(.05f, 0, .05f);
        else
            m_trailVec += new Vector3(.005f, 0, .005f);


        yield return m_delayTime2;

        StartCoroutine(SetTrailVec());
    }

}
