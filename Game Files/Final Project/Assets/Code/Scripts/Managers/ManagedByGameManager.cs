using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class ManagedByGameManager : MonoBehaviour
{
    public abstract void Init();
    public abstract void CustomStart();
}
