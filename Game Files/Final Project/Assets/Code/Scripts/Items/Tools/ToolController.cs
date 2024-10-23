using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

[RequireComponent(typeof(HandPositionController))]
public class ToolController : ManagedByGameManager
{
    private HandPositionController _handPositionController;
    private List<ToolsParent> _tools = new List<ToolsParent>();
    private Dictionary<II_Tool, Vector2Int> _equipedTools = new Dictionary<II_Tool, Vector2Int>();
    private int _equippedTool = -1;
    [field :SerializeField] public bool debugMode { get; private set; } = false;

    public override void Init()
    {
        if (_initilized)
        {
            return;
        }
        base.Init();
        _handPositionController = GameManager.Instance.GetManagedComponent<HandPositionController>();
        Type[] toolsTypes = Assembly.GetAssembly(typeof(ToolsParent)).GetTypes().Where(t => t.IsSubclassOf(typeof(ToolsParent))).ToArray();
        for (int t = 0; t < toolsTypes.Length; t++)
        {
            ToolsParent tool = GetComponentInChildren(toolsTypes[t], false) as ToolsParent;
            if (tool == null) 
            {
                continue;
            }
            _tools.Add(tool);
            tool.Init();            
        }

    }

    public override void CustomStart()
    {
        base.CustomStart();

        for (int t = 0; t < _tools.Count; t++)
        {
            _tools[t].CustomStart();
        }
        if (_equippedTool >= 0 && _equippedTool < _tools.Count)
        {
            _tools[_equippedTool].SetToolEnabled(true);
        }
    }

    public void SwapTool(int direction)
    {
        if (direction == 0)
        {
            Debug.LogWarning("Tried to swap tool in no direction!");
            return;
        }
        if (_equippedTool != -1)
        {
            _tools[_equippedTool].SetToolEnabled(false);
        }
        direction = (int)Mathf.Sign(direction);
        _equippedTool += direction;
        if (_equippedTool >= _tools.Count)
        {
            _equippedTool = -1;
        }
        else if (_equippedTool < -1)
        {
            _equippedTool = _tools.Count - 1;
        }
        if (_equippedTool != -1)
        {
            _tools[_equippedTool].SetToolEnabled(true);
        }
    }

    private void UseEquippedTool(CustomPlayerInput.CustomInputData data)
    {
        if (debugMode)
        {
            Debug.Log("Use input detected!");
        }
        if (_tools.Count == 0)
        {
            return;
        }

        if (_equippedTool == -1)
        {
            return;
        }
        
        if (_equippedTool >= _tools.Count || 
            _equippedTool < -1)
        {
            Debug.LogWarning("Equiped tool out of tools range!");
            return;
        }

        if (debugMode)
        {
            Debug.Log(_equippedTool);
        }

        switch (data)
        {
            case CustomPlayerInput.CustomInputData.PRESSED:
                _tools[_equippedTool].UseTool();
                break;
            case CustomPlayerInput.CustomInputData.RELEASED:
                _tools[_equippedTool].CancelUseTool();
                break;
        }
        
    }

    public void AddTool(II_Tool tool, Vector2Int gridOriginPos)
    {

    }

    public void RemoveTool(II_Tool tool)
    {

    }

    private void SetToolOrder()
    {

    }

    private void OnEnable()
    {
        CustomPlayerInput.SwapTool += SwapTool;
        CustomPlayerInput.UseTool += UseEquippedTool;
    }

    private void OnDisable()
    {
        CustomPlayerInput.SwapTool -= SwapTool;
        CustomPlayerInput.UseTool -= UseEquippedTool;
    }
}
