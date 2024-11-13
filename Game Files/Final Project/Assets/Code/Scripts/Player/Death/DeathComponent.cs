using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class DeathComponent : MonoBehaviour
{
    public abstract void StartDeathComponent();
    public abstract void UpdateDeathComponent(float normalizedTime);
    public abstract void EndDeathComponent();
}
