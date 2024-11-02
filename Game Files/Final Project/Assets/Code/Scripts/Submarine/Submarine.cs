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

    private PlayerController _playerController;

    public bool playerInSubmarine { get; private set; }
    private bool _inTransit = false;
    public bool landed = false;

    private Transform _landedTransform;
    private Transform _shoppingPlacementTransform;
    private Transform _playerDefaultParent;

    public override void Init()
    {
        base.Init();
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

        if (_movementParentTransform == null)
        {
            _movementParentTransform = transform.GetChild(0);
        }
    }

    public override void CustomStart()
    {
        base.CustomStart();
        _playerController = GameManager.Instance.GetManagedComponent<PlayerController>();
        _playerDefaultParent = _playerController.transform.parent;
        _playerController.transform.parent = _movementParentTransform;
        playerInSubmarine = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<PlayerController>(out PlayerController playerController))
        {
            if (_playerDefaultParent == null)
            {
                _playerDefaultParent = playerController.transform.parent;
            }
            playerInSubmarine = true;
            playerController.gameObject.transform.parent = _movementParentTransform;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent<PlayerController>(out PlayerController playerController))
        {
            playerInSubmarine = false;
            playerController.gameObject.transform.parent = _playerDefaultParent;
        }
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

    private IEnumerator WaitForAnimation()
    {
        yield return null;
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
            transform.position = newPosition;
            if (transform.position == _landedTransform.position)
                landed = true;
            else
                landed = false;

            yield return null;
        }

        GameManager.Instance.SubmarineAnimationFinished();
        _inTransit = false;
    }
}
