using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FacilityExit : MonoBehaviour, IInteractable
{
    public Transform facilityExit;

    private void Awake()
    {
        transform.tag = "Interactable";
    }

    private void Start()
    {
        facilityExit = GameObject.Find("FacilityExit").transform;
    }

    public void Interact()
    {
        if(!facilityExit)
        {
            facilityExit = GameObject.Find("FacilityExit").transform;
        }
        print("interacted");
        if (facilityExit)
        {
            GameManager.Instance.GetManagedComponent<PlayerController>().TeleportPlayer(facilityExit.position);
            GameManager.Instance.GetManagedComponent<DungeonGenerator>().StartGeneration();
        }
        else
            print("No transform found");
    }
}
