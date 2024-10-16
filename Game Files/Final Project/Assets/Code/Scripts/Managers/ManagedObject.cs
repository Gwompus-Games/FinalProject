using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ManagedObject : MonoBehaviour
{
    protected bool _initilized;
    public virtual void Init()
    {
        if (_initilized)
        {
            return;
        }
        _initilized = true;
    }
    public virtual void CustomStart()
    {

    }
}
