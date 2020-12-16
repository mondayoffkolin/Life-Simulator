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
        this.transform.position += Vector3.back * m_groundMoveSpeed * Time.deltaTime;
    }
}
