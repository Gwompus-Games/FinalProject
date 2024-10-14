using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDataSO : ScriptableObject
{
    [Header("General Item Data")]
    public string itemName = "Item Name";
    [TextArea]
    public string itemDescription = "";
    public bool usedForSelling;
    [Header("World Item Data")]
    public GameObject worldObject;
    public float density = 1f;
    [Range(2.5f, 100f)] public float drag = 15f;
    public float angularDrag = 0.15f;
    [Header("Inventory Item Data")]
    public GameObject inventoryObject;
    public Sprite inventoryItemSprite;
    [Header("Selling Data")]
    public int baseValue = 0;
}
