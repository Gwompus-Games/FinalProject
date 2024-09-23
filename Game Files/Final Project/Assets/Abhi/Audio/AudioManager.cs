using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;
    public EventReference test;

    private void Awake()
    {
        if(instance != null)
        {
            Debug.LogError("Found more than one audio manager in the scene.");
        }
        instance = this;
    }

    public void PlayOneShot(EventReference sound, Vector3 worldPos)
    {
        RuntimeManager.PlayOneShot(sound, worldPos);
    }

    private void Update()
    {
        if(Input.GetKeyUp(KeyCode.T))
        {
            Test();
        }   
    }

    public void Test()
    {
        PlayOneShot(FMODEvents.instance.test, transform.position);
    }
}
