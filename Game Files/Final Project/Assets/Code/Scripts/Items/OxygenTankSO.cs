using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Items/Oxygen Tank")]
public class OxygenTankSO : ToolSO
{
    [Header("Oxygen Tank Data")]
    public float minutesUntilEmpty = 5f;
}
