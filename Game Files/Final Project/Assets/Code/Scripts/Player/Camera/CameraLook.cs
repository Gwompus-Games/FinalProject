using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraLook : MonoBehaviour
{
    public float mouseSensitivity = 400f;
    public Transform playerBody;

    float _xRotation = 0f;
    private Vector2 _cursorInput = Vector2.zero;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void OnEnable()
    {
        CustomPlayerInput.UpdateCursorDelta += CursorUpdate;
    }

    private void OnDisable()
    {
        CustomPlayerInput.UpdateCursorDelta -= CursorUpdate;
    }

    private void LateUpdate()
    {
        if (GameManager.Instance.isPaused)
            return;

        Vector2 mouseInput = _cursorInput * mouseSensitivity * Time.deltaTime;

        _xRotation -= mouseInput.y;
        _xRotation = Mathf.Clamp(_xRotation, -80f, 80f);

        transform.localRotation = Quaternion.Euler(_xRotation, 0f, 0f);
        playerBody.Rotate(Vector3.up * mouseInput.x);
    }

    public void CursorUpdate(Vector2 cursorDelta)
    {
        _cursorInput = cursorDelta;
    }
}
