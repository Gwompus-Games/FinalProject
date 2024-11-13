using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using FMOD.Studio;
using FMODUnity;
using UnityEngine.SceneManagement;
using TMPro;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(OxygenSystem))]
[RequireComponent(typeof(SuitSystem))]
[RequireComponent(typeof(StudioEventEmitter))]
[RequireComponent(typeof(DeathHandler))]
[RequireComponent(typeof(ToolController))]
public class PlayerController : ManagedByGameManager
{
    public static Action<int> UpdateMoney;
    public static Action<Vector3> PlayerPosUpdated;

    public enum PlayerState
    {
        Idle,
        Walking,
        Running,
        Inventory,
        Paused,
        Dying
    }

    [Header("Debug Settings")]
    [SerializeField] private bool _debugMode = false;

    [Header("Movement Settings")]
    [SerializeField] private float _walkSpeed = 3f;
    [SerializeField] private float _runSpeed = 5f;
    [SerializeField] private float _gravity = -2f;
    [SerializeField] [Range(1f, 5f)] private float _runningOxygenDrainMultiplier = 2f;
    [SerializeField] private float _secondsToRunSpeed = 1.5f;

    [Header("Grounded Settings")]
    [SerializeField] private LayerMask _groundMask;
    [SerializeField] private Transform _groundCheck;
    [SerializeField] private float _groundCheckRadius = 0.4f;

    [Header("Interacting Settings")]
    [SerializeField] private float _grabLength = 2f;
    private IInteractable _interactableLookingAt;

    [Header("Money Settings")]
    [SerializeField] private int _startingMoney;
    [SerializeField] public int money
    {
        get
        {
            return _money;
        }
        private set
        {
            _money = value;
            UpdateMoney?.Invoke(_money);
        }
    }
    private int _money;

    [Header("Oxygen Settings")]
    [SerializeField] private float _bufferSecondsFromNoOxygen = 10f;
    private bool _outOfOxygen = false;

    public PlayerState currentState { get; private set; } = PlayerState.Idle;
    public bool isGrounded { get; private set; }
    public float moveSpeed { get; private set; }
    private float _targetMoveSpeed;
    public bool isRunning { get; private set; }
    public bool lockedInput { get; private set; } = false;
    public bool onSub { get; private set; } = true;

    private CharacterController _controller;

    private Vector3 _movement;
    private Vector3 _velocity;

    private float _defaultStepOffset;

    private Vector2 _movementInput = Vector2.zero;
    private Vector3 _lastMoveDirection = Vector2.zero;
    private Vector3 _lastPosition;
    private Transform _headTransform;

    public OxygenSystem oxygenSystem { get; private set; }
    public SuitSystem suitSystem { get; private set; }
    public OxygenDrainer runningDrainer { get; private set; }
    public InventoryController inventoryController { get; private set; }
    public UIManager uiManager { get; private set; }
    public DeathHandler deathHandler { get; private set; }


    private EventInstance playerFootsteps;
    private EventInstance playerHeartbeat;

    private Coroutine _oxygenOutCoroutine;
    private Coroutine _dyingCoroutine;
    
    private List<IHeartbeat> _heartbeatElements = new List<IHeartbeat>();

    private Transform _playerSpawnPoint;

    public override void Init()
    {
        if (_initilized)
        {
            return;
        }
        base.Init();
        if (_debugMode)
        {
            Debug.Log("Player Has been initilized.");
        }
        runningDrainer = gameObject.AddComponent<OxygenDrainer>();
        runningDrainer.SetDrainMultiplier(_runningOxygenDrainMultiplier);
        suitSystem = GetComponent<SuitSystem>();
        oxygenSystem = GetComponent<OxygenSystem>();
        if (_debugMode)
        {
            Debug.Log($"{oxygenSystem} gotten for {gameObject.name}!");
        }
        _controller = GetComponent<CharacterController>();
        _headTransform = GetComponentInChildren<CameraLook>().transform;
        inventoryController = GameManager.Instance.GetManagedComponent<InventoryController>();
        uiManager = GameManager.Instance.GetManagedComponent<UIManager>();
        deathHandler = GameManager.Instance.GetManagedComponent<DeathHandler>();
        lockedInput = false;
    }

