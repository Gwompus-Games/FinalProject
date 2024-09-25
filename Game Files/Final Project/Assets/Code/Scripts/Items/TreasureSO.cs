using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Treasure", menuName = "Items/Treasure Item")]
public class TreasureSO : ItemDataSO
{
    public enum TreasureRarity
    {
        COMMON,
        UNCOMMON,
        RARE,
        UNIQUE
    }
    
    public TreasureRarity rarity;
}
