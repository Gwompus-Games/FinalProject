using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FacilityEntrance : MonoBehaviour, IInteractable
{
    private void Awake()
    {
        transform.tag = "Interactable";
    }

    public void Interact()
    {
        print("interacted");
        GameManager.Instance.GetManagedComponent<PlayerController>().TeleportPlayer(GameManager.Instance.GetManagedComponent<DungeonGenerator>().transform.position);
    }
}
