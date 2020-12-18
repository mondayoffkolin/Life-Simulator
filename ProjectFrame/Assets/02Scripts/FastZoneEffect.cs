using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FastZoneEffect : MonoBehaviour
{
    private ParticleSystem m_myEffect = null;


    private void Awake()
    {
        m_myEffect = GetComponent<ParticleSystem>();
    }


    private void OnEnable()
    {
        m_myEffect.Play();
    }


    private void OnDisable()
    {
        InGameManager.m_fastZoneTrail.EnqueueTrailEffect(this.gameObject);
    }
}
