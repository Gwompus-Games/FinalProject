using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(StandaloneManagersList))]
public class GameManager : MonoBehaviour
{
    public enum GameState
    {
        InBetweenFacitilies,
        LandedAtFacility
    }

    [Header("Needed Components for Game")]
    [SerializeField] private List<ManagedByGameManager> _neededStandaloneScripts = new List<ManagedByGameManager>();
    [SerializeField] private List<GameObject> _neededPrefabs = new List<GameObject>();
    [Header("Debuging")]
    [SerializeField] private bool _debugMode = false;

    private Transform _playerSpawnPoint;
    private Transform _dungeonSpawnPoint;
    private Transform _submarineSpawnPoint;

    public static GameManager Instance { get; private set; }
    public static Action<GameState> UpdateGameState;

    private DungeonGenerator _dungeonGenerator;

    private bool _waitingForSubmarineAnimation = false;

    public GameState currentGameState 
    { 
        get
        {
            return _curState;
        }
        private set
        {
            _curState = value;
            UpdateGameState?.Invoke(_curState);
        }
    }
    private GameState _curState;
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
        if (_debugMode)
        {
            Debug.Log($"Standalone managers found: {_standaloneManagers != null}");
        }
        _standaloneManagers.SetUpList();
        _waitingForSubmarineAnimation = false;
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

        SubmarineShoppingPoint submarineShoppingPoint = FindObjectOfType<SubmarineShoppingPoint>();
        if (submarineShoppingPoint == null)
        {
            Debug.LogError("No submarine shopping point located in scene! Please place the ShoppingPoint prefab into the scene!");
        }
        else
        {
            _submarineSpawnPoint = submarineShoppingPoint.transform;
        }

        GameObject managersParent = new GameObject("Dedicated Managers");
        managersParent.transform.parent = transform.parent;

        //Adding all needed standalone manager scripts
        for (int ms = 0; ms < _neededStandaloneScripts.Count; ms++)
        {
            if (_debugMode)
            {
                Debug.Log($"Creating {_neededStandaloneScripts[ms].GetType().Name}");
            }
            GameObject standaloneManager = new GameObject(_neededStandaloneScripts[ms].GetType().Name, _neededStandaloneScripts[ms].GetType());
            standaloneManager.transform.parent = managersParent.transform;
        }

        //Adding all needed prefabs to the scene
        for (int o = 0; o < _neededPrefabs.Count; o++)
        {
            Instantiate(_neededPrefabs[o]);
        }

        //Set up all managed objects
        for (int s = 0; s < _standaloneManagers.managedComponents.Count; s++)
        {
            ManagedByGameManager managedScript = FindObjectOfType(_standaloneManagers.managedComponents[s], true).GetComponent(_standaloneManagers.managedComponents[s]) as ManagedByGameManager;
            if (managedScript == null)
            {
                Debug.LogWarning($"No {_standaloneManagers.managedComponents[s].Name} found");
                continue;
            }
            if (_debugMode)
            {
                Debug.Log($"{managedScript.GetType().Name} found");
            }
            if (managedScript.GetType() == typeof(DungeonGenerator))
            {
                _dungeonGenerator = managedScript as DungeonGenerator;
            }
            Setup(managedScript, managersParent);
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
        UnityEngine.Object[] objects = FindObjectsByType(typeof(T), FindObjectsSortMode.None);
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

    private void Setup(ManagedByGameManager managedScript, GameObject managersParent)
    {
        _managedObjects.Add(managedScript);
        if (_standaloneManagers.standaloneManagers.Contains(managedScript.GetType()) &&
                managedScript.transform.parent != managersParent.transform)
        {
            managedScript.transform.parent = managersParent.transform;
        }
        Type type = managedScript.GetType();
        if (type == typeof(PlayerController))
        {
            PlayerController player = managedScript as PlayerController;
            if (player != null && _playerSpawnPoint != null)
            {
                player.TeleportPlayer(_playerSpawnPoint.transform.position);
            }
        }
        else if (type == typeof(DungeonGenerator))
        {
            managedScript.transform.position = _dungeonSpawnPoint.position;
        }
        else if (type == typeof(Submarine) && _submarineSpawnPoint != null)
        {
            managedScript.transform.position = _submarineSpawnPoint.position;
        }

    }

    public T GetManagedComponent<T>() where T : ManagedByGameManager
    {
        T managedObject = _managedObjects.Find(x => x.GetComponent<T>() != null).GetComponent<T>();
        if (_debugMode)
        {
            //Debug.Log($"{managedObject.GetType()} gotten!");
        }
        return managedObject;
    }

    /// <summary>
    /// This function should only be called from the Submarine and PlayerController scripts
    /// </summary>
    public void ExitLevel(bool takingSubmarine = false)
    {
        if (!takingSubmarine)
        {
            ApplyGameState(GameState.InBetweenFacitilies);
            return;
        }
        if (_waitingForSubmarineAnimation)
        {
            return;
        }
        StartCoroutine(WaitForSubmarineAnimation());
    }

    /// <summary>
    /// This function should only be called from the Submarine script and only
    /// from the animation coroutine
    /// </summary>
    public void SubmarineAnimationFinished()
    {
        _waitingForSubmarineAnimation = false;
    }

    /// <summary>
    /// This function should only be called from the Submarine script
    /// </summary>
    public void EnterLevel()
    {
        ApplyGameState(GameState.LandedAtFacility);
    }

    private IEnumerator WaitForSubmarineAnimation()
    {
        _waitingForSubmarineAnimation = true;
        while (_waitingForSubmarineAnimation)
        {
            yield return null;
        }
        ApplyGameState(GameState.InBetweenFacitilies);
    }

    private void ApplyGameState(GameState gameState)
    {
        currentGameState = gameState;
        switch (gameState)
        {
            case GameState.InBetweenFacitilies:
                break;
            case GameState.LandedAtFacility:
                _dungeonGenerator.StartGeneration();
                break;
        }
    }
}
