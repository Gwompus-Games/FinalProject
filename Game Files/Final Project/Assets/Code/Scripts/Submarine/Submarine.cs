using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Submarine : ManagedByGameManager
{
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
    }

    public override void CustomStart()
    {
        base.CustomStart();

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<PlayerController>(out PlayerController playerController))
        {
            playerInSubmarine = true;
            _playerDefaultParent = playerController.transform.parent;
            playerController.gameObject.transform.parent = transform;
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
    internal void ButtonPushed()
    {
        if (_inTransit)
        {
            return;
        }
        switch (GameManager.Instance.currentGameState)
        {
            case GameManager.GameState.InBetweenFacitilies:
                GameManager.Instance.EnterLevel();
                break;
            case GameManager.GameState.LandedAtFacility:
                GameManager.Instance.ExitLevel();
                break;
        }
    }
}
