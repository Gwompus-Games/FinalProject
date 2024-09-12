using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(InventoryGrid))]
public class GridInteract : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    InventoryController inventoryController;
    InventoryGrid inventoryGrid;

    private void Awake()
    {
        inventoryController = FindObjectOfType(typeof(InventoryController)) as InventoryController;
        inventoryGrid = GetComponent<InventoryGrid>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        inventoryController.selectedItemGrid = inventoryGrid;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        inventoryController.selectedItemGrid = null;
    }
}
