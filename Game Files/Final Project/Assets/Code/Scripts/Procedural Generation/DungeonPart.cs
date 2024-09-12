using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonPart : MonoBehaviour
{
    public enum DungeonPartType
    {
        Room,
        Hallway
    }

    [SerializeField] LayerMask roomLayerMask;
    [SerializeField] private DungeonPartType dungeonPartType;
    [SerializeField] private GameObject fillerWall;

    public List<Transform> entryPoints;
    public new Collider collider;

    private BoxCollider boxCollider;

    private void Awake()
    {
        boxCollider = GetComponent<BoxCollider>();
        FixBoxCollider();
    }

    public bool HasAvailableEntryPoint(out Transform entryPoint)
    {
        Transform resultingEntry = null;
        bool result = false;

        int totalRetries = 100;
        int retryIndex = 0;

        if (entryPoints.Count == 1)
        {
            Transform entry = entryPoints[0];
            if (entry.TryGetComponent<EntryPoint>(out EntryPoint res))
            {
                if (res.IsOccupied())
                {
                    result = false;
                    resultingEntry = null;
                }
                else
                {
                    result = true;
                    resultingEntry = entry;
                    res.SetOccupied();
                }
                entryPoint = resultingEntry;
                return result;
            }
        }

        while (resultingEntry == null && retryIndex < totalRetries)
        {
            int randomEntryIndex = Random.Range(0, entryPoints.Count);
            Transform entry = entryPoints[randomEntryIndex];

            if (entry.TryGetComponent(out EntryPoint res))
            {
                if (!res.IsOccupied())
                {
                    resultingEntry = entry;
                    result = true;
                    res.SetOccupied();
                    break;
                }
            }
            retryIndex++;
        }

        entryPoint = resultingEntry;

        return result;
    }

    public void UnuseEntryPoint(Transform entryPoint)
    {
        if (entryPoint.TryGetComponent(out EntryPoint entry))
        {
            entry.SetOccupied(false);
        }
    }

    public void FillEmptyDoors()
    {
        entryPoints.ForEach((entry) =>
        {
            if (entry.TryGetComponent(out EntryPoint entryPoint))
            {
                if (!entryPoint.IsOccupied())
                {
                    GameObject wall = Instantiate(fillerWall);
                    wall.transform.position = entry.transform.position;
                    wall.transform.rotation = entry.transform.rotation;
                }
            }
        });
    }

    private void FixBoxCollider()
    {
        boxCollider.size = new Vector3(boxCollider.size.x - 0.1f, boxCollider.size.y - 0.1f, boxCollider.size.z - 0.1f);
    }

    private void OnDrawGizmos()
    {
        if (collider == null)
            return;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(collider.bounds.center, collider.bounds.size);
    }
} 
