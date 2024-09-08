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

    [Header("Movement Settings")]
    [SerializeField] private float walkSpeed = 3f;
    [SerializeField] private float runSpeed = 5f;
    [SerializeField] private float jumpForce = 2f;
    [SerializeField] private float gravity = -2f;

    [Header("Stamina Settings")]
    [SerializeField] private float maxStamina = 100;
    [SerializeField] private float staminaLossSpeed = 10f;
    [SerializeField] private float staminaRegenSpeed = 12f;
    [SerializeField] private float staminaJumpCost = 20f;
    [SerializeField] private Slider staminaBar;

    [Header("Grounded Settings")]
    [SerializeField] private float groundCheckRadius = 0.4f;
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private Transform groundCheck;

    public PlayerState currentState { get; private set; } = PlayerState.Idle;
    public bool isGrounded { get; private set; }
    public float moveSpeed { get; private set; }

    CharacterController controller;

    private Vector3 movement;
    private Vector3 velocity;

    private float defaultStepOffset;
    private float stamina;

    private void Start()
    {
        controller = GetComponent<CharacterController>();

        defaultStepOffset = controller.stepOffset;
        moveSpeed = walkSpeed;

        staminaBar.maxValue = maxStamina;
        UpdateStamina(maxStamina);
    }

    private void Update()
    {
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
        if (movement.magnitude == 0)
        {
            ChangeState(PlayerState.Idle);
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.LeftShift) && isGrounded && stamina > 1)
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
                moveSpeed = walkSpeed;
                RegenerateStamina(0.5f);
                break;
            case PlayerState.Running:
                moveSpeed = runSpeed;
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

        isGrounded = Physics.CheckSphere(groundCheck.position, groundCheckRadius, groundMask);

        if (isGrounded != oldIsGrounded)
        {
            IsGroundedChanged();
        }

        if (isGrounded && velocity.y < 0.5f)
        {
            velocity.y = -2f;
        }

        Vector2 movementInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;

        movement = (transform.right * movementInput.x + transform.forward * movementInput.y);
    }

    private void JumpInput()
    {
        if (Input.GetButtonDown("Jump") && isGrounded && velocity.y <= 0.1f)
        {
            Jump();
        }
    }

    private void ApplyMovement()
    {
        controller.Move(movement * moveSpeed * Time.deltaTime);
    }

    private void CalculateGravity()
    {
        if (isGrounded)
            return;

        velocity.y += gravity * Time.deltaTime;
    }

    private void ApplyGravity()
    {
        controller.Move(velocity * Time.deltaTime);
    }

    private void Jump()
    {
        if (stamina < staminaJumpCost)
            return;


        velocity.y = Mathf.Sqrt(jumpForce * 0.1f * -2 * gravity);
        UpdateStamina(stamina - staminaJumpCost);
    }

    private void IsGroundedChanged()
    {
        if (isGrounded)
        {
            controller.stepOffset = defaultStepOffset;
        }
        else
        {
            controller.stepOffset = 0;
        }
    }

    private void DecreaseStamina(float multiplier = 1)
    {
        if (stamina == 0 || !isGrounded)
            return;

        UpdateStamina(stamina - (staminaRegenSpeed * multiplier * Time.deltaTime));

        if (stamina <= 1)
            ChangeState(PlayerState.Walking);
    }

    private void RegenerateStamina(float multiplier = 1)
    {
        if (stamina >= maxStamina || !isGrounded)
            return;

        UpdateStamina(stamina + (staminaRegenSpeed * multiplier * Time.deltaTime));
    }

    private void UpdateStamina(float newStamina)
    {
        stamina = Mathf.Clamp(newStamina, 0, maxStamina);
        staminaBar.value = stamina;
    }
}
