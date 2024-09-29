using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FMODEvents : MonoBehaviour
{
    [field: Header ("Heartbeat SFX")]
    [field: SerializeField] public EventReference heartbeat { get; private set; }
    
    [field: Header ("BGM")]
    [field: SerializeField] public EventReference bgm { get; private set; }
    
    [field: Header ("Footsteps SFX")]
    [field: SerializeField] public EventReference footsteps { get; private set; }
    public static FMODEvents instance { get; private set; }

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("Found more than one audio manager in the scene.");
        }
        instance = this;
    }
}
