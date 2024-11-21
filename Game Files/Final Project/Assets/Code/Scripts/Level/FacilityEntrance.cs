using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FacilityEntrance : InteractableObject
{
    protected override void Awake()
    {
        base.Awake();
        transform.tag = "Interactable";
    }

    public override void Interact()
    {
        GameManager.Instance.isPlayerInsideFacility = true;
        GameManager.Instance.GetManagedComponent<PlayerController>().TeleportPlayer(GameManager.Instance.GetManagedComponent<DungeonGenerator>().transform.position);
    }
}
