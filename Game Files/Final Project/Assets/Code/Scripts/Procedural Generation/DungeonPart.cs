using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Xml;
using Unity.Burst.Intrinsics;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

public class DungeonPart : MonoBehaviour, IHideable
{
    [SerializeField] private GameObject fillerWall;

    public List<Transform> entryPoints;
    private List<Transform> availableEntryPoints = new List<Transform>();

    [SerializeField] private Transform enemySpawnPoint;

    private MeshRenderer[] meshRenderersToHide;
    private Light[] lightsToHide;

    public new Collider collider;

    private void Awake()
    {
        availableEntryPoints.AddRange(entryPoints);

        GetComponentsToHide();
    }

    public void SetupPart()
    {
        Show(false);
    }

    public void SpawnLoot()
    {
        TreasureSpawnPoint[] spawnPoints = GetComponentsInChildren<TreasureSpawnPoint>();

        if (spawnPoints.Length == 0 )
            return;

        foreach (TreasureSpawnPoint point in spawnPoints)
        {
            point.SpawnTreasure();
        }
    }

    public bool HasAvailableEntryPoint(out Transform entryPoint)
    {
        Transform resultingEntry = null;
        bool result = false;

        int totalRetries = 100;
        int retryIndex = 0;

        while (resultingEntry == null && retryIndex < totalRetries)
        {
            int randomEntryIndex = Random.Range(0, availableEntryPoints.Count);
            Transform entry = availableEntryPoints[randomEntryIndex];

            if (entry.TryGetComponent(out EntryPoint res))
            {
                if (!res.IsOccupied())
                {
                    resultingEntry = entry;
                    result = true;
                    break;
                }
            }
            retryIndex++;
        }

        entryPoint = resultingEntry;

        return result;
    }

    public void UseEntryPoint(Transform entryPoint)
    {
        if (entryPoint.TryGetComponent(out EntryPoint entry))
        {
            if (entry.IsOccupied())
                return;

            entry.SetOccupied(true);
            availableEntryPoints.Remove(entryPoint);

            if (availableEntryPoints.Count == 0)
            GameManager.Instance.GetManagedComponent<DungeonGenerator>().RemoveAvailableRoom(this);
        }
    }

    public void UnuseEntryPoint(Transform entryPoint)
    {
        if (entryPoint.TryGetComponent(out EntryPoint entry))
        {
            if (entry.IsOccupied() == false)
                return;

            entry.SetOccupied(false);
            availableEntryPoints.Add(entryPoint);
            GameManager.Instance.GetManagedComponent<DungeonGenerator>().AddAvailableRoom(this);
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
            if (enemyScript.HasValidPath(GameManager.Instance.GetManagedComponent<DungeonGenerator>().transform.position))
            {
                enemyScript.SetupEnemy();
                GameManager.Instance.GetManagedComponent<DungeonGenerator>().enemies.Add(newEnemy.GetComponent<Enemy>());
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

    public void Show(bool isShowing = true)
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
