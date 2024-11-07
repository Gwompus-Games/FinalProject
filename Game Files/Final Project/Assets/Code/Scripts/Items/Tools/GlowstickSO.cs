using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Items/Tools/Glowstick", fileName = "GlowstickToolData")]
public class GlowstickSO : HoldableToolSO
{
    [Header("Glowstick Settings")]
    public Color[] possibleColours;
}
