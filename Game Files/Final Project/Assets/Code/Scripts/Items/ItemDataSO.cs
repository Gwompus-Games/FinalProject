using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDataSO : ScriptableObject
{
    public string itemName = "Item Name";
    public GameObject prefab;
    public Sprite inventoryIcon;
    public int baseValue = 0;
}
