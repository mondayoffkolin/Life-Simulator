using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnowBallFootPrint : MonoBehaviour
{
    [SerializeField] bool m_isOnTheSnow = true;
    [SerializeField] FastZoneTrailManager m_fastZoneTrail = null;

    private void OnEnable()
    {
        StartCoroutine(SetDisable());
    }


    private void OnDisable()
    {
        if(m_isOnTheSnow == true)
            m_fastZoneTrail.EnqueueTrailOnSnowEffect(this.gameObject);
        else
            m_fastZoneTrail.EnqueueTrailOnSoilEffect(this.gameObject);
    }


    WaitForSeconds m_delayTime = new WaitForSeconds(1.3f);
    private IEnumerator SetDisable()
    {
        yield return m_delayTime;

        this.gameObject.SetActive(false);
    }
}
