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
    


    public bool playerInSubmarine { get; private set; }
    private bool _inTransit = false;
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
        _playerDefaultParent = GameManager.Instance.GetManagedComponent<PlayerController>().transform.parent;
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

        Vector3 startingPosition = transform.position;
        Vector3 newPosition = Vector3.zero;

        //Play animation
        while (newPosition != target)
        {
            //Calculate next position based on curves
            newPosition.x = Mathf.Lerp(startingPosition.x, target.x, movementCurves._sideMovementCurve.Evaluate(Time.time));
            newPosition.y = Mathf.Lerp(startingPosition.y, target.y, movementCurves._upMovementCurve.Evaluate(Time.time));
            newPosition.z = Mathf.Lerp(startingPosition.z, target.z, movementCurves._forwardMovementCurve.Evaluate(Time.time));
            
            //Check if submarine is withing snapping distance
            if (Vector3.Distance(newPosition, target) <= _movementSnap)
            {
                newPosition = target;
            }

            //Apply movement 
            transform.position = newPosition;
        }

        GameManager.Instance.SubmarineAnimationFinished();
        _inTransit = false;
    }
}
