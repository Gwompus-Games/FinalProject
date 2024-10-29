using UnityEngine;

public class SubmarineLandingButton : InteractableObject
{
    Submarine _submarine;

    protected override void Awake()
    {
        base.Awake();
        _submarine = GameManager.Instance.GetManagedComponent<Submarine>();
    }

    public override void Interact()
    {
        base.Interact();
        _submarine.ButtonPushed();
    }
}
