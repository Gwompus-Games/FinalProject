using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CharacterController))]
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
    [SerializeField] private float _maxStamina = 100;
    [SerializeField] private float _staminaLossSpeed = 10f;
    [SerializeField] private float _staminaJumpCost = 20f;
    [SerializeField] private float _staminaRegenSpeed = 12f;
    [SerializeField] private float _staminaRegenDelay = 1f;
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

    private void Start()
    {
        _controller = GetComponent<CharacterController>();

        _defaultStepOffset = _controller.stepOffset;
        moveSpeed = _walkSpeed;

        UpdateStamina(_maxStamina);
    }

    private void Update()
    {
        // Update timers
        UpdateTimers();

        // Take player inputs
        MovementInput();
        JumpInput();

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
                RegenerateStamina();
                break;
            case PlayerState.Walking:
                moveSpeed = _walkSpeed;
                RegenerateStamina(0.5f);
                break;
            case PlayerState.Running:
                moveSpeed = _runSpeed;
                DecreaseStamina();
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

    private void JumpInput()
    {
        if (Input.GetButtonDown("Jump") && isGrounded && _velocity.y <= 0.1f)
        {
            Jump();
        }
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

    private void Jump()
    {
        if (_stamina < _staminaJumpCost)
            return;


        _velocity.y = Mathf.Sqrt(_jumpForce * 0.1f * -2 * _gravity);
        UpdateStamina(_stamina - _staminaJumpCost);
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

    private void DecreaseStamina(float multiplier = 1)
    {
        if (_stamina == 0 || !isGrounded)
            return;

        UpdateStamina(_stamina - (_staminaRegenSpeed * multiplier * Time.deltaTime));

        if (_stamina <= 1)
            ChangeState(PlayerState.Walking);
    }

    private void RegenerateStamina(float multiplier = 1)
    {
        if (_stamina >= _maxStamina || !isGrounded || _staminaRegenDelayTimer < _staminaRegenDelay)
            return;

        UpdateStamina(_stamina + (_staminaRegenSpeed * multiplier * Time.deltaTime));
    }

    private void UpdateStamina(float newStamina)
    {
        if (newStamina < _stamina)
        {
            if (_infiniteStamina)
                return;

            _staminaRegenDelayTimer = 0;
        }

        _stamina = Mathf.Clamp(newStamina, 0, _maxStamina);
        _staminaBar.fillAmount = _stamina / _maxStamina;
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
