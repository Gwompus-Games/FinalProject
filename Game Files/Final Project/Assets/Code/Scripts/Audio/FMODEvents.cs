using FMODUnity;
using UnityEngine;

public class FMODEvents : MonoBehaviour
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

    [field: Header("BGM")]
    [field: SerializeField] public EventReference bgm { get; private set; }
    [field: SerializeField] public EventReference menuMusic { get; private set; }

    [field: Header("UI")]
    [field: SerializeField] public EventReference hover { get; private set; }
    [field: SerializeField] public EventReference click { get; private set; }


    public static FMODEvents Instance;
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
}
