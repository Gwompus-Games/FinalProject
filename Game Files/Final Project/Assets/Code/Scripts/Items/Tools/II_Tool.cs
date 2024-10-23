using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class II_Tool : InventoryItem
{
    protected HoldableToolSO holdableToolData;

    protected override void Awake()
    {
        base.Awake();
        holdableToolData = itemData as HoldableToolSO;
        if (holdableToolData == null)
        {
            throw new System.Exception("Item data is not a Holdable Tool type!");
        }
    }

    public override void ItemPlacedInInventory()
    {
        base.ItemPlacedInInventory();
        if (transform.parent.TryGetComponent<ToolBarGridScript>(out ToolBarGridScript toolBarGrid))
        {
            toolBarGrid.AddItemToTools(this, holdableToolData, originTile);
        }
    }

    public override void ItemRemovedFromInventory()
    {
        base.ItemRemovedFromInventory();

        if (transform.parent.TryGetComponent<ToolBarGridScript>(out ToolBarGridScript toolBarGrid))
        {
            toolBarGrid.RemoveItemFromTools(this);
        }
    }
}
