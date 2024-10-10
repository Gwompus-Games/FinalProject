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
        print("interacted");
        GameManager.PlayerControllerInstance.GetComponent<CharacterController>().enabled = false;
        GameManager.PlayerControllerInstance.transform.position = teleport.transform.position;
        GameManager.PlayerControllerInstance.GetComponent<CharacterController>().enabled = true;
    }
}
