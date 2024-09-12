using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CustomPlayerInput : MonoBehaviour
{
    public static CustomPlayerInput INSTANCE;
    public static Action<Vector2> UpdateMousePosition;
    public static Action<int> Rotate;
    public static Action<Vector2> UpdateMovement;
    public static Action OpenInventory;
    public static Action<CustomInputData> LeftMouseButton;

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

    public void InputMousePosition(InputAction.CallbackContext context)
    {
        Vector2 mousePosition = context.ReadValue<Vector2>();
        UpdateMousePosition?.Invoke(mousePosition);
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
}
