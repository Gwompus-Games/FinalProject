using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ToolsParent : MonoBehaviour
{
    [SerializeField] protected GameObject _itemGameObject;
    [SerializeField] protected Transform[] _handPositionTransforms;
    [SerializeField] protected HoldableToolSO toolData;
    protected Vector3[] _handPositions;
    public bool toolEnabled { get; protected set; }

    public static List<ToolsParent> activeTools { get; protected set; } = new List<ToolsParent>();

    protected virtual void Awake()
    {
        if (!activeTools.Contains(this))
        {
            activeTools.Add(this);
        }
    }

    protected virtual void OnEnable()
    {
        
    }

    protected virtual void OnDisable()
    {

    }

    public void HideThisTool()
    {
        toolEnabled = false;
    }

    public void SwapToThisTool()
    {
        for (int t = 0; t < activeTools.Count; t++)
        {

        }
    }

    public abstract void UseTool();
}
