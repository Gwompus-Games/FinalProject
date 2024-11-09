using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using Unity.VisualScripting;
using UnityEngine;

public class InventoryController : ManagedByGameManager
{
    [HideInInspector]
    public InventoryGrid selectedItemGrid;
    [HideInInspector]
    public SellingZone selectedSellingZone;
    private InventoryItem _itemToPlace;
    [SerializeField] private InventoryGrid _inventory;

    [SerializeField] GameObject testInventoryItemPrefab;
    [SerializeField] GameObject testInventoryItemPrefab2;

    [Header("Debugging")]
    [SerializeField] private bool _debugMode = false;

    private Vector3 _mousePosition = Vector3.zero;
    private List<ManagedObject> _managedObjects = new List<ManagedObject>();
    public InventoryUI inventoryUI { get; private set; }

    public override void Init()
    {
        base.Init();
        if (_debugMode)
        {
            Debug.Log("Init inventory controller");
        }
        InventoryGrid inventoryGrid = FindObjectOfType<InventoryTag>().GetComponent<InventoryGrid>();
        if (inventoryGrid != null)
        {
            _inventory = inventoryGrid;
        }

        inventoryUI = FindFirstObjectByType<InventoryUI>();
        ManagedObject[] inventoryGrids = FindObjectsByType<InventoryGrid>(FindObjectsSortMode.None);
        
        _managedObjects.Add(inventoryUI);
        _managedObjects.AddRange(inventoryGrids);

        if (_debugMode)
        {
            Debug.Log($"Number of managed objects by Inventory Controller: {_managedObjects.Count}");
        }
        for (int m = 0; m < _managedObjects.Count; m++)
        {
            if (_managedObjects[m] == this)
            {
                continue;
            }
            if (_debugMode)
            {
                Debug.Log($"Initializing: {_managedObjects[m].name}");
            }
            _managedObjects[m].Init();
        }
    }

    public override void CustomStart()
    {
        base.CustomStart();
        OnEnable();
        for (int m = 0; m < _managedObjects.Count; m++)
        {
            if (_managedObjects[m] == this)
            {
                continue;
            }
            if (_debugMode)
            {
                Debug.Log($"Starting: {_managedObjects[m].name}");
            }
            _managedObjects[m].CustomStart();
        }
    }

    private void OnEnable()
    {
        if (!_initilized)
        {
            return;
        }
        CustomPlayerInput.UpdateCursorPosition += UpdateMousePos;
        CustomPlayerInput.Rotate += RotateItem;
        CustomPlayerInput.LeftMouseButton += PlaceInput;
        CustomPlayerInput.RightMouseButton += DropItemInput;
    }

    private void OnDisable()
    {
        if (!_initilized)
        {
            return;
        }
        CustomPlayerInput.UpdateCursorPosition -= UpdateMousePos;
        CustomPlayerInput.Rotate -= RotateItem;
        CustomPlayerInput.LeftMouseButton -= PlaceInput;
        CustomPlayerInput.RightMouseButton -= DropItemInput;
    }

    private void Update()
    {
#if UNITY_EDITOR
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
#endif
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

    public InventoryItem AddItemToInventory(ItemDataSO itemData)
    {
        bool startedOpen = (GameManager.Instance.GetManagedComponent<PlayerController>().currentState == PlayerController.PlayerState.Inventory);
        if (!startedOpen)
        {
            GameManager.Instance.GetManagedComponent<PlayerController>().OpenInventory();
        }
        InventoryItem inventoryItem;
        if (CheckInventorySpaceAvailable(itemData, out inventoryItem))
        {
            if (startedOpen == false)
            {
                GameManager.Instance.GetManagedComponent<PlayerController>().CloseInventory();
            }
            return inventoryItem;
        }
        _itemToPlace = inventoryItem;
        _itemToPlace.GetComponent<RectTransform>().SetParent(FindFirstObjectByType<InventoryCanvasTag>().GetComponent<RectTransform>());
        _itemToPlace.InitializeInventoryItem(itemData);
        return _itemToPlace;
    }

    public II_OxygenTank AddItemToInventory(OxygenTankSO tankData, float oxygen)
    {
        II_OxygenTank oxygenTank = AddItemToInventory(tankData as ItemDataSO) as II_OxygenTank;
        if (oxygenTank == null)
        {
            Debug.Log($"Added null oxygen tank to inventory!");
            return null;
        }
        oxygenTank.SetTankOxygenLevel(oxygen);
        return oxygenTank;
    }

    public InventoryItem AddItemToInventory(ItemDataSO itemData, Vector2Int gridPosition)
    {
        InventoryItem inventoryItem = Instantiate(itemData.inventoryObject).GetComponent<InventoryItem>();
        if (!_inventory.CheckIfSlotsAvailable(gridPosition, inventoryItem.tilesUsed.ToArray()))
        {
            Debug.LogWarning("Tried to add to a specific slot that isn't free!");
            return AddItemToInventory(itemData);
        }

        return PlaceItem(inventoryItem, gridPosition);
    }

    private bool CheckInventorySpaceAvailable(ItemDataSO itemData, out InventoryItem inventoryItem)
    {
        GameObject inventoryObject = Instantiate(itemData.inventoryObject, _inventory.transform);
        Vector2Int minSpaceDistance, maxSpaceDistance;
        if (inventoryObject.TryGetComponent<II_OxygenTank>(out II_OxygenTank oxygenTankItem))
        {
            oxygenTankItem.InitializeTank(oxygenTankItem.oxygenTankData);
            inventoryItem = oxygenTankItem;
            inventoryItem.FindExtremes(out minSpaceDistance, out maxSpaceDistance);
        }
        else
        {
            inventoryItem = inventoryObject.GetComponent<InventoryItem>();
            inventoryItem.FindExtremes(out minSpaceDistance, out maxSpaceDistance);
        }
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
                Vector2Int coordinate = new Vector2Int(x, y);
                if (y + minSpaceDistance.y < 0 ||
                    y + maxSpaceDistance.y > _inventory.gridSizeHeight)
                {
                    continue;
                }
                if (_inventory.CheckIfSlotsAvailable(coordinate, inventoryItem.tilesUsed.ToArray()))
                {
                    PlaceItem(inventoryItem, coordinate);
                    return true;
                }
            }
        }

