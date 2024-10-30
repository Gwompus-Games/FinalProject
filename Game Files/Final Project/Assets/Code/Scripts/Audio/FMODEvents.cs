using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FMODEvents : ManagedByGameManager
{
    [field: Header ("Heartbeat SFX")]
    [field: SerializeField] public EventReference heartbeat { get; private set; }
    
    [field: Header ("BGM")]
    [field: SerializeField] public EventReference bgm { get; private set; }
    [field: SerializeField] public EventReference facilityAmbience { get; private set; }
    
    [field: Header ("Footsteps SFX")]
    [field: SerializeField] public EventReference footsteps { get; private set; }

    public override void Init()
    {
        base.Init();
    }

    public override void CustomStart()
    {
        base.CustomStart();
    }
}
