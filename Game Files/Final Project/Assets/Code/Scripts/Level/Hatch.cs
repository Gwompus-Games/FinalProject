using Unity.VisualScripting;
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
        if (GameManager.Instance.GetManagedComponent<Submarine>().landed)
            anim.SetBool("open", !anim.GetBool("open"));
    }

    public void CloseHatch()
    {
        anim.SetBool("open", false);
    }
}
