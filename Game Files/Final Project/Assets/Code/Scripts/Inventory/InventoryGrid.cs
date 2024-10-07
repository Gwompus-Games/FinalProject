using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryGrid : MonoBehaviour
{
    [SerializeField] private InventoryGlobalDataSO _globalData;
    public static InventoryGlobalDataSO globalItemData;

    public InventoryItem[,] inventoryItemSlot { get; private set; }

    RectTransform rectTransform;

    Vector2 positionOnTheGrid = new Vector2();
    Vector2Int tileGridPosition = new Vector2Int();

    [field: SerializeField] public int gridSizeWidth { get; private set; }
    [field: SerializeField] public int gridSizeHeight { get; private set; }
    private float _xScaler;
    private float _yScaler;

    private bool _initialized = false;

    private void Awake()
    {
        globalItemData = _globalData;
        rectTransform = GetComponent<RectTransform>();
        _xScaler = (float)Screen.width / (float)globalItemData.referenceResolution.x;
        _yScaler = (float)Screen.height / (float)globalItemData.referenceResolution.y;
        Init(gridSizeWidth, gridSizeHeight);
    }

    private void Start()
    {
        // where init used to be

    }

    private void Init(int width, int height)
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
        if (InventoryUI.Instance != null)
        {
            InventoryUI.Instance.SyncBarSize(rectTransform.sizeDelta.x);
        }
    }

    private Vector2Int GetTileGridPosition(Vector2 worldPosition)
    {
        positionOnTheGrid.x = worldPosition.x - rectTransform.position.x;
        positionOnTheGrid.y = rectTransform.position.y - worldPosition.y;

        tileGridPosition.x = (int)(positionOnTheGrid.x / (globalItemData.tileWidth * _xScaler));
        tileGridPosition.y = (int)(positionOnTheGrid.y / (globalItemData.tileHeight * _yScaler));

        //Debug.Log(tileGridPosition);

        return tileGridPosition;
    }
    
    public InventoryItem PickupItem(Vector2Int gridPosition)
    {
        if (CheckIfSlotAvailable(gridPosition))
        {
            return null;
        }
        InventoryItem item = GetItemInSlot(gridPosition);
        item.ItemRemovedFromInventory();
        ClearItemInSlot(gridPosition);
        return item;
    }

    public InventoryItem PickupItem(Vector2 placementPoint)
    {
        Vector2Int gridPosition = GetTileGridPosition(placementPoint);
        return PickupItem(gridPosition);
    }


    public bool PlaceItem(InventoryItem inventoryItem, Vector2Int gridPosition, out InventoryItem returnItem)
    {
        returnItem = null;

        if (gridPosition.x >= gridSizeWidth || gridPosition.y >= gridSizeHeight)
        {
            Debug.LogWarning("Cannot place on a space bigger than the grid size!");
            return false;
        }
        if (gridPosition.x < 0 || gridPosition.y < 0)
        {
            Debug.LogWarning("Cannot place on a negative space.");
            return false;
        }
        if (!CheckIfSlotsAvailable(gridPosition, inventoryItem.tilesUsed.ToArray()))
        {
            List<InventoryItem> itemsInSlots = GetItemsInSlots(gridPosition, inventoryItem.tilesUsed.ToArray());
            if (itemsInSlots == null)
            {
                Debug.LogWarning("Tried to place in a bad spot.");
                return false;
            }
            if (itemsInSlots.Count > 1)
            {
                Debug.LogWarning("Too many items blocking spaces");
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
        Init(gridSizeWidth, gridSizeHeight);
        if (gridTile.x < 0 || gridTile.y < 0)
        {
            return false;
        }
        if (gridTile.x >= gridSizeWidth || gridTile.y >= gridSizeHeight)
        {
            return false;
        }
        if (inventoryItemSlot[gridTile.x, gridTile.y] != null)
        {
            return false;
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
                Debug.LogWarning("Tried to access a negative slot.");
                return null;
            }
            if (gridTile.x >= gridSizeWidth || gridTile.y >= gridSizeHeight)
            {
                Debug.LogWarning("Tried to access a slot greater than the grid size.");
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
        }
    }

    private void ClearItemSlots(Vector2Int origin, Vector2Int[] tileCoordinates)
    {
        InventoryItem item = GetItemInSlot(origin);
        if (item == null)
        {
            Debug.LogError("No item to clear.");
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
    }

    private void ClearItemInSlot(Vector2Int gridPosition)
    {
        InventoryItem item = GetItemInSlot(gridPosition);
        if (item == null)
        {
            Debug.LogError("No item in slot that is attempting to be cleared");
        }
        ClearItemSlots(item.originTile, item.tilesUsed.ToArray());
    }

    private void ClearSlotsWithItem(InventoryItem item)
    {
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
}
