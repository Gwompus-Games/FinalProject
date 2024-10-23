using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SellingUIInteract : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private InventoryController _inventoryController;
    private SellingZone _sellingZone;
    private SellValueUIScript _uiComponent;

    private void Start()
    {
        _inventoryController = GameManager.Instance.GetManagedComponent<InventoryController>();
        _sellingZone = GetComponent<SellingZone>();
        _uiComponent = _sellingZone.GetSellComponentUI();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        _inventoryController.selectedSellingZone = _sellingZone;
        if (_uiComponent != null)
        {
            InventoryItem item = _inventoryController.GetItemInHand();
            if (item != null)
            {
                _uiComponent.UpdateUI(item.sellValue);
            }
            else
            {
                _uiComponent.UpdateUI(0);
            }
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _inventoryController.selectedSellingZone = null;
        if (_uiComponent != null)
        {
            _uiComponent.UpdateUI(0);
        }
    }
}
