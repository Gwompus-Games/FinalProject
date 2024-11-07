using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FacilityExit : MonoBehaviour, IInteractable
{
    public GameObject[] facilityExits;

    private void Awake()
    {
        transform.tag = "Interactable";
    }

    private void Start()
    {
        facilityExits = GameObject.FindGameObjectsWithTag("ExitMarker");
        if(facilityExits.Length <= 0 )
        {
            Debug.LogError("No facility Exit Markers found");
        }
    }

    public void Interact()
    {
        GameManager.Instance.isPlayerInsideFacility = false;
        Transform randomFacilityEntrance = facilityExits[Random.Range(0, facilityExits.Length)].transform;
        GameManager.Instance.GetManagedComponent<PlayerController>().TeleportPlayer(randomFacilityEntrance.position);
        GameManager.Instance.GetManagedComponent<DungeonGenerator>().StartGeneration();
    }
}