        return false;
    }

    private InventoryItem PlaceItem(InventoryItem item, Vector2Int placementPosition)
    {
        _inventory.PlaceItem(item, placementPosition);
        RectTransform rectTransform = item.GetComponent<RectTransform>();
        rectTransform.SetParent(_inventory.GetComponent<RectTransform>());
        Vector2 position = new Vector2();
        position.x = (float)placementPosition.x * InventoryGrid.globalItemData.tileWidth + InventoryGrid.globalItemData.tileWidth / 2f;
        position.y = -((float)placementPosition.y * InventoryGrid.globalItemData.tileHeight + InventoryGrid.globalItemData.tileHeight / 2f);

        rectTransform.localPosition = position;
        item.InitializeInventoryItem(item.itemData);
        return item;
    } 

    public void RemoveItemFromInventory(InventoryItem itemToRemove)
    {
        _inventory.RemoveItem(itemToRemove);
    }

    public void RemoveItemFromInventory(Vector2Int gridPosToRemove)
    {
        _inventory.RemoveItem(gridPosToRemove);
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
        II_OxygenTank oxygenTank = _itemToPlace as II_OxygenTank;
        if (oxygenTank != null)
        {
            DropOxygenTank(oxygenTank);
            return;
        }
        WorldItem worldItemWO = Instantiate(_itemToPlace.itemData.worldObject).GetComponent<WorldItem>();
        worldItemWO.transform.parent = FindObjectOfType<WorldItemsTag>().transform;
        Vector3 spawnPoint = GameManager.Instance.GetManagedComponent<PlayerController>().transform.position + (GameManager.Instance.GetManagedComponent<PlayerController>().transform.forward * 1.25f);
        worldItemWO.SpawnItem(spawnPoint, _itemToPlace.itemData);
        Destroy(_itemToPlace.gameObject);
        SwapItemInHand(null);
    }

    private void DropOxygenTank(II_OxygenTank oxygenTankToPlace)
    {
        Debug.Log($"Dropping OxygenTank: {oxygenTankToPlace}");
        WI_OxygenTank wI_OxygenTank = Instantiate(_itemToPlace.itemData.worldObject).GetComponent<WI_OxygenTank>();
        wI_OxygenTank.transform.parent = FindObjectOfType<WorldItemsTag>().transform;
        Vector3 spawnPoint = GameManager.Instance.GetManagedComponent<PlayerController>().transform.position + (GameManager.Instance.GetManagedComponent<PlayerController>().transform.forward * 1.25f);
        wI_OxygenTank.SpawnItem(spawnPoint, oxygenTankToPlace.oxygenLeft, oxygenTankToPlace.oxygenTankData);
        Destroy(_itemToPlace.gameObject);
        SwapItemInHand(null);
    }

    internal void DisablePopup()
    {
        if (selectedItemGrid == null)
        {
            return;
        }
        InventoryPopupUI popup = null;
        InventoryUI ui = selectedItemGrid.GetComponentInParent<InventoryUI>();
        if (ui != null)
        {
            popup = ui.popupUI;
        }
        if (popup == null)
        {
            ToolBarUI tbUI = selectedItemGrid.GetComponentInParent<ToolBarUI>();
            if (tbUI != null)
            {
                popup = tbUI.popupUI;
            }
        }

        if (popup == null)
        {
            throw new Exception($"No popup UI found!");
        }

        popup.DisablePopup();
    }
}
