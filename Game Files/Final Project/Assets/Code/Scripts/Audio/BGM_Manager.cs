using UnityEngine;
using UnityEngine.SceneManagement;
using FMODUnity;
using FMOD.Studio;
using System.Collections.Generic;
using Unity.VisualScripting;

public class BGM_Manager : MonoBehaviour
{
    public static BGM_Manager Instance;
    public string endScene, menuScene, gameScene;

    [field: Header("BGM")]
    [field: SerializeField] public EventReference bgm { get; private set; }
    [field: SerializeField] public EventReference menuMusic { get; private set; }

    [field: Header("UI")]
    [field: SerializeField] public EventReference hover { get; private set; }
    [field: SerializeField] public EventReference click { get; private set; }

    private EventInstance bgmInstance, menuInstance;

    public enum SceneName
    {
        Menu,
        Game,
        End
    }
    public SceneName currentScene;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        //if (!bgmInstance.isValid())
        //    bgmInstance = CreateEventInstance(bgm);
        //if (!menuInstance.isValid())
        //    menuInstance = CreateEventInstance(menuMusic);
        //CleanUp();
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

    public void PlayOneShot(EventReference sound, Vector3 worldPos)
    {
        RuntimeManager.PlayOneShot(sound, worldPos);
    }

    private void UpdateBGM(SceneName scene)
    {
        switch(scene)
        {
            case SceneName.Menu:
                //RuntimeManager.PlayOneShot(menuMusic, transform.position);
                bgmInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
                menuInstance.start();
                break;
            case SceneName.Game:
                //RuntimeManager.PlayOneShotAttached(bgm, GameManager.Instance.GetManagedComponent<PlayerController>().gameObject);
                menuInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
                bgmInstance.start();
                break;
            case SceneName.End:
                //RuntimeManager.PlayOneShot(menuMusic, transform.position);
                bgmInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
                menuInstance.start();
                break;
        }
    }

    public void OnHover()
    {
        PlayOneShot(hover, transform.position);
    }
    
    public void OnClick()
    {
        PlayOneShot(click, transform.position);
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        CleanUp();
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private List<EventInstance> eventInstances;
    private List<StudioEventEmitter> eventEmitters;

    private void Start()
    {
        eventInstances = new List<EventInstance>();
        eventEmitters = new List<StudioEventEmitter>();
        bgmInstance = CreateEventInstance(bgm);
        menuInstance = CreateEventInstance(menuMusic);
        menuInstance.start();
    }

    public StudioEventEmitter InitializeEventEmitter(EventReference eventReference, GameObject emitterSource)
    {
        StudioEventEmitter emitter = emitterSource.GetComponent<StudioEventEmitter>();
        emitter.EventReference = eventReference;
        eventEmitters.Add(emitter);
        return emitter;
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
        if(eventInstances != null)
        {
            foreach (EventInstance eventInstance in eventInstances)
            {
                eventInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
                eventInstance.release();
            }
        }
        if(eventEmitters != null)
        {
            foreach (StudioEventEmitter emitter in eventEmitters)
            {
                emitter.Stop();
            }
        }
    }
}