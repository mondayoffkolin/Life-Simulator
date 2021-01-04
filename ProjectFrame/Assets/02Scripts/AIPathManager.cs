using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIPathManager : MonoBehaviour
{
    /// <summary>
    /// (GameStage = Ready/Play) Ai가 지나갈 Path 설정 
    /// </summary>
    public Vector3[] SetPathRoot(eLevel a_aiLevel)
    {
        Transform[] m_pathRoots = null;
        m_pathRoots = new Transform[this.transform.childCount];                 // Path 개수만큼 할당

        for (int n = 0; n < m_pathRoots.Length; n++)
            m_pathRoots[n] = this.transform.GetChild(n).transform;                // Path 대입


        Vector3[] m_childPathRoot = null;
        int a_rndPath = Random.Range(0, m_pathRoots.Length);
        m_childPathRoot = new Vector3[m_pathRoots[a_rndPath].childCount];     // 특정 Path 지정해서 자식들 대입


        for (int n = 0; n < m_childPathRoot.Length; n++)
        {
            m_childPathRoot[n] = m_pathRoots[a_rndPath].GetChild(n).position;
            m_childPathRoot[n] += FixPathVec(a_aiLevel);
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
                a_x = Random.Range(-20, 20);
                break;

            case eLevel.Middle:        // AI 난이도 
                a_x = Random.Range(-400, 400);
                break;

            case eLevel.High:        // AI 난이도 상
                //a_x = 0;
                a_x = Random.Range(-50, 50);
                break;
        }



        return new Vector3(a_x, 0, 0);
    }
}
