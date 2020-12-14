using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class FloorManager : MonoBehaviour
{
    [SerializeField] GameObject[] m_floorObjs = null;
    [SerializeField] Transform[] m_floorTf = null;
    [SerializeField] Transform m_floorApearTf = null;

    Sequence floorSeq;

    // Start is called before the first frame update
    void Start()
    {
        //floorSeq = DOTween.Sequence()
        //                  .
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
