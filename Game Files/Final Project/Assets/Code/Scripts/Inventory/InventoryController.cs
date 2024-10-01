using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class InventoryController : MonoBehaviour
{
    public static InventoryController INSTANCE;

    [HideInInspector]
    public InventoryGrid selectedItemGrid;
    private InventoryItem _itemToPlace;

    [SerializeField] GameObject testInventoryItemPrefab;
    [SerializeField] GameObject testInventoryItemPrefab2;

    private Vector3 _mousePosition = Vector3.zero;

    private void Awake()
    {
        if (INSTANCE != null)
        {
            Destroy(gameObject);
            return;
        }
        INSTANCE = this;
    }

    private void OnEnable()
    {
        CustomPlayerInput.UpdateCursorPosition += UpdateMousePos;
        CustomPlayerInput.Rotate += RotateItem;
        CustomPlayerInput.LeftMouseButton += PlaceInput;
        CustomPlayerInput.RightMouseButton += DropItemInput;
    }

    private void OnDisable()
    {
        CustomPlayerInput.UpdateCursorPosition -= UpdateMousePos;
        CustomPlayerInput.Rotate -= RotateItem;
        CustomPlayerInput.LeftMouseButton -= PlaceInput;
        CustomPlayerInput.RightMouseButton -= DropItemInput;
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
    }

    public void SwapItemInHand(InventoryItem item)
    {
        if (item != null)
        {
            item.GetComponent<RectTransform>().SetParent(FindFirstObjectByType<Canvas>().GetComponent<RectTransform>());
            item.GetComponent<RectTransform>().position = _mousePosition;
        }
        _itemToPlace = item;
    }

    public void InventoryClosing()
    {
        DropItemIntoWorld();
    }

    private void DropItemInput(CustomPlayerInput.CustomInputData data)
    {
        if (data != CustomPlayerInput.CustomInputData.PRESSED)
        {
            DropItemIntoWorld();
        }
    }

    private void PlaceInput(CustomPlayerInput.CustomInputData data)
    {
        if (data != CustomPlayerInput.CustomInputData.PRESSED)
        {
            return;
        }

        if (selectedItemGrid == null)
        {
            if (_itemToPlace != null)
            {
                DropItemIntoWorld();
            }
            return;
        }

        if (!selectedItemGrid.gameObject.activeInHierarchy)
        {
            return;
        }

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
            if (selectedItemGrid.PlaceItem(_itemToPlace, _mousePosition, out InventoryItem returnItem))
            {
                SwapItemInHand(returnItem);
            }
            else
            {
                _itemToPlace.InvalidPlacementFlash();
            }
        }
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

    public void AddItemToInventory(ItemDataSO itemData)
    {
        _itemToPlace = Instantiate(itemData.inventoryObject).GetComponent<InventoryItem>();
        _itemToPlace.GetComponent<RectTransform>().SetParent(FindFirstObjectByType<Canvas>().GetComponent<RectTransform>());
        _itemToPlace.InitializeInventoryItem(itemData);
        PlayerController.INSTANCE.OpenInventory();
    }

    private void DropItemIntoWorld()
    {
        if (_itemToPlace == null)
        {
            return;
        }
        WorldItem worldItemWO = Instantiate(_itemToPlace.itemData.worldObject).GetComponent<WorldItem>();
        worldItemWO.transform.parent = FindObjectOfType<WorldItemsTag>().transform;
        Vector3 spawnPoint = PlayerController.INSTANCE.transform.position + (PlayerController.INSTANCE.transform.forward * 1.25f);
        worldItemWO.SpawnItem(spawnPoint, _itemToPlace.itemData);
        Destroy(_itemToPlace.gameObject);
        SwapItemInHand(null);
    }
}
