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
        inventoryGrid = GetComponent<InventoryGrid>();
    }

    private void Start()
    {
        inventoryController = GameManager.Instance.GetManagedComponent<InventoryController>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        //Debug.Log($"Entered Grid {transform.parent.name}");
        inventoryController.selectedItemGrid = inventoryGrid;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        //Debug.Log($"Exited Grid {transform.parent.name}");
        inventoryController.DisablePopup();
        inventoryController.selectedItemGrid = null;
    }
}
