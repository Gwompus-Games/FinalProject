using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IHeartbeat 
{
    public abstract void AddHeartbeat();

    public abstract void RemoveHeartbeat();
}
