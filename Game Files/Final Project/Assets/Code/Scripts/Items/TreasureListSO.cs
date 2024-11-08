using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Treasure List", menuName = "Treasure List/Treasure List")]
public class TreasureListSO : ScriptableObject
{
    [Header("Treasure List Items")]
    public TreasureSO[] treasures;
}
