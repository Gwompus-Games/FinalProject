using UnityEngine;

public class Hatch : InteractableObject
{
    //public Transform teleport;
    public Animator anim;

    protected override void Awake()
    {
        base.Awake();
    }

    public override void Interact()
    {
        //GameManager.Instance.GetManagedComponent<PlayerController>().TeleportPlayer(teleport.position);
        anim.SetBool("open", !anim.GetBool("open"));
    }
}
