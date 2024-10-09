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
        if (!OxygenSystem.Instance.DrainingSourceActive(this))
        {
            OxygenSystem.Instance.AddDrainingSource(this);
        }
    }

    public virtual void DeactivateDrainer()
    {
        if (OxygenSystem.Instance.DrainingSourceActive(this))
        {
            OxygenSystem.Instance.RemoveDrainingSource(this);
        }
    }
}
