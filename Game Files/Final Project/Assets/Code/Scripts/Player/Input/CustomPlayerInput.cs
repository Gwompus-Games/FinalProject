using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CustomPlayerInput : MonoBehaviour
{
    public static CustomPlayerInput INSTANCE;
    public static Action<Vector2> UpdateCursorPosition;
    public static Action<Vector2> UpdateCursorDelta;
    public static Action<int> Rotate;
    public static Action<Vector2> UpdateMovement;
    public static Action OpenInventory;
    public static Action<CustomInputData> LeftMouseButton;
    public static Action<CustomInputData> RightMouseButton;
    public static Action<bool> UpdateRunning;

    public enum CustomInputData
    {
        PRESSED,
        RELEASED
    }

    private void Awake()
    {
        if (INSTANCE != null)
        {
            Destroy(gameObject);
            return;
        }
        INSTANCE = this;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void InputMovement(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Vector2 movementInput = context.ReadValue<Vector2>();
            UpdateMovement?.Invoke(movementInput);
        }
    }

    public void InputRotate(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            float rotateInput = context.ReadValue<float>();
            int roundedInput = (int)MathF.Round(rotateInput, 0, MidpointRounding.AwayFromZero);
            Rotate?.Invoke(roundedInput);
        }
    }

    public void InputCursorPosition(InputAction.CallbackContext context)
    {
        Vector2 mousePosition = context.ReadValue<Vector2>();
        UpdateCursorPosition?.Invoke(mousePosition);
    }

    public void InputCursorDelta(InputAction.CallbackContext context)
    {
        Vector2 cursorDelta = context.ReadValue<Vector2>();
        UpdateCursorDelta?.Invoke(cursorDelta);
    }

    public void InputOpenInventory(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            OpenInventory?.Invoke();
        }
    }

    public void InputLeftMouseButton(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            LeftMouseButton?.Invoke(CustomInputData.PRESSED);
        }
        if (context.canceled)
        {
            LeftMouseButton?.Invoke(CustomInputData.RELEASED);
        }
    }

    public void InputRightMouseButton(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            RightMouseButton?.Invoke(CustomInputData.PRESSED);
        }
        if (context.canceled)
        {
            RightMouseButton?.Invoke(CustomInputData.RELEASED);
        }
    }

    public void InputRunning(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            UpdateRunning?.Invoke(true);
        }
        if (context.canceled)
        {
            UpdateRunning?.Invoke(false );
        }
    }
}
