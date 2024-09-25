using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

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
    private List<DungeonPart> availableRooms;
    private bool isGenerated = false;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        generatedRooms = new List<DungeonPart>();
        availableRooms = new List<DungeonPart>();

        StartGeneration();
    }

    private void Update()
    {
        // Only for testing in editor play mode
        if (Input.GetKeyDown(KeyCode.G))
        {
            StartGeneration();
        }
    }

    public void StartGeneration()
    {
        if (isGenerated == true)
        {
            Despawn();
        }

        Generate();
        //GenerateAlternateEntrances();
        FillEmptyEntrances();
        isGenerated = true;

        Debug.Log($"Room Count: {generatedRooms.Count}");
    }

    private void Despawn()
    {
        foreach (DungeonPart room in generatedRooms)
        {
            Destroy(room.gameObject);
        }

        generatedRooms.Clear();
        availableRooms.Clear();
        isGenerated = false;
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
                    AddNewRoom(dungeonPart);
                }
            }
            else
            {
                bool shouldPlaceHallway = Random.Range(0f, 1f) < 0.95f;
                DungeonPart randomGeneratedRoom = null;
                Transform room1EntryPoint = null;
                int totalRetries = 1000;
                int retryIndex = 0;

                while (randomGeneratedRoom == null && retryIndex < totalRetries)
                {
                    int randomLinkRoomIndex = Random.Range(0, availableRooms.Count);
                    DungeonPart roomToTest = availableRooms[randomLinkRoomIndex];
                    if (roomToTest.HasAvailableEntryPoint(out room1EntryPoint))
                    {
                        randomGeneratedRoom = roomToTest;
                        break;
                    }
                    retryIndex++;
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
                            AddNewRoom(dungeonPart);

                            doorToAlign.transform.position = room1EntryPoint.transform.position;
                            doorToAlign.transform.rotation = room1EntryPoint.transform.rotation;

                            AlignRooms(randomGeneratedRoom.transform, generatedHallway.transform, room1EntryPoint, room2EntryPoint);

                            if (HandleIntersection(dungeonPart))
                            {
                                randomGeneratedRoom.UnuseEntryPoint(room1EntryPoint);
                                RemoveRoom(dungeonPart);
                                Destroy(generatedHallway);
                                Destroy(doorToAlign);
                                continue;
                            }

                            //if (HandleIntersection(dungeonPart))
                            //{
                            //    dungeonPart.UnuseEntryPoint(room2EntryPoint);
                            //    randomGeneratedRoom.UnuseEntryPoint(room1EntryPoint);
                            //    RetryPlacement(generatedHallway, doorToAlign);
                            //    continue;
                            //}
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
                            AddNewRoom(dungeonPart);
                            doorToAlign.transform.position = room1EntryPoint.transform.position;
                            doorToAlign.transform.rotation = room1EntryPoint.transform.rotation;
                            AlignRooms(randomGeneratedRoom.transform, generatedRoom.transform, room1EntryPoint, room2EntryPoint);

                            if (HandleIntersection(dungeonPart))
                            {
                                dungeonPart.UnuseEntryPoint(room1EntryPoint);
                                RemoveRoom(dungeonPart);
                                Destroy(generatedRoom);
                                Destroy(doorToAlign);
                                continue;
                            }
                        }
                    }
                }
            }
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

    //private void TryNewRoom(GameObject roomToIgnore, GameObject doorToPlace)
    //{
    //    DungeonPart randomGeneratedRoom = null;
    //    Transform room1EntryPoint = null;
    //    int totalRetries = 100;
    //    int retryIndex = 0;

    //    while (randomGeneratedRoom == null && retryIndex < totalRetries)
    //    {
    //        int randomLinkRoomIndex = Random.Range(0, generatedRooms.Count - 1);
    //        DungeonPart roomToTest = generatedRooms[randomLinkRoomIndex];

    //        if (roomToTest.HasAvailableEntryPoint(out room1EntryPoint))
    //        {
    //            randomGeneratedRoom = roomToTest;
    //            Debug.Log(randomLinkRoomIndex);
    //            break;
    //        }
    //        retryIndex++;
    //    }

    //    if (itemToPlace.TryGetComponent(out DungeonPart dungeonPart))
    //    {
    //        if (dungeonPart.HasAvailableEntryPoint(out Transform room2EntryPoint))
    //        {
    //            doorToPlace.transform.position = room1EntryPoint.transform.position;
    //            doorToPlace.transform.rotation = room1EntryPoint.transform.rotation;
    //            AlignRooms(randomGeneratedRoom.transform, itemToPlace.transform, room1EntryPoint, room2EntryPoint);

    //            if (HandleIntersection(dungeonPart))
    //            {
    //                dungeonPart.UnuseEntryPoint(room2EntryPoint);
    //                randomGeneratedRoom.UnuseEntryPoint(room1EntryPoint);
    //                Debug.Log("Stack overflow test, " + retryIndex);
    //                RetryPlacement(itemToPlace, doorToPlace);
    //            }
    //        }
    //    }
    //}

    private void AddNewRoom(DungeonPart dungeonPart)
    {
        generatedRooms.Add(dungeonPart);
        availableRooms.Add(dungeonPart);
    }

    private void RemoveRoom(DungeonPart dungeonPart)
    {
        generatedRooms.Remove(dungeonPart);
        availableRooms.Remove(dungeonPart);
    }

    public void RemoveAvailableRoom(DungeonPart room)
    {
        if (availableRooms.Contains(room))
            availableRooms.Remove(room);
    }

    public void AddAvailableRoom(DungeonPart room)
    {
        if (!availableRooms.Contains(room))
            availableRooms.Add(room);
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
                Debug.Log(randomLinkRoomIndex);
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
                    Debug.Log("Stack overflow test, " + retryIndex);
                    RetryPlacement(itemToPlace, doorToPlace);
                }
            }
        }
    }

    private void FillEmptyEntrances()
    {
        availableRooms.ForEach(room => room.FillEmptyDoors());
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

    private bool HasAvailablePath(DungeonPart dungeonPart)
    {
        bool hasAvailablePath = false;

        foreach (EntryPoint entry in dungeonPart.GetAvailableEntryPoints())
        {
            bool didIntersect = false;

            Collider[] hits = Physics.OverlapBox(
                entry.transform.position + (entry.transform.forward * 2),
                new Vector3(1,1,1),
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

            if (!didIntersect)
            {
                hasAvailablePath = true;
                break;
            }
        }
        

        return hasAvailablePath;
    }

    private void AlignRooms(Transform room1, Transform room2, Transform room1Entry, Transform room2Entry)
    {
        float angle = Vector3.SignedAngle(room1Entry.forward, room2Entry.forward, Vector3.up);
        //Debug.Log($"Angle: {angle}");

        //room2.TransformPoint(room2Entry.position);
        room2.eulerAngles = new Vector3(room2.eulerAngles.x, room2.eulerAngles.y + (180 - angle), room2.eulerAngles.z);
        //room2.RotateAround(room2Entry.position, Vector3.up, angle);

        Vector3 offset = room1Entry.position - room2Entry.position;
        //Debug.Log($"Offset: {offset}");

        room2.position += offset;

        Physics.SyncTransforms();
    }

    public List<DungeonPart> GetGeneratedRooms() => generatedRooms;

    public bool IsGenerated() => isGenerated;
}
