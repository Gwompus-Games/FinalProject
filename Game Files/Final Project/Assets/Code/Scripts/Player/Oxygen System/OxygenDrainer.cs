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
        if (!GameManager.Instance.GetManagedComponent<OxygenSystem>().DrainingSourceActive(this))
        {
            GameManager.Instance.GetManagedComponent<OxygenSystem>().AddDrainingSource(this);
        }
    }

    public virtual void DeactivateDrainer()
    {
        if (GameManager.Instance.GetManagedComponent<OxygenSystem>().DrainingSourceActive(this))
        {
            GameManager.Instance.GetManagedComponent<OxygenSystem>().RemoveDrainingSource(this);
        }
    }
}
