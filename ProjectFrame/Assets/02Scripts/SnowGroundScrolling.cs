using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class SnowGroundScrolling : MonoBehaviour
{
    //[Header("맵바닥 움직이는 스피드")]
    //[SerializeField] float m_groundMoveSpeed = .1f;         // 맵바닥 움직이는 스피드

    [SerializeField] Vector2 m_offset = Vector2.zero;

    MeshRenderer m_meshRender = null;

    private void Awake()
    {
        m_meshRender = GetComponent<MeshRenderer>();
    }

    // Start is called before the first frame update
    void Start()
    {
    }


    /// <summary>
    /// (GameState = Start) 맵바닥 움직이게하는 함수
    /// </summary>
    public void GroundMoving()
    {
        //this.transform.DOMove(Vector3.back, m_groundMoveSpeed)
        //                .SetEase(Ease.Linear)
        //                .SetLoops(-1, LoopType.Incremental);

        m_meshRender.material.DOOffset(m_offset, 25f).SetEase(Ease.Linear).SetLoops(-1, LoopType.Incremental);
    }


    /// <summary>
    /// (GameState = End) 맵바닥 멈추게하는 함
    /// </summary>
    public void StopGroundMoving()
    {
        //this.transform.DOKill();
        m_meshRender.material.DOKill();
    }
}
