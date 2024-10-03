using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

public class DungeonPart : MonoBehaviour
{
    [SerializeField] private GameObject fillerWall;

    public List<Transform> entryPoints;
    private List<Transform> avaiableEntryPoints = new List<Transform>();

    [SerializeField] private Transform enemySpawnPoint;

    private MeshRenderer[] meshRenderersToHide;
    private Light[] lightsToHide;

    public new Collider collider;

    private void Awake()
    {
        avaiableEntryPoints.AddRange(entryPoints);

        GetComponentsToHide();
    }

    public void SetupPart()
    {
        ShowMesh(false);
        //SpawnLoot();
    }

    private void SpawnLoot()
    {
        TreasureSpawnPoint[] spawnPoints = GetComponentsInChildren<TreasureSpawnPoint>();

        foreach (TreasureSpawnPoint point in spawnPoints)
        {
            point.SpawnTreasureByRarity();
        }
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
                    UseEntryPoint(entry, res);
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
                    UseEntryPoint(entry, res);
                    break;
                }
            }
            retryIndex++;
        }

        entryPoint = resultingEntry;

        return result;
    }

    private void UseEntryPoint(Transform entryTransform, EntryPoint entryPoint)
    {
        entryPoint.SetOccupied(true);
        avaiableEntryPoints.Remove(entryTransform);

        if (avaiableEntryPoints.Count <= 0)
            DungeonGenerator.Instance.RemoveAvailableRoom(this);
    }

    public void UnuseEntryPoint(Transform entryPoint)
    {
        if (entryPoint.TryGetComponent(out EntryPoint entry))
        {
            entry.SetOccupied(false);
            avaiableEntryPoints.Add(entryPoint);
            DungeonGenerator.Instance.AddAvailableRoom(this);
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
                    wall.transform.parent = transform;
                    wall.transform.position = entry.transform.position;
                    wall.transform.rotation = entry.transform.rotation;
                }
            }
        });
    }

    public List<EntryPoint> GetAvailableEntryPoints()
    {
        List<EntryPoint> availableEntryPoints = new List<EntryPoint>();

        foreach (Transform entryTransform in entryPoints)
        {
            if (entryTransform.gameObject.TryGetComponent(out EntryPoint entryPoint))
            {
                if (entryPoint.IsOccupied())
                    continue;

                availableEntryPoints.Add(entryPoint);
            }
        }

        return availableEntryPoints;
    }

    public bool SpawnEnemy(GameObject enemyPrefab)
    {
        Vector3 spawnPoint = transform.position;

        if (enemySpawnPoint != null)
            spawnPoint = enemySpawnPoint.position;

        GameObject newEnemy = Instantiate(enemyPrefab, spawnPoint, Quaternion.identity);
        if (newEnemy.TryGetComponent(out Enemy enemyScript))
        {
            if (enemyScript.HasValidPath(PlayerController.Instance.transform.position))
            {
                enemyScript.SetupEnemy();
                return true;
            }
            else
            {
                Destroy(newEnemy);
                return false;
            }
        }

        return false;
    }

    public void ShowMesh(bool isShowing = true)
    {
        foreach (Light obj in lightsToHide)
        {
            obj.enabled = isShowing;
        }

        foreach (MeshRenderer renderer in meshRenderersToHide)
        {
            renderer.enabled = isShowing;
        }
    }

    private void GetComponentsToHide()
    {
        meshRenderersToHide = gameObject.GetComponentsInChildren<MeshRenderer>();
        lightsToHide = gameObject.GetComponentsInChildren<Light>();
    }

    //private void FixBoxCollider()
    //{
    //    boxCollider.size = new Vector3(boxCollider.size.x - 0.2f, boxCollider.size.y - 0.2f, boxCollider.size.z - 0.2f);
    //}

    //private void OnDrawGizmos()
    //{
    //    if (collider == null)
    //        return;

    //    Gizmos.color = Color.yellow;
    //    Gizmos.DrawWireCube(collider.bounds.center, collider.bounds.size);
    //}
} 
