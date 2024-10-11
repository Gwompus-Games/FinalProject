using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraLook : MonoBehaviour
{
    [Header("PlayerRotate Properties")]
    [SerializeField] private Transform _cameraHolder;
    [SerializeField] private float _speed = 200;
    [SerializeField] private float _rotationLimit = 90;

    [Header("PlayerRotateSmooth Properties")]
    [SerializeField] private float _smoothTime = 0.02f;

    private Transform _horiRotHelper;
    private float _vertRot;
    private float _verOld;
    private float _vertAngularVelocity;
    private float _horiAngularVelocity;

    private void Start()
    {
        _horiRotHelper = new GameObject("Horizontal Rotation Helper").transform;
        _horiRotHelper.localRotation = transform.localRotation;
    } 

    private void LateUpdate()
    {
        Rotate();
    }

    public virtual void Rotate()
    {
        _verOld = _vertRot;

        _vertRot -= GetVerticalValue();
        _vertRot = _vertRot <= -_rotationLimit ? -_rotationLimit :
                  _vertRot >= _rotationLimit ? _rotationLimit :
                  _vertRot;

        RotateVertical();
        RotateHorizontal();
    }

    private void RotateHorizontal()
    {
        _horiRotHelper.Rotate(Vector3.up * GetHorizontalValue(), Space.Self);
        transform.localRotation
            = Quaternion.Euler(
                0f,
                Mathf.SmoothDampAngle(
                    transform.localEulerAngles.y,
                    _horiRotHelper.localEulerAngles.y,
                    ref _horiAngularVelocity,
                    _smoothTime),
                    0f
                );
    }

    private void RotateVertical()
    {
        _vertRot = Mathf.SmoothDampAngle(_verOld, _vertRot, ref _vertAngularVelocity, _smoothTime);

        _cameraHolder.localRotation = Quaternion.Euler(_vertRot, 0f, 0f);
    }

    protected float GetVerticalValue() => Input.GetAxis("Mouse Y") * _speed * Time.deltaTime;
    protected float GetHorizontalValue() => Input.GetAxis("Mouse X") * _speed * Time.deltaTime;
}
