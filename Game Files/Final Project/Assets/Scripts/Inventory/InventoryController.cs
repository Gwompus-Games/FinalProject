using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryController : MonoBehaviour
{
    [HideInInspector]
    public InventoryGrid selectedItemGrid;
    private InventoryItem _itemToPlace;

    [SerializeField] GameObject testInventoryItemPrefab;
    [SerializeField] GameObject testInventoryItemPrefab2;

    private void Update()
    {
        if (_itemToPlace == null)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                _itemToPlace = Instantiate(testInventoryItemPrefab).GetComponent<InventoryItem>();
                _itemToPlace.GetComponent<RectTransform>().SetParent(FindFirstObjectByType<Canvas>().GetComponent<RectTransform>());
            }

            if (Input.GetKeyDown(KeyCode.E))
            {
                _itemToPlace = Instantiate(testInventoryItemPrefab2).GetComponent<InventoryItem>();
                _itemToPlace.GetComponent<RectTransform>().SetParent(FindFirstObjectByType<Canvas>().GetComponent<RectTransform>());
            }
        }

        if (_itemToPlace != null)
        {
            _itemToPlace.GetComponent<RectTransform>().position = Input.mousePosition;
            if (Input.GetKeyDown(KeyCode.R))
            {
                _itemToPlace.RotateClockwise();
            }
            if (Input.GetKeyDown(KeyCode.Q))
            {
                _itemToPlace.RotateCounterClockwise();
            }
            if (Input.GetMouseButtonDown(1))
            {
                Destroy(_itemToPlace.gameObject);
                _itemToPlace = null;
            }
        }

        if (selectedItemGrid == null)
        {
            return;
        }
        if (!selectedItemGrid.gameObject.activeInHierarchy)
        {
            return;
        }

        if (Input.GetMouseButtonDown(0))
        {
            if (_itemToPlace == null)
            {
                InventoryItem item = selectedItemGrid.PickupItem(Input.mousePosition);
                if (item != null)
                {
                    SwapItemInHand(item);
                }
            }
            else
            {
                if(selectedItemGrid.PlaceItem(_itemToPlace, Input.mousePosition, out InventoryItem returnItem))
                {
                    SwapItemInHand(returnItem);
                }
                else
                {
                    _itemToPlace.InvalidPlacementFlash();
                }
            }
        }
    }

    public void SwapItemInHand(InventoryItem item)
    {
        Debug.Log(item);
        if (item != null)
        {
            item.GetComponent<RectTransform>().SetParent(FindFirstObjectByType<Canvas>().GetComponent<RectTransform>());
        }
        _itemToPlace = item;
    }
}