    public override void CustomStart()
    {
        base.CustomStart();

        PlayerSpawnPointTag playerSpawnPoint = FindObjectOfType<PlayerSpawnPointTag>();
        if (playerSpawnPoint == null)
        {
            throw new Exception("No spawn point in scene!");
        }
        _playerSpawnPoint = playerSpawnPoint.transform;

        _defaultStepOffset = _controller.stepOffset;
        moveSpeed = _walkSpeed;
        isRunning = false;
        CloseInventory();
        money = _startingMoney;
        _outOfOxygen = false;

        playerFootsteps = GameManager.Instance.GetManagedComponent<AudioManager>().CreateEventInstance(GameManager.Instance.GetManagedComponent<FMODEvents>().footsteps);
        playerHeartbeat = GameManager.Instance.GetManagedComponent<AudioManager>().CreateEventInstance(GameManager.Instance.GetManagedComponent<FMODEvents>().heartbeat);

        TeleportPlayer(_playerSpawnPoint.transform.position);
        Camera.main.gameObject.GetComponent<StudioListener>().attenuationObject = gameObject;
    }

    private void Update()
    {
        if (currentState == PlayerState.Dying)
        {
            return;
        }

        if (_lastPosition != transform.position)
        {
            PlayerPosUpdated?.Invoke(_headTransform.position);
            _lastPosition = transform.position;
        }

        // Take player inputs
        MovementInput();

        //Check for looking at an Interactable
        CheckIfLookingAtInteractable();

        // Update player state and apply state logic
        UpdateState();
        ApplyState();

        // Calculate new movespeed based on acceleration
        CalculateMoveSpeed();

        // Apply player movement
        ApplyMovement();

        // Check for jump input and calculate then apply gravity
        CalculateGravity();
        ApplyGravity();

        //FMOD
        UpdateSound();
    }

    /// <summary>
    /// ONLY USE THIS FUNCTION IN THE GAMEMANAGER!!!!
    /// </summary>
    public void SetLockInput(bool locked)
    {
        lockedInput = locked;
    }

    private void UpdateSound()
    {
        playerFootsteps.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(gameObject));
        if (_movement.magnitude != 0 && isGrounded)
        {
            PLAYBACK_STATE playbackState;
            playerFootsteps.getPlaybackState(out playbackState);
            if(playbackState.Equals(PLAYBACK_STATE.STOPPED))
            {
                playerFootsteps.start();
            }
        }
        else
        {
            playerFootsteps.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        }

