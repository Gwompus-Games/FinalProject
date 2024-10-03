using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform))]
public class InventoryItem : MonoBehaviour
{
    public List<Vector2Int> tilesUsed { get; private set; } = new List<Vector2Int>();
    private List<Image> tileBackgroundImages = new List<Image>();
    [HideInInspector]
    public Vector2Int originTile = new Vector2Int(-1, -1);
    private RectTransform _myRectTransform;
    private Coroutine _flashingRoutune = null;
    public ItemDataSO itemData { get; private set; }
    private bool _initialized = false;

    private void Awake()
    {
        _myRectTransform = GetComponent<RectTransform>();
    }

    private void Start()
    {
        InitializeGridSpaces();
    }

    public void FindExtremes(out Vector2Int minSpaceDistance, out Vector2Int maxSpaceDistance)
    {
        InitializeGridSpaces();
        minSpaceDistance = new Vector2Int(int.MaxValue, int.MaxValue);
        maxSpaceDistance = new Vector2Int(int.MinValue, int.MinValue);
        for (int t = 0; t < tilesUsed.Count; t++)
        {
            minSpaceDistance.x = (int)MathF.Min(minSpaceDistance.x, tilesUsed[t].x);
            maxSpaceDistance.x = (int)MathF.Max(maxSpaceDistance.x, tilesUsed[t].x);
            minSpaceDistance.y = (int)MathF.Min(minSpaceDistance.y, tilesUsed[t].y);
            maxSpaceDistance.y = (int)MathF.Max(maxSpaceDistance.y, tilesUsed[t].y);
        }
    }

    private void InitializeGridSpaces()
    {
        if (_initialized)
        {
            return;
        }
        _initialized = true;
        InventoryTileComponent[] inventoryTiles = GetComponentsInChildren<InventoryTileComponent>();
        for (int tile = 0; tile < inventoryTiles.Length; tile++)
        {
            inventoryTiles[tile].InitializeTileComponent();
            if (tilesUsed.Contains(inventoryTiles[tile].gridPosition))
            {
                Destroy(inventoryTiles[tile].gameObject);
                continue;
            }

            tilesUsed.Add(inventoryTiles[tile].gridPosition);
            tileBackgroundImages.Add(inventoryTiles[tile].GetComponent<Image>());
        }
        ChangeTileColours(InventoryGrid.globalItemData.normalTileColour, InventoryGrid.globalItemData.tileAlpha);
    }

    public void InitializeInventoryItem(ItemDataSO data)
    {
        itemData = data;
    }

    public virtual void ItemPlacedInInventory()
    {
        Debug.Log(gameObject.name + "Added to inventory!");
    }

    public virtual void ItemRemovedFromInventory()
    {

    }

    public void RotateClockwise()
    {
        for (int tile = 0; tile < tilesUsed.Count; tile++)
        {
            Vector2Int tempTile = new Vector2Int(-tilesUsed[tile].y, tilesUsed[tile].x);
            tilesUsed[tile] = tempTile;
        }
        _myRectTransform.Rotate(new Vector3(0f, 0f, -90f));
    }

    public void RotateCounterClockwise()
    {
        for (int tile = 0; tile < tilesUsed.Count; tile++)
        {
            Vector2Int tempTile = new Vector2Int(tilesUsed[tile].y, -tilesUsed[tile].x);
            tilesUsed[tile] = tempTile;
        }
        _myRectTransform.Rotate(new Vector3(0f, 0f, 90f));
    }

    public void InvalidPlacementFlash()
    {
        if (_flashingRoutune != null)
        {
            StopCoroutine( _flashingRoutune );
        }
        _flashingRoutune = StartCoroutine(InvalidPlacementFlashing());
    }

    public void StopInvalidPlacementFlashing()
    {
        if (_flashingRoutune != null)
        {
            StopCoroutine(_flashingRoutune);
        }
        ChangeTileColours(InventoryGrid.globalItemData.normalTileColour, InventoryGrid.globalItemData.tileAlpha);
    }

    private void ChangeTileColours(Color colourToChangeTo, float alpha)
    {
        colourToChangeTo.a = alpha;
        for (int tile = 0; tile < tileBackgroundImages.Count; tile++)
        {
            tileBackgroundImages[tile].color = colourToChangeTo;
        }
    }

    private IEnumerator InvalidPlacementFlashing()
    {
        ChangeTileColours(InventoryGrid.globalItemData.invalidTileColours[0], InventoryGrid.globalItemData.flashingAlpha);
        int colour = 0;
        int numberOfFlashes = 0;
        yield return null;
        while (numberOfFlashes <= InventoryGrid.globalItemData.numberOfInvalidFlashes)
        {
            colour++;
            if (colour >= InventoryGrid.globalItemData.invalidTileColours.Length)
            {
                colour = 0;
            }
            ChangeTileColours(InventoryGrid.globalItemData.invalidTileColours[colour], InventoryGrid.globalItemData.flashingAlpha);
            yield return new WaitForSeconds(InventoryGrid.globalItemData.flashingDuration / InventoryGrid.globalItemData.numberOfInvalidFlashes);
            numberOfFlashes++;
        }
        ChangeTileColours(InventoryGrid.globalItemData.normalTileColour, InventoryGrid.globalItemData.tileAlpha);
    }
}
