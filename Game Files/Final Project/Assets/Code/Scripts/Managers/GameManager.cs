using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Needed Components for Game")]
    [SerializeField] private List<ManagedByGameManager> _neededStandaloneScripts = new List<ManagedByGameManager>();
    [SerializeField] private List<GameObject> _neededPrefabs = new List<GameObject>();

    private Transform _playerSpawnPoint;
    private Transform _dungeonSpawnPoint;

    public static GameManager Instance { get; private set; }
    private List<ManagedByGameManager> _managedObjects = new List<ManagedByGameManager>();

    public bool isPaused { get; private set; } = false;

    private void Awake()
    {
        InitilizeGameScene();
    }

    private void InitilizeGameScene()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        //Setting Singleton
        Instance = this;

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

        List<GameObject> neededObjects = new List<GameObject>();
        GameObject managers = new GameObject("Dedicated Managers");
        managers.transform.parent = transform.parent;

        //Adding all needed standalone manager scripts
        for (int ms = 0; ms < _neededStandaloneScripts.Count; ms++)
        {
            GameObject standaloneManager = new GameObject(_neededStandaloneScripts[ms].GetType().Name, _neededStandaloneScripts[ms].GetType());
            standaloneManager.transform.parent = managers.transform;
            neededObjects.Add(standaloneManager);
        }

        //Adding all needed prefabs to the scene
        for (int o = 0; o < _neededPrefabs.Count; o++)
        {
            neededObjects.Add(Instantiate(_neededPrefabs[o]));
        }

        //add all the managers contained within the assigned prefabs
        //adding all canvas related managers
        neededObjects.Add(FindFirstObjectByType<UIManager>().gameObject);
        neededObjects.Add(FindFirstObjectByType<ShopUIManager>().gameObject);
        neededObjects.Add(FindFirstObjectByType<InventoryUI>().gameObject);
        neededObjects.Add(FindFirstObjectByType<BuyingManager>().gameObject);

        //adding all player related managers
        neededObjects.Add(FindFirstObjectByType<CustomPlayerInput>().gameObject);
        neededObjects.Add(FindFirstObjectByType<PlayerController>().gameObject);
        neededObjects.Add(FindFirstObjectByType<SuitSystem>().gameObject);
        neededObjects.Add(FindFirstObjectByType<OxygenSystem>().gameObject);
        neededObjects.Add(FindFirstObjectByType<InventoryController>().gameObject);
        neededObjects.Add(FindFirstObjectByType<HandPositionController>().gameObject);

        //adding other managers
        GameObject fmodGO = FindFirstObjectByType<FMODEvents>().gameObject;
        fmodGO.transform.parent = managers.transform;
        neededObjects.Add(fmodGO);

        GameObject dungGen = FindFirstObjectByType<DungeonGenerator>().gameObject;
        dungGen.transform.position = _dungeonSpawnPoint.position;
        neededObjects.Add(dungGen);

        //Set up all managed objects
        for (int o = 0; o < neededObjects.Count; o++)
        {
            if (neededObjects[o].TryGetComponent<ManagedByGameManager>(out ManagedByGameManager managedObject))
            {
                Setup(managedObject);
            }
        }

        //Initialize all managed objects
        for (int o = 0; o < _managedObjects.Count; o++)
        {
            _managedObjects[o].Init();
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

    private GameObject CustomFindObjectByName(List<GameObject> objects, string nameToFind)
    {
        for (int o = 0; o < objects.Count; o++)
        {
            if (objects[o].name == nameToFind)
            {
                return objects[o];
            }
        }
        return null;
    }

    private GameObject CustomFindObjectByType<T>(List<GameObject> objects)
    {
        for (int o = 0; o < objects.Count; o++)
        {
            if (objects[o].TryGetComponent<T>(out T type))
            {
                return objects[o];
            }
        }
        return null;
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

    public T GetManagedComponent<T>()
    {
        for (int c = 0; c < _managedObjects.Count; c++)
        {
            if (_managedObjects[c].gameObject.TryGetComponent<T>(out T component))
            {
                return component;
            }
        }
        return default(T);
    }

    private void Update()
    {
        //if (Input.GetKeyDown(KeyCode.Escape))
        //{
        //    isPaused = !isPaused;
        //}

        //if (Input.GetMouseButton(0) && Application.isFocused)
        //{
        //    isPaused = false;
        //}
    }
}
