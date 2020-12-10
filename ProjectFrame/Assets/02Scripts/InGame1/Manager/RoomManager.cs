using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomManager : MonoBehaviour
{
    [Header("액자 관련")]
    [SerializeField] private GameObject[] m_photoFrameObj = null;           // 액자 오브젝트
    [SerializeField] private Transform m_photoFramePoolingTf = null;        // 액자 풀링시 부모 설정용

    [Header("스테이지 시작시 액자 위치 설정")]
    [SerializeField] private Transform[] m_photoFrameParentTf = null;       // 액자 부모 설정용
    [SerializeField] private Transform[] m_photoFramePosTf = null;          // 액자 걸릴 위치

    [SerializeField] private int m_pickCount;

    //List<GameObject> m_infancyQueue;
    //List<GameObject> m_boyhoodQueue;
    //List<GameObject> m_adolescencQueue;
    //List<GameObject> m_matureQueue;
    //List<GameObject> m_oldAgeQueue;

    private List<GameObject> m_doorPictureList;


    private void Awake()
    {
        m_doorPictureList = new List<GameObject>();
    }

    // Start is called before the first frame update
    private void Start()
    {
        #region 액자 풀링
        for (int n = 0; n < m_photoFrameObj.Length; n++)
        {
            GameObject a_pic = Instantiate(m_photoFrameObj[n], m_photoFrameObj[n].transform.position, Quaternion.identity);
            a_pic.transform.SetParent(m_photoFramePoolingTf);
            a_pic.SetActive(false);

            m_doorPictureList.Add(a_pic);
        }

        for (int n = 0; n < m_pickCount; n++)
        {
            int a_rnd = Random.Range(0, m_doorPictureList.Count);
            m_doorPictureList[a_rnd].SetActive(true);
            m_doorPictureList[a_rnd].transform.SetParent(m_photoFrameParentTf[n]);
            m_doorPictureList[a_rnd].transform.position = m_photoFramePosTf[n].transform.position;
            m_doorPictureList[a_rnd].transform.rotation = m_photoFramePosTf[n].transform.rotation;

            m_doorPictureList.RemoveAt(a_rnd);
        }
        #endregion
    }

}
