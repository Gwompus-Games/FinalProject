using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntryPoint : MonoBehaviour
{
    public bool isOccupied = false;
    public bool IsOccupied() => isOccupied;
    public void SetOccupied(bool value = true) => isOccupied = value;
}
