using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum eObstacleLevel
{
    Small,
    Normal,
    Middle,
    Large,

    None
}

public class ObstacleManager : MonoBehaviour
{
    public enum eObstacleType
    {
        Tree,
        Rock,

        None
    }


    [Header("장애물 피해레벨")]
    public eObstacleLevel m_curObstacleLevel;


    [Header("장애물 타입")]
    [SerializeField] eObstacleType m_curObstacleType;


    BoxCollider m_boxCollider = null;           // 장애물 BoxCollider


    // Start is called before the first frame update
    void Start()
    {
        m_boxCollider = GetComponent<BoxCollider>();
    }



    #region (GameState = End) 플레이어가 죽었을시 호출되는 영역
    /// <summary>
    /// 장애물(나무) 폭발 함수
    /// </summary>
    public void AddExplosion()
    {
        Rigidbody a_rigid = GetComponent<Rigidbody>();
        a_rigid.isKinematic = false;
        m_boxCollider.enabled = true;
        m_boxCollider.isTrigger = false;

        Vector3 a_vec = ReturnForceVec();
        a_rigid.AddForce(a_vec, ForceMode.Impulse);
    }
    /// <summary>
    /// 터지는 방향 벡터 함
    /// </summary>
    /// <returns></returns>
    private Vector3 ReturnForceVec()
    {
        float a_x = Random.Range(-20, 20);
        float a_y = Random.Range(5, 10);
        float a_z = Random.Range(-20, 20);

        return new Vector3(a_x, a_y, a_z);
    }
    #endregion



    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer == LayerMask.NameToLayer("SnowBall"))
        {

            switch(m_curObstacleType)
            {
                case eObstacleType.Tree:

                    m_boxCollider.enabled = false;

                    switch(m_curObstacleLevel)
                    {
                        case eObstacleLevel.Small:      
                            InGameManager.m_snowBallMgr.AttachToSnowBall(this.gameObject);        // 현재 장애물 눈덩이 속으로
                            break;


                        case eObstacleLevel.Normal:
                            if (InGameManager.m_snowBallMgr.m_curSnowBallSize == SnowBallManager.eSnowBallSize.Ninety)
                            {
                                InGameManager.m_snowBallMgr.AttachToSnowBall(this.gameObject);        // 현재 장애물 눈덩이 속으로
                            }
                            else
                            {
                                m_boxCollider.enabled = false;

                                InGameManager.m_plyMgr.PlayCrashEffect(0);
                                InGameManager.m_snowBallMgr.StopIncreaseSnowBall();                   // 눈덩이 크기 감소
                            }
                            break;


                        case eObstacleLevel.Middle:
                            if (InGameManager.m_snowBallMgr.m_curSnowBallSize == SnowBallManager.eSnowBallSize.HundredFIf)
                            {
                                InGameManager.m_snowBallMgr.AttachToSnowBall(this.gameObject);        // 현재 장애물 눈덩이 속으로
                            }
                            else
                            {
                                m_boxCollider.enabled = false;

                                InGameManager.m_plyMgr.PlayCrashEffect(0);
                                InGameManager.m_snowBallMgr.StopIncreaseSnowBall();                   // 눈덩이 크기 감소
                            }
                            break;


                        case eObstacleLevel.Large:
                            m_boxCollider.enabled = false;

                            InGameManager.m_plyMgr.PlayCrashEffect(0);

                            // === 플레이어 바로 죽음 === //
                            InGameManager.m_snowBallMgr.m_curSnowBallSize = SnowBallManager.eSnowBallSize.One;
                            InGameManager.m_snowBallMgr.SetSnowBallSize(false);
                            // === 플레이어 바로 죽음 === //
                            break;
                    }

                    break;
                case eObstacleType.Rock:
                    m_boxCollider.enabled = false;

                    InGameManager.m_plyMgr.PlayCrashEffect(1);

                    // === 플레이어 바로 죽음 === //
                    InGameManager.m_snowBallMgr.m_curSnowBallSize = SnowBallManager.eSnowBallSize.One;
                    InGameManager.m_snowBallMgr.SetSnowBallSize(false);
                    // === 플레이어 바로 죽음 === //

                    InGameManager.m_snowBallMgr.StopIncreaseSnowBall();                   // 눈덩이 크기 감소
                    break;
            }
        }
    }
}
