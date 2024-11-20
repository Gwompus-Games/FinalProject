using FMODUnity;
using UnityEngine;

public class FMODEvents : MonoBehaviour
{
    [field: Header ("Player SFX")]
    [field: SerializeField] public EventReference heartbeat { get; private set; }
    [field: SerializeField] public EventReference footsteps { get; private set; }
    [field: SerializeField] public EventReference breathing { get; private set; }
    [field: SerializeField] public EventReference suffocating { get; private set; }
    [field: SerializeField] public EventReference pickup { get; private set; }

    [field: Header("Tools SFX")]
    [field: SerializeField] public EventReference flashlight { get; private set; }
    [field: SerializeField] public EventReference radio { get; private set; }

    [field: Header ("Enemy SFX")]
    [field: SerializeField] public EventReference AFDistantOutside { get; private set; }
    [field: SerializeField] public EventReference AFDistantInside { get; private set; }
    [field: SerializeField] public EventReference AFSpotted { get; private set; }
    [field: SerializeField] public EventReference AFAttacking { get; private set; }

    [field: Header("Object SFX")]
    [field: SerializeField] public EventReference generatorLooping { get; private set; }
    [field: SerializeField] public EventReference entranceAura { get; private set; }

    [field: Header("BGM")]
    [field: SerializeField] public EventReference bgm { get; private set; }
    [field: SerializeField] public EventReference menuMusic { get; private set; }

    [field: Header("UI")]
    [field: SerializeField] public EventReference hover { get; private set; }
    [field: SerializeField] public EventReference click { get; private set; }
    [field: SerializeField] public EventReference denied { get; private set; }

    [field: Header("Ambiance")]
    [field: SerializeField] public EventReference bells { get; private set; }
    [field: SerializeField] public EventReference bark { get; private set; }
    [field: SerializeField] public EventReference choir { get; private set; }
    [field: SerializeField] public EventReference station { get; private set; }


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
