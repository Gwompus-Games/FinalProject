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
        //SceneManager.LoadScene(2);
        GameManager.Instance.GetManagedComponent<PlayerController>().TeleportPlayer(DungeonGenerator.Instance.transform.position);

        //Depreciated Code
        /*
        GameManager.PlayerControllerInstance.GetComponent<CharacterController>().enabled = false;
        GameManager.PlayerControllerInstance.transform.position =  DungeonGenerator.Instance.transform.position;
        GameManager.PlayerControllerInstance.GetComponent<CharacterController>().enabled = true;
        */
    }
}
