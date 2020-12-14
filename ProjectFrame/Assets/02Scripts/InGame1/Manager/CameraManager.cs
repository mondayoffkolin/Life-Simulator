using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CameraManager : MonoBehaviour
{
    public static CameraManager uniqueInstance;

    [Header("InGameScene")]
    [SerializeField] LayerMask m_doorlayerMask;

    [Header("InGameScene3")]
    [SerializeField] Ease m_cameraShakeEase;

    Sequence camSeq;

    private void Start()
    {
        uniqueInstance = this;
    }

    // Update is called once per frame
    void Update()
    {
        #region InGameSccene
        //if (Input.GetMouseButtonDown(0))
        //{
        //    Vector3 a_mousePos = Input.mousePosition;
        //    Ray ray = Camera.main.ScreenPointToRay(a_mousePos);
        //    RaycastHit hit;

        //    if(Physics.Raycast(ray, out hit, Mathf.Infinity, m_doorlayerMask))
        //    {
        //        print("Door hit");

        //        GameObject a_obj = hit.transform.gameObject;
        //        a_obj.GetComponent<DoorProduction>().DoorOpen();
        //    }
        //}
        #endregion
    }

    public void CamShaking()
    {
        camSeq = DOTween.Sequence()
                        .Append(this.transform.DOMoveY(.85f, 1f).SetEase(Ease.InQuad))
                        .Append(this.transform.DOMoveY(.96f, .7f).SetEase(Ease.OutQuad))
                        .AppendInterval(0.2f)
                        .SetLoops(-1);
    }
    public void StopShaking()
    {
        camSeq.Kill();
    }
}
