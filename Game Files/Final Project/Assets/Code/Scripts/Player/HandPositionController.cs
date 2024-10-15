using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandPositionController : ManagedByGameManager
{
    [SerializeField] private Transform _rightHandTransform;
    [SerializeField] private Transform _leftHandTransform;
    private Vector3 _rightHandDefaultPosition;
    private Vector3 _leftHandDefaultPosition;

    public override void Init()
    {
        
    }

    public override void CustomStart()
    {
        if (_rightHandTransform == null)
        {
            throw new System.Exception("No right hand transform assigned");
        }
        if (_leftHandTransform == null)
        {
            throw new System.Exception("No left hand transform assigned");
        }

        _rightHandDefaultPosition = _rightHandTransform.localPosition;
        _leftHandDefaultPosition = _leftHandTransform.localPosition;
        ResetHandPositions();
    }

    /// <summary>
    /// This function should be used to reset hand positions when unequiping a tool
    /// </summary>
    public void ResetHandPositions()
    {
        //Reset where hands are for tools
        _rightHandTransform.localPosition = _rightHandDefaultPosition;
        _leftHandTransform.localPosition = _leftHandDefaultPosition;
    }

    /// <summary>
    /// This overload should be used for setting one handed tools
    /// </summary>
    public void SetHandPositions(Transform rHTransform)
    {
        //Set where right hand is for a tool and reset left hand
        _rightHandTransform.localPosition = rHTransform.localPosition;
        _leftHandTransform.localPosition = _leftHandDefaultPosition;
    }

    /// <summary>
    /// This overload should be used for setting two handed tools
    /// </summary>
    public void SetHandPositions(Transform rHTransform, Transform lHTransform)
    {
        _rightHandTransform.localPosition = rHTransform.localPosition;
        _leftHandTransform.localPosition = lHTransform.localPosition;
    }
}
