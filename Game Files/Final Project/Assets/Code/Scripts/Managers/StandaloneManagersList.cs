using System;
using System.Collections.Generic;
using UnityEngine;

public class StandaloneManagersList : MonoBehaviour
{
    public List<Type> standaloneManagers { get; private set; }
    public void SetUpList()
    {
        standaloneManagers = new List<Type> 
        { 
            typeof(AudioManager),
            typeof(FMODEvents)
        };
    }
}
