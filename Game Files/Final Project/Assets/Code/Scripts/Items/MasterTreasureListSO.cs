using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Master Treasure List", menuName = "Treasure List/Master Treasure List")]
public class MasterTreasureListSO : TreasureListSO
{
    [Header("Master Treasure List Settings")]
    public bool useWeightedSpawning = true;
}
