using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

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
    public eObstacleLevel m_curObstacleLevel = eObstacleLevel.None;


    [Header("장애물 타입")]
    [SerializeField] eObstacleType m_curObstacleType = eObstacleType.None;


    BoxCollider m_boxCollider = null;           // 장애물 BoxCollider
    Rigidbody m_rigid = null;


    private void Awake()
    {
        m_boxCollider = this.GetComponent<BoxCollider>();
        m_rigid = this.GetComponent<Rigidbody>();
    }


    #region (GameState = End) 플레이어가 죽었을시 호출되는 영역
    /// <summary>
    /// 장애물(나무) 폭발 함수
    /// </summary>
    public void AddExplosion()
    {
        if (m_rigid != null)
            m_rigid.isKinematic = false;
        else
            print("ObstacleManager Rigidbody 없음");


        //if (m_boxCollider != null)
        //{
        //    m_boxCollider.enabled = true;
        //    m_boxCollider.isTrigger = false;
        //}
        //else
        //    print("ObstacleManager Boxcollider 없음");


        Vector3 a_vec = ReturnForceVec();

        m_rigid.AddForce(a_vec, ForceMode.Impulse);
        //m_rigid.AddExplosionForce(1000f, this.transform.position, 10 , 300);
        this.transform.DOScale(Vector3.zero, 4f);
    }
    /// <summary>
    /// 터지는 방향 벡터 함
    /// </summary>
    /// <returns></returns>
    private Vector3 ReturnForceVec()
    {
        float a_x = Random.Range(-70, 70);
        float a_y = Random.Range(40, 70);
        float a_z = Random.Range(-70, 70);

        return new Vector3(a_x, a_y, a_z);
    }
    #endregion



    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer == LayerMask.NameToLayer("SnowBall"))
        {
            if(InGameManager.m_plyMgr.m_playerState == ePlayerState.Run)
            {
                switch(m_curObstacleType)
            {
                case eObstacleType.Tree:

                    m_boxCollider.enabled = false;

                    switch(m_curObstacleLevel)
                    {
                        case eObstacleLevel.Small:      
                            InGameManager.m_snowBallMgr.AttachToSnowBall(eObstacleLevel.Small, this.gameObject);        // 현재 장애물 눈덩이 속으로
                            break;


                        case eObstacleLevel.Normal:
                            if (InGameManager.m_snowBallMgr.m_curSnowBallSize == SnowBallManager.eSnowBallSize.ThirtyFive)
                            {
                                InGameManager.m_snowBallMgr.AttachToSnowBall(eObstacleLevel.Normal, this.gameObject);        // 현재 장애물 눈덩이 속으로
                            }
                            else
                            {
                                InGameManager.m_plyMgr.PlayCrashEffect(0);
                                InGameManager.m_snowBallMgr.StopIncreaseSnowBall(InGameManager.m_plyMgr);                   // 눈덩이 크기 감소
                            }
                            break;


                        case eObstacleLevel.Middle:
                            if (InGameManager.m_snowBallMgr.m_curSnowBallSize == SnowBallManager.eSnowBallSize.Ninety)
                            {
                                InGameManager.m_snowBallMgr.AttachToSnowBall(eObstacleLevel.Middle, this.gameObject);        // 현재 장애물 눈덩이 속으로
                            }
                            else
                            {
                                InGameManager.m_plyMgr.PlayCrashEffect(0);
                                InGameManager.m_snowBallMgr.StopIncreaseSnowBall(InGameManager.m_plyMgr);                   // 눈덩이 크기 감소
                            }
                            break;


                        case eObstacleLevel.Large:
                            InGameManager.m_plyMgr.PlayCrashEffect(0);

                            // === 플레이어 바로 죽음 & 눈덩이 크기증가 Stop === //
                            InGameManager.m_snowBallMgr.StopIncreaseSnowBall(InGameManager.m_plyMgr, true);
                            // === 플레이어 바로 죽음 & 눈덩이 크기증가 Stop === //
                            break;
                    }

                    break;


                case eObstacleType.Rock:
                    //m_boxCollider.enabled = false;

                    InGameManager.m_plyMgr.PlayCrashEffect(1);

                    // === 플레이어 바로 죽음 === //
                    InGameManager.m_snowBallMgr.StopIncreaseSnowBall(InGameManager.m_plyMgr, true);
                    // === 플레이어 바로 죽음 === //
                    break;
            }
            }
        }
        else if(other.gameObject.layer == LayerMask.NameToLayer("SnowBall_E"))
        {
            SnowBallManager a_snowBallMgr = other.transform.parent.GetComponent<SnowBallManager>();
            AIManager a_aiMgr = a_snowBallMgr.m_parentTf.GetComponent<AIManager>();

            if(a_aiMgr.m_playerState == ePlayerState.Run)
            {
                switch (m_curObstacleType)
            {
                case eObstacleType.Tree:

                    m_boxCollider.enabled = false;

                    switch (m_curObstacleLevel)
                    {
                        case eObstacleLevel.Small:
                            a_snowBallMgr.AttachToSnowBall(eObstacleLevel.Small, this.gameObject);
                            break;


                        case eObstacleLevel.Normal:
                            if (a_snowBallMgr.m_curSnowBallSize == SnowBallManager.eSnowBallSize.ThirtyFive)
                            {
                                a_snowBallMgr.AttachToSnowBall(eObstacleLevel.Normal, this.gameObject);        // 현재 장애물 눈덩이 속으로
                            }
                            else
                            {
                                a_aiMgr.PlayCrashEffect(0);
                                a_snowBallMgr.StopIncreaseAiSnowBall(a_aiMgr);                   // 눈덩이 크기 감소
                            }
                            break;


                        case eObstacleLevel.Middle:
                            if (a_snowBallMgr.m_curSnowBallSize == SnowBallManager.eSnowBallSize.Ninety)
                            {
                                a_snowBallMgr.AttachToSnowBall(eObstacleLevel.Middle, this.gameObject);        // 현재 장애물 눈덩이 속으로
                            }
                            else
                            {
                                a_aiMgr.PlayCrashEffect(0);
                                a_snowBallMgr.StopIncreaseAiSnowBall(a_aiMgr);                   // 눈덩이 크기 감소
                            }
                            break;


                        case eObstacleLevel.Large:
                            a_aiMgr.PlayCrashEffect(0);

                            // === Ai 바로 죽음 & 눈덩이 크기증가 Stop === //
                            a_snowBallMgr.StopIncreaseAiSnowBall(a_aiMgr, true);
                            // === Ai 바로 죽음 & 눈덩이 크기증가 Stop === //
                            break;
                    }

                    break;


                case eObstacleType.Rock:
                    a_aiMgr.PlayCrashEffect(1);

                    // === Ai 바로 죽음 === //
                    a_snowBallMgr.StopIncreaseAiSnowBall(a_aiMgr, true);
                    // === Ai 바로 죽음 === //
                    break;
            }
            }
        }
    }
}
