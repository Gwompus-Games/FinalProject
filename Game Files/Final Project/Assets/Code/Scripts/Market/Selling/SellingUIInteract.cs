using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SellingUIInteract : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private InventoryController _inventoryController;
    private ScrappingZone _scrapping;

    private void Awake()
    {
        _inventoryController = FindObjectOfType(typeof(InventoryController)) as InventoryController;
        _scrapping = GetComponent<ScrappingZone>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        _inventoryController.selectedScrappingZone = _scrapping;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _inventoryController.selectedScrappingZone = null;
    }
}
