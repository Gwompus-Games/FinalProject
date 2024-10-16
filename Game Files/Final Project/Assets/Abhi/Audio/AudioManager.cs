using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;

public class AudioManager : ManagedByGameManager
{
    private List<EventInstance> eventInstances;
    private List<StudioEventEmitter> eventEmitters;

    public override void Init()
    {
        base.Init();
        eventInstances = new List<EventInstance>();
        eventEmitters = new List<StudioEventEmitter>();
    }

    public override void CustomStart()
    {
        base.CustomStart();
        PlayOneShotAttached(GameManager.Instance.GetManagedComponent<FMODEvents>().bgm);
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
        RuntimeManager.PlayOneShotAttached(sound, GameManager.Instance.GetManagedComponent<PlayerController>().gameObject);
    }

    public EventInstance CreateEventInstance(EventReference eventReference)
    {
        EventInstance eventInstance = RuntimeManager.CreateInstance(eventReference);
        eventInstances.Add(eventInstance);
        return eventInstance;
    }

    public void CleanUp()
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
