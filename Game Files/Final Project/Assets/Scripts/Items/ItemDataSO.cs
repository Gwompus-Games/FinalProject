using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemData", menuName = "Items/ItemData")]
public class ItemDataSO : ScriptableObject
{
    public string itemName = "Item_Name";
    public Vector2Int minMaxItemValueBase;
    public Mesh model;
    public Sprite inventoryIcon;

}
