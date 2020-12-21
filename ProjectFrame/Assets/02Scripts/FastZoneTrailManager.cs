using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FastZoneTrailManager : MonoBehaviour
{
    [Header("FastZone에서의 이펙트")]
    [SerializeField] ParticleSystem[] m_fastZoneTrailEffect = null;


    private Queue<GameObject> m_trailQueue = new Queue<GameObject>();


    public bool m_isTrailOn = false;                // 트레일 이펙트가 켜진상태인가 여부
    private Transform m_playerObj = null;          // 플레이어 위치 받기위해 미리 할당받을 변숫


    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void Update()
    {
        if(m_isTrailOn == true)
        {
            DequeueTrailEffect();
        }
    }


    /// <summary>
    /// (GameState = Start) 트레일 이펙트 풀링 함수
    /// </summary>
    public void SetTrailPooling()
    {
        for(int n = 0; n < m_fastZoneTrailEffect.Length; n++)
        {
            EnqueueTrailEffect(m_fastZoneTrailEffect[n].gameObject);
        }

        if (m_playerObj == null)
            m_playerObj = InGameManager.m_plyMgr.gameObject.transform;

        InGameManager.uniqueInstance.m_curGameState = InGameManager.eGameState.Start;
    }


    public void EnqueueTrailEffect(GameObject a_trailObj)
    {
        a_trailObj.SetActive(false);
        m_trailQueue.Enqueue(a_trailObj);
    }


    public void DequeueTrailEffect()
    {
        GameObject a_trailObj = m_trailQueue.Dequeue();
        a_trailObj.transform.position = m_playerObj.position;
        a_trailObj.SetActive(true);
    }
}
