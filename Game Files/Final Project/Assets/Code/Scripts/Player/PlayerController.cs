using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using FMOD;
using FMOD.Studio;
using FMODUnity;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(OxygenSystem))]
[RequireComponent(typeof(SuitSystem))]
[RequireComponent(typeof(StudioEventEmitter))]
public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance;

    public enum PlayerState
    {
        Idle,
        Walking,
        Running,
        Inventory
    }

    [Header("Debug Settings")]
    [SerializeField] private bool _debugMode = false;

    [Header("Movement Settings")]
    [SerializeField] private float _walkSpeed = 3f;
    [SerializeField] private float _runSpeed = 5f;
    [SerializeField] private float _gravity = -2f;
    [SerializeField] [Range(1f, 5f)] private float _runningOxygenDrainMultiplier = 2f;

    [Header("Grounded Settings")]
    [SerializeField] private LayerMask _groundMask;
    [SerializeField] private Transform _groundCheck;
    [SerializeField] private float _groundCheckRadius = 0.4f;

    [Header("Interacting Settings")]
    [SerializeField] private float _grabLength = 2f;
    private IInteractable _interactableLookingAt;

    public PlayerState currentState { get; private set; } = PlayerState.Idle;
    public bool isGrounded { get; private set; }
    public float moveSpeed { get; private set; }
    public bool isRunning { get; private set; }

    CharacterController _controller;

    private Vector3 _movement;
    private Vector3 _velocity;

    private float _defaultStepOffset;

    private Vector2 _movementInput = Vector2.zero;

    private OxygenSystem _oxygenSystem;
    private SuitSystem _suitSystem;
    private OxygenDrainer _runningDrainer;

    private EventInstance playerFootsteps;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        _runningDrainer = gameObject.AddComponent<OxygenDrainer>();
        _runningDrainer.SetDrainMultiplier(_runningOxygenDrainMultiplier);
    }

    private void Start()
    {
        _controller = GetComponent<CharacterController>();

        _defaultStepOffset = _controller.stepOffset;
        moveSpeed = _walkSpeed;
        isRunning = false;
        ChangeInventoryUIState(false);
        playerFootsteps = AudioManager.instance.CreateEventInstance(FMODEvents.instance.footsteps);
    }

    private void Update()
    {
        // Take player inputs
        MovementInput();

        //Check for looking at an Interactable
        CheckIfLookingAtInteractable();

        // Update player state and apply state logic
        UpdateState();
        ApplyState();

        // Apply player movement
        ApplyMovement();

        // Check for jump input and calculate then apply gravity
        CalculateGravity();
        ApplyGravity();

        //FMOD
        UpdateSound();
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
            playerFootsteps.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        }
        //enemy heartbeat logic
        //AudioManager.instance.PlayOneShot(FMODEvents.instance.heartbeat, transform.position);
    }

    private void UpdateState()
    {
        if (currentState == PlayerState.Inventory)
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
                moveSpeed = 0;
                break;
            case PlayerState.Walking:
                moveSpeed = _walkSpeed;
                break;
            case PlayerState.Running:
                moveSpeed = _runSpeed;
                break;
            default:
                moveSpeed = 0;
                break;
        }
    }

    public void ChangeState(PlayerState newState)
    {
        if (newState == PlayerState.Running)
        {
            if (!OxygenSystem.INSTANCE.DrainingSourceActive(_runningDrainer))
            {
                OxygenSystem.INSTANCE.AddDrainingSource(_runningDrainer);
            }
        }
        else
        {
            if (OxygenSystem.INSTANCE.DrainingSourceActive(_runningDrainer))
            {
                OxygenSystem.INSTANCE.RemoveDrainingSource(_runningDrainer);
            }
        }
        currentState = newState;
    }

    private void MovementInput()
    {
        bool oldIsGrounded = isGrounded;

        isGrounded = Physics.CheckSphere(_groundCheck.position, _groundCheckRadius, _groundMask);

        if (_velocity.y > 0.2f)
            isGrounded = false;

        if (isGrounded != oldIsGrounded)
        {
            IsGroundedChanged();
        }

        if (isGrounded && _velocity.y < 0.5f)
        {
            _velocity.y = -5f;
        }

        _movement = (transform.right * _movementInput.x + transform.forward * _movementInput.y);
    }

    private void ApplyMovement()
    {
        _controller.Move(_movement * moveSpeed * Time.deltaTime);
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

    public void NoOxygenLeft()
    {
        KillPlayer();
    }

    public void KillPlayer()
    {

    }

    public void RestartGame()
    {
        SceneManager.LoadScene(0);
    }

    // Input functions using CustomPlayerInput

    private void OnEnable()
    {
        CustomPlayerInput.UpdateMovement += UpdateMovement;
        CustomPlayerInput.OpenInventory += ToggleInventory;
        CustomPlayerInput.UpdateRunning += RunInput;
        CustomPlayerInput.LeftMouseButton += InteractWithInteractable;
    }

    private void OnDisable()
    {
        CustomPlayerInput.UpdateMovement -= UpdateMovement;
        CustomPlayerInput.OpenInventory -= ToggleInventory;
        CustomPlayerInput.UpdateRunning -= RunInput;
        CustomPlayerInput.LeftMouseButton -= InteractWithInteractable;
    }

    public void UpdateMovement(Vector2 newMovementInput)
    {
        _movementInput = newMovementInput;
    }

    public void RunInput(bool running)
    {
        isRunning = running;
    }

    public void InteractWithInteractable(CustomPlayerInput.CustomInputData data)
    {
        if (currentState == PlayerState.Inventory)
        {
            return;
        }
        if (data == CustomPlayerInput.CustomInputData.RELEASED)
        {
            if (_interactableLookingAt != null)
            {
                _interactableLookingAt.Interact();
            }
        }
    }

    private void ChangeInventoryUIState(bool enabled)
    {
        UIManager.INSTANCE.SetInventoryUI(enabled);
        GetComponentInChildren<InventoryController>().enabled = enabled;
        GetComponentInChildren<CameraLook>().enabled = !enabled;
        if (!enabled)
        {
            InventoryController.INSTANCE.InventoryClosing();
        }
    }

    public void ToggleInventory()
    {
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

    public void OpenInventory()
    {
        Cursor.lockState = CursorLockMode.None;
        ChangeInventoryUIState(true);
        ChangeState(PlayerState.Inventory);
    }

    public void CloseInventory()
    {
        Cursor.lockState = CursorLockMode.Locked;
        ChangeInventoryUIState(false);
        ChangeState(PlayerState.Idle);
    }
}
