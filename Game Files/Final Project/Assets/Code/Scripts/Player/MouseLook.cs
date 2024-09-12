using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseLook : MonoBehaviour
{
    public float mouseSensitivity = 40f;
    public Transform playerBody;

    float xRotation = 0f;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void LateUpdate()
    {
        if (GameManager.Instance.isPaused)
            return;

        Vector2 mouseInput = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y")) * mouseSensitivity * Time.deltaTime * 10;

        xRotation -= mouseInput.y;
        xRotation = Mathf.Clamp(xRotation, -80f, 80f);

        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        playerBody.Rotate(Vector3.up * mouseInput.x);
    }
}
