using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flashlight : MonoBehaviour
{
    private Light lightSource;

    private void Awake()
    {
        lightSource = GetComponent<Light>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            lightSource.enabled = !lightSource.enabled;
        }
    }
}
