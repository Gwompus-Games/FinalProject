using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class InventoryController : MonoBehaviour
{
    public static InventoryController Instance;

    [HideInInspector]
    public InventoryGrid selectedItemGrid;
    [HideInInspector]
    public SellingZone selectedSellingZone;
    private InventoryItem _itemToPlace;
    [SerializeField] private InventoryGrid _inventory;

    [SerializeField] GameObject testInventoryItemPrefab;
    [SerializeField] GameObject testInventoryItemPrefab2;

    private Vector3 _mousePosition = Vector3.zero;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        InventoryGrid inventoryGrid = FindObjectOfType<InventoryTag>().GetComponent<InventoryGrid>();
        if (inventoryGrid != null)
        {
            _inventory = inventoryGrid;
        }
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
            item.GetComponent<RectTransform>().SetParent(FindFirstObjectByType<InventoryCanvasTag>().GetComponent<RectTransform>());
            item.GetComponent<RectTransform>().position = _mousePosition;
        }
        _itemToPlace = item;
    }

    public InventoryItem GetItemInHand()
    {
        return _itemToPlace;
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

        if (selectedItemGrid == null && selectedSellingZone == null)
        {
            if (_itemToPlace != null)
            {
                DropItemIntoWorld();
            }
            return;
        }

        if (selectedItemGrid != null)
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
        else if (selectedSellingZone != null)
        {
            if (_itemToPlace != null)
            {
                selectedSellingZone.SellItem(_itemToPlace);
            }
            else
            {

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
        bool startedOpen = (GameManager.PlayerControllerInstance.currentState == PlayerController.PlayerState.Inventory);
        if (!startedOpen)
        {
            GameManager.PlayerControllerInstance.OpenInventory();
        }
        InventoryItem inventoryItem;
        if (CheckInventorySpaceAvailable(itemData, out inventoryItem))
        {
            if (startedOpen == false)
            {
                GameManager.PlayerControllerInstance.CloseInventory();
            }
            return;
        }
        _itemToPlace = inventoryItem;
        _itemToPlace.GetComponent<RectTransform>().SetParent(FindFirstObjectByType<InventoryCanvasTag>().GetComponent<RectTransform>());
        _itemToPlace.InitializeInventoryItem(itemData);
    }

    private bool CheckInventorySpaceAvailable(ItemDataSO itemData, out InventoryItem inventoryItem)
    {
        inventoryItem = Instantiate(itemData.inventoryObject, _inventory.transform).GetComponent<InventoryItem>();
        inventoryItem.FindExtremes(out Vector2Int minSpaceDistance, out Vector2Int maxSpaceDistance);
        if (_inventory == null)
        {
            return false;
        }
        for (int x = 0; x < _inventory.gridSizeWidth; x++)
        {
            if (x + minSpaceDistance.x < 0  ||
                x + maxSpaceDistance.x > _inventory.gridSizeWidth)
            {
                continue;
            }
            for (int y = 0; y < _inventory.gridSizeHeight; y++)
            {
                if (y + minSpaceDistance.y < 0 ||
                    y + maxSpaceDistance.y > _inventory.gridSizeHeight)
                {
                    continue;
                }
                if (_inventory.CheckIfSlotsAvailable(new Vector2Int(x, y), inventoryItem.tilesUsed.ToArray()))
                {
                    _inventory.PlaceItem(inventoryItem, new Vector2Int(x, y));
                    RectTransform rectTransform = inventoryItem.GetComponent<RectTransform>();
                    rectTransform.SetParent(_inventory.GetComponent<RectTransform>());
                    Vector2 position = new Vector2();
                    position.x = (float)x * InventoryGrid.globalItemData.tileWidth + InventoryGrid.globalItemData.tileWidth / 2f;
                    position.y = -((float)y * InventoryGrid.globalItemData.tileHeight + InventoryGrid.globalItemData.tileHeight / 2f);

                    rectTransform.localPosition = position;
                    inventoryItem.InitializeInventoryItem(itemData);
                    return true;
                }
            }
        }

        return false;
    }

    private void ScrapItem()
    {

    }

    private void DropItemIntoWorld()
    {
        if (_itemToPlace == null)
        {
            return;
        }
        if (_itemToPlace.itemData == null)
        {
            return;
        }
        WorldItem worldItemWO = Instantiate(_itemToPlace.itemData.worldObject).GetComponent<WorldItem>();
        worldItemWO.transform.parent = FindObjectOfType<WorldItemsTag>().transform;
        Vector3 spawnPoint = GameManager.PlayerControllerInstance.transform.position + (GameManager.PlayerControllerInstance.transform.forward * 1.25f);
        worldItemWO.SpawnItem(spawnPoint, _itemToPlace.itemData);
        Destroy(_itemToPlace.gameObject);
        SwapItemInHand(null);
    }
}
