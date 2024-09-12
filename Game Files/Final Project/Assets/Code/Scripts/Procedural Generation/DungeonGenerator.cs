using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonGenerator : MonoBehaviour
{
    public static DungeonGenerator Instance {  get; private set; }

    [SerializeField] private GameObject entrance;
    [SerializeField] private List<GameObject> rooms;
    [SerializeField] private List<GameObject> specialRooms;
    [SerializeField] private List<GameObject> alternateEntrances;
    [SerializeField] private List<GameObject> hallways;
    [SerializeField] private GameObject door;
    [SerializeField] private int numOfRooms = 10;
    [SerializeField] private LayerMask roomLayerMask;

    private List<DungeonPart> generatedRooms;
    private bool isGenerated = false;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        generatedRooms = new List<DungeonPart>();
        StartGeneration();
    }

    private void Update()
    {
        // Only for testing in editor
        if (Input.GetKeyDown(KeyCode.G))
        {
            StartGeneration();
        }
    }

    public void StartGeneration()
    {
        Generate();
        GenerateAlternateEntrances();
        FillEmptyEntrances();
        isGenerated = true;
    }

    private void Generate()
    {
        for (int i = 0; i < numOfRooms - alternateEntrances.Count; i++)
        {
            if (generatedRooms.Count < 1)
            {
                GameObject generatedRoom = Instantiate(entrance, transform.position, transform.rotation);

                generatedRoom.transform.SetParent(null);

                if (generatedRoom.TryGetComponent(out DungeonPart dungeonPart) )
                {
                    generatedRooms.Add(dungeonPart);
                }
            }
            else
            {
                bool shouldPlaceHallway = Random.Range(0f, 1f) > 0.2f;
                DungeonPart randomGeneratedRoom = null;
                Transform room1EntryPoint = null;
                int totalRetries = 100;
                int retryIndex = 0;

                while (randomGeneratedRoom == null && retryIndex < totalRetries)
                {
                    int randomLinkRoomIndex = Random.Range(0, generatedRooms.Count);
                    DungeonPart roomToTest = generatedRooms[randomLinkRoomIndex];
                    if (roomToTest.HasAvailableEntryPoint(out room1EntryPoint))
                    {
                        randomGeneratedRoom = roomToTest;
                        break;
                    }
                    retryIndex++;

                    if (retryIndex >= totalRetries)
                    {
                        Debug.Log("Test1");
                    }
                }

                GameObject doorToAlign = Instantiate(door, transform.position, transform.rotation);

                if (shouldPlaceHallway)
                {
                    int randomIndex = Random.Range(0, hallways.Count);
                    GameObject generatedHallway = Instantiate(hallways[randomIndex], transform.position, transform.rotation);
                    generatedHallway.transform.SetParent(null);

                    if (generatedHallway.TryGetComponent(out DungeonPart dungeonPart))
                    {
                        if (dungeonPart.HasAvailableEntryPoint(out Transform room2EntryPoint))
                        {
                            generatedRooms.Add(dungeonPart);
                            doorToAlign.transform.position = room1EntryPoint.transform.position;
                            doorToAlign.transform.rotation = room1EntryPoint.transform.rotation;
                            AlignRooms(randomGeneratedRoom.transform, generatedHallway.transform, room1EntryPoint, room2EntryPoint);

                            if (HandleIntersection(dungeonPart))
                            {
                                dungeonPart.UnuseEntryPoint(room2EntryPoint);
                                randomGeneratedRoom.UnuseEntryPoint(room1EntryPoint);
                                RetryPlacement(generatedHallway, doorToAlign);
                                continue;
                            }
                        }
                    }
                }
                else
                {
                    GameObject generatedRoom;

                    if (specialRooms.Count > 0)
                    {
                        bool shouldPlaceSpecialRoom = Random.Range(0f, 1f) > 0.9f;

                        if (shouldPlaceSpecialRoom)
                        {
                            int randomIndex = Random.Range(0, specialRooms.Count);
                            generatedRoom = Instantiate(specialRooms[randomIndex], transform.position, transform.rotation);
                        }
                        else
                        {
                            int randomIndex = Random.Range(0, rooms.Count);
                            generatedRoom = Instantiate(rooms[randomIndex], transform.position, transform.rotation);
                        }
                    }
                    else
                    {
                        int randomIndex = Random.Range(0, rooms.Count);
                        generatedRoom = Instantiate(rooms[randomIndex], transform.position, transform.rotation);
                    }

                    generatedRoom.transform.SetParent(null);

                    if (generatedRoom.TryGetComponent(out DungeonPart dungeonPart))
                    {
                        if (dungeonPart.HasAvailableEntryPoint(out Transform room2EntryPoint))
                        {
                            generatedRooms.Add(dungeonPart);
                            doorToAlign.transform.position = room1EntryPoint.transform.position;
                            doorToAlign.transform.rotation = room1EntryPoint.transform.rotation;
                            AlignRooms(randomGeneratedRoom.transform, generatedRoom.transform, room1EntryPoint, room2EntryPoint);

                            if (HandleIntersection(dungeonPart))
                            {
                                dungeonPart.UnuseEntryPoint(room2EntryPoint);
                                randomGeneratedRoom.UnuseEntryPoint(room1EntryPoint);
                                RetryPlacement(generatedRoom, doorToAlign);
                                continue;
                            }
                        }
                    }
                }
            }

            Debug.Log("Test2");
        }
    }

    private void GenerateAlternateEntrances()
    {
        if (alternateEntrances.Count < 1) return;

        for (int i = 0; i < alternateEntrances.Count; i++)
        {
            {
                DungeonPart randomGeneratedRoom = null;
                Transform room1EntryPoint = null;
                int totalRetries = 100;
                int retryIndex = 0;

                while (randomGeneratedRoom == null && retryIndex < totalRetries)
                {
                    int randomLinkRoomIndex = Random.Range(0, generatedRooms.Count);
                    DungeonPart roomToTest = generatedRooms[randomLinkRoomIndex];
                    if (roomToTest.HasAvailableEntryPoint(out room1EntryPoint))
                    {
                        randomGeneratedRoom = roomToTest;
                        break;
                    }
                    retryIndex++;
                }

                int randomIndex = Random.Range(0, alternateEntrances.Count);
                GameObject generatedRoom = Instantiate(alternateEntrances[randomIndex], transform.position, transform.rotation);

                generatedRoom.transform.SetParent(null);

                GameObject doorToAlign = Instantiate(door, transform.position, transform.rotation);

                if (generatedRoom.TryGetComponent(out DungeonPart dungeonPart))
                {
                    if (dungeonPart.HasAvailableEntryPoint(out Transform room2EntryPoint))
                    {
                        generatedRooms.Add(dungeonPart);
                        doorToAlign.transform.position = room1EntryPoint.transform.position;
                        doorToAlign.transform.rotation = room1EntryPoint.transform.rotation;
                        AlignRooms(randomGeneratedRoom.transform, generatedRoom.transform, room1EntryPoint, room2EntryPoint);

                        if (HandleIntersection(dungeonPart))
                        {
                            dungeonPart.UnuseEntryPoint(room2EntryPoint);
                            randomGeneratedRoom.UnuseEntryPoint(room1EntryPoint);
                            RetryPlacement(generatedRoom, doorToAlign);
                            continue;
                        }
                    }
                }
            }
        }
    }

    private void RetryPlacement(GameObject itemToPlace, GameObject doorToPlace)
    {
        DungeonPart randomGeneratedRoom = null;
        Transform room1EntryPoint = null;
        int totalRetries = 100;
        int retryIndex = 0;

        while (randomGeneratedRoom == null && retryIndex < totalRetries)
        {
            int randomLinkRoomIndex = Random.Range(0, generatedRooms.Count - 1);
            DungeonPart roomToTest = generatedRooms[randomLinkRoomIndex];

            if (roomToTest.HasAvailableEntryPoint(out room1EntryPoint))
            {
                randomGeneratedRoom = roomToTest;
                break;
            }
            retryIndex++;
        }

        if (itemToPlace.TryGetComponent(out DungeonPart dungeonPart))
        {
            if (dungeonPart.HasAvailableEntryPoint(out Transform room2EntryPoint))
            {
                doorToPlace.transform.position = room1EntryPoint.transform.position;
                doorToPlace.transform.rotation = room1EntryPoint.transform.rotation;
                AlignRooms(randomGeneratedRoom.transform, itemToPlace.transform, room1EntryPoint, room2EntryPoint);

                if (HandleIntersection(dungeonPart))
                {
                    dungeonPart.UnuseEntryPoint(room2EntryPoint);
                    randomGeneratedRoom.UnuseEntryPoint(room1EntryPoint);
                    RetryPlacement(itemToPlace, doorToPlace);
                }
            }
        }
    }

    private void FillEmptyEntrances()
    {
        generatedRooms.ForEach(room => room.FillEmptyDoors());
    }

    private bool HandleIntersection(DungeonPart dungeonPart)
    {
        bool didIntersect = false;

        Collider[] hits = Physics.OverlapBox(
            dungeonPart.collider.bounds.center, 
            dungeonPart.collider.bounds.size / 2, 
            Quaternion.identity, 
            roomLayerMask);

        foreach (Collider hit in hits)
        {
            if (hit == dungeonPart.collider) continue;

            if (hit != dungeonPart.collider)
            {
                didIntersect = true;
                break;
            }
        }

        return didIntersect;
    }

    private void AlignRooms(Transform room1, Transform room2, Transform room1Entry, Transform room2Entry)
    {
        float angle = Vector3.Angle(room1Entry.forward, room2Entry.forward);

        room2.TransformPoint(room2Entry.position);
        room2.eulerAngles = new Vector3(room2.eulerAngles.x, room2.eulerAngles.y + angle, room2.eulerAngles.z);

        Vector3 offset = room1Entry.position - room2Entry.position;

        room2.position += offset;

        Physics.SyncTransforms();
    }

    public List<DungeonPart> GetGeneratedRooms() => generatedRooms;

    public bool IsGenerated() => isGenerated;
}
