using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Submarine : ManagedByGameManager
{
    [Serializable]
    public struct AnimationCurves
    {
        public AnimationCurve _upMovementCurve;
        public AnimationCurve _forwardMovementCurve;
        public AnimationCurve _sideMovementCurve;
    }

    [SerializeField] private Transform _movementParentTransform;
    [SerializeField] private float _movementSnap = 0.05f;
    [SerializeField] private AnimationCurves _landingCurves;
    [SerializeField] private AnimationCurves _takeOffCurves;
    [SerializeField] private float _speed = 0.25f;
    [SerializeField] private Camera _deadCamera;

    private PlayerController _playerController;

    private bool _inTransit = false;
    public bool landed = false;

    private Transform _landedTransform;
    private Transform _shoppingPlacementTransform;
    private Rigidbody _rb;

    public override void Init()
    {
        base.Init();

        _rb = GetComponent<Rigidbody>();

        SubmarineLandingPoint submarineLandingPoint = FindObjectOfType<SubmarineLandingPoint>();
        SubmarineShoppingPoint submarineShoppingPoint = FindObjectOfType<SubmarineShoppingPoint>();
        if (submarineLandingPoint == null)
        {
            throw new System.Exception("No landing point found in scene! Please add LandingPoint prefab to scene!");
        }

        if (submarineShoppingPoint == null)
        {
            throw new System.Exception("No shopping point found in scene! Please add ShoppingPoint prefab to scene!");
        }

        _landedTransform = submarineLandingPoint.transform;
        _shoppingPlacementTransform = submarineShoppingPoint.transform;
        transform.position = _shoppingPlacementTransform.position;
        _deadCamera.enabled = false;

        if (_movementParentTransform == null)
        {
            _movementParentTransform = transform.GetChild(0);
        }
    }

    public override void CustomStart()
    {
        base.CustomStart();
        _playerController = GameManager.Instance.GetManagedComponent<PlayerController>();
    }


    /// <summary>
    /// This function shouldn't be called anywhere other than in the SubmarineLandingButton script
    /// </summary>
    public void ButtonPushed()
    {
        GetComponentInChildren<Hatch>().CloseHatch();
        if (_inTransit)
        {
            return;
        }
        _inTransit = true;
        StartCoroutine(WaitForAnimation());
    }

    public void ExitLevel()
    {
        if (GameManager.Instance.currentGameState != GameManager.GameState.LandedAtFacility)
        {
            return;
        }
        GetComponentInChildren<Hatch>().CloseHatch();
        if (_inTransit)
        {
            return;
        }
        _inTransit = true;
        StartCoroutine(WaitForAnimation());
    }
    
    public Camera GetDeadCamera()
    {
        return _deadCamera;
    }

    private IEnumerator WaitForAnimation()
    {
        yield return null;

        //_playerController.SetApplyGravity(false);

        Vector3 target = Vector3.zero;
        AnimationCurves movementCurves = new AnimationCurves();

        //Select proper change
        switch (GameManager.Instance.currentGameState)
        {
            case GameManager.GameState.InBetweenFacitilies:
                target = _landedTransform.position;
                movementCurves = _landingCurves;
                GameManager.Instance.EnterLevel();
                break;
            case GameManager.GameState.LandedAtFacility:
                target = _shoppingPlacementTransform.position;
                movementCurves = _takeOffCurves;
                GameManager.Instance.ExitLevel(true);
                break;
        }

        float timeTakenForAnimation = Vector3.Distance(transform.position, target) / _speed;
        float timeStep = 0f;
        Vector3 startingPosition = transform.position;
        Vector3 newPosition = Vector3.zero;

        //Play animation
        while (newPosition != target)
        {
            float timeLerpAmount = timeStep / timeTakenForAnimation;

            //Calculate next position based on curves
            newPosition.x = Mathf.Lerp(startingPosition.x, target.x, movementCurves._sideMovementCurve.Evaluate(timeLerpAmount));
            newPosition.y = Mathf.Lerp(startingPosition.y, target.y, movementCurves._upMovementCurve.Evaluate(timeLerpAmount));
            newPosition.z = Mathf.Lerp(startingPosition.z, target.z, movementCurves._forwardMovementCurve.Evaluate(timeLerpAmount));
            
            timeStep += Time.deltaTime;

            //Check if submarine is withing snapping distance
            if (Vector3.Distance(newPosition, target) <= _movementSnap)
            {
                newPosition = target;
            }

            //Apply movement 
            Vector3 lastPos = transform.position;
            transform.position = newPosition;
            _playerController.MovePlayer(transform.position - lastPos);
            if (transform.position == _landedTransform.position)
                landed = true;
            else
                landed = false;

            yield return null;
        }

        //_playerController.SetApplyGravity(true);
        GameManager.Instance.SubmarineAnimationFinished();
        _inTransit = false;
    }
}
