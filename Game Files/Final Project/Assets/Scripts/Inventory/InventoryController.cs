using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryController : MonoBehaviour
{
    [HideInInspector]
    public InventoryGrid selectedItemGrid;
    [SerializeField] private InventoryItem _itemToPlace;

    private void Update()
    {
        if (_itemToPlace != null)
        {
            _itemToPlace.GetComponent<RectTransform>().position = Input.mousePosition - new Vector3(InventoryGrid.tileSizeWidth / 2f, -InventoryGrid.tileSizeHeight / 2f, 0);
        }

        if (selectedItemGrid == null)
        {
            return;
        }
        if (!selectedItemGrid.gameObject.activeInHierarchy)
        {
            return;
        }

        //Debug.Log(selectedItemGrid.GetTileGridPosition(Input.mousePosition));
        if (Input.GetMouseButtonDown(0))
        {
            if (_itemToPlace == null)
            {
                SwapItemInHand(selectedItemGrid.PickupItem(Input.mousePosition));
            }
            else
            {
                if(selectedItemGrid.PlaceItem(_itemToPlace, Input.mousePosition, out InventoryItem returnItem))
                {
                    SwapItemInHand(returnItem);
                }
            }
        }
    }

    public void SwapItemInHand(InventoryItem item)
    {
        Debug.Log(item);
        _itemToPlace = item;
    }
}
