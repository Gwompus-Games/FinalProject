using System.Collections;
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
    }
}
