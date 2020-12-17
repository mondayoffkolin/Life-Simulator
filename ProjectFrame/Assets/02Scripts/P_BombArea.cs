using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class P_BombArea : MonoBehaviour
{
    [SerializeField] Transform m_bombAreaTf = null;

    [SerializeField] private Vector3 m_bombAreaVec = Vector3.zero;

    Sequence bombSeq;

    // Start is called before the first frame update
    void Start()
    {
        StartBombAreaDG();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartBombAreaDG()
    {
        this.transform.DORotate(Vector3.up * 360, 1).SetEase(Ease.Linear).SetLoops(-1, LoopType.Incremental);

        Vector3 a_dir = Vector3.zero;

        bombSeq = DOTween.Sequence()
                        .OnStart(() =>
                        {
                            m_bombAreaVec = BombArea();

                            a_dir = (this.transform.position - m_bombAreaVec).normalized;
                            a_dir.Normalize();
                        })
                        .Append(m_bombAreaTf.transform.DOLookAt(m_bombAreaVec, 1.5f))
                        .AppendInterval(1f)
                        .AppendCallback(() =>
                        {
                            this.transform.position = m_bombAreaVec;
                        })
                        .AppendInterval(3f)
                        .SetLoops(-1);
    }

    private Vector3 BombArea()
    {
        float a_x = Random.Range(m_bombAreaTf.position.x - 500, m_bombAreaTf.position.x + 500);
        float a_z = Random.Range(m_bombAreaTf.position.z - 500, m_bombAreaTf.position.z + 500);

        return new Vector3(a_x, 0, a_z);
    }
}
