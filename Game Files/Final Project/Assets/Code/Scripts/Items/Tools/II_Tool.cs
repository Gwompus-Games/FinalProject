using System.Diagnostics;
using UnityEngine;

public class II_Tool : InventoryItem
{
    protected HoldableToolSO _holdableToolData;
    protected ToolController _toolController;

    protected override void Awake()
    {
        base.Awake();
        _holdableToolData = itemData as HoldableToolSO;
        if (_holdableToolData == null)
        {
            throw new System.Exception("Item data is not a Holdable Tool type!");
        }

        _toolController = GameManager.Instance.GetManagedComponent<ToolController>();
        if (_toolController == null)
        {
            throw new System.Exception("Tool controller not found!");
        }
    }

    public override void ItemPlacedInInventory()
    {
        base.ItemPlacedInInventory();
        if (transform.parent.TryGetComponent<ToolBarGridScript>(out ToolBarGridScript toolBarGrid))
        {
            toolBarGrid.AddItemToTools(this, _holdableToolData, originTile);
        }
    }

    public override void ItemRemovedFromInventory()
    {
        base.ItemRemovedFromInventory();

        if (transform.parent.TryGetComponent<ToolBarGridScript>(out ToolBarGridScript toolBarGrid))
        {
            toolBarGrid.RemoveItemFromTools(this);
            ToolDeselected();
        }
    }

    public void ToolSelected()
    {
        if (_toolController.debugMode)
        {
            UnityEngine.Debug.Log($"{gameObject.name} selected!");
        }

        StopInvalidPlacementFlashing();
        ChangeTileColours(InventoryGrid.globalItemData.selectedToolTileColour, InventoryGrid.globalItemData.tileAlpha);
    }
    public void ToolDeselected()
    {
        if (_toolController.debugMode)
        {
            UnityEngine.Debug.Log($"{gameObject.name} deselected!");
        }

        StopInvalidPlacementFlashing();
        ChangeTileColours(InventoryGrid.globalItemData.normalTileColour, InventoryGrid.globalItemData.tileAlpha);
    }
}
