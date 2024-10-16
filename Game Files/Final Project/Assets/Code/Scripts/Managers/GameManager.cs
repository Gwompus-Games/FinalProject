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

        MonoBehaviour[] objects = FindObjectsByType<ManagedByGameManager>(FindObjectsSortMode.None);
        List<GameObject> gameObjects = new List<GameObject>();
        List<ManagedByGameManager> managedScripts = new List<ManagedByGameManager>();
        for (int o = 0; o < objects.Length; o++)
        {
            if (gameObjects.Contains(objects[o].gameObject))
            {
                continue;
            }
            gameObjects.Add(objects[o].gameObject);
            ManagedByGameManager[] scripts = objects[o].GetComponents<ManagedByGameManager>();
            for (int s = 0; s < scripts.Length; s++)
            {
                if (managedScripts.Contains(scripts[s]))
                {
                    continue;
                }
                managedScripts.Add(scripts[s]);
            }
        }

        //Set up all managed objects
        for (int s = 0; s < managedScripts.Count; s++)
        {
            ManagedByGameManager managedComponent = managedScripts[s].GetComponent<ManagedByGameManager>();
            if (_standaloneManagers.standaloneManagers.Contains(managedComponent.GetType()) &&
                managedComponent.transform.parent != managers.transform)
            {
                managedComponent.transform.parent = managers.transform;
            }

            if (managedComponent.GetType() == typeof(DungeonGenerator))
            {
                managedComponent.transform.position = _dungeonSpawnPoint.position;
            }

            Setup(managedComponent);
        }

        Debug.Log($"INITIALIZING: {_managedObjects.Count}");

        //Initialize all managed objects
        for (int o = 0; o < _managedObjects.Count; o++)
        {
            _managedObjects[o].Init();
            Debug.Log($"{_managedObjects[o].GetType().Name} initialized");
        }

        GetManagedComponent<PlayerController>().TeleportPlayer(_playerSpawnPoint.position);

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
