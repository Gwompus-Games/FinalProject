using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using static ToolController;

[RequireComponent(typeof(HandPositionController))]
public class ToolController : ManagedByGameManager
{
    public struct ToolData
    {
        public void SetValues(HoldableToolSO htd, Vector2Int gPos, ToolsParent pTool)
        {
            holdableToolData = htd;
            gridOriginPosition = gPos;
            inventoryTool = pTool;
        }

        public void SetValues(ToolData toolData)
        {
            holdableToolData = toolData.holdableToolData;
            gridOriginPosition = toolData.gridOriginPosition;
            inventoryTool = toolData.inventoryTool;
        }

        public HoldableToolSO holdableToolData;
        public Vector2Int gridOriginPosition;
        public ToolsParent inventoryTool;
    }

    private HandPositionController _handPositionController;
    private UIManager _uiManager;

    private List<ToolsParent> _parentTools = new List<ToolsParent>();
    private Dictionary<II_Tool, ToolData> _equippedTools = new Dictionary<II_Tool, ToolData>();
    private List<II_Tool> _toolsOrder = new List<II_Tool>();
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
            _parentTools.Add(tool);
            tool.Init();
            if (debugMode)
            {
                Debug.Log($"Initilizing: {tool.gameObject.name}");
            }
        }

    }

    public override void CustomStart()
    {
        base.CustomStart();
        for (int t = 0; t < _parentTools.Count; t++)
        {
            _parentTools[t].CustomStart();
            if (debugMode)
            {
                Debug.Log($"Starting: {_parentTools[t].gameObject.name}");
            }
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
                Debug.Log($"Deactivating tool: {_toolsOrder[_equippedTool].name} | In position: {_equippedTool}");
            }

            _toolsOrder[_equippedTool].ToolDeselected();
            _equippedTools[_toolsOrder[_equippedTool]].inventoryTool.SetToolEnabled(false);
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
            II_Tool toolEquiped = _toolsOrder[_equippedTool];
            ToolsParent activeTool = _equippedTools[toolEquiped].inventoryTool;
            activeTool.SetToolEnabled(true);
            toolEquiped.ToolSelected();
            if (_glowstickPositions.ContainsKey(_equippedTool))
            {
                GlowstickTool glowstickTool = activeTool as GlowstickTool;
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
        for (int t = 0; t < _parentTools.Count; t++)
        {
            glowstickTool = _parentTools[t] as GlowstickTool;
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

        if (_parentTools.Count == 0)
        {
            if (debugMode)
            {
                Debug.LogError("Parent tools not set up properly!");
            }
            return;
        }

        if (_equippedTools.Count == 0)
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
                _equippedTools[_toolsOrder[_equippedTool]].inventoryTool.UseTool();
                if (debugMode)
                {
                    Debug.Log($"Using tool: {_equippedTool}");
                }
                break;
            case CustomPlayerInput.CustomInputData.RELEASED:
                _equippedTools[_toolsOrder[_equippedTool]].inventoryTool.CancelUseTool();
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
        
        int toolIndex = _parentTools.IndexOf(_parentTools.Find(x => x.GetToolData() == holdableToolData));

        ToolData toolData = new ToolData();
        toolData.SetValues(holdableToolData, gridOriginPos, _parentTools[toolIndex]);
        
        _equippedTools.Add(tool, toolData);

        if (debugMode)
        {
            Debug.Log($"{tool.gameObject.name} added to tools list!");
        }

        SetToolOrder();
    }

    public void RemoveTool(II_Tool tool)
    {
        if (_equippedTool > -1 && _equippedTool < _toolsOrder.Count)
        {
            _equippedTools[_toolsOrder[_equippedTool]].inventoryTool.SetToolEnabled(false);
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

        if (debugMode)
        {
            Debug.Log($"{tool.gameObject.name} removed from tools list!");
        }

        SetToolOrder();
    }

    private void SetToolOrder()
    {
        _equippedTool = -1;
        _toolsOrder.Clear();
        _glowstickPositions.Clear();
        List<II_Tool> ii_Tools;

        switch (_equippedTools.Count)
        {
            case 0:
                break;
            case 1:
                ii_Tools = new List<II_Tool>(_equippedTools.Keys);
                HoldableToolSO holdableToolData = ii_Tools[0].itemData as HoldableToolSO;
                if (holdableToolData == null)
                {
                    throw new Exception("Null tool data or non holdable tool data found!");
                }
                _toolsOrder.Add(ii_Tools[0]);
                II_Glowstick glowstick = ii_Tools[0] as II_Glowstick;
                if (glowstick != null)
                {
                    _glowstickPositions.Add(0, glowstick);
                }
                break;
            default:
                ii_Tools = new List<II_Tool>(_equippedTools.Keys);
                List<II_Tool> orderedTools = new List<II_Tool>();
                orderedTools.Add(ii_Tools[0]);
                for (int t = 1; t < ii_Tools.Count; t++)
                {
                    int orderedCount = orderedTools.Count;
                    Vector2Int currentElementGridOrigin = ii_Tools[t].originTile;
                    for (int o = 0; o < orderedCount; o++)
                    {
                        Vector2Int orderedGridOrigin = ii_Tools[t].originTile;
                        if (currentElementGridOrigin.x < orderedGridOrigin.x)
                        {
                            orderedTools.Insert(o, ii_Tools[t]);
                            break;
                        }
                        if (currentElementGridOrigin.x == orderedGridOrigin.x)
                        {
                            if (currentElementGridOrigin.y <= orderedGridOrigin.y)
                            {
                                orderedTools.Insert(o, ii_Tools[t]);
                                break;
                            }
                        }
                        if (o == orderedCount - 1)
                        {
                            orderedTools.Add(ii_Tools[t]);
                        }
                    }
                }
                for (int i = 0; i < orderedTools.Count; i++)
                {
                    _toolsOrder.Add(orderedTools[i]);

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
