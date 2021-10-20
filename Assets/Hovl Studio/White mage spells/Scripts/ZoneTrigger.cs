using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZoneTrigger : MonoBehaviour
{
    public ParticleSystem stableEffect;
    public ParticleSystem triggerEffect;
    public float repeatTime = 20;
    private bool canRepear = true;

    private AudioClip clip;
    private AudioSource soundComponent;

    void Start()
    {
        if (soundComponent == null)
        {
            soundComponent = GetComponent<AudioSource>();
            clip = soundComponent.clip;
        }
        stableEffect.Play();
    }

    void OnTriggerEnter(Collider other)
    {
        if (canRepear)
        {
            canRepear = false;
            stableEffect.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            triggerEffect.Play();
            soundComponent.PlayOneShot(clip);
            StartCoroutine(ExecuteAfterTime());
        }
    }

    IEnumerator ExecuteAfterTime()
    {
        yield return new WaitForSeconds(repeatTime);
        canRepear = true;
        Start();
    }
}
