using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class II_Tool : InventoryItem
{
    protected override void Awake()
    {
        base.Awake();
        HoldableToolSO holdableToolSO = itemData as HoldableToolSO;
        if (holdableToolSO == null)
        {
            throw new System.Exception("Item data is not a Holdable Tool type!");
        }
    }

    public override void ItemPlacedInInventory()
    {
        base.ItemPlacedInInventory();

        if (transform.parent.TryGetComponent<ToolBarGridScript>(out ToolBarGridScript toolBarGrid))
        {
            toolBarGrid.AddItemToTools(this, originTile);
        }
    }

    public override void ItemRemovedFromInventory()
    {
        base.ItemRemovedFromInventory();

        if (transform.parent.TryGetComponent<ToolBarGridScript>(out ToolBarGridScript toolBarGrid))
        {

        }
    }
}
