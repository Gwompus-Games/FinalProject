using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RenderDistance : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out IHideable hideable))
        {
            hideable.Show(true);
        }

    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out IHideable hideable))
        {
            hideable.Show(false);
        }
    }
}
