using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public static CameraManager uniqueInstance;


    [SerializeField] Transform m_camStartPos = null;
    [SerializeField] float _movSpeed;
    [SerializeField] float _rotSpeed;
    public Vector3 _followOffset;

    [SerializeField] GameObject m_snowBallObj = null;
    [SerializeField] Vector3 m_plusOffset;

    [SerializeField] Transform m_player = null;
    [SerializeField] Transform m_lookPos = null;

    public Quaternion TargetRotation;  // 최종적으로 축적된 Gap이 이 변수에 저장됨.
    public float RotationSpeed = 10;        // 회전 스피드.

    private Vector3 Gap;               // 회전 축적 값.
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
                    InGameManager.m_curGameState = InGameManager.eGameStage.Start;
                }
                else
                {
                    this.transform.position = Vector3.MoveTowards(this.transform.position, m_camStartPos.position, 50 * Time.deltaTime);
                    this.transform.LookAt(m_lookPos.position);
                }

                break;

            case InGameManager.eGameStage.Start:

                m_timeCheck += Time.deltaTime;

                if(m_timeCheck >= .2f)
                {
                    m_timeCheck = 0;
                    PlayerManager.uniqueInstance.SetPushAnim();
                    InGameManager.m_curGameState = InGameManager.eGameStage.Play;
                }

                break;

            case InGameManager.eGameStage.Play:

                if (Input.GetMouseButton(0) || Input.GetMouseButton(1))
                {
                    if (transform.rotation != TargetRotation)
                        m_player.transform.rotation = Quaternion.Slerp(m_player.transform.rotation, TargetRotation, RotationSpeed * Time.deltaTime);

                    // 값을 축적.
                    //Gap.x += Input.GetAxis("Mouse Y") * RotationSpeed * -1;
                    Gap.y += Input.GetAxis("Mouse X") * RotationSpeed;
                    
                    a_camX = Gap.y > 0 ? a_camX += 0.25f : a_camX -= 0.25f;

                    a_camX = Mathf.Clamp(a_camX, -15, 15);
                    _followOffset = new Vector3(a_camX, _followOffset.y, _followOffset.z);

                    // 카메라 회전범위 제한.
                    Gap.y = Mathf.Clamp(Gap.y, -65f, 65f);
                    // 회전 값을 변수에 저장.

                    TargetRotation = Quaternion.Euler(Gap);
                }


                this.transform.position
                        = Vector3.MoveTowards(this.transform.position, m_player.transform.position + _followOffset, _movSpeed * Time.deltaTime);

                //this.transform.LookAt(m_lookPos.transform);
                break;
        }
    }

    public void SetFollowOffset()
    {
        // === 카메라 줌아웃 === //
        float a_y = _followOffset.y + 85;
        float a_z = _followOffset.z - 120;

        _followOffset = new Vector3(_followOffset.x, a_y, a_z);
        // === 카메라 줌아웃 === //

        // === 캐릭터 회전 감도 === //
        RotationSpeed -= .4f;
        // === 캐릭터 회전 감도 === //
    }
}
