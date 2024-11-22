using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;
using UnityEngine.SceneManagement;
using static UnityEngine.Rendering.DebugUI;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance; 

    private List<EventInstance> eventInstances;
    private List<StudioEventEmitter> eventEmitters;
    //Music
    private EventInstance bgmInstance, menuInstance;
    //Ambiance
    private EventInstance bark, bells, choir, station;
    public float ambianceTime = 60f;

    public enum Ground
    {
        Sand,
        Metal
    }
    public Ground currentGround;


    #region Scene_Vars
    public string endScene, menuScene, gameScene;
    //public enum SceneName
    //{
    //    Menu,
    //    Game,
    //    End
    //}
    private int currentSceneIndex;
    #endregion

    #region Volume_Bus
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
    #endregion

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
        //Initialize lists 
        eventInstances = new List<EventInstance>();
        eventEmitters = new List<StudioEventEmitter>();
        //Music
        bgmInstance = CreateEventInstance(FMODEvents.Instance.bgm);
        menuInstance = CreateEventInstance(FMODEvents.Instance.menuMusic);
        //Ambiance
        bark = CreateEventInstance(FMODEvents.Instance.bark);
        bells = CreateEventInstance(FMODEvents.Instance.bells);
        choir = CreateEventInstance(FMODEvents.Instance.choir);
        station = CreateEventInstance(FMODEvents.Instance.station);

        UpdateBGM(SceneManager.GetActiveScene().buildIndex);
        InvokeRepeating(nameof(PlayAmbiance), ambianceTime, ambianceTime);
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

    public void PlayAmbiance()
    {
        bark.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        bells.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        choir.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        station.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);

        int rand = Random.Range(0, 4);
        switch(rand)
        {
            case 0:
                bark.start();
                break; 
            case 1:
                bells.start();
                break;             
            case 2: 
                choir.start();
                break; 
            case 3: 
                station.start();
                break;
            default:
                choir.start();
                break;
        }
    }

    #region Public_Functions
    #region UI
    public void OnHover()
    {
        PlayOneShot(FMODEvents.Instance.hover, transform.position);
    }

    public void OnClick()
    {
        PlayOneShot(FMODEvents.Instance.click, transform.position);
    }

    public void OnDenied()
    {
        PlayOneShot(FMODEvents.Instance.denied, transform.position);
    }
    #endregion
    #region Shop
    public void BuySound()
    {
        PlayOneShot(FMODEvents.Instance.buying, transform.position);
    }
    public void SellSound()
    {
        PlayOneShot(FMODEvents.Instance.selling, transform.position);
    }
    public void BrokeSound()
    {
        PlayOneShot(FMODEvents.Instance.broke, transform.position);
    }
    #endregion
    #region Player
    public void OnPickup()
    {
        PlayOneShot(FMODEvents.Instance.pickup, transform.position);
    }
    #endregion
    #endregion

    public void UpdateBGM(int sceneIndex)
    {
        switch (sceneIndex)
        {
            case 0:
                bgmInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
                menuInstance.start();
                break;
            case 1:
                menuInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
                bgmInstance.start();
                break;
            case 2:
                bgmInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
                menuInstance.start();
                break;
            default:
                menuInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
                bgmInstance.start();
                break;
        }
    }

    public void SetInstanceParameter(EventInstance eventInstance, string name, float value)
    {
        eventInstance.setParameterByName(name, value);
    }

    public void StopAllInstances()
    {
        foreach(EventInstance eventInstance in eventInstances)
        {
            eventInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        }
    }

    public void SetInstanceParameter(EventInstance eventInstance, Ground ground)
    {
        if(currentGround == ground)
            return; 

        currentGround = ground;
        int value = 0;
        if(currentGround == Ground.Sand)
        {
            value = 1;
        }
        else if(currentGround == Ground.Metal)
        {
            value = 0;
        }
        else
        {
            Debug.LogError("Wrong layer");
            return;
        }
        eventInstance.setParameterByName("Ground", value);
    }

    public void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        CleanUp();

        switch(scene.buildIndex)
        {
            case 0:
                currentSceneIndex = 0;
                break;
            case 1:
                currentSceneIndex = 1;
                break;
            case 2:
                currentSceneIndex = 2;
                break;
        }        
        UpdateBGM(currentSceneIndex);
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
