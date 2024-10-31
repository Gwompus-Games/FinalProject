using UnityEngine;

public class PlayerSpawnPointTag : MonoBehaviour
{
    private void Start()
    {
        Invoke(nameof(SetParent), 5f);
    }

    private void SetParent()
    {
        transform.parent = FindObjectOfType<Submarine>().transform;
        transform.localPosition = Vector3.up;
    }
}
