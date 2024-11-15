using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandSway : MonoBehaviour
{
    [SerializeField] private float smoothing = 10;
    [SerializeField] private float swayMultiplier = 3;

    private void Update()
    {
        Vector2 mouseInput = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y")) * swayMultiplier;

        Quaternion rotationX = Quaternion.AngleAxis(-mouseInput.y, Vector3.right);
        Quaternion rotationY = Quaternion.AngleAxis(mouseInput.x, Vector3.up);

        Quaternion targetRotation = rotationX * rotationY;

        transform.localRotation = Quaternion.Slerp(transform.localRotation, targetRotation, smoothing * 0.01f);
    }
}