        playerHeartbeat.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(gameObject));
        if (_debugMode)
        {
            Debug.Log($"Number of heartbeat elements: {_heartbeatElements.Count}");
        }
        if (_heartbeatElements.Count == 0)
        {
            playerHeartbeat.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        }
        else
        {
            PLAYBACK_STATE playbackState;
            playerHeartbeat.getPlaybackState(out playbackState);
            if (playbackState.Equals(PLAYBACK_STATE.STOPPED))
            {
                playerHeartbeat.start();
            }
        }
    }

    private void UpdateState()
    {
        if (currentState == PlayerState.Inventory ||
            currentState == PlayerState.Paused || 
            currentState == PlayerState.Dying)
        {
            return;
        }
        if (_movement.magnitude == 0)
        {
            ChangeState(PlayerState.Idle);
        }
        else if (isRunning)
        {
            ChangeState(PlayerState.Running);
        }
        else
        {
            ChangeState(PlayerState.Walking);
        }
    }

    private void ApplyState()
    {
        switch (currentState)
        {
            case PlayerState.Idle:
                _targetMoveSpeed = 0;
                break;
            case PlayerState.Walking:
                _targetMoveSpeed = _walkSpeed;
                break;
            case PlayerState.Running:
                _targetMoveSpeed = _runSpeed;
                break;
            default:
                _targetMoveSpeed = 0;
                break;
        }
    }

    public void ChangeState(PlayerState newState)
    {
        if (currentState == PlayerState.Running)
        {
            runningDrainer.DeactivateDrainer();
        }
        if (newState == PlayerState.Running)
        {
            runningDrainer.ActivateDrainer();
        }

        currentState = newState;
    }

    private void MovementInput()
    {
        bool oldIsGrounded = isGrounded;

        isGrounded = Physics.CheckSphere(_groundCheck.position, _groundCheckRadius, _groundMask);

        if (isGrounded != oldIsGrounded)
        {
            IsGroundedChanged();
        }

        if (isGrounded)
        {
            _velocity.y = -1.5f;
        }

        if (_movement != Vector3.zero)
        {
            _lastMoveDirection = _movement;
        }
        _movement = (transform.right * _movementInput.x + transform.forward * _movementInput.y);
    }

    private void CalculateMoveSpeed()
    {
        int accelerationDirection;
        if (moveSpeed == _targetMoveSpeed)
        {
            accelerationDirection = 0;
        }
        else
        {
            accelerationDirection = MathF.Sign(_targetMoveSpeed - moveSpeed);
        }
        float moveMod = (_runSpeed / _secondsToRunSpeed) * Time.deltaTime;
        if (accelerationDirection != 0)
        {
            moveSpeed = moveSpeed + (moveMod * accelerationDirection);
        }
        switch (accelerationDirection)
        {
            case 1:
                moveSpeed = Mathf.Clamp(moveSpeed, 0, _targetMoveSpeed);
                break;
            case -1:
                moveSpeed = Mathf.Clamp(moveSpeed, 0, _runSpeed);
                break;
        }

        //Debug.Log(moveSpeed);
    }

    private void ApplyMovement()
    {
        if (currentState == PlayerState.Dying)
        {
            return;
        }
        if (_movement != Vector3.zero)
        {
            _controller.Move(_movement * moveSpeed * Time.deltaTime);
        }
        else
        {
            _controller.Move(_lastMoveDirection * moveSpeed * Time.deltaTime);
        }
    }

    private void CalculateGravity()
    {
        if (isGrounded)
            return;

        _velocity.y += _gravity * Time.deltaTime;
    }

    private void ApplyGravity()
    {
        _controller.Move(_velocity * Time.deltaTime);
    }

    private void IsGroundedChanged()
    {
        if (isGrounded)
        {
            _controller.stepOffset = _defaultStepOffset;
        }
        else
        {
            _controller.stepOffset = 0;
        }
    }

    private bool CheckIfLookingAtInteractable()
    {
        Transform cameraTransform = Camera.main.transform;
        MonoBehaviour interactable = _interactableLookingAt as MonoBehaviour;
        if (interactable != null)
        {
            if (interactable.TryGetComponent<InteractableObject>(out InteractableObject interactableObject))
            {
                interactableObject.DisablePopup();
            }
        }
        RaycastHit hit;
        bool foundInteractable = false;
        IInteractable interactableItem;
        _interactableLookingAt = null;
        if (Physics.Raycast(cameraTransform.position, cameraTransform.forward, out hit, _grabLength))
        {
            if (hit.transform.tag == "Interactable")
            {
                if (hit.rigidbody.gameObject.TryGetComponent<IInteractable>(out interactableItem))
                {
                    if (hit.rigidbody.gameObject.TryGetComponent<InteractableObject>(out InteractableObject interactableObject))
                    {
                        interactableObject.EnablePopup();
                    }
                    _interactableLookingAt = interactableItem;
                    foundInteractable = true;
                }
            }
        }

        if (_debugMode)
        {
            Color rayColour = Color.red;
            if (foundInteractable)
            {
                rayColour = Color.green;
            }
            UnityEngine.Debug.DrawRay(cameraTransform.position, cameraTransform.forward, rayColour);
        }

        return foundInteractable;
    }

    public void GainMoney(int income)
    {
        if (income < 0)
        {
            throw new Exception("Tried to gain a negative amount of money!");
        }
        if (income == 0)
        {
            UnityEngine.Debug.LogWarning("Gaining $0!");
        }
        money += income;
    }

    public bool SpendMoney(int cost)
    {
        if (cost < 0)
        {
            throw new Exception("Tried to spend a negative amount of money!");
        }

        if (cost == 0)
        {
            UnityEngine.Debug.LogWarning("Spending $0!");
        }

        if (cost > money)
        {
            UnityEngine.Debug.Log("Tried to spend more money than player has!");
            return false;
        }

        money -= cost;
        return true;
    }

    public void TeleportPlayer(Vector3 position)
    {
        if (_controller == null)
        {
            _controller = GetComponent<CharacterController>();
        }
        _controller.enabled = false;
        transform.position = position;
        _controller.enabled = true;
    }

    public void SetupDeath()
    {
        if (_controller == null)
        {
            _controller = GetComponent<CharacterController>();
        }
        _controller.enabled = false;
        lockedInput = true;
        ChangeState(PlayerState.Dying);
        CameraLook cameraLook = GetComponentInChildren<CameraLook>();
        cameraLook.enabled = false;
    }

    public void DisableDeath()
    {
        if (_controller == null)
        {
            _controller = GetComponent<CharacterController>();
        }
        transform.up = Vector3.up;
        _controller.enabled = true;
        lockedInput = false;
        CameraLook cameraLook = GetComponentInChildren<CameraLook>();
        cameraLook.enabled = true;
        ChangeState(PlayerState.Idle);
    }

    public void NoOxygenLeft()
    {
        if (currentState == PlayerState.Dying)
        {
            return;
        }
        _outOfOxygen = true;
        _oxygenOutCoroutine = StartCoroutine(OxygenOutTimer());
    }

    public void OxygenRegained()
    {
        if (currentState == PlayerState.Dying)
        {
            return;
        }
        _outOfOxygen = false;
        if (_oxygenOutCoroutine != null)
        {
            StopCoroutine(_oxygenOutCoroutine);
        }
    }

    public void KillPlayer(DeathObject.DeathType deathType)
    {
        if (currentState == PlayerState.Dying)
        {
            if (_debugMode)
            {
                Debug.Log($"Player already dying!");
            }
            return;
        }

        if (_dyingCoroutine != null)
        {
            if (_debugMode)
            {
                Debug.Log("Already playing a death routine!");
            }
            return;
        }
        _dyingCoroutine = StartCoroutine(DeathRoutine(deathType));
    }

    public void RespawnPlayer()
    {
        TeleportPlayer(_playerSpawnPoint.position);
        DisableDeath();
    }

    private IEnumerator OxygenOutTimer()
    {
        yield return null;
        //add code for any animations and sounds

        yield return new WaitForSeconds(_bufferSecondsFromNoOxygen);
        //add any code for things happening after player runs out of buffer seconds
        if (_outOfOxygen)
        {
            KillPlayer(DeathObject.DeathType.Suffocation);
        }
    }

    private IEnumerator DeathRoutine(DeathObject.DeathType deathType)
    {
        SetupDeath();
        ChangeUIState(UIManager.UIToDisplay.GAME);
        yield return null;

        //add code for any animations and sounds
        deathHandler.CreateDeath(deathType);

        while (!deathHandler.finished)
        {
            yield return null;
        }

        //add any code for doing things after player dies
        if (deathType == DeathObject.DeathType.Won)
        {
            GameManager.Instance.EndScene(EndScreenManager.EndState.Won);
        }
        else
        {
            GameManager.Instance.PlayerDied();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<Submarine>(out Submarine sub))
        {
            onSub = true;
            transform.SetParent(sub.transform);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent<Submarine>(out Submarine sub))
        {
            onSub = false;
            transform.SetParent(null);
        }
    }

    // Input functions using CustomPlayerInput

    private void OnEnable()
    {
        CustomPlayerInput.UpdateMovement += UpdateMovement;
        CustomPlayerInput.OpenInventory += ToggleInventory;
        CustomPlayerInput.Pause += TogglePause;
        CustomPlayerInput.UpdateRunning += RunInput;
        CustomPlayerInput.Interact += InteractWithInteractable;
    }

    private void OnDisable()
    {
        CustomPlayerInput.UpdateMovement -= UpdateMovement;
        CustomPlayerInput.OpenInventory -= ToggleInventory;
        CustomPlayerInput.Pause -= TogglePause;
        CustomPlayerInput.UpdateRunning -= RunInput;
        CustomPlayerInput.Interact -= InteractWithInteractable;
    }

    public void UpdateMovement(Vector2 newMovementInput)
    {
        if (lockedInput)
        {
            _movementInput = Vector2.zero;
            return;
        }
        _movementInput = newMovementInput;
    }

    public void RunInput(bool running)
    {
        if (lockedInput)
        {
            isRunning = false;
            return;
        }
        isRunning = running;
    }

    public void InteractWithInteractable()
    {
        if (lockedInput)
        {
            return;
        }
        if (currentState == PlayerState.Inventory ||
            currentState == PlayerState.Paused ||
            currentState == PlayerState.Dying)
        {
            return;
        }
        if (_interactableLookingAt == null)
        {
            return;
        }
        _interactableLookingAt.Interact();
    }

    public void ChangeUIState(UIManager.UIToDisplay ui)
    {
        uiManager.SetUI(ui);
        bool enabled = false;
        switch (ui)
        {
            case UIManager.UIToDisplay.GAME:
                ChangeState(PlayerState.Idle);
                enabled = false;
                break;
            case UIManager.UIToDisplay.PAUSE:
                ChangeState(PlayerState.Paused);
                enabled = false;
                break;
            default:
                ChangeState(PlayerState.Inventory);
                enabled = true;
                break;
        }
        GetComponentInChildren<InventoryController>().enabled = enabled;
        GetComponentInChildren<CameraLook>().enabled = !enabled;
        if (!enabled)
        {
            inventoryController.InventoryClosing();
        }
    }

    public void ToggleInventory()
    {
        if (lockedInput)
        {
            return;
        }
        switch (currentState)
        {
            case PlayerState.Inventory:
                CloseInventory();
                break;
            default:
                OpenInventory();
                break;
        }
    }

    public void TogglePause()
    {
        if (lockedInput)
        {
            return;
        }
        switch (currentState)
        {
            case PlayerState.Paused:
                ClosePauseMenu();
                break;
            default:
                OpenPauseMenu();
                break;
        }
    }

    public void OpenInventory()
    {
        ChangeUIState(UIManager.UIToDisplay.INVENTORY);
    }

    public void CloseInventory()
    {
        ChangeUIState(UIManager.UIToDisplay.GAME);
    }

    public void OpenPauseMenu()
    {
        ChangeUIState(UIManager.UIToDisplay.PAUSE);
        //adding timescale stop to pause game while menu is open - Logan
        Time.timeScale = 0;
    }

    public void ClosePauseMenu()
    {
        ChangeUIState(UIManager.UIToDisplay.GAME);
        //adding timescale to unpause game when menu is closed and relocking player cursor - Logan
        Time.timeScale = 1;
    }

    public void AddHeartbeat(IHeartbeat heartbeatElement)
    {
        if (CheckHeartbeatInList(heartbeatElement))
        {
            return;
        }
        _heartbeatElements.Add(heartbeatElement);
    }

    public bool CheckHeartbeatInList(IHeartbeat heartbeatElement)
    {
        return _heartbeatElements.Contains(heartbeatElement);
    }

    public void RemoveHeartbeat(IHeartbeat heartbeatElement)
    {
        if (!CheckHeartbeatInList(heartbeatElement))
        {
            return;
        }
        _heartbeatElements.Remove(heartbeatElement);
    }
}
