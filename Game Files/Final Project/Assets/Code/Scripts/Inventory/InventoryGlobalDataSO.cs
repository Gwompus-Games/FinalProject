using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GlobalInventoryData", menuName = "GlobalInventoryData")]
public class InventoryGlobalDataSO : ScriptableObject
{
    public Color normalTileColour;
    public Color[] invalidTileColours;
    public Color selectedToolTileColour;
    public int numberOfInvalidFlashes = 5;
    public float flashingDuration = 3f;
    public float tileWidth = 50f;
    public float tileHeight = 50f;
    public float tileAlpha = 0.25f;
    public float flashingAlpha = 0.5f;
    public Vector2Int referenceResolution;
}
