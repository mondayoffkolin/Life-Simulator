using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnowBallFootPrint : MonoBehaviour
{
    [SerializeField] FastZoneTrailManager m_fastZoneTrail = null;

    private void OnEnable()
    {
        StartCoroutine(SetDisable());
    }


    private void OnDisable()
    {
        m_fastZoneTrail.EnqueueTrailEffect(this.gameObject);
    }


    WaitForSeconds m_delayTime = new WaitForSeconds(1f);
    private IEnumerator SetDisable()
    {
        yield return m_delayTime;

        this.gameObject.SetActive(false);
    }
}
