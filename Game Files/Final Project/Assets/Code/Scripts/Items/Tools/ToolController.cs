using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

[RequireComponent(typeof(HandPositionController))]
public class ToolController : ManagedByGameManager
{
    public struct ToolData
    {
        public ToolData(HoldableToolSO htd, Vector2Int gPos, int tIndex = -1)
        {
            holdableToolData = htd;
            gridOriginPosition = gPos;
            toolIndex = tIndex;
        }

        public HoldableToolSO holdableToolData;
        public Vector2Int gridOriginPosition;
        public int toolIndex;
    }

    private HandPositionController _handPositionController;
    private List<ToolsParent> _tools = new List<ToolsParent>();
    private Dictionary<II_Tool, ToolData> _equippedTools = new Dictionary<II_Tool, ToolData>();
    private List<int> _toolsOrder = new List<int>();
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
        SetToolOrder();
    }

    public void SwapTool(int direction = 0)
    {
        if (direction == 0)
        {
            if (debugMode)
            {
                Debug.LogWarning("Swapping tool in no direction!");
            }
        }
        if (_equippedTool != -1)
        {
            _tools[_toolsOrder[_equippedTool]].SetToolEnabled(false);
        }
        direction = (int)Mathf.Sign(direction);
        _equippedTool += direction;
        if (_equippedTool >= _toolsOrder.Count)
        {
            _equippedTool = -1;
        }
        else if (_equippedTool < -1)
        {
            _equippedTool = _toolsOrder.Count - 1;
        }
        if (_equippedTool != -1)
        {
            _tools[_toolsOrder[_equippedTool]].SetToolEnabled(true);
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
            if (debugMode)
            {
                Debug.LogWarning("No tools found!");
            }
            return;
        }
        if (_toolsOrder.Count == 0)
        {
            if (debugMode)
            {
                Debug.Log("No equipped tools to use.");
            }
            return;
        }
        if (_equippedTool == -1)
        {
            if (debugMode)
            {
                Debug.Log("Current equipped tool is none.");
            }
            return;
        }
        if (_equippedTool >= _toolsOrder.Count || 
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
                _tools[_toolsOrder[_equippedTool]].UseTool();
                break;
            case CustomPlayerInput.CustomInputData.RELEASED:
                _tools[_toolsOrder[_equippedTool]].CancelUseTool();
                break;
        }
        
    }

    public void AddTool(II_Tool tool, HoldableToolSO holdableToolData, Vector2Int gridOriginPos)
    {
        if (_equippedTools.ContainsKey(tool))
        {
            if (debugMode)
            {
                Debug.Log($"{tool} already in equiped tools!");
            }
            return;
        }
        int index = _tools.IndexOf(_tools.Find(x => x.GetToolData() == holdableToolData));
        
        ToolData toolData = new ToolData(holdableToolData, gridOriginPos, index);
        _equippedTools.Add(tool, toolData);

        SetToolOrder();
    }

    public void RemoveTool(II_Tool tool)
    {
        if (!_equippedTools.ContainsKey(tool))
        {
            if (debugMode)
            {
                Debug.Log($"{tool} not found in equiped tools!");
            }
            return;
        }
        _equippedTools.Remove(tool);

        SetToolOrder();
    }

    private void SetToolOrder()
    {
        _equippedTool = -1;
        _toolsOrder.Clear();
        List<II_Tool> tools;
        switch (_equippedTools.Count)
        {
            case 0:
                break;
            case 1:
                tools = new List<II_Tool>(_equippedTools.Keys);
                HoldableToolSO holdableToolData = tools[0].itemData as HoldableToolSO;
                if (holdableToolData == null)
                {
                    throw new Exception("Null tool data or non holdable tool data found!");
                }
                _toolsOrder.Add(_tools.IndexOf(_tools.Find(x => x.GetToolData() == holdableToolData)));
                break;
            default:
                tools = new List<II_Tool>(_equippedTools.Keys);
                List<II_Tool> orderedTools = new List<II_Tool>();
                orderedTools.Add(tools[0]);
                for (int t = 1; t < tools.Count; t++)
                {
                    int orderedCount = orderedTools.Count;
                    Vector2Int currentElementGridOrigin = tools[t].originTile;
                    for (int o = 0; o < orderedCount; o++)
                    {
                        Vector2Int orderedGridOrigin = tools[t].originTile;
                        if (currentElementGridOrigin.x < orderedGridOrigin.x)
                        {
                            orderedTools.Insert(o, tools[t]);
                            break;
                        }
                        if (currentElementGridOrigin.x == orderedGridOrigin.x)
                        {
                            if (currentElementGridOrigin.y <= orderedGridOrigin.y)
                            {
                                orderedTools.Insert(o, tools[t]);
                                break;
                            }
                        }
                        if (o == orderedCount - 1)
                        {
                            orderedTools.Add(tools[t]);
                        }
                    }
                }
                Debug.Log(orderedTools);
                for (int i = 0; i < orderedTools.Count; i++)
                {
                    _toolsOrder.Add(_tools.IndexOf(_tools.Find(x => x.GetToolData() == orderedTools[i].itemData)));
                }
                break;
        }
        Debug.Log(_toolsOrder);
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
