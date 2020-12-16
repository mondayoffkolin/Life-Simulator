using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager uniqueInstance;

    [Header("플레이어 스탯")]
    public float m_moveSpeed = 50f;

    [Header("스노우볼")]
    [SerializeField] GameObject m_snowBallObj = null;

    [Header("이펙트")]
    [SerializeField] GameObject m_splashEffect = null;

    private Animator m_animCtrl;

    public static float m_multiplyCamZ = 1f;

    Sequence snowBallSeq;

    private void Awake()
    {
        m_animCtrl = GetComponent<Animator>();
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

    public void SetPushAnimSpeedUp()
    {
        m_animCtrl.speed += 1;
    }
}
