using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

public class ToolController : ManagedByGameManager
{
    private HandPositionController _handPositionController;
    private List<ToolsParent> _tools = new List<ToolsParent>();
    private int _equippedTool = 0;

    private void Awake()
    {
        CustomPlayerInput.LeftMouseButton += UseEquippedTool;
    }
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

    private void UseEquippedTool(CustomPlayerInput.CustomInputData data)
    {
        if (_tools.Count == 0)
        {
            return;
        }

        if (_equippedTool >= _tools.Count)
        {
            Debug.LogWarning("Equiped tool out of tools range!");
            return;
        }

        if (_equippedTool < 0)
        {
            return;
        }

        if (data == CustomPlayerInput.CustomInputData.PRESSED)
        {
            _tools[_equippedTool].UseTool();
        }
    }
}
