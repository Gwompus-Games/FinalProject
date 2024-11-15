using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

public class StandaloneManagersList : MonoBehaviour
{
    [SerializeField] private bool _debugMode = false;
    public List<Type> standaloneManagers { get; private set; }
    public List<Type> managedComponents { get; private set; }
    public void SetUpList()
    {
        if (_debugMode)
        {
            Debug.Log("Setting Up Standalone managers");
        }
        standaloneManagers = new List<Type>();

        managedComponents = Assembly.GetAssembly(typeof(ManagedByGameManager)).GetTypes().Where(t => t.IsSubclassOf(typeof(ManagedByGameManager))).ToList();
        if (_debugMode)
        {
            string standaloneManagersString = "";
            if (standaloneManagers.Count > 0)
            {
                for (int s = 0; s < standaloneManagers.Count; s++)
                {
                    standaloneManagersString += standaloneManagers[s].Name;
                    if (s != standaloneManagers.Count - 1)
                    {
                        standaloneManagersString += ", ";
                    }
                }
            }

            string managedManagersString = "";
            if (managedComponents.Count > 0)
            {
                for (int m = 0; m < managedComponents.Count; m++)
                {
                    managedManagersString += managedComponents[m].Name;
                    if(m != managedComponents.Count - 1)
                    {
                        managedManagersString += ", ";
                    }
                }
            }

            Debug.Log($"\nStandalone managers: {standaloneManagersString}\nAll managed components: {managedManagersString}");
            Debug.Log("Setting Up Standalone managers complete");
        }
    }
}
