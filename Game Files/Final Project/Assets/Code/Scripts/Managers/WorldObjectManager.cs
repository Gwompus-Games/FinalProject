using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(WorldItemsTag))]
public class WorldObjectManager : ManagedByGameManager
{
    public override void Init()
    {
        base.Init();
        WorldObjectManager[] worldObjectManagers = FindObjectsByType<WorldObjectManager>(FindObjectsSortMode.None);
        
        for (int w = 0; w < worldObjectManagers.Length; w++)
        {
            if (worldObjectManagers[w].gameObject != gameObject)
            {
                Destroy(worldObjectManagers[w].gameObject);
            }
        }

        WorldItemsTag[] worldItemsTags = FindObjectsByType<WorldItemsTag>(FindObjectsSortMode.None);
        
        for (int w = 0; w < worldItemsTags.Length; w++)
        {
            if (worldItemsTags[w].gameObject != gameObject)
            {
                Destroy(worldItemsTags[w].gameObject);
            }
        }
    }

    public void DestroyAllItems()
    {
        if (transform.childCount == 0)
        {
            return;
        }
        List<GameObject> childGameObjects = new List<GameObject>();
        for (int c = 0; c < transform.childCount; c++)
        {
            childGameObjects.Add(transform.GetChild(c).gameObject);
        }

        for (int c = 0; c < childGameObjects.Count; c++)
        {
            Destroy(childGameObjects[c]);
        }
    }
}
