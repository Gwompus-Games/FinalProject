using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(OxygenSystem))]
[RequireComponent(typeof(SuitSystem))]
public class PlayerController : MonoBehaviour
{
    public enum PlayerState
    {
        Idle,
        Walking,
        Running
    }

    [Header("Debug Settings")]
    [SerializeField] private bool _debugMode = false;
    [SerializeField] private bool _infiniteStamina = false;

    [Header("Movement Settings")]
    [SerializeField] private float _walkSpeed = 3f;
    [SerializeField] private float _runSpeed = 5f;
    [SerializeField] private float _jumpForce = 2f;
    [SerializeField] private float _gravity = -2f;

    [Header("Stamina Settings")]
    [SerializeField] private Image _staminaBar;

    [Header("Grounded Settings")]
    [SerializeField] private LayerMask _groundMask;
    [SerializeField] private Transform _groundCheck;
    [SerializeField] private float _groundCheckRadius = 0.4f;

    public PlayerState currentState { get; private set; } = PlayerState.Idle;
    public bool isGrounded { get; private set; }
    public float moveSpeed { get; private set; }

    CharacterController _controller;

    private Vector3 _movement;
    private Vector3 _velocity;

    private float _defaultStepOffset;
    private float _stamina;
    private float _staminaRegenDelayTimer;

    private Vector2 _movementInput = Vector2.zero;

    private OxygenSystem _oxygenSystem;
    private SuitSystem _suitSystem;

    private void Start()
    {
        _controller = GetComponent<CharacterController>();

        _defaultStepOffset = _controller.stepOffset;
        moveSpeed = _walkSpeed;
    }

    private void Update()
    {
        // Update timers
        UpdateTimers();

        // Take player inputs
        MovementInput();

        // Update player state and apply state logic
        UpdateState();
        ApplyState();

        // Apply player movement
        ApplyMovement();

        // Check for jump input and calculate then apply gravity
        CalculateGravity();
        ApplyGravity();
    }

    private void UpdateState()
    {
        if (_movement.magnitude == 0)
        {
            ChangeState(PlayerState.Idle);
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.LeftShift) && isGrounded && _stamina > 1)
            {
                ChangeState(PlayerState.Running);
            }
            else if (!Input.GetKey(KeyCode.LeftShift) && isGrounded && currentState == PlayerState.Running)
            {
                ChangeState(PlayerState.Walking);
            }
            else if (currentState == PlayerState.Idle)
            {
                ChangeState(PlayerState.Walking);
            }
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
        }
    }

    public void ChangeState(PlayerState newState)
    {
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

    private void UpdateTimers()
    {
        if (isGrounded)
            _staminaRegenDelayTimer += Time.deltaTime;
    }

    // Input functions using CustomPlayerInput

    private void OnEnable()
    {
        CustomPlayerInput.UpdateMovement += UpdateMovement;
    }

    private void OnDisable()
    {
        CustomPlayerInput.UpdateMovement -= UpdateMovement;
    }

    public void UpdateMovement(Vector2 newMovementInput)
    {
        _movementInput = newMovementInput;
    }
}
