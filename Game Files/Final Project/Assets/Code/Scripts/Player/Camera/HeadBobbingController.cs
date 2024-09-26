using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadBobbingController : MonoBehaviour
{
    [SerializeField] private bool enable = true;

    [SerializeField, Range(0f, 0.1f)] private float amplitude = 0.005f;
    [SerializeField, Range(0f, 30)] private int frequency = 12;

    [SerializeField] private Transform cam = null;
    [SerializeField] private Transform camHolder = null;

    private float toggleSpeed = 1f;
    private Vector3 startPos;
    private CharacterController controller;
    private PlayerController playerController;

    private void Awake()
    {
        playerController = GetComponent<PlayerController>();
        controller = GetComponent<CharacterController>();
        startPos = cam.localPosition;
    }

    private void Update()
    {
        if (!enabled) return;

        CheckMotion();
        ResetPosition();
        cam.LookAt(FocusTarget());
    }

    private void CheckMotion()
    {
        if (playerController.currentState == PlayerController.PlayerState.Idle) return;
        if (!playerController.isGrounded) return;

        PlayMotion(FootStepMotion());
    }

    private Vector3 FootStepMotion()
    {
        float moveSpeedMultiplier = playerController.moveSpeed;

        Vector3 pos = Vector3.zero;
        pos.y += Mathf.Sin(Time.time * frequency) * amplitude * 0.05f * moveSpeedMultiplier;
        pos.x += Mathf.Cos(Time.time * frequency / 2) * amplitude * 0.1f * moveSpeedMultiplier;
        return pos;
    }

    private void PlayMotion(Vector3 motion)
    {
        cam.localPosition += motion;
    }

    private void ResetPosition()
    {
        if (cam.localPosition == startPos) return;
        cam.localPosition = Vector3.Lerp(cam.localPosition, startPos, 1 * Time.deltaTime);
    }

    private Vector3 FocusTarget()
    {
        Vector3 pos = new Vector3(transform.position.x, transform.position.y + camHolder.localPosition.y, transform.position.z);
        pos += camHolder.forward * 15.0f;
        return pos;
    }
}
