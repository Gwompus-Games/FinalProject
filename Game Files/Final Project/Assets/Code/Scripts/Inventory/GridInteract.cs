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
        //Debug.Log($"Entered Grid {transform.parent.name}");
        inventoryController.selectedItemGrid = inventoryGrid;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        //Debug.Log($"Exited Grid {transform.parent.name}");
        inventoryController.selectedItemGrid = null;
    }
}
