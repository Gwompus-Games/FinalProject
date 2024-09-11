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

    private Vector3 _mousePosition = Vector3.zero;

    private void OnEnable()
    {
        CustomPlayerInput.UpdateMousePosition += UpdateMousePos;
        CustomPlayerInput.Rotate += RotateItem;
    }

    private void OnDisable()
    {
        CustomPlayerInput.UpdateMousePosition -= UpdateMousePos;
        CustomPlayerInput.Rotate -= RotateItem;
    }

    private void Update()
    {
        if (_itemToPlace == null)
        {
            if (Input.GetKeyDown(KeyCode.O))
            {
                _itemToPlace = Instantiate(testInventoryItemPrefab).GetComponent<InventoryItem>();
                _itemToPlace.GetComponent<RectTransform>().SetParent(FindFirstObjectByType<Canvas>().GetComponent<RectTransform>());
            }

            if (Input.GetKeyDown(KeyCode.P))
            {
                _itemToPlace = Instantiate(testInventoryItemPrefab2).GetComponent<InventoryItem>();
                _itemToPlace.GetComponent<RectTransform>().SetParent(FindFirstObjectByType<Canvas>().GetComponent<RectTransform>());
            }
        }

        if (_itemToPlace != null)
        {
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
                InventoryItem item = selectedItemGrid.PickupItem(_mousePosition);
                if (item != null)
                {
                    SwapItemInHand(item);
                }
            }
            else
            {
                if(selectedItemGrid.PlaceItem(_itemToPlace, _mousePosition, out InventoryItem returnItem))
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
            item.GetComponent<RectTransform>().position = _mousePosition;
        }
        _itemToPlace = item;
    }

    public void UpdateMousePos(Vector2 mousePosition)
    {
        _mousePosition = mousePosition;
        if (_itemToPlace != null)
        {
            _itemToPlace.GetComponent<RectTransform>().position = _mousePosition;
        }
    }

    public void RotateItem(int direction)
    {
        if (_itemToPlace == null)
        {
            return;
        }

        switch (direction)
        {
            case 0:
                return;
            case 1:
                _itemToPlace.RotateClockwise();
                break;
            case -1:
                _itemToPlace.RotateCounterClockwise();
                break;
            default:
                Debug.LogError("Rotate failed, invalid input received.");
                return;
        }
    }
}
