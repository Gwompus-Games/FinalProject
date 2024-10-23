using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OxygenDrainer : MonoBehaviour
{
    [field:SerializeField] public float drainMultiplier { get; private set; } = 1.0f;
    private OxygenSystem _oxygenSystem;


    public virtual void SetDrainMultiplier(float newMultiplier)
    {
        drainMultiplier = newMultiplier;
    }

    public virtual void ActivateDrainer()
    {
        if (_oxygenSystem == null)
        {
            _oxygenSystem = GameManager.Instance.GetManagedComponent<OxygenSystem>();
            //Debug.Log($"{_oxygenSystem} gotten for {gameObject.name}'s oxygen drainer!");
            if (_oxygenSystem == null)
            {
                throw new System.Exception("No Oxygen System assigned in Game Manager");
            }
        }
        if (!_oxygenSystem.DrainingSourceActive(this))
        {
            _oxygenSystem.AddDrainingSource(this);
        }
    }

    public virtual void DeactivateDrainer()
    {
        if (_oxygenSystem == null)
        {
            _oxygenSystem = GameManager.Instance.GetManagedComponent<OxygenSystem>();
            //Debug.Log($"{_oxygenSystem} gotten for {gameObject.name}!");
            if (_oxygenSystem == null)
            {
                throw new System.Exception("No Oxygen System assigned in Game Manager");
            }
        }
        if (_oxygenSystem.DrainingSourceActive(this))
        {
            _oxygenSystem.RemoveDrainingSource(this);
        }
    }
}
