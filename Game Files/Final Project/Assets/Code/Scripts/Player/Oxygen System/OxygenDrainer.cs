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
        if (!GameManager.OxygenSystemInstance.DrainingSourceActive(this))
        {
            GameManager.OxygenSystemInstance.AddDrainingSource(this);
        }
    }

    public virtual void DeactivateDrainer()
    {
        if (GameManager.OxygenSystemInstance.DrainingSourceActive(this))
        {
            GameManager.OxygenSystemInstance.RemoveDrainingSource(this);
        }
    }
}
