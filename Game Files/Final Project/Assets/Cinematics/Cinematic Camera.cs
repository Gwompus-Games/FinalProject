using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CinematicCamera : MonoBehaviour
{
    private Camera m_Camera;

    [Serializable]
    struct TransformCurves
    {
        public AnimationCurve xCurve;
        public AnimationCurve yCurve;
        public AnimationCurve zCurve;
    }

    [SerializeField] private TransformCurves _positionCurves;
    [SerializeField] private Transform _lookAtPoint;
    [SerializeField] private Transform _startPoint;
    [SerializeField] private Transform[] _midPoints;
    [SerializeField] private Transform _endPoint;
    [SerializeField] private float _totalTimeTaken;

    private float _time = 0;
    private float _totalTime = 0;
    private float _swapTime;
    private int _currentSection = 0;

    private void Awake()
    {
        m_Camera = GetComponent<Camera>();
    }

    private void Start()
    {
        _time = 0;
        _totalTime = 0;
        _swapTime = _totalTimeTaken / ((float)_midPoints.Length + 1f);
        _currentSection = -1;
    }

    private void Update()
    {
        if (_currentSection == _midPoints.Length)
        {
            return;
        }

        Vector3 startPosition;
        Vector3 endPosition;

        if (_midPoints.Length == 0)
        {
            startPosition = _startPoint.position;
            endPosition = _endPoint.position;
        }
        else
        {
            if (_currentSection == -1)
            {
                startPosition = _startPoint.position;
                endPosition = _midPoints[0].position;
            }
            else if (_currentSection == _midPoints.Length - 1)
            {
                startPosition = _midPoints[_currentSection].position;
                endPosition = _endPoint.position;
            }
            else
            {
                startPosition = _midPoints[_currentSection].position;
                endPosition = _midPoints[_currentSection + 1].position;
            }
        }

        float normalizedTime = _time / _swapTime;

        //Debug.Log($"Time: {_time} | Swap Time: {_swapTime}");
        Debug.Log($"Start Pos: {startPosition} | End Pos: {endPosition}");

        transform.position = CalculateNextPosition(normalizedTime, startPosition, endPosition);
        LookAtTarget();

        _time += Time.deltaTime;
        _totalTime += Time.deltaTime;

        if (_time >= _swapTime)
        {
            _time = 0;
            _currentSection++;
        }
    }

    private Vector3 CalculateNextPosition(float normalizedTime, Vector3 startPosition, Vector3 endPosition)
    {
        float x = Mathf.Lerp(startPosition.x, endPosition.x, _positionCurves.xCurve.Evaluate(normalizedTime));
        float y = Mathf.Lerp(startPosition.y, endPosition.y, _positionCurves.yCurve.Evaluate(normalizedTime));
        float z = Mathf.Lerp(startPosition.z, endPosition.z, _positionCurves.zCurve.Evaluate(normalizedTime));

        //Debug.Log($"Normalized Time:{normalizedTime} | X: {x} | Y: {y} | Z: {z}");

        return new Vector3(x, y, z);
    }

    private void LookAtTarget()
    {
        if (_lookAtPoint == null)
        {
            return;
        }

        transform.LookAt(_lookAtPoint);
    }
}
