using FMODUnity;
using UnityEngine;

public class FMODEvents : ManagedByGameManager
{
    [field: Header ("Heartbeat SFX")]
    [field: SerializeField] public EventReference heartbeat { get; private set; }
    
    [field: Header ("Footsteps SFX")]
    [field: SerializeField] public EventReference footsteps { get; private set; }

    [field: Header ("Angler Fish SFX")]
    [field: SerializeField] public EventReference AFDistantOutside { get; private set; }
    [field: SerializeField] public EventReference AFDistantInside { get; private set; }
    [field: SerializeField] public EventReference AFSpotted { get; private set; }
    [field: SerializeField] public EventReference AFAttacking { get; private set; }

    //TODO: MISC SFX

    public override void Init()
    {
        base.Init();
    }

    public override void CustomStart()
    {
        base.CustomStart();
    }
}
