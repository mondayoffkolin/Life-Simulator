using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIPathManager : MonoBehaviour
{
    [SerializeField] private Transform[] m_pathRoots = null;
    [SerializeField] private Transform[] m_childPathRoot = null;
    [SerializeField] private List<Transform> m_pathList;


    private void Awake()
    {
        m_pathList = new List<Transform>();
    }

    /// <summary>
    /// (GameStage = Ready/Play) Ai가 지나갈 Path 설정 
    /// </summary>
    public List<Transform> SetPathRoot()
    {
        m_pathRoots = new Transform[this.transform.childCount];                 // Path 개수만큼 할당

        for (int n = 0; n < m_pathRoots.Length; n++)
            m_pathRoots[n] = this.transform.GetChild(n).transform;              // Path 대입


        int a_rndPath = Random.Range(0, m_pathRoots.Length);
        m_childPathRoot = new Transform[m_pathRoots[a_rndPath].childCount];     // 특정 Path 지정해서 자식들 대입


        for (int n = 0; n < m_childPathRoot.Length; n++)
        {
            m_childPathRoot[n].position += FixPathVec();
            m_pathList.Add(m_childPathRoot[n]);
        }

        return m_pathList;
    }


    /// <summary>
    /// (GameStage = Ready/Play) 기존 경로에 x값을 -40~40만큼 빼고 더해주는 함수 (살짝의 랜덤성 위해)
    /// </summary>
    /// <returns></returns>
    private Vector3 FixPathVec()
    {
        float a_x = Random.Range(-40, 40);

        return new Vector3(a_x, 0, 0);
    }
}
