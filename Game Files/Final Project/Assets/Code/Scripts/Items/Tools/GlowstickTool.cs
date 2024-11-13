using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlowstickTool : ToolsParent
{
    private Material _material;
    private Color _chosenColour;
    private II_Glowstick _myInventoryItem;
    private ToolController _toolController;

    public override void Init()
    {
        base.Init();
        GlowstickSO toolData = _myTool.toolData as GlowstickSO;
        if (toolData == null)
        {
            throw new System.Exception($"Wrong data type assigned for {gameObject}");
        }
        _material = GetComponentInChildren<MeshRenderer>().material;
        _toolController = GameManager.Instance.GetManagedComponent<ToolController>();
    }

    public void SetToolData(II_Glowstick item)
    {
        _myInventoryItem = item;
        AssignColour(_myInventoryItem.GetColour());
    }

    public void AssignColour(Color colourToAssign)
    {
        _chosenColour = colourToAssign;
        _material.color = colourToAssign;
    }

    public II_Glowstick GetToolInventoryItem()
    {
        return _myInventoryItem;
    }

    public override void UseTool()
    {
        if (_myInventoryItem == null)
        {
            return;
        }
        Transform worldItemParent = FindFirstObjectByType<WorldItemsTag>().transform;
        WI_Glowstick glowstickWorldItem = Instantiate(_myInventoryItem.itemData.worldObject, worldItemParent, true).GetComponent<WI_Glowstick>();
        glowstickWorldItem.transform.position = transform.position;
        glowstickWorldItem.UseGlowstick(_chosenColour);
        Destroy(_myInventoryItem.gameObject);
        SetToolEnabled(false);
        _toolController.ResetTools();
    }

    public override void CancelUseTool()
    {
        
    }
}