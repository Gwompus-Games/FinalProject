using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FacilityExit : MonoBehaviour, IInteractable
{
    public FacilityEntrance[] facilityEntrances;

    private void Awake()
    {
        transform.tag = "Interactable";
    }

    private void Start()
    {
        facilityEntrances = FindObjectsOfType<FacilityEntrance>();
        if(facilityEntrances.Length <= 0 )
        {
            Debug.LogError("No facility entrances found");
        }
    }

    public void Interact()
    {
        Transform randomFacilityEntrance = facilityEntrances[Random.Range(0, facilityEntrances.Length)].transform;
        GameManager.Instance.GetManagedComponent<PlayerController>().TeleportPlayer(randomFacilityEntrance.position);
        GameManager.Instance.GetManagedComponent<DungeonGenerator>().StartGeneration();
    }
}
