using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform))]
[RequireComponent(typeof(Image))]
public class InventoryTileComponent : MonoBehaviour
{
    private RectTransform _myRectTransform;
    private Image _myTileImage;
    [HideInInspector]
    public Vector2Int gridPosition;
    [SerializeField] private InventoryGlobalDataSO globalData;

    private void Awake()
    {
        _myRectTransform = GetComponent<RectTransform>();
        _myTileImage = GetComponent<Image>();

        Vector2 size = new Vector2(globalData.tileWidth, globalData.tileHeight);
        _myRectTransform.sizeDelta = size;

        Vector2 snappedPos = _myRectTransform.localPosition;

        float remainderX = _myRectTransform.localPosition.x % size.x;
        float remainderY = _myRectTransform.localPosition.y % size.y;

        if (remainderX != 0)
        {
            if (remainderX > size.x / 2)
            {
                snappedPos.x += size.x - remainderX;
            }
            else
            {
                snappedPos.x -= remainderX;
            }
        }

        if (remainderY != 0)
        {
            if (remainderY > size.y / 2)
            {
                snappedPos.y += size.y - remainderY;
            }
            else
            {
                snappedPos.y -= remainderY;
            }
        }

        _myRectTransform.localPosition = snappedPos;
        gridPosition.x = (int)(snappedPos.x / size.x);
        gridPosition.y = -(int)(snappedPos.y / size.y);
    }
}
