using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;
    private List<EventInstance> eventInstances;
    private List<StudioEventEmitter> eventEmitters;

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("Found more than one audio manager in the scene.");
        }
        instance = this;


        eventInstances = new List<EventInstance>();
        eventEmitters = new List<StudioEventEmitter>();
    }

    private void Start()
    {
        PlayOneShotAttached(FMODEvents.instance.bgm);
    }

    public StudioEventEmitter InitializeEventEmitter(EventReference eventReference, GameObject emitterSource)
    {
        StudioEventEmitter emitter = emitterSource.GetComponent<StudioEventEmitter>();
        emitter.EventReference = eventReference;
        eventEmitters.Add(emitter);
        return emitter;
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
        RuntimeManager.PlayOneShotAttached(sound, PlayerController.INSTANCE.gameObject);
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
        foreach(StudioEventEmitter emitter in eventEmitters)
        {
            emitter.Stop();
        }
    }

    private void OnDestroy()
    {
        CleanUp();
    }
}
