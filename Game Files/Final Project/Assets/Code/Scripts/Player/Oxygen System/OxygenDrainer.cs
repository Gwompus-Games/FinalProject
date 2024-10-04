using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OxygenDrainer : MonoBehaviour
{
    [field:SerializeField] public float drainMultiplier { get; private set; } = 1.0f;

    public virtual void SetDrainMultiplier(float newMultiplier)
    {
        drainMultiplier = newMultiplier;
    }

    public virtual void ActivateDrainer()
    {
        if (!OxygenSystem.INSTANCE.DrainingSourceActive(this))
        {
            OxygenSystem.INSTANCE.AddDrainingSource(this);
        }
    }

    public virtual void DeactivateDrainer()
    {
        if (OxygenSystem.INSTANCE.DrainingSourceActive(this))
        {
            OxygenSystem.INSTANCE.RemoveDrainingSource(this);
        }
    }
}
