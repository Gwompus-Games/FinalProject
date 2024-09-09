using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryGrid : MonoBehaviour
{
    const float tileSizeWidth = 50f;
    const float tileSizeHeight = 50f;

    InventoryItem[,] inventoryItemSlot;

    RectTransform rectTransform;

    Vector2 positionOnTheGrid = new Vector2();
    Vector2Int tileGridPosition = new Vector2Int();

    [SerializeField] int gridSizeWidth;
    [SerializeField] int gridSizeHeight;

    private void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        Init(gridSizeWidth, gridSizeHeight);
    }

    private void Init(int width, int height)
    {
        RectTransform parentRectTransform = null;

        if (!transform.parent.gameObject.TryGetComponent<RectTransform>(out parentRectTransform))
        {
            Debug.Log("No Parent Found");
        }

        inventoryItemSlot = new InventoryItem[width, height];
        Vector2 size = new Vector2(width * tileSizeWidth, height * tileSizeHeight);
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

    public Vector2Int GetTileGridPosition(Vector2 mousePosition)
    {
        positionOnTheGrid.x = mousePosition.x - rectTransform.position.x;
        positionOnTheGrid.y = rectTransform.position.y - mousePosition.y;

        tileGridPosition.x = (int)(positionOnTheGrid.x / tileSizeWidth);
        tileGridPosition.y = (int)(positionOnTheGrid.y / tileSizeHeight);

        return tileGridPosition;
    }
}
