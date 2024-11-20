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
    [SerializeField] private GameObject _endScreenManagerPrefab;
    [Header("Debuging")]
    [SerializeField] private bool _debugMode = false;

    private Transform _dungeonSpawnPoint;
    private Transform _submarineSpawnPoint;

    public static GameManager Instance { get; private set; }
    public static Action<GameState> UpdateGameState;

    private DungeonGenerator _dungeonGenerator;

    private SuitSystem _suitSystem;

    private bool _waitingForSubmarineAnimation = false;

    private Coroutine _deathSequence;

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

    public bool isPaused
    {
        get
        {
            return _paused;
        }
        private set
        {
            _paused = value;
            PlayerController playerController = GetManagedComponent<PlayerController>();
            if (playerController != null)
            {
                playerController.SetLockInput(_paused);
            }
        }
    }
    private bool _paused;


    public bool isPlayerInsideFacility = false;

    private void Awake()
    {
        isPaused = false;
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
        _deathSequence = null;
        _standaloneManagers.SetUpList();
        _waitingForSubmarineAnimation = false;
        InitilizeGameScene();
    }

    private void InitilizeGameScene()
    {
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
        if (_neededStandaloneScripts.Count > 0)
        {
            for (int ms = 0; ms < _neededStandaloneScripts.Count; ms++)
            {
                if (_debugMode)
                {
                    Debug.Log($"Creating {_neededStandaloneScripts[ms].GetType().Name}");
                }
                GameObject standaloneManager = new GameObject(_neededStandaloneScripts[ms].GetType().Name, _neededStandaloneScripts[ms].GetType());
                standaloneManager.transform.parent = managersParent.transform;
            }
        }

        //Adding all needed prefabs to the scene
        if (_neededPrefabs.Count > 0)
        {
            for (int o = 0; o < _neededPrefabs.Count; o++)
            {
                Instantiate(_neededPrefabs[o]);
            }
        }

        //Set up all managed objects
        for (int s = 0; s < _standaloneManagers.managedComponents.Count; s++)
        {
            if (_debugMode)
            {
                Debug.Log($"Attempting to find {_standaloneManagers.managedComponents[s]}");
            }
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

        _dungeonGenerator = GetManagedComponent<DungeonGenerator>();
        _suitSystem = GetManagedComponent<SuitSystem>();
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
        if (type == typeof(DungeonGenerator))
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
        if (_managedObjects.Count == 0)
        {
            return null;
        }
        T managedObject = null;
        _managedObjects.Find(x => x.TryGetComponent<T>(out managedObject));
        if (_debugMode && managedObject != null)
        {
            Debug.Log($"{managedObject.GetType()} gotten!");
        }
        return managedObject;
    }

    /// <summary>
    /// This function should only be called from the Submarine and PlayerController scripts
    /// </summary>
    public void ExitLevel(bool takingSubmarine = false)
    {
        if (currentGameState == GameState.InBetweenFacitilies)
        {
            if (_debugMode)
            {
                Debug.Log("ALREADY LEFT THE LEVEL!");
            }
            return;
        }
        if (!takingSubmarine)
        {
            if (_debugMode)
            {
                Debug.Log("NOT TAKING SUB!");
            }
            ApplyGameState(GameState.InBetweenFacitilies);
            return;
        }
        if (_waitingForSubmarineAnimation)
        {
            return;
        }
        StartCoroutine(WaitForSubmarineAnimation(GameState.InBetweenFacitilies));
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
    public void EnterLevel(bool takingSubmarine)
    {
        if (currentGameState == GameState.LandedAtFacility)
        {
            return;
        }

        _dungeonGenerator.StartGeneration();
        _suitSystem.UpdateSectionOnDive();

        if (!takingSubmarine)
        {
            ApplyGameState(GameState.LandedAtFacility);
            return;
        }
        if (_waitingForSubmarineAnimation)
        {
            return;
        }

        StartCoroutine(WaitForSubmarineAnimation(GameState.LandedAtFacility));
    }

    private IEnumerator WaitForSubmarineAnimation(GameState stateToApply)
    {
        if (_debugMode)
        {
            Debug.Log($"Game Manager waiting for submarine animation to play before applying: {stateToApply}");
        }
        _waitingForSubmarineAnimation = true;
        while (_waitingForSubmarineAnimation)
        {
            yield return null;
        }
        ApplyGameState(stateToApply);
    }

    private void ApplyGameState(GameState gameState)
    {
        if (_debugMode)
        {
            Debug.Log($"Applying state {gameState} as current state");
        }
        currentGameState = gameState;
        switch (gameState)
        {
            case GameState.InBetweenFacitilies:
                TreasureSpawnPoint.ResetUniqueTreasures();
                break;
            case GameState.LandedAtFacility:
                
                break;
        }
    }

    public void PlayerDied()
    {
        if (currentGameState == GameState.InBetweenFacitilies)
        {
            return;
        }

        if (_deathSequence != null)
        {
            return;
        }
        _deathSequence = StartCoroutine(PlayerDeadSequence());
    }

    public void EndScene(EndScreenManager.EndState endState)
    {
        EndScreenManager endScreenManager = Instantiate(_endScreenManagerPrefab).GetComponent<EndScreenManager>();
        endScreenManager.SetUpEndState(endState);
    }

    private IEnumerator PlayerDeadSequence()
    {
        Submarine submarine = GetManagedComponent<Submarine>();
        PlayerController player = GetManagedComponent<PlayerController>();
        Camera submarineCamera = submarine.GetDeadCamera();
        yield return null;
        submarineCamera.enabled = true;
        submarine.ExitLevel();
        yield return new WaitForSeconds(0.5f);
        bool waitForSub = true;
        while (waitForSub)
        {
            yield return null;
            waitForSub = _waitingForSubmarineAnimation;
        }
        player.RespawnPlayer();
        submarineCamera.enabled = false;
    }
}
