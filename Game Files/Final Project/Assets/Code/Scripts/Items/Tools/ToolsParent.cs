using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ToolsParent : ManagedObject
{

    [Serializable]
    public struct Tool
    {
        public GameObject itemGameObject;
        public HoldableToolSO toolData;
        public Transform rightHandPositionTransform;
        public Transform leftHandPositionTransform;
    }

    [SerializeField] protected Tool _myTool;

    protected HandPositionController _handPositionController;

    public bool toolEnabled 
    {
        get
        {
            if (_myTool.itemGameObject == null)
            {
                return false;
            }
            return _myTool.itemGameObject.activeInHierarchy;
        }
        protected set 
        {
            if (_myTool.itemGameObject != null)
            {
                _myTool.itemGameObject.SetActive(value);
            }
        } 
    }

    public override void Init()
    {
        base.Init();
        if (_myTool.toolData == null)
        {
            throw new Exception("Tool data not set!");
        }
        if (_myTool.itemGameObject == null)
        {
            _myTool.itemGameObject.transform.GetChild(0);
        }
        if (_myTool.itemGameObject == null)
        {
            throw new Exception("No child object to manage!");
        }
        _handPositionController = GameManager.Instance.GetManagedComponent<HandPositionController>();
    }

    public override void CustomStart()
    {
        base.CustomStart();
        if (_myTool.rightHandPositionTransform == null &&
            _myTool.leftHandPositionTransform == null)
        {
            throw new Exception("No hand positions set!");
        }
        SetToolEnabled(false);
    }

    protected virtual void OnEnable()
    {
        
    }

    protected virtual void OnDisable()
    {

    }

    public void SetToolEnabled(bool set)
    {
        toolEnabled = set;
    }

    public HoldableToolSO GetToolData()
    {
        return _myTool.toolData;
    }

    public abstract void UseTool();
    public abstract void CancelUseTool();
}
