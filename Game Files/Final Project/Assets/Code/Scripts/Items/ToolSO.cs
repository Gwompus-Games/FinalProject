using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Items/Tools/GenericTool")]
public class ToolSO : ItemDataSO
{
    [Header("Tool Data")]
    public Sprite shopIcon;
    public int buyValue = 0;
}
