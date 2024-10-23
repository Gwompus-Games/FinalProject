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
    private int _equippedTool = 0;

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
        }
    }

    public override void CustomStart()
    {
        base.CustomStart();
    }

    public void SwapTool(int direction)
    {
        if (direction == 0)
        {
            Debug.LogWarning("Tried to swap tool in no direction!");
            return;
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

    }

    private void UseEquippedTool(CustomPlayerInput.CustomInputData data)
    {
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


        if (data == CustomPlayerInput.CustomInputData.PRESSED)
        {
            _tools[_equippedTool].UseTool();
        }
    }

    private void OnEnable()
    {
        CustomPlayerInput.SwapTool += SwapTool;
    }

    private void OnDisable()
    {
        CustomPlayerInput.SwapTool -= SwapTool;
    }
}
