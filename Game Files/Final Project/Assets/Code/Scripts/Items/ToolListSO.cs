using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Items/Tools Master List", fileName = "Tools Master List")]
public class ToolListSO : ScriptableObject
{
    public List<ToolSO> tools;
}
