using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathMovementComponent : DeathComponent
{
    [SerializeField] private AnimationCurve _movementCurve;
    [SerializeField] private AnimationCurve _rotationCurve;
    [SerializeField] private Transform _targetTransform;
    private PlayerController _playerController;
    private Transform _playerTransform;
    private Vector3 _startingPosition;
    private Quaternion _startingRotation;
    private Vector3 _targetPosition;
    private Quaternion _targetRotation;

    private void Awake()
    {
        _playerController = GameManager.Instance.GetManagedComponent<PlayerController>();
        _playerTransform = _playerController.transform;
        _startingPosition = _playerTransform.position;
        _startingRotation = _playerTransform.rotation;
        _targetPosition = _targetTransform.position;
        _targetRotation = _targetTransform.rotation;
    }

    public override void StartDeathComponent()
    {
        
    }

    public override void UpdateDeathComponent(float normalizedTime)
    {
        _playerTransform.position = Vector3.Lerp(_startingPosition, _targetPosition, _movementCurve.Evaluate(normalizedTime));
        _playerTransform.rotation = Quaternion.Lerp(_startingRotation, _targetRotation, _rotationCurve.Evaluate(normalizedTime));
    }

    public override void EndDeathComponent()
    {
        
    }
}
