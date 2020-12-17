using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.VFX;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager uniqueInstance;

    [Header("플레이어 스탯")]
    public float m_moveSpeed = 50f;

    [Header("스노우볼")]
    [SerializeField] GameObject m_snowBallObj = null;

    [Header("이펙트")]
    [SerializeField] GameObject m_splashEffect = null;
    [SerializeField] VisualEffect m_playerRunSplashEffect = null;       // 플레이어 달릴때 이펙트

    [Header("조이스틱 힘")]
    [SerializeField] float m_moveForce = 0;

    private Animator m_animCtrl = null;
    private Rigidbody m_rigid = null;
    private Joystick m_joystick = null;

    public static float m_multiplyCamZ = 1f;

    Sequence snowBallSeq;

    private void Awake()
    {
        m_animCtrl = GetComponent<Animator>();
        m_rigid = GetComponent<Rigidbody>();
        m_joystick = FindObjectOfType<Joystick>();
    }

    // Start is called before the first frame update
    void Start()
    {
        uniqueInstance = this;
    }

    // Update is called once per frame
    void Update()
    {
        switch(InGameManager.m_curGameState)
        {
            case InGameManager.eGameStage.Play:

                // === 플레이어 이동 === //
                this.transform.Translate(Vector3.forward * m_moveSpeed * Time.deltaTime, Space.Self);
                // === 플레이어 이동 === //
                
                break;

            case InGameManager.eGameStage.End:
                m_animCtrl.SetTrigger("Idle");
                break;
        }
    }

    public void SetPushAnim()
    {
        m_animCtrl.SetTrigger("Push");
        m_splashEffect.SetActive(true);
    }
    public void SetGameOverAnim()
    {
        m_animCtrl.SetTrigger("GameOver");
        m_snowBallObj.SetActive(false);         // 굴리던 눈덩이 오브젝트 끄기
        m_splashEffect.SetActive(false);        // 플레이어 달리는 이펙트 끄기
    }

    public void SetPushAnimSpeedUp()
    {
        m_animCtrl.speed += 1;
    }


    [Header("Fast Zone 이펙트")]
    [SerializeField] ParticleSystem m_camSplahEffect = null;
    Sequence playerFastSeq;
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer == LayerMask.NameToLayer("FastZone"))
        {
            playerFastSeq = DOTween.Sequence()
                                   .AppendCallback(() =>
                                   {
                                       m_camSplahEffect.Play();
                                       m_moveSpeed += 20;
                                   })
                                   .AppendInterval(1.5f)
                                   .OnComplete(() =>
                                   {
                                       m_camSplahEffect.Stop();
                                       m_moveSpeed -= 20;
                                   });
        }
    }
}
