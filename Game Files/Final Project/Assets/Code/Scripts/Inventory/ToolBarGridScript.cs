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

    public void AddItemToTools(II_Tool tool, HoldableToolSO holdableToolData, Vector2Int gridOriginPos)
    {
        _toolController.AddTool(tool, holdableToolData, gridOriginPos);
    }

    public void RemoveItemFromTools(II_Tool tool)
    {
        _toolController.RemoveTool(tool);
    }

    protected override IEnumerator Hover(InventoryItem item)
    {
        InventoryPopupUI popupUI = GetComponentInParent<ToolBarUI>().popupUI;
        if (popupUI == null)
        {
            item = null;
        }
        yield return null;
        if (_hoveredItem == item && item != null)
        {
            yield return new WaitForSeconds(popupUI.popupHoverTimeInSeconds);
            if (_hoveredItem == item)
            {
                popupUI.EnablePopup(_cursorPos);
                popupUI.SetPopupData(item);
                II_OxygenTank oxygenItem = item as II_OxygenTank;
                do
                {
                    if (oxygenItem != null)
                    {
                        popupUI.UpdatePopup(_cursorPos, oxygenItem);
                    }
                    else
                    {
                        popupUI.UpdatePopup(_cursorPos);
                    }
                    yield return new WaitForSeconds(0.15f);
                } while (_hoveredItem == item);
            }
        }
        popupUI.DisablePopup();
    }
}
