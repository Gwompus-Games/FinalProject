using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;


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
            //set parameter intensity in fmod
            bool playHeartbeat = false;         
            if(player.currentState == PlayerController.PlayerState.Dying)
            {
                AudioManager.Instance.SetInstanceParameter(player.playerHeartbeat, parameterName, 0);
                player.playerHeartbeat.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
                player.playerFootsteps.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
                playHeartbeat = false;
                return;
            }
            distanceFromPlayer = Vector3.Distance(transform.position, _playerController.transform.position);
            parameterIntensity = ((_heartbeatRange - distanceFromPlayer) / _heartbeatRange);
            playHeartbeat = distanceFromPlayer <= _heartbeatRange ? true : false;

            if (playHeartbeat)
            {
                AudioManager.Instance.SetInstanceParameter(player.playerHeartbeat, parameterName, parameterIntensity);
                AddHeartbeat();                
            }
            else
            {
                AudioManager.Instance.SetInstanceParameter(player.playerHeartbeat, parameterName, 0);
                RemoveHeartbeat();
            }
        }
    }
}
