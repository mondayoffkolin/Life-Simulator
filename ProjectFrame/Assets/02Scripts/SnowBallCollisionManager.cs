using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnowBallCollisionManager : MonoBehaviour
{
    public static SnowBallCollisionManager uniqueInstance;

    [Header("[Player] 오브젝트를 부모로")]
    [SerializeField] Transform m_playerParentTf = null;

    [SerializeField] SnowBallManager.eSnowBallSize m_curSnowBallSize;

    // Start is called before the first frame update
    void Start()
    {
        uniqueInstance = this;
    }


    //private void OnTriggerEnter(Collider other)
    //{
    //    if(other.gameObject.layer == LayerMask.NameToLayer("Obstacle"))
    //    {
    //        print("Trigger to Obstacle");
    //        other.gameObject.SetActive(false);

    //        m_curSnowBallSize = SnowBallManager.m_curSnowBallSize;

    //        SnowBallManager.m_upgradeSnowBall = true;
    //        CameraManager.uniqueInstance.SetFollowOffset2(m_curSnowBallSize);        // 눈덩이 다운그레이
    //    }
    //}
}
