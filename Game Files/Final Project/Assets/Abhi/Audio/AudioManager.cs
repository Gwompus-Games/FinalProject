using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;
    private List<EventInstance> eventInstances = new List<EventInstance>();

    private void Awake()
    {
        if(instance != null)
        {
            Debug.LogError("Found more than one audio manager in the scene.");
        }
        instance = this;
    }

    private void Start()
    {
        PlayOneShotAttached(FMODEvents.instance.bgm);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.H))
        {
            PlayOneShot(FMODEvents.instance.heartbeat, PlayerController.Instance.transform.position);
        }
    }

    public void PlayOneShot(EventReference sound, Vector3 worldPos)
    {
        RuntimeManager.PlayOneShot(sound, worldPos);
    }
    
    public void PlayOneShotAttached(EventReference sound, GameObject source)
    {
        RuntimeManager.PlayOneShotAttached(sound, source);
    }
    
    public void PlayOneShotAttached(EventReference sound)
    {
        RuntimeManager.PlayOneShotAttached(sound, PlayerController.Instance.gameObject);
    }

    public EventInstance CreateEventInstance(EventReference eventReference)
    {
        EventInstance eventInstance = RuntimeManager.CreateInstance(eventReference);
        eventInstances.Add(eventInstance);
        return eventInstance;
    }

    private void CleanUp()
    {
        foreach(EventInstance eventInstance in eventInstances)
        {
            eventInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            eventInstance.release();
        }
    }

    private void OnDestroy()
    {
        CleanUp();
    }
}
