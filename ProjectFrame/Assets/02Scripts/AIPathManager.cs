using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIPathManager : MonoBehaviour
{
    [Header("플레이하고 있는 스테이지 경로 개수")]
    [SerializeField] private Transform[] m_pathRoots = null;


    [Header("특정 경로 오브젝트의 자식개수만큼의 Tf")]
    [SerializeField] private Transform[] m_childPathRoot = null;


    /// <summary>
    /// (GameStage = Ready/Play) Ai가 지나갈 Path 설정 
    /// </summary>
    public Transform[] SetPathRoot(eLevel a_aiLevel)
    {
        m_pathRoots = new Transform[this.transform.childCount];                 // Path 개수만큼 할당

        for (int n = 0; n < m_pathRoots.Length; n++)
            m_pathRoots[n] = this.transform.GetChild(n).transform;              // Path 대입


        int a_rndPath = Random.Range(0, m_pathRoots.Length);
        m_childPathRoot = new Transform[m_pathRoots[a_rndPath].childCount];     // 특정 Path 지정해서 자식들 대입


        for (int n = 0; n < m_childPathRoot.Length; n++)
        {
            m_childPathRoot[n] = m_pathRoots[a_rndPath].GetChild(n);
            m_childPathRoot[n].position += FixPathVec(a_aiLevel);
        }


        return m_childPathRoot;
    }


    /// <summary>
    /// (GameStage = Ready/Play) 기존 경로에 x값을 -40~40만큼 빼고 더해주는 함수 (살짝의 랜덤성 위해)
    /// </summary>
    /// <returns></returns>
    private Vector3 FixPathVec(eLevel a_aiLevel)
    {
        float a_x = 0;

        switch (a_aiLevel)
        {
            case eLevel.Low:        // AI 난이도 하
                a_x = Random.Range(-40, 40);
                break;

            case eLevel.Middle:        // AI 난이도 하
                a_x = Random.Range(-15, 15);
                break;

            case eLevel.High:        // AI 난이도 하
                a_x = 0;
                break;
        }    

        return new Vector3(a_x, 0, 0);
    }
}
