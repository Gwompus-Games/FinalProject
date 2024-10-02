using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;


public class AnglerFish : Enemy
{
    private StudioEventEmitter emitter;

    private void Start()
    {
        emitter = AudioManager.instance.InitializeEventEmitter(FMODEvents.instance.heartbeat, gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            emitter.Play();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            emitter.Stop();
        }
    }
}
