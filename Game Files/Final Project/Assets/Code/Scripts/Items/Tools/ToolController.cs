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
        public void SetValues(HoldableToolSO htd, Vector2Int gPos, II_Tool iTool, int tIndex, int eIndex)
        {
            holdableToolData = htd;
            gridOriginPosition = gPos;
            inventoryTool = iTool;
            toolIndex = tIndex;
            equippedIndex = eIndex;
        }

        public void SetValues(ToolData toolData)
        {
            holdableToolData = toolData.holdableToolData;
            gridOriginPosition = toolData.gridOriginPosition;
            inventoryTool = toolData.inventoryTool;
            toolIndex = toolData.toolIndex;
            equippedIndex = toolData.equippedIndex;
        }

        public HoldableToolSO holdableToolData;
        public Vector2Int gridOriginPosition;
        public II_Tool inventoryTool;
        public int toolIndex;
        public int equippedIndex;
    }

    private HandPositionController _handPositionController;
    private UIManager _uiManager;

    private List<ToolsParent> _tools = new List<ToolsParent>();
    private Dictionary<II_Tool, ToolData> _equippedTools = new Dictionary<II_Tool, ToolData>();
    private List<int> _toolsOrder = new List<int>();
    private int _equippedTool = -1;
    private Dictionary<int, II_Glowstick> _glowstickPositions = new Dictionary<int, II_Glowstick>();
    [field :SerializeField] public bool debugMode { get; private set; } = false;
    private Queue<II_Glowstick> _glowsticksToUse = new Queue<II_Glowstick>();

    public override void Init()
    {
        if (_initilized)
        {
            return;
        }
        base.Init();

        _handPositionController = GameManager.Instance.GetManagedComponent<HandPositionController>();
        _uiManager = GameManager.Instance.GetManagedComponent<UIManager>();

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
        if (_uiManager.currentUIState != UIManager.UIToDisplay.GAME)
        {
            if (debugMode)
            {
                Debug.Log("Not currently on game view!");
            }
            return;
        }

        if (direction == 0)
        {
            if (debugMode)
            {
                Debug.LogWarning("Swapping tool in no direction!");
            }
        }
        if (_equippedTool >= 0 && _equippedTool < _toolsOrder.Count)
        {
            if (debugMode)
            {
                Debug.Log($"Deactivating tool: {_tools[_toolsOrder[_equippedTool]].name} | In position: {_equippedTool}");
            }
            ToolData toolData = _equippedTools.Values.FirstOrDefault(t => t.toolIndex == _equippedTool);

            if (debugMode)
            {
                Debug.Log($"Found {toolData} as for tool data! {toolData.inventoryTool} is found for the inventory tool!");
            }

            if (toolData.inventoryTool != null)
            {
                if (debugMode)
                {
                    Debug.Log($"{toolData.inventoryTool.name} being deselected.");
                }

                toolData.inventoryTool.ToolDeselected();
            }
            _tools[_toolsOrder[_equippedTool]].SetToolEnabled(false);
        }

        if (debugMode)
        {
            Debug.Log($"Swapping from tool {_equippedTool}");
        }

        direction = -(int)Mathf.Sign(direction);
        _equippedTool += direction;


        if (_equippedTool >= _toolsOrder.Count)
        {
            _equippedTool = -1;
        }
        else if (_equippedTool < -1)
        {
            _equippedTool = _toolsOrder.Count - 1;
        }

        if (debugMode)
        {
            Debug.Log($"Swapping to tool {_equippedTool}");
        }

        if (_equippedTool != -1)
        {
            int toolEquiped = _toolsOrder[_equippedTool];
            ToolData toolData = _equippedTools.Values.FirstOrDefault(t => t.toolIndex == _equippedTool);
            if (toolData.inventoryTool != null)
            {
                toolData.inventoryTool.ToolSelected();
            }
            _tools[toolEquiped].SetToolEnabled(true);
            if (_glowstickPositions.ContainsKey(_equippedTool))
            {
                GlowstickTool glowstickTool = _tools[toolEquiped] as GlowstickTool;
                if (glowstickTool != null)
                {
                    if (debugMode)
                    {
                        Debug.Log($"Swapping to: {_glowstickPositions[_equippedTool].itemData.itemName} | Colour: {_glowstickPositions[_equippedTool].GetColour()}");
                    }
                    glowstickTool.SetToolData(_glowstickPositions[_equippedTool]);
                }
            }
        }
    }

    /// <summary>
    /// Use this function in the glowstick inventory item script to add it to the queue whenever it is added to inventory
    /// </summary>
    /// <param name="glowstickToAdd">this is for what glowstick is supposed to be added</param>
    public void AddGlowstickToQueue(II_Glowstick glowstickToAdd)
    {
        if (_glowsticksToUse.Contains(glowstickToAdd))
        {
            return;
        }
        _glowsticksToUse.Enqueue(glowstickToAdd);
    }

    /// <summary>
    /// Use this function to remove a glowstick from the queue
    /// </summary>
    /// <param name="glowstickToRemove">this is for what glowstick is supposed to be removed</param>
    public void RemoveGlowstickFromQueue(II_Glowstick glowstickToRemove)
    {
        if (!_glowsticksToUse.Contains(glowstickToRemove))
        {
            return;
        }

        int glowstickCount = _glowsticksToUse.Count;
        if (glowstickCount == 0)
        {
            return;
        }

        II_Glowstick firstGlowstickInQueue = _glowsticksToUse.Peek();

        if (firstGlowstickInQueue == glowstickToRemove)
        {
            _glowsticksToUse.Dequeue();
            return;
        }

        _glowsticksToUse.Enqueue(_glowsticksToUse.Dequeue());
        while (_glowsticksToUse.Peek() != firstGlowstickInQueue)
        {
            if (_glowsticksToUse.Peek() == glowstickToRemove)
            {
                _glowsticksToUse.Dequeue();
                continue;
            }

            _glowsticksToUse.Enqueue(_glowsticksToUse.Dequeue());
        }
    }

    /// <summary>
    /// Use the next glowstick in the queue
    /// </summary>
    public void UseNextGlowstickFromQueue()
    {
        //Exit out if we aren't in game view
        if (_uiManager.currentUIState != UIManager.UIToDisplay.GAME)
        {
            if (debugMode)
            {
                Debug.Log("Not currently on game view!");
            }
            return;
        }

        //Exit out if glowsticks count is 0
        if (_glowsticksToUse.Count == 0)
        {
            return;
        }

        //Make sure we never let a null value through
        while (_glowsticksToUse.Peek() == null)
        {
            _glowsticksToUse.Dequeue();
            //Exit out if glowsticks count is 0
            if (_glowsticksToUse.Count == 0)
            {
                return;
            }
        }

        //Get the glowstick tool
        GlowstickTool glowstickTool = null;
        for (int t = 0; t < _tools.Count; t++)
        {
            glowstickTool = _tools[t] as GlowstickTool;
            if (glowstickTool != null)
            {
                break;
            }
        }

        //Break out if no glowstick tool in tools list
        if (glowstickTool == null)
        {
            Debug.LogError("No glowstick tool created in player!");
            return;
        }

        //Get data for next glowstick
        II_Glowstick glowstick = _glowsticksToUse.Dequeue();
        Color glowstickColour = glowstick.GetColour();
        
        //Use the next glowstick in the queue
        glowstickTool.ThrowGlowstick(glowstick, glowstickColour);
    }

    private void UseEquippedTool(CustomPlayerInput.CustomInputData data)
    {
        if (debugMode)
        {
            Debug.Log("Use input detected!");
        }

        if (_uiManager.currentUIState != UIManager.UIToDisplay.GAME)
        {
            if (debugMode)
            {
                Debug.Log("Not currently on game view!");
            }
            return;
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

        switch (data)
        {
            case CustomPlayerInput.CustomInputData.PRESSED:
                _tools[_toolsOrder[_equippedTool]].UseTool();
                if (debugMode)
                {
                    Debug.Log($"Using tool: {_equippedTool}");
                }
                break;
            case CustomPlayerInput.CustomInputData.RELEASED:
                _tools[_toolsOrder[_equippedTool]].CancelUseTool();
                if (debugMode)
                {
                    Debug.Log($"Canceling Using tool: {_equippedTool}");
                }
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
        int toolIndex = _tools.IndexOf(_tools.Find(x => x.GetToolData() == holdableToolData));

        ToolData toolData = new ToolData();
        toolData.SetValues(holdableToolData, gridOriginPos, tool, toolIndex, -1);
        
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
        ToolData toolData = new ToolData();

        switch (_equippedTools.Count)
        {
            case 0:
                break;
            case 1:
                tools = new List<II_Tool>(_equippedTools.Keys);
                toolData.SetValues(_equippedTools[tools[0]]);
                toolData.equippedIndex = 0;
                toolData.inventoryTool = tools[0];
                _equippedTools[tools[0]].SetValues(toolData);
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

                    toolData.SetValues(_equippedTools[orderedTools[i]]);
                    toolData.equippedIndex = i;
                    toolData.inventoryTool = orderedTools[i];
                    _equippedTools[orderedTools[i]].SetValues(toolData);

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
        SwapTool(1);
    }

    private void OnEnable()
    {
        CustomPlayerInput.SwapTool += SwapTool;
        CustomPlayerInput.UseTool += UseEquippedTool;
        CustomPlayerInput.QuickUseGlowstick += UseNextGlowstickFromQueue;
    }

    private void OnDisable()
    {
        CustomPlayerInput.SwapTool -= SwapTool;
        CustomPlayerInput.UseTool -= UseEquippedTool;
        CustomPlayerInput.QuickUseGlowstick -= UseNextGlowstickFromQueue;
    }
}
