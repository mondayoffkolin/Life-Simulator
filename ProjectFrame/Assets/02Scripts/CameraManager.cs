using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CameraManager : MonoBehaviour
{
    public static CameraManager uniqueInstance;


    [SerializeField] Transform m_camStartPos = null;
    [SerializeField] float _movSpeed = 0;
    [SerializeField] float _rotSpeed = 0;
    public Vector3 m_followOffset = Vector3.zero;

    [SerializeField] Vector3 m_plusOffset = Vector3.zero;

    [SerializeField] Transform m_player = null;
    [SerializeField] Rigidbody m_playerRigid = null;
    [SerializeField] Transform m_lookPos = null;

    [SerializeField] Joystick m_joystick = null;

    [Header("장애물 나무에 박히기 관련")]
    [SerializeField] Transform m_snowBallTf = null;
    [SerializeField] Transform[] m_enterTf = null;

    public Quaternion TargetRotation;  // 최종적으로 축적된 Gap이 이 변수에 저장됨.
    public float m_rotationSpeed = 10;        // 플렝이어 회전 스피드.

    private Vector3 Gap = Vector3.zero;               // 회전 축적 값.
    private float a_camX = 0;

    private float m_timeCheck = 0;

    private void Start()
    {
        uniqueInstance = this;
    }

    void Update()
    {
        switch (InGameManager.m_curGameState)
        {
            case InGameManager.eGameStage.Ready:

                float a_dis = (this.transform.position - m_camStartPos.position).sqrMagnitude;

                if (a_dis < 0.2f)
                {
                    this.gameObject.transform.SetParent(m_player);
                    InGameManager.m_curGameState = InGameManager.eGameStage.Start;
                }
                else
                {
                    this.transform.position = Vector3.MoveTowards(this.transform.position, m_camStartPos.position, _movSpeed * Time.deltaTime);
                    this.transform.LookAt(m_lookPos.position);
                }

                break;

            case InGameManager.eGameStage.Start:

                m_timeCheck += Time.deltaTime;

                if(m_timeCheck >= .2f)
                {
                    m_timeCheck = 0;
                    PlayerManager.uniqueInstance.SetPushAnim();         // 플레이어 Push 애니메이션 시작!
                    SnowBallManager.uniqueInstance.RotateSnowBall();
                    InGameManager.m_curGameState = InGameManager.eGameStage.Play;
                }

                break;

            case InGameManager.eGameStage.Play:

                if (Input.GetMouseButton(0) || Input.GetMouseButton(1))
                {
                    if (transform.rotation != TargetRotation)
                        m_player.transform.rotation = Quaternion.Slerp(m_player.transform.rotation, TargetRotation, m_rotationSpeed * Time.deltaTime);

                    // 값을 축적.
                    //Gap.x += Input.GetAxis("Mouse Y") * m_rotationSpeed * -1;
                    Gap.y += Input.GetAxis("Mouse X") * m_rotationSpeed;


                    //a_camX = Gap.y > 0 ? a_camX += 0.25f : a_camX -= 0.25f;

                    //a_camX = Mathf.Clamp(a_camX, -15, 15);
                    //m_followOffset = new Vector3(a_camX, m_followOffset.y, m_followOffset.z);


                    //print("Gap.y : " + Gap.y);
                    //print("m_joystick.Horizontal : " + m_joystick.Horizontal);

                    // 카메라 회전범위 제한.
                    Gap.y += m_joystick.Horizontal;
                    Gap.y = Mathf.Clamp(Gap.y, -45f, 45f);
                    // 회전 값을 변수에 저장.

                    TargetRotation = Quaternion.Euler(Gap);
                }


                //this.transform.position
                //        = Vector3.MoveTowards(this.transform.position, this.transform.position + m_followOffset, _movSpeed * Time.deltaTime);

                //this.transform.LookAt(m_lookPos.transform);
                break;
        }
    }

    /// <summary>
    /// 5배수 단위로 스노우볼 크기에 따라 호출되는 함5
    /// </summary>
    public void SetFollowOffset()
    {
        // === 카메라 줌아웃 === //
        float a_y = m_followOffset.y + 65;
        float a_z = m_followOffset.z - 80;

        m_followOffset = new Vector3(m_followOffset.x, a_y, a_z);
        m_followOffset = this.transform.localPosition + m_followOffset;

        this.transform.DOLocalMove(m_followOffset, 1f);
        // === 카메라 줌아웃 === //

        // === 캐릭터 회전 감도 === //
        m_rotationSpeed -= .5f;
        // === 캐릭터 회전 감도 === //

        // === 스노우볼 회전 스피드 === //
        SnowBallManager.m_snowBallRotateSpeed += 25;
        // === 스노우볼 회전 스피드 === //
    }

    /// <summary>
    /// 단계 낮추기
    /// </summary>
    [SerializeField] GameObject m_snowBallObj = null;
    public void SetFollowOffset2(eObstacleType a_obstacleType, GameObject a_obstacleObj)
    {
        if ((int)a_obstacleType >= 2)
        {
            // === 눈덩이 크기 === //
            switch (a_obstacleType)
            {
                case eObstacleType.Huge:
                    SnowBallManager.m_curSnowBallSize = SnowBallManager.eSnowBallSize.Thirty;
                    m_snowBallObj.transform.DOScale(new Vector3(25f, 25f, 25f), 1f);
                    m_snowBallObj.transform.DOLocalMove(new Vector3(0, 0, 56), 1f);
                    break;

                case eObstacleType.Large:
                    SnowBallManager.m_curSnowBallSize = SnowBallManager.eSnowBallSize.Twenty;
                    m_snowBallObj.transform.DOScale(new Vector3(20f, 20f, 20f), 1f);
                    m_snowBallObj.transform.DOLocalMove(new Vector3(0, 0, 50), 1f);
                    break;

                case eObstacleType.Middle:
                    SnowBallManager.m_curSnowBallSize = SnowBallManager.eSnowBallSize.Fifteen;
                    m_snowBallObj.transform.DOScale(new Vector3(15f, 15f, 15f), 1f);
                    m_snowBallObj.transform.DOLocalMove(new Vector3(0, 0, 40), 1f);
                    break;

                case eObstacleType.Normal:
                    SnowBallManager.m_curSnowBallSize = SnowBallManager.eSnowBallSize.Ten;
                    m_snowBallObj.transform.DOScale(new Vector3(10f, 10f, 10f), 1f);
                    m_snowBallObj.transform.DOLocalMove(new Vector3(0, 0, 33), 1f);
                    break;

                case eObstacleType.Small:
                    SnowBallManager.m_curSnowBallSize = SnowBallManager.eSnowBallSize.Five;
                    m_snowBallObj.transform.DOScale(new Vector3(5f, 5f, 5f), 1f);
                    // === 스노우볼 위치 === //
                    m_snowBallObj.transform.DOLocalMove(new Vector3(0, 0, 21f), 1f);
                    // === 스노우볼 위치 === //
                    break;
            }
            // === 눈덩이 크기 === //

       
            // === 카메라 줌아웃 === //
            float a_y = m_followOffset.y - 65;
            float a_z = m_followOffset.z + 80;

            m_followOffset = new Vector3(m_followOffset.x, a_y, a_z);

            this.transform.DOLocalMove(m_followOffset, 1f)
                    .OnComplete(() =>
                    {
                        SnowBallManager.m_upgradeSnowBall = false;
                    });
            // === 카메라 줌아웃 === //



            // === 캐릭터 회전 감도 === //
            m_rotationSpeed += .5f;
            // === 캐릭터 회전 감도 === //



            // === 스노우볼 회전 스피드 === //
            SnowBallManager.m_snowBallRotateSpeed -= 25;
            // === 스노우볼 회전 스피드 === //


            // === 장애물 눈덩이에 박히기 === //
            int a_rnd = Random.Range(0, m_enterTf.Length);
            a_obstacleObj.transform.SetParent(m_snowBallTf.transform);
            a_obstacleObj.transform.position = m_enterTf[a_rnd].position;
            a_obstacleObj.transform.rotation = m_enterTf[a_rnd].rotation;
            a_obstacleObj.transform.localScale = m_enterTf[a_rnd].localScale;
            // === 장애물 눈덩이에 박히기 === //
        }
        else
        {
            PlayerManager.uniqueInstance.m_moveSpeed = 0;
            PlayerManager.uniqueInstance.SetGameOverAnim();

            InGameManager.m_curGameState = InGameManager.eGameStage.End;
        }
    }
}
