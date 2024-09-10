using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class InventoryItem : MonoBehaviour
{
    public List<Vector2Int> tilesUsed { get; private set; } = new List<Vector2Int>();
    public Vector2Int originTile = new Vector2Int(-1, -1);
    private RectTransform _myRectTransform;

    private void Awake()
    {
        _myRectTransform = GetComponent<RectTransform>();
        if (!tilesUsed.Contains(Vector2Int.zero))
        {
            tilesUsed.Add(Vector2Int.zero);
        }
    }

    private void Start()
    {
        InventoryTileComponent[] inventoryTiles = GetComponentsInChildren<InventoryTileComponent>();
        for (int tile = 0; tile < inventoryTiles.Length; tile++)
        {
            if (tilesUsed.Contains(inventoryTiles[tile].gridPosition))
            {
                continue;
            }

            tilesUsed.Add(inventoryTiles[tile].gridPosition);
        }
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
}
