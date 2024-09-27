using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DungeonGenerator : MonoBehaviour
{
    public static DungeonGenerator Instance { get; private set; }

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
            Despawn();
            StartGeneration();
        }
    }

    public void StartGeneration()
    {
        int tries = 0;

        while (generatedRooms.Count < numOfRooms)
        {
            tries++;
            Debug.Log(generatedRooms.Count);

            if (tries > 100)
            {
                Debug.LogError("Reached maximum generation attempts without success!");
                return;
            }

            Generate();
        }

        //GenerateAlternateEntrances();
        FillEmptyEntrances();

        isGenerated = true;
        Debug.Log($"Room Count: {generatedRooms.Count}, Tries: {tries}");
    }

    private void Despawn()
    {
        foreach (DungeonPart room in generatedRooms)
        {
            DestroyImmediate(room.gameObject);
        }

        generatedRooms.Clear();
        availableRooms.Clear();
        isGenerated = false;
    }

    private void Generate()
    {
        if (generatedRooms.Count > 0)
            Despawn();

        CreateEntranceRoom();

        while (generatedRooms.Count < numOfRooms && availableRooms.Count > 0)
        {
            bool shouldPlaceHallway = Random.Range(0f, 1f) < 0.95f;
            DungeonPart previousRoom = null;
            Transform previousEntryPoint = null;
            int totalRetries = 1000;
            int retryIndex = 0;

            while (previousRoom == null && retryIndex < totalRetries)
            {
                int randomLinkRoomIndex = Random.Range(0, availableRooms.Count);
                DungeonPart roomToTest = availableRooms[randomLinkRoomIndex];
                if (roomToTest.HasAvailableEntryPoint(out previousEntryPoint))
                {
                    previousRoom = roomToTest;
                    break;
                }
                retryIndex++;
            }

            if (shouldPlaceHallway)
            {
                int randomIndex = Random.Range(0, hallways.Count);
                GameObject generatedHallway = Instantiate(hallways[randomIndex], transform.position, transform.rotation);
                generatedHallway.transform.SetParent(transform);
                generatedHallway.name += " " + generatedRooms.Count;

                SetRandomRotation(generatedHallway.transform);

                if (!CreateRoom(previousRoom, previousEntryPoint, generatedHallway))
                    continue;
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

                generatedRoom.transform.SetParent(transform);
                generatedRoom.name += " " + generatedRooms.Count;

                SetRandomRotation(generatedRoom.transform);

                if (!CreateRoom(previousRoom, previousEntryPoint, generatedRoom))
                {
                    if (!RetryPlacement(previousRoom, previousEntryPoint))
                    {
                        availableRooms.Remove(previousRoom);
                        previousRoom.FillEmptyDoors();
                        continue;
                    }
                }
            }
        }
    }

    //private void GenerateAlternateEntrances()
    //{
    //    if (alternateEntrances.Count < 1) return;

    //    for (int i = 0; i < alternateEntrances.Count; i++)
    //    {
    //        {
    //            DungeonPart randomGeneratedRoom = null;
    //            Transform room1EntryPoint = null;
    //            int totalRetries = 100;
    //            int retryIndex = 0;

    //            while (randomGeneratedRoom == null && retryIndex < totalRetries)
    //            {
    //                int randomLinkRoomIndex = Random.Range(0, generatedRooms.Count);
    //                DungeonPart roomToTest = generatedRooms[randomLinkRoomIndex];
    //                if (roomToTest.HasAvailableEntryPoint(out room1EntryPoint))
    //                {
    //                    randomGeneratedRoom = roomToTest;
    //                    break;
    //                }
    //                retryIndex++;
    //            }

    //            int randomIndex = Random.Range(0, alternateEntrances.Count);
    //            GameObject generatedRoom = Instantiate(alternateEntrances[randomIndex], transform.position, transform.rotation);

    //            generatedRoom.transform.SetParent(null);

    //            GameObject doorToAlign = Instantiate(door, transform.position, transform.rotation);

    //            if (generatedRoom.TryGetComponent(out DungeonPart dungeonPart))
    //            {
    //                if (dungeonPart.HasAvailableEntryPoint(out Transform room2EntryPoint))
    //                {
    //                    generatedRooms.Add(dungeonPart);
    //                    doorToAlign.transform.position = room1EntryPoint.transform.position;
    //                    doorToAlign.transform.rotation = room1EntryPoint.transform.rotation;
    //                    AlignRooms(randomGeneratedRoom.transform, generatedRoom.transform, room1EntryPoint, room2EntryPoint);

    //                    if (HandleIntersection(dungeonPart))
    //                    {
    //                        dungeonPart.UnuseEntryPoint(room2EntryPoint);
    //                        randomGeneratedRoom.UnuseEntryPoint(room1EntryPoint);
    //                        //RetryPlacement(generatedRoom, doorToAlign);
    //                        continue;
    //                    }
    //                }
    //            }
    //        }
    //    }
    //}

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

    private bool CreateRoom(DungeonPart previousRoom, Transform previousEntryPoint, GameObject newRoom)
    {
        if (newRoom.TryGetComponent(out DungeonPart newPart))
        {
            if (newPart.HasAvailableEntryPoint(out Transform newEntryPoint))
            {
                AddNewRoom(newPart);

                GameObject newDoor = Instantiate(door, transform.position, transform.rotation);

                newDoor.transform.parent = newRoom.transform;
                newDoor.transform.position = previousEntryPoint.transform.position;
                newDoor.transform.rotation = previousEntryPoint.transform.rotation;

                AlignRooms(previousRoom.transform, newRoom.transform, previousEntryPoint, newEntryPoint);

                if (IsIntersecting(newPart, previousRoom, newRoom))
                {
                    previousRoom.UnuseEntryPoint(previousEntryPoint);
                    RemoveRoom(newPart);
                    DestroyImmediate(newRoom);
                    //Debug.Log($"Destroyed room attached to {previousRoom.name}");
                    return false;
                }
            }
        }

        return true;
    }

    private bool HasAvailablePath(DungeonPart dungeonPart)
    {
        bool hasAvailablePath = false;

        foreach (EntryPoint entry in dungeonPart.GetAvailableEntryPoints())
        {
            bool didIntersect = false;

            Collider[] hits = Physics.OverlapBox(
                entry.transform.position + (entry.transform.forward * 2),
                new Vector3(1, 1, 1),
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

    private bool RetryPlacement(DungeonPart previousRoom, Transform previousEntryPoint)
    {
        List<GameObject> shuffledHallways = new List<GameObject>();
        shuffledHallways.AddRange(hallways);
        Shuffle(hallways);

        for (int i = 0; i < shuffledHallways.Count; i++)
        {
            for (int r = 0; r < 4; r++)
            {
                GameObject generatedHallway = Instantiate(shuffledHallways[i], transform.position, transform.rotation);
                generatedHallway.transform.SetParent(transform);
                generatedHallway.name += " " + generatedRooms.Count;

                SetYRotation(generatedHallway.transform, r * 90);

                if (CreateRoom(previousRoom, previousEntryPoint, generatedHallway))
                    return true;
            }
        }

        return false;
    }

    private void CreateEntranceRoom()
    {
        GameObject generatedRoom = Instantiate(entrance, transform.position, transform.rotation);

        generatedRoom.transform.SetParent(transform);

        if (generatedRoom.TryGetComponent(out DungeonPart dungeonPart))
        {
            AddNewRoom(dungeonPart);
        }
    }

    private void SetRandomRotation(Transform objectToRotate)
    {
        int randomRotation = Random.Range(0, 4) * 90;
        SetYRotation(objectToRotate, randomRotation);
    }

    private void SetYRotation(Transform objectToRotate, float yRot)
    {
        objectToRotate.eulerAngles = new Vector3(
            objectToRotate.eulerAngles.x,
            yRot,
            objectToRotate.eulerAngles.z
        );
    }

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

    private void FillEmptyEntrances()
    {
        availableRooms.ForEach(room => room.FillEmptyDoors());
    }

    private bool IsIntersecting(DungeonPart dungeonPart, DungeonPart previousRoom, GameObject newRoom)
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
                //Debug.Log($"Trying to place {newRoom.transform.name}, touching {hit.transform.name} from {previousRoom.transform.name}");
                break;
            }
        }

        return didIntersect;
    }

    public void Shuffle<T>(IList<T> ts)
    {
        var count = ts.Count;
        var last = count - 1;
        for (var i = 0; i < last; ++i)
        {
            var r = UnityEngine.Random.Range(i, count);
            var tmp = ts[i];
            ts[i] = ts[r];
            ts[r] = tmp;
        }
    }

    public List<DungeonPart> GetGeneratedRooms() => generatedRooms;

    public bool IsGenerated() => isGenerated;
}
