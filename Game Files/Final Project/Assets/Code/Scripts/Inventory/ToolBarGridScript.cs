using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolBarGridScript : InventoryGrid
{
    private ToolController _toolController;

    public override void Init()
    {
        base.Init();
        _toolController = GameManager.Instance.GetManagedComponent<ToolController>();
    }

    public override bool PlaceItem(InventoryItem inventoryItem, Vector2Int gridPosition, out InventoryItem returnItem)
    {
        HoldableToolSO holdableToolData = inventoryItem.itemData as HoldableToolSO;
        returnItem = null;
        if (holdableToolData == null)
        {
            return false;
        }
        return base.PlaceItem(inventoryItem, gridPosition, out returnItem);
    }

    public void AddItemToTools(HoldableToolSO holdableToolSO, Vector2Int gridOriginPos)
    {

    }

    public void RemoveItemFromTools(HoldableToolSO holdableToolSO)
    {

    }
}
