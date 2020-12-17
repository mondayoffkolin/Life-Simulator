using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnowGroundScrolling : MonoBehaviour
{
    [SerializeField] float m_groundMoveSpeed = 5f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(InGameManager.m_curGameState == InGameManager.eGameStage.Play)
            this.transform.position += Vector3.back * m_groundMoveSpeed * Time.deltaTime;
    }
}
