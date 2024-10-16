using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;


public class AnglerFish : Enemy
{
    public static AnglerFish Instance;

    private StudioEventEmitter emitter;

    protected override void Awake()
    {
        base.Awake();
    }

    public override void SetupEnemy()
    {
        Instance = this;

        base.SetupEnemy();

        emitter = GameManager.Instance.GetManagedComponent<AudioManager>().InitializeEventEmitter(GameManager.Instance.GetManagedComponent<FMODEvents>().heartbeat, gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (emitter != null)
                emitter.Play();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (emitter != null)
                emitter.Stop();
        }
    }
}
