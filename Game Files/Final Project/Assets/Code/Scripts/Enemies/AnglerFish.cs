using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;


public class AnglerFish : Enemy
{
    [SerializeField] private float _heartbeatRange = 30;

    private StudioEventEmitter emitter;

    protected override void Awake()
    {
        base.Awake();
    }

    public override void SetupEnemy()
    {
        base.SetupEnemy();

        emitter = GameManager.Instance.GetManagedComponent<AudioManager>().InitializeEventEmitter(GameManager.Instance.GetManagedComponent<FMODEvents>().heartbeat, gameObject);
    }

    private void Update()
    {
        if (_playerController != null)
        {
            if(Vector3.Distance(transform.position, _playerController.transform.position) <= _heartbeatRange)
            {
                IHeartbeat wtfIsThisWorkAround = this;
                wtfIsThisWorkAround.AddHeartbeat(_playerController);
            }
        }
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
