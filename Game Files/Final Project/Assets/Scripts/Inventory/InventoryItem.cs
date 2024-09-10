using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryItem : MonoBehaviour
{
    public List<Vector2Int> tilesUsed { get; private set; } = new List<Vector2Int>();
    public Vector2Int originTile = new Vector2Int(-1, -1);

    private void Awake()
    {
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
}
