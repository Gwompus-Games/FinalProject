using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathAudioComponent : DeathComponent
{
    [SerializeField] private AudioClip[] _startingAudio;
    [SerializeField] private AudioClip[] _endingAudio;
    private AudioManager _audioManager;

    public override void StartDeathComponent()
    {
        _audioManager = GameManager.Instance.GetManagedComponent<AudioManager>();

        if (_startingAudio.Length > 0)
        {
            for (int a = 0; a < _startingAudio.Length; a++)
            {
                //Call playing sound
            }
        }
    }

    public override void UpdateDeathComponent(float normalizedTime)
    {
        
    }

    public override void EndDeathComponent()
    {
        if (_endingAudio.Length > 0)
        {
            for (int a = 0; a < _endingAudio.Length; a++)
            {
                //Call playing sound
            }
        }
    }
}
