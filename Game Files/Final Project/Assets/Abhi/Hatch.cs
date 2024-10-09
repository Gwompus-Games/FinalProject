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
        PlayerController.Instance.GetComponent<CharacterController>().enabled = false;
        PlayerController.Instance.transform.position = teleport.transform.position;
        PlayerController.Instance.GetComponent<CharacterController>().enabled = true;
    }
}
