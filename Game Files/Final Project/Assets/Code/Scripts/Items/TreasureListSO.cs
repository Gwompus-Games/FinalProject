using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Treasure Master List", menuName = "Treasure Master List")]
public class TreasureListSO : ScriptableObject
{
    public TreasureSO[] treasures;
}
