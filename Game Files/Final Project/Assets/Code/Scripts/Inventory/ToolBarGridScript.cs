using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolBarGridScript : InventoryGrid
{
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
}
