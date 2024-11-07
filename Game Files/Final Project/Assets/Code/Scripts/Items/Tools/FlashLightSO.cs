using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Items/Tools/Flashlight Tool", fileName = "FlashlightToolData")]
public class FlashLightSO : HoldableToolSO
{
    [Header("Flashlight Settings")]
    public float lightRange;
}
