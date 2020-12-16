using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum eObstacleType
{
    Small = 2,
    Normal,
    Middle,
    Large,
    Huge,

    None
}

public class ObstacleManager : MonoBehaviour
{
    public eObstacleType m_curObstacleType;

    BoxCollider m_boxCollider = null;

    // Start is called before the first frame update
    void Start()
    {
        m_boxCollider = GetComponent<BoxCollider>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer == LayerMask.NameToLayer("SnowBall"))
        {
            SnowBallManager.m_upgradeSnowBall = true;

            m_boxCollider.enabled = false;
            CameraManager.uniqueInstance.SetFollowOffset2(m_curObstacleType, this.gameObject);
            
            //this.gameObject.SetActive(false);
        }
    }
}
