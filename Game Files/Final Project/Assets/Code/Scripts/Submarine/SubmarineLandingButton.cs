using UnityEngine;

public class SubmarineLandingButton : InteractableObject
{
    Submarine _submarine;

    protected override void Awake()
    {
        base.Awake();
    }

    protected override void Start()
    {
        base.Start();
        _submarine = GameManager.Instance.GetManagedComponent<Submarine>();
    }

    public override void Interact()
    {
        base.Interact();
        _submarine.ButtonPushed();
    }
}
