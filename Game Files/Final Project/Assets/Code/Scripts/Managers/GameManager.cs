using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(StandaloneManagersList))]
public class GameManager : MonoBehaviour
{
    [Header("Needed Components for Game")]
    [SerializeField] private List<ManagedByGameManager> _neededStandaloneScripts = new List<ManagedByGameManager>();
    [SerializeField] private List<GameObject> _neededPrefabs = new List<GameObject>();
    [Header("Debuging")]
    [SerializeField] private bool _debugMode = false;

    private Transform _playerSpawnPoint;
    private Transform _dungeonSpawnPoint;

    public static GameManager Instance { get; private set; }
    private List<ManagedByGameManager> _managedObjects = new List<ManagedByGameManager>();
    private StandaloneManagersList _standaloneManagers;

    public bool isPaused { get; private set; } = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        //Setting Singleton
        Instance = this;
        _standaloneManagers = GetComponent<StandaloneManagersList>();
        _standaloneManagers.SetUpList();
        InitilizeGameScene();
    }

    private void InitilizeGameScene()
    {
        PlayerSpawnPointTag playerSpawnPoint = FindObjectOfType<PlayerSpawnPointTag>();
        if (playerSpawnPoint == null)
        {
            playerSpawnPoint = new GameObject("PlayerSpawnPoint", typeof(PlayerSpawnPointTag)).GetComponent<PlayerSpawnPointTag>();
            playerSpawnPoint.gameObject.transform.parent = null;
            playerSpawnPoint.gameObject.transform.position = Vector3.zero;
        }
        _playerSpawnPoint = playerSpawnPoint.transform;

        DungeonGeneratorSpawnPointTag dungeonGeneratorSpawnPoint = FindObjectOfType<DungeonGeneratorSpawnPointTag>();
        if (dungeonGeneratorSpawnPoint == null)
        {
            dungeonGeneratorSpawnPoint = new GameObject("FacilityGeneratorSpawnPoint", typeof(DungeonGeneratorSpawnPointTag)).GetComponent<DungeonGeneratorSpawnPointTag>();
            dungeonGeneratorSpawnPoint.gameObject.transform.parent = null;
            dungeonGeneratorSpawnPoint.gameObject.transform.position = Vector3.zero;

        }
        _dungeonSpawnPoint = dungeonGeneratorSpawnPoint.transform;

        GameObject managers = new GameObject("Dedicated Managers");
        managers.transform.parent = transform.parent;

        //Adding all needed standalone manager scripts
        for (int ms = 0; ms < _neededStandaloneScripts.Count; ms++)
        {
            GameObject standaloneManager = new GameObject(_neededStandaloneScripts[ms].GetType().Name, _neededStandaloneScripts[ms].GetType());
            standaloneManager.transform.parent = managers.transform;
        }

        //Adding all needed prefabs to the scene
        for (int o = 0; o < _neededPrefabs.Count; o++)
        {
            Instantiate(_neededPrefabs[o]);
        }

        //Set up all managed objects
        for (int s = 0; s < _standaloneManagers.managedChildren.Count; s++)
        {
            ManagedByGameManager managedScript = FindObjectOfType(_standaloneManagers.managedChildren[s], true).GetComponent(_standaloneManagers.managedChildren[s]) as ManagedByGameManager;
            if (managedScript == null)
            {
                Debug.LogWarning($"No {_standaloneManagers.managedChildren[s].Name} found");
                continue;
            }
            if (_debugMode)
            {
                Debug.Log($"{managedScript.GetType().Name} found");
            }
            if (_standaloneManagers.standaloneManagers.Contains(managedScript.GetType()) &&
                managedScript.transform.parent != managers.transform)
            {
                managedScript.transform.parent = managers.transform;
            }

            if (managedScript.GetType() == typeof(DungeonGenerator))
            {
                managedScript.transform.position = _dungeonSpawnPoint.position;
            }

            Setup(managedScript);
        }

        if (_debugMode)
        {
            Debug.Log($"INITIALIZING: {_managedObjects.Count}");
        }

        //Initialize all managed objects
        for (int o = 0; o < _managedObjects.Count; o++)
        {
            _managedObjects[o].Init();
            if (_debugMode)
            {
                Debug.Log($"{_managedObjects[o].GetType().Name} initialized");
            }
        }

        GetManagedComponent<PlayerController>().TeleportPlayer(_playerSpawnPoint.position);
    }

    private void Start()
    {
        StartGameScene();
    }

    private void StartGameScene()
    {
        //Call all managed objects' Custom Start Function
        for (int i = 0; i < _managedObjects.Count; i++)
        {
            _managedObjects[i].CustomStart();
        }
    }

    private T FindFirstObjectAndDestroyOthers<T>()
    {
        Object[] objects = FindObjectsByType(typeof(T), FindObjectsSortMode.None);
        GameObject[] gameObjects = new GameObject[objects.Length];
        int index = 0;
        for (int o = 0; o < objects.Length; o++)
        {
            if (objects[o].GameObject() != null)
            {
                gameObjects[index] = objects[o].GameObject();
                index++;
            }
        }
        if (gameObjects.Length == 0)
        {
            return default(T);
        }
        if (gameObjects.Length != 1)
        {
            for (int i = gameObjects.Length; i >= 1; i++)
            {
                DestroyImmediate(gameObjects[i].gameObject);
            }
        }
        return gameObjects[0].GetComponent<T>();
    } 

    private void Setup(ManagedByGameManager managedObject)
    {
        _managedObjects.Add(managedObject);
    }

    public T GetManagedComponent<T>() where T : ManagedByGameManager
    {
        return _managedObjects.Find(x => x.GetComponent<T>() != null).GetComponent<T>();
    }
}
