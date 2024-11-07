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
        public ToolData(HoldableToolSO htd, Vector2Int gPos, II_Tool iTool, int tIndex = -1)
        {
            holdableToolData = htd;
            gridOriginPosition = gPos;
            inventoryTool = iTool;
            toolIndex = tIndex;
        }

        public HoldableToolSO holdableToolData;
        public Vector2Int gridOriginPosition;
        public II_Tool inventoryTool;
        public int toolIndex;
    }

    private HandPositionController _handPositionController;
    private List<ToolsParent> _tools = new List<ToolsParent>();
    private Dictionary<II_Tool, ToolData> _equippedTools = new Dictionary<II_Tool, ToolData>();
    private List<int> _toolsOrder = new List<int>();
    private int _equippedTool = -1;
    private Dictionary<int, II_Glowstick> _glowstickPositions = new Dictionary<int, II_Glowstick>();
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
        if (_equippedTool >= 0 && _equippedTool < _toolsOrder.Count)
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
            int toolEquiped = _toolsOrder[_equippedTool];
            _tools[toolEquiped].SetToolEnabled(true);
            if (_glowstickPositions.ContainsKey(_equippedTool))
            {
                GlowstickTool glowstickTool = _tools[toolEquiped] as GlowstickTool;
                if (glowstickTool != null)
                {
                    glowstickTool.SetToolData(_glowstickPositions[_equippedTool]);
                }
            }
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

        ToolData toolData = new ToolData(holdableToolData, gridOriginPos, tool, index);
        
        _equippedTools.Add(tool, toolData);

        SetToolOrder();
    }

    public void RemoveTool(II_Tool tool)
    {
        if (_equippedTool > -1 && _equippedTool < _toolsOrder.Count)
        {
            _tools[_toolsOrder[_equippedTool]].SetToolEnabled(false);
        }
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
        _glowstickPositions.Clear();
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
                II_Glowstick glowstick = tools[0] as II_Glowstick;
                if (glowstick != null)
                {
                    _glowstickPositions.Add(0, glowstick);
                }
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
                for (int i = 0; i < orderedTools.Count; i++)
                {
                    _toolsOrder.Add(_tools.IndexOf(_tools.Find(x => x.GetToolData() == orderedTools[i].itemData)));
                    II_Glowstick iGlowstick = orderedTools[i] as II_Glowstick;
                    if (iGlowstick != null)
                    {
                        if (_glowstickPositions.ContainsKey(i))
                        {
                            throw new Exception($"Glowstick key {i} already added!");
                        }
                        _glowstickPositions.Add(i, iGlowstick);
                    }
                }
                break;
        }
    }

    public void ResetTools()
    {
        int equipedTool = _equippedTool;
        SetToolOrder();
        _equippedTool = equipedTool;
        SwapTool(-1);
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
