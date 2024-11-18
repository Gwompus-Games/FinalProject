using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;


public class AnglerFish : Enemy
{
    [SerializeField] private float _heartbeatRange = 30;
    private string parameterName = "Heartbeat_Intensity";
    private float parameterIntensity = 0f, distanceFromPlayer;
    private PlayerController player;

    protected override void Awake()
    {
        base.Awake();
    }

    public override void SetupEnemy()
    {
        base.SetupEnemy();
        player = GameManager.Instance.GetManagedComponent<PlayerController>();
    }

    private void Update()
    {
        if (_playerController != null)
        {
            distanceFromPlayer = Vector3.Distance(transform.position, _playerController.transform.position);
            if (distanceFromPlayer <= _heartbeatRange)
            {
                //uh
                AddHeartbeat();

                //set parameter intensity in fmod
                parameterIntensity = ((_heartbeatRange - distanceFromPlayer)/_heartbeatRange);
                AudioManager.Instance.SetInstanceParameter(player.playerHeartbeat, parameterName, parameterIntensity);
            }
            else
            {
                RemoveHeartbeat();
            }
        }
    }
}
