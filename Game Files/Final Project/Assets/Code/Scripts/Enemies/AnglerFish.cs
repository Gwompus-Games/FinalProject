using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;


public class AnglerFish : Enemy
{
    [SerializeField] private float _heartbeatRange = 30;
    private string parameterName = "Heartbeat_Intensity";
    private float parameterIntensity = 0f, distanceFromPlayer;
    public bool playHeartbeat = false;

    protected override void Awake()
    {
        base.Awake();
    }

    public override void SetupEnemy()
    {
        base.SetupEnemy();
    }

    private void Update()
    {
        if (_playerController != null)
        {
            distanceFromPlayer = Vector3.Distance(transform.position, _playerController.transform.position);
            if (distanceFromPlayer <= _heartbeatRange)
            {
                //uh
                IHeartbeat wtfIsThisWorkAround = this;
                wtfIsThisWorkAround.AddHeartbeat(_playerController);

                //set parameter intensity in fmod
                parameterIntensity = ((_heartbeatRange - distanceFromPlayer)/_heartbeatRange);
                GameManager.Instance.GetManagedComponent<AudioManager>().SetHeartbeatParameter(parameterName, parameterIntensity);
                playHeartbeat = true;
            }
            else
            {
                playHeartbeat = false;
            }
        }
    }
}
