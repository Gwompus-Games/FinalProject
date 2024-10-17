using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

public class StandaloneManagersList : MonoBehaviour
{
    public List<Type> standaloneManagers { get; private set; }
    public List<Type> managedChildren { get; private set; }
    public void SetUpList()
    {
        standaloneManagers = new List<Type> 
        { 
            typeof(AudioManager),
            typeof(FMODEvents)
        };

        managedChildren = Assembly.GetAssembly(typeof(ManagedByGameManager)).GetTypes().Where(t => t.IsSubclassOf(typeof(ManagedByGameManager))).ToList();
    }
}
