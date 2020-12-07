using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomManager : MonoBehaviour
{
    [Header("액자 관련")]
    [SerializeField] GameObject[] m_photoFrameObj = null;           // 액자 오브젝트
    [SerializeField] Transform m_photoFramePoolingTf = null;        // 액자 풀링시 부모 설정용

    [Header("스테이지 시작시 액자 위치 설정")]
    [SerializeField] Transform[] m_photoFrameParentTf = null;       // 액자 부모 설정용
    [SerializeField] Transform[] m_photoFramePosTf = null;          // 액자 걸릴 위치

    [SerializeField] bool[] m_isPhotoPick;

    List<GameObject> m_infancyQueue;
    List<GameObject> m_boyhoodQueue;
    List<GameObject> m_adolescencQueue;
    List<GameObject> m_matureQueue;
    List<GameObject> m_oldAgeQueue;

    // Start is called before the first frame update
    private void Start()
    {
        m_isPhotoPick = new bool[m_photoFrameObj.Length];

        #region 액자 풀링
        m_infancyQueue = new List<GameObject>();
        m_boyhoodQueue = new List<GameObject>();
        m_adolescencQueue = new List<GameObject>();
        m_matureQueue = new List<GameObject>();
        m_oldAgeQueue = new List<GameObject>();


        for (int n = 0; n < m_photoFrameObj.Length; n++)
        {

        }
        #endregion
    }


    #region 액자 풀링
    #endregion


    /// <summary>
    /// 액자 활성화 함수
    /// </summary>
    private void SetPhotoFrame()
    {
        int a_cnt = 0;

        while(true)
        {
            int a_rnd = Random.Range(0, m_isPhotoPick.Length);

            if(m_isPhotoPick[a_rnd] == false)
            {
                m_isPhotoPick[a_rnd] = true;


                a_cnt += 1;
            }    
        }
    }

    private void ResetPhotoPick()
    {
        for (int n = 0; n < m_isPhotoPick.Length; n++)
            m_isPhotoPick[n] = false;
    }
}
