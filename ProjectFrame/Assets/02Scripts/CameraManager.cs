using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CameraManager : MonoBehaviour
{
    [Header("(GameState = Start) 관련")]
    [SerializeField] Transform m_camStartPos = null;            // (GameState = Start)게임 시작시 플레이어를 뒤에서 바라볼 위치 
    [SerializeField] Transform m_player = null;                 // (GameState = Start)플레이어를 부모위치로 잡을 Tf
    [SerializeField] Transform m_lookPos = null;                // (GameState = Start)카메라가 이동중 바라볼 Tf


    public static bool m_isPlay = false;                           // (GameState = Play)인지 여부
    public Quaternion TargetRotation;                              // (GameState = Play)최종적으로 축적된 Gap이 이 변수에 저장됨.
    public float m_rotationSpeed = 10;                             // (GameState = Play)터치시 플레이어 회전 스피드.
    private Vector3 Gap = Vector3.zero;                            // (GameState = Play)회전 축적 값.
    private Vector3 m_followOffset = new Vector3(0, 60, -90);      // (GameState = Play)카메라 Zoom In/Out 관련 벡터



    private void Start()
    {
    }


    /// <summary>
    /// (GameState = Ready)카메라 => 플레이어 뒤로 이동 함수 (연출)
    /// </summary>
    public void FirstCamProduction()
    {
        Sequence camPdSeq;

        camPdSeq = DOTween.Sequence()
                          .Append(this.transform.DOMove(m_camStartPos.position, 2f))
                          .Append(this.transform.DOLookAt(m_lookPos.position, 1f))
                          .OnComplete(() =>
                          {
                              this.transform.SetParent(m_player.transform);

                              InGameManager.uniqueInstance.m_curGameState = InGameManager.eGameState.Start;
                          });
    }


    /// <summary>
    /// (GameState = Play) 화면 터치시 화면회전하는 부부
    /// </summary>
    private void LateUpdate()
    {
        if(m_isPlay == true)
        {
            #region (GameState = Play) 카메라 회전 부분
            if (Input.GetMouseButton(0))
            {
                  if (transform.rotation != TargetRotation)
                      m_player.transform.rotation = Quaternion.Slerp(m_player.transform.rotation, TargetRotation, m_rotationSpeed * Time.deltaTime);
              
                  //Gap.x += Input.GetAxis("Mouse Y") * m_rotationSpeed * -1;
                  Gap.y += Input.GetAxis("Mouse X") * m_rotationSpeed;
              
                  Gap.y += InGameManager.m_joystick.Horizontal;
                  Gap.y = Mathf.Clamp(Gap.y, -45f, 45f);

                  TargetRotation = Quaternion.Euler(Gap);
            }
            #endregion
        }
    }


    /// <summary>
    /// (GameState = Play) 스노우볼 크기에 따라 호출되는 함수
    /// </summary>
    public void SetFollowOffset(bool a_zoomOut)
    {
        if(a_zoomOut == true)
        {
            // === 카메라 줌아웃 === //
            Vector3  a_followOffset = this.transform.localPosition + m_followOffset;
            // print("Cam Pos : " + a_followOffset);

            this.transform.DOLocalMove(a_followOffset, .4f)
                          .SetEase(Ease.InQuad);
            // === 카메라 줌아웃 === //


            // === 캐릭터 회전 감도 === //
            m_rotationSpeed -= 1.5f;
            // === 캐릭터 회전 감도 === //
        }
        else
        {
            // === 카메라 줌인 === //
            Vector3 a_followOffset = this.transform.localPosition - m_followOffset;
            print("Cam Pos : " + a_followOffset);

            this.transform.DOLocalMove(a_followOffset, .3f)
                          .SetEase(Ease.InQuad);
            // === 카메라 줌인 === //


            // === 캐릭터 회전 감도 === //
            m_rotationSpeed += 1.5f;
            // === 캐릭터 회전 감도 === //
        }
    }
    
}
