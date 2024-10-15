using UnityEngine;

public class Hatch : MonoBehaviour, IInteractable
{
    public Transform teleport;

    private void Awake()
    {
        transform.tag = "Interactable";        
    }

    public void Interact()
    {
        GameManager.Instance.GetManagedComponent<PlayerController>().TeleportPlayer(teleport.position);

        //Depreciated Code
        /*
        print("interacted");
        GameManager.PlayerControllerInstance.GetComponent<CharacterController>().enabled = false;

        if (teleport == null)        
            teleport.position = GameObject.Find("FacilityExit").transform.position;        
        
        GameManager.PlayerControllerInstance.transform.position = teleport.transform.position;

        GameManager.PlayerControllerInstance.GetComponent<CharacterController>().enabled = true;
        */
    }
}
