using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoldableToolSO : ToolSO
{
    public enum HandsUsed
    {
        Right,
        Left,
        Both
    }

    [Header("Holdable Settings")]
    public HandsUsed handsUsed;
    public GameObject toolPrefab;
}
