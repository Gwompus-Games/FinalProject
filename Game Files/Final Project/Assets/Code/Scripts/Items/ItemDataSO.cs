using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDataSO : ScriptableObject
{
    public string itemName = "Item Name";
    public Mesh mesh;
    public Material meshMaterial;
    public GameObject worldObject;
    public GameObject inventoryObject;
    public int baseValue = 0;
}
