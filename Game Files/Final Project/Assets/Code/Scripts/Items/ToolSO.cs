using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Items/Tool")]
public class ToolSO : ItemDataSO
{
    [Header("Tool Data")]
    public Sprite shopIcon;
    public int buyValue = 0;
}
