using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.VFX;
using UnityEngine.Rendering.PostProcessing;

public class PlayerManager : MonoBehaviour
{
    [Header("플레이어 스탯")]
    public float m_playerMovSpd = 2f;                                   // 플레이어 움직임 속도


    [Header("스노우볼 Obj")]
    [SerializeField] GameObject m_snowBallObj = null;                   // 스노우볼 오브젝트
    [SerializeField] MeshRenderer m_snowBallMeshRdr = null;
    private Transform m_playerParentTf = null;                          // 플레이어 부모 오브젝트Tf


    [Header("플레이어 이펙트")]
    [SerializeField] GameObject m_splashEffect = null;                  //  플레이어 달릴때 이펙트
    [SerializeField] VisualEffect m_playerRunSplashEffect = null;       // 플레이어 달릴때 이펙트(조절용)
    [SerializeField] ParticleSystem m_camSplashEffect = null;           // 플레이어 달릴때 카메라에 나올 이펙트


    [SerializeField] ParticleSystem m_crashHitEffect = null;
    [SerializeField] ParticleSystem m_crashRockEffect = null;


    [Header("클리어시 Tf")]
    [SerializeField] Transform m_clearTf = null;


    public PostProcessVolume m_volume;
    private Vignette m_vignette = null;
    private Animator m_animCtrl = null;


    Sequence playerFastSeq;             // (FastZone) 밟았을 때 DoTween
    Sequence playerClearSeq;
    Sequence playerGameOverSeq;

    private void Awake()
    {
        m_animCtrl = GetComponent<Animator>();

        m_playerParentTf = this.transform.parent;
    }


    // Start is called before the first frame update
    private void Start()
    {
        //m_volume.profile.TryGetSettings(out m_vignette);
        //m_vignette.intensity.value = 0;
    }



    #region 플레이어 애니메이션 실행
    /// <summary>
    /// (GameState = Play)플레이어 Push 애니메이션 실행함수
    /// </summary>
    public void SetAnim_Push()
    {
        m_animCtrl.SetTrigger("Push");
        m_splashEffect.SetActive(true);
    }
    /// <summary>
    /// (GameState = Play)플레이어 Clear 애니메이션 실행함수
    /// </summary>
    public void SetAnim_Clear()
    {
        InGameManager.uniqueInstance.m_curGameState = InGameManager.eGameState.Clear;

        // === ClearPos로 이동 === //
        playerClearSeq = DOTween.Sequence()
                                .Append(this.transform.DOMove(m_clearTf.position, 1.5f))
                                .Join(this.transform.DORotate(m_clearTf.eulerAngles, 1.5f))
                                .OnComplete(() =>
                                {
                                    m_animCtrl.SetTrigger("Clear");
                                    m_splashEffect.SetActive(false);

                                    // === 눈덩이 Player Obj에서 떼어낸 후 => 굴러가게 === //
                                    m_snowBallObj.transform.SetParent(m_playerParentTf.transform);
                                    m_snowBallObj.transform.DORotate(Vector3.zero, 1f);
                                    // === 눈덩이 Player Obj에서 떼어낸 후 => 굴러가게 === //
                                });
        // === ClearPos로 이동 === //
    }
    /// <summary>
    /// (GameState = End)플레이어 GameOver 애니메이션 실행함수
    /// </summary>
    public void SetAnim_GameOver()
    {
        InGameManager.uniqueInstance.m_curGameState = InGameManager.eGameState.End;

        InGameManager.m_snowBallMgr.StopRotateSnowBall();       // 스노우볼 굴러가는 DoTween Stop
        m_animCtrl.SetTrigger("GameOver");                      // 플레이어 Die 애니메이션 실행
        m_snowBallMeshRdr.enabled = false;                      // 눈덩이 메쉬렌더러 끄기
        m_splashEffect.SetActive(false);                        // 플레이어 달리는 이펙트 끄기
        StopPlayerMoving();                                     // 플레이어 움직임 Stop


        playerGameOverSeq = DOTween.Sequence()
                                   .AppendInterval(1.5f)
                                   .Append(Camera.main.transform.DOLocalMove(new Vector3(0, 20f, -36f), 1f))
                                   .AppendCallback(() =>
                                   {
                                       //StartCoroutine(StartVignette());
                                   });
    }
    private IEnumerator StartVignette()
    {
        if (m_vignette.intensity.value >= 0.6f)
            StopCoroutine(StartVignette());

        m_vignette.intensity.value = Mathf.Lerp(m_vignette.intensity.value, 0.6f, .5f * Time.deltaTime);

        yield return null;

        StartCoroutine(StartVignette());
    }
    #endregion


    #region 플레이어 이동관련
    /// <summary>
    /// (GameState = Play) 플레이어 앞으로 움직이는 코루틴함수
    /// </summary>
    public static bool m_isMoving = false;
    public IEnumerator PlayerMoving()
    {
        if(m_isMoving == true)
        {
            this.transform.Translate(Vector3.forward * m_playerMovSpd, Space.Self);

            yield return null;

            StartCoroutine(PlayerMoving());
        }
    }
    /// <summary>
    /// (GameState = End) 플레이어 게임 Clear/GameOver시 호출되는 함수
    /// </summary>
    public void StopPlayerMoving()
    {
        m_isMoving = false;
        SetAnimSpeedOrigin();
        StopCoroutine(PlayerMoving());
    }


    /// <summary>
    /// (GameState = Play)눈덩이 커질때마다 호출되는 함수 (증가)
    /// </summary>
    public void SetAnimSpeedUp()
    {
        m_animCtrl.speed += 1;
        m_playerMovSpd += 2;
    }
    /// <summary>
    /// (GameState = Clear/End)골인/플레이어죽음 때 호출되는 함수
    /// </summary>
    public void SetAnimSpeedOrigin()
    {
        m_animCtrl.speed = 1;
    }
    #endregion


    #region 플레이어 충돌 이펙트
    public void PlayCrashEffect(int a_num)
    {
        if (a_num == 0)
            m_crashHitEffect.Play();
        else if (a_num == 1)
            m_crashRockEffect.Play();
    }
    #endregion


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("FastZone"))
        {
            playerFastSeq = DOTween.Sequence()
                                   .AppendCallback(() =>
                                   {
                                       InGameManager.m_fastZoneTrail.m_isTrailOn = true;

                                       m_camSplashEffect.Play();
                                       m_playerRunSplashEffect.SetFloat("Speed", 70);
                                       m_playerMovSpd += 4;
                                   })
                                   .AppendInterval(2f)
                                   .OnComplete(() =>
                                   {
                                       InGameManager.m_fastZoneTrail.m_isTrailOn = false;

                                       m_camSplashEffect.Stop();
                                       m_playerRunSplashEffect.SetFloat("Speed", 18);
                                       m_playerMovSpd -= 4;
                                   });
        }
        else if(other.gameObject.layer == LayerMask.NameToLayer("GoalLine"))
        {
            InGameManager.m_snowGround.StopGroundMoving();
            InGameManager.m_snowBallMgr.StopIncreaseSnowBall(true);
            StopPlayerMoving();
            SetAnim_Clear();
        }
    }



}
