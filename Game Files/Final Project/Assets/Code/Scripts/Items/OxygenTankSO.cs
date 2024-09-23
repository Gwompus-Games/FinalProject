using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Items/Oxygen Tank")]
public class OxygenTankSO : ToolSO
{
    public float totalOxygen = 100;
    public float minutesUntilEmpty = 5f;
}
