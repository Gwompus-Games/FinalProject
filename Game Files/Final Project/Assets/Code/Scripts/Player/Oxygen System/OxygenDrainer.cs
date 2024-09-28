using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OxygenDrainer : MonoBehaviour
{
    [field:SerializeField] public float drainMultiplier { get; private set; } = 1.0f;

    public void SetDrainMultiplier(float newMultiplier)
    {
        drainMultiplier = newMultiplier;
    }
}
