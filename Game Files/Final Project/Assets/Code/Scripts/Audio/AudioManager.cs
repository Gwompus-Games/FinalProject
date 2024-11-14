using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance; 

    private List<EventInstance> eventInstances;
    private List<StudioEventEmitter> eventEmitters;

    private EventInstance heartbeatInstance;
    private EventInstance bgmInstance, menuInstance;

    public string endScene, menuScene, gameScene;
    public enum SceneName
    {
        Menu,
        Game,
        End
    }
    public SceneName currentScene;
    
    public enum VolumeBus
    {
        Master,
        Music,
        SFX,
        UI
    }

    [Header("Volume Control")]
    [Range(0,1)]
    public float masterVolume = 1;
    [Range(0,1)]
    public float musicVolume = 1;
    [Range(0,1)]
    public float sfxVolume = 1;
    [Range(0,1)]
    public float uiVolume = 1;

    private Bus masterBus;
    private Bus musicBus;
    private Bus sfxBus;
    private Bus uiBus;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        eventInstances = new List<EventInstance>();
        eventEmitters = new List<StudioEventEmitter>();

        masterBus = RuntimeManager.GetBus("bus:/");
        musicBus = RuntimeManager.GetBus("bus:/Music");
        sfxBus = RuntimeManager.GetBus("bus:/SFX");
        uiBus = RuntimeManager.GetBus("bus:/UI");
    }

    private void Start()
    {
        heartbeatInstance = CreateEventInstance(FMODEvents.Instance.heartbeat); 
        eventInstances = new List<EventInstance>();
        eventEmitters = new List<StudioEventEmitter>();
        bgmInstance = CreateEventInstance(FMODEvents.Instance.bgm);
        menuInstance = CreateEventInstance(FMODEvents.Instance.menuMusic);
        //menuInstance.start();
        UpdateBGM(SceneName.Menu);
    }

    public void ChangeVolume(VolumeBus bus, float value)
    {
        switch (bus)
        {
            case VolumeBus.Master:
                masterVolume = Mathf.Clamp(value, 0 , 1);
                masterBus.setVolume(masterVolume);
                break;
            case VolumeBus.Music:
                musicVolume = Mathf.Clamp(value, 0, 1);
                musicBus.setVolume(musicVolume);
                break;
            case VolumeBus.SFX:
                sfxVolume = Mathf.Clamp(value, 0 , 1);
                sfxBus.setVolume(sfxVolume);
                break;
            case VolumeBus.UI:
                uiVolume = Mathf.Clamp(value, 0 , 1);
                uiBus.setVolume(uiVolume);
                break;
            default: Debug.LogWarning("Bruh");
                break;
        }
    }

    public void OnHover()
    {
        PlayOneShot(FMODEvents.Instance.hover, transform.position);
    }

    public void OnClick()
    {
        PlayOneShot(FMODEvents.Instance.click, transform.position);
    }

    public void UpdateBGM(SceneName scene)
    {
        switch (scene)
        {
            case SceneName.Menu:
                bgmInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
                menuInstance.start();
                break;
            case SceneName.Game:
                menuInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
                bgmInstance.start();
                break;
            case SceneName.End:
                bgmInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
                menuInstance.start();
                break;
        }
    }

    public void SetHeartbeatParameter(string name, float value)
    {
        heartbeatInstance.setParameterByName(name, value);
    }
    public void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == endScene)
        {
            currentScene = SceneName.End;
        }
        if (scene.name == gameScene)
        {
            currentScene = SceneName.Game;
        }
        if (scene.name == menuScene)
        {
            currentScene = SceneName.Menu;
        }
        UpdateBGM(currentScene);
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    #region Helper_Functions
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
        if(eventInstances.Count > 0)
        {
            foreach(EventInstance eventInstance in eventInstances)
            {
                eventInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
                eventInstance.release();
            }
        }
        if(eventEmitters.Count > 0)
        {
            foreach(StudioEventEmitter emitter in eventEmitters)
            {
                emitter.Stop();
            }
        }
    }

    //private void OnDestroy()
    //{
    //    CleanUp();
    //}
#endregion
}
