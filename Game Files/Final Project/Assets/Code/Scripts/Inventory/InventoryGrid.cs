using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryGrid : ManagedObject
{
    [SerializeField] private InventoryGlobalDataSO _globalData;
    public static InventoryGlobalDataSO globalItemData;
    private InventoryController _inventoryController;
    protected InventoryItem _hoveredItem;
    protected Vector2 _cursorPos = Vector2.zero;

    public InventoryItem[,] inventoryItemSlot { get; private set; }

    RectTransform rectTransform;

    Vector2 positionOnTheGrid = new Vector2();
    Vector2Int tileGridPosition = new Vector2Int();

    [field: SerializeField] public int gridSizeWidth { get; private set; }
    [field: SerializeField] public int gridSizeHeight { get; private set; }
    private float _xScaler;
    private float _yScaler;

    private bool _initialized = false;

    [SerializeField] private bool _debugMode = false;

    public override void Init()
    {
        base.Init();
        globalItemData = _globalData;
        rectTransform = GetComponent<RectTransform>();
        _xScaler = (float)Screen.width / (float)globalItemData.referenceResolution.x;
        _yScaler = (float)Screen.height / (float)globalItemData.referenceResolution.y;
        _inventoryController = FindObjectOfType<InventoryController>();
        InitGrid(gridSizeWidth, gridSizeHeight);
    }

    public override void CustomStart()
    {
        base.CustomStart();
        _inventoryController.inventoryUI.SyncBarSize(rectTransform.sizeDelta.x);
        OnEnable();
    }

    private void OnEnable()
    {
        if (!_initialized)
        {
            return;
        }
        CustomPlayerInput.UpdateCursorPosition += CursorPosition;
    }

    private void OnDisable()
    {
        if (!_initialized)
        {
            return;
        }
        CustomPlayerInput.UpdateCursorPosition -= CursorPosition;
    }

    private void InitGrid(int width, int height)
    {
        if (_initialized)
        {
            return;
        }
        _initialized = true;

        RectTransform parentRectTransform = null;

        if (!transform.parent.gameObject.TryGetComponent<RectTransform>(out parentRectTransform))
        {
            Debug.Log("No Parent Found");
        }

        inventoryItemSlot = new InventoryItem[width, height];
        for (int x = 0; x < inventoryItemSlot.GetLength(0); x++)
        {
            for (int y = 0; y < inventoryItemSlot.GetLength(1); y++)
            {
                inventoryItemSlot[x, y] = null;
            }
        }
        Vector2 size = new Vector2(width * globalItemData.tileWidth, height * globalItemData.tileHeight);
        rectTransform.sizeDelta = size;

        Vector2 position = Vector2.zero;
        if (parentRectTransform == null)
        {
            position.x = (Screen.width - rectTransform.sizeDelta.x) / 2;
            position.y = (Screen.height + rectTransform.sizeDelta.y) / 2;
        }
        else
        {
            position.x = parentRectTransform.position.x - (rectTransform.sizeDelta.x / 2);
            position.y = parentRectTransform.position.y + (parentRectTransform.sizeDelta.y - rectTransform.sizeDelta.y) / 2;
        }
        rectTransform.position = position;
    }

    private Vector2Int GetTileGridPosition(Vector2 worldPosition)
    {
        if (rectTransform == null)
        {
            return new Vector2Int(-1, -1);
        }
        positionOnTheGrid.x = worldPosition.x - rectTransform.position.x;
        positionOnTheGrid.y = rectTransform.position.y - worldPosition.y;

        tileGridPosition.x = (int)(positionOnTheGrid.x / (globalItemData.tileWidth * _xScaler));
        tileGridPosition.y = (int)(positionOnTheGrid.y / (globalItemData.tileHeight * _yScaler));

        //Debug.Log(tileGridPosition);

        return tileGridPosition;
    }
    
    public InventoryItem PickupItem(Vector2Int gridPosition)
    {
        if (_debugMode)
        {
            Debug.Log($"Trying to pickup from grid position: {gridPosition}");
        }
        if (CheckIfSlotAvailable(gridPosition))
        {
            if (_debugMode)
            {
                Debug.Log($"No item in slot {gridPosition}");
            }
            return null;
        }
        InventoryItem item = GetItemInSlot(gridPosition);
        if (_debugMode)
        {
            Debug.Log($"Picking up: {item.name}");
        }
        item.ItemRemovedFromInventory();
        ClearItemInSlot(gridPosition);
        return item;
    }

    public InventoryItem PickupItem(Vector2 placementPoint)
    {
        Vector2Int gridPosition = GetTileGridPosition(placementPoint);
        return PickupItem(gridPosition);
    }


    public virtual bool PlaceItem(InventoryItem inventoryItem, Vector2Int gridPosition, out InventoryItem returnItem)
    {
        returnItem = null;

        if (gridPosition.x >= gridSizeWidth || gridPosition.y >= gridSizeHeight)
        {
            if (_debugMode)
            {
                Debug.LogWarning("Cannot place on a space bigger than the grid size!");
            }
            return false;
        }
        if (gridPosition.x < 0 || gridPosition.y < 0)
        {
            if (_debugMode)
            {
                Debug.LogWarning("Cannot place on a negative space.");
            }
            return false;
        }
        if (!CheckIfSlotsAvailable(gridPosition, inventoryItem.tilesUsed.ToArray()))
        {
            List<InventoryItem> itemsInSlots = GetItemsInSlots(gridPosition, inventoryItem.tilesUsed.ToArray());
            if (itemsInSlots == null)
            {
                if (_debugMode)
                {
                    Debug.LogWarning("Tried to place in a bad spot.");
                }
                return false;
            }
            if (itemsInSlots.Count > 1)
            {
                if (_debugMode)
                {
                    Debug.LogWarning("Too many items blocking spaces");
                }
                return false;
            }
            if (itemsInSlots.Count <= 0)
            {
                throw new Exception("Somehow we got an amount of items to be less than 1.");
            }
            returnItem = itemsInSlots[0];
        }
        if (returnItem != null)
        {
            returnItem.ItemRemovedFromInventory();
            ClearSlotsWithItem(returnItem);
        }

        inventoryItem.StopInvalidPlacementFlashing();
        RectTransform rectTransform = inventoryItem.GetComponent<RectTransform>();
        rectTransform.SetParent(this.rectTransform);
        SetItemSlots(gridPosition, inventoryItem.tilesUsed.ToArray(), inventoryItem);
        inventoryItem.originTile = gridPosition;
        inventoryItem.ItemPlacedInInventory();

        Vector2 position = new Vector2();
        position.x = (float)gridPosition.x * globalItemData.tileWidth + globalItemData.tileWidth / 2f;
        position.y = -((float)gridPosition.y * globalItemData.tileHeight + globalItemData.tileHeight / 2f);

        rectTransform.localPosition = position;

        if (_debugMode)
        {
            Debug.Log($"Placed {inventoryItem.name} in slot {gridPosition}");
        }

        return true;
    }

    public bool PlaceItem(InventoryItem inventoryItem, Vector2 worldPosition, out InventoryItem returnItem)
    {
        Vector2Int gridPosition = GetTileGridPosition(worldPosition);
        return PlaceItem(inventoryItem, gridPosition, out returnItem);
    }

    public bool PlaceItem(InventoryItem inventoryItem, Vector2Int gridPosition)
    {
        InventoryItem returnItem;
        return PlaceItem(inventoryItem, gridPosition, out returnItem);
    }

    public bool PlaceItem(InventoryItem inventoryItem, Vector2 worldPosition)
    {
        Vector2Int gridPosition = GetTileGridPosition(worldPosition);
        return PlaceItem(inventoryItem, gridPosition);
    }
    
    private bool CheckIfSlotAvailable(Vector2Int gridTile)
    {
        InitGrid(gridSizeWidth, gridSizeHeight);
        if (_debugMode)
        {
            Debug.Log($"Checking slot: {gridTile}");
        }
        if (gridTile.x < 0 || gridTile.y < 0)
        {
            if (_debugMode)
            {
                Debug.Log($"One or both grid tile positions are negative!");
            }
            return false;
        }
        if (gridTile.x >= gridSizeWidth || gridTile.y >= gridSizeHeight)
        {
            if (_debugMode)
            {
                Debug.Log("One or both grid tile positions are outside the size of the grid!");
            }
            return false;
        }
        if (_debugMode)
        {
            Debug.Log($"Item in slot: {inventoryItemSlot[gridTile.x, gridTile.y]}");
        }
        if (inventoryItemSlot[gridTile.x, gridTile.y] != null)
        {
            if (_debugMode)
            {
                Debug.Log($"Item found at: {gridTile}");
            }
            return false;
        }

        if (_debugMode)
        {
            Debug.Log($"No item found!");
        }
        
        return true;
    }
    
    public bool CheckIfSlotsAvailable(Vector2Int origin, Vector2Int[] tileCoordinates)
    {
        for (int t = 0; t < tileCoordinates.Length; t++)
        {
            Vector2Int gridTile = origin + tileCoordinates[t];
            if (!CheckIfSlotAvailable(gridTile))
            {
                return false;
            }
        }

        return true;
    }

    private InventoryItem GetItemInSlot(Vector2Int gridPosition)
    {
        if (gridPosition.x < 0 || gridPosition.y < 0)
        {
            return null;
        }
        if (gridPosition.x >= gridSizeWidth || gridPosition.y >= gridSizeHeight)
        {
            return null;
        }
        return inventoryItemSlot[gridPosition.x, gridPosition.y];
    }

    private InventoryItem GetItemInSlot(Vector2 worldPoint)
    {
        Vector2Int gridPosition = GetTileGridPosition(worldPoint);
        return GetItemInSlot(gridPosition);
    }

    private List<InventoryItem> GetItemsInSlots(Vector2Int origin, Vector2Int[] tileCoordinates)
    {
        List<InventoryItem> items = new List<InventoryItem>();
        for (int t = 0; t < tileCoordinates.Length; t++)
        {
            Vector2Int gridTile = origin + tileCoordinates[t];
            if (gridTile.x < 0 || gridTile.y < 0)
            {
                if (_debugMode)
                {
                    Debug.LogWarning("Tried to access a negative slot.");
                }
                return null;
            }
            if (gridTile.x >= gridSizeWidth || gridTile.y >= gridSizeHeight)
            {
                if (_debugMode)
                {
                    Debug.LogWarning("Tried to access a slot greater than the grid size.");
                }
                return null;
            }
            if (inventoryItemSlot[gridTile.x, gridTile.y] != null && !items.Contains(inventoryItemSlot[gridTile.x, gridTile.y]))
            {
                items.Add(inventoryItemSlot[gridTile.x, gridTile.y]);
            }
        }

        return items;
    }

    private void SetItemSlots(Vector2Int origin, Vector2Int[] tileCoordinates, InventoryItem item)
    {
        if (_debugMode)
        {
            Debug.Log($"Trying to place item: {item.name} in origin slot {origin}");
        }
        for (int t = 0; t < tileCoordinates.Length; t++)
        {
            Vector2Int gridTile = origin + tileCoordinates[t];
            if (gridTile.x < 0 || gridTile.y < 0)
            {
                throw new Exception("Tried to set a slot that is less than the inventory grid size.");
            }
            if (gridTile.x >= gridSizeWidth || gridTile.y >= gridSizeHeight)
            {
                throw new Exception("Tried to set a slot that is greater than the inventory grid size.");
            }
        }
        for (int t = 0; t < tileCoordinates.Length; t++)
        {
            Vector2Int gridTile = origin + tileCoordinates[t];
            inventoryItemSlot[gridTile.x, gridTile.y] = item;
            if (_debugMode)
            {
                Debug.Log($"Setting {gridTile} to equal {item.name}");
            }
        }
    }

    private void ClearItemSlots(Vector2Int origin, Vector2Int[] tileCoordinates)
    {
        InventoryItem item = GetItemInSlot(origin);
        if (item == null)
        {
            if (_debugMode)
            {
                Debug.LogWarning("No item to clear.");
            }
            return;
        }
        for (int t = 0; t < tileCoordinates.Length; t++)
        {
            Vector2Int gridTile = origin + tileCoordinates[t];
            if (gridTile.x < 0 || gridTile.y < 0)
            {
                throw new Exception("Tried to set a slot that is less than the inventory grid size.");
            }
            if (gridTile.x >= gridSizeWidth || gridTile.y >= gridSizeHeight)
            {
                throw new Exception("Tried to set a slot that is greater than the inventory grid size.");
            }
        }
        for (int t = 0; t < tileCoordinates.Length; t++)
        {
            Vector2Int gridTile = origin + tileCoordinates[t];
            inventoryItemSlot[gridTile.x, gridTile.y] = null;
        }
        item.originTile = new Vector2Int(-1, -1);

        if (_debugMode)
        {
            Debug.Log($"Clearing {item.name} from inventory");
        }
    }

    private void ClearItemInSlot(Vector2Int gridPosition)
    {
        InventoryItem item = GetItemInSlot(gridPosition);
        if (item == null)
        {
            if (_debugMode)
            {
                Debug.LogWarning("No item in slot that is attempting to be cleared");
            }
        }
        ClearItemSlots(item.originTile, item.tilesUsed.ToArray());
    }

    internal void ClearHoveredItem()
    {
        _hoveredItem = null;
    }

    private void ClearSlotsWithItem(InventoryItem item)
    {
        if (item == null)
        {
            return;
        }
        if (GetItemInSlot(item.originTile) != item)
        {
            return;
        }
        ClearItemSlots(item.originTile, item.tilesUsed.ToArray());
    }

    public InventoryItem[] GetAllInventoryItems()
    {
        List<InventoryItem> items = new List<InventoryItem>();
        for (int x = 0; x < inventoryItemSlot.GetLength(0); x++)
        {
            for (int y = 0; y < inventoryItemSlot.GetLength(1); y++)
            {
                InventoryItem item = GetItemInSlot(new Vector2Int(x, y));
                if (!items.Contains(item))
                {
                    items.Add(item);
                }
            }
        }
        return items.ToArray();
    }

    public InventoryItem[] GetSellingItems()
    {
        if (inventoryItemSlot.Length <= 0)
            return new InventoryItem[0];

        List<InventoryItem> items = new List<InventoryItem>();
        for (int x = 0; x < inventoryItemSlot.GetLength(0); x++)
        {
            for (int y = 0; y < inventoryItemSlot.GetLength(1); y++)
            {
                InventoryItem item = GetItemInSlot(new Vector2Int(x, y));
                if (item == null)
                {
                    continue;
                }
                if (!items.Contains(item) && item.itemData.usedForSelling)
                {
                    items.Add(item);
                }
            }
        }
        return items.ToArray();
    }

    public void CursorPosition(Vector2 position)
    {
        _cursorPos = position;
        if (_inventoryController.selectedItemGrid != this)
        {
            return;
        }
        Vector2Int gridPos = GetTileGridPosition(position);
        if (gridPos.x < 0 || gridPos.y < 0 ||
            gridPos.x >= inventoryItemSlot.GetLength(0) ||
            gridPos.y >= inventoryItemSlot.GetLength(1))
        {
            return;
        }
        InventoryItem newItem = GetItemInSlot(gridPos);
        if (_hoveredItem != newItem)
        {
            _hoveredItem = newItem;
            StartCoroutine(Hover(_hoveredItem));
        }
    }

    protected virtual IEnumerator Hover(InventoryItem item)
    {
        InventoryPopupUI popupUI = GetComponentInParent<InventoryUI>().popupUI;
        if (popupUI == null)
        {
            item = null;
        }
        yield return null;
        if (_hoveredItem == item && item != null)
        {
            yield return new WaitForSeconds(popupUI.popupHoverTimeInSeconds);
            if (_hoveredItem == item)
            {
                popupUI.EnablePopup(_cursorPos);
                popupUI.SetPopupData(item);
                II_OxygenTank oxygenItem = item as II_OxygenTank;
                do
                {
                    if (oxygenItem != null)
                    {
                        popupUI.UpdatePopup(_cursorPos, oxygenItem);
                    }
                    else
                    {
                        popupUI.UpdatePopup(_cursorPos);
                    }
                    yield return null;
                } while (_hoveredItem == item);
            }
        }
        popupUI.DisablePopup();
    }

    public void RemoveItem(InventoryItem item, bool calledFromItemDestroy = false)
    {
        ClearSlotsWithItem(item);
        item.ItemRemovedFromInventory();
        if (!calledFromItemDestroy)
        {
            Destroy(item.gameObject);
        }
    }

    public void RemoveItem(Vector2Int gridPosition)
    {
        RemoveItem(GetItemInSlot(gridPosition));
    }
}
