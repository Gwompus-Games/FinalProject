using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flashlight : MonoBehaviour
{
    private Light lightSource;
    public GameObject glowstickPrefab;

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
        if (Input.GetKeyDown(KeyCode.G))
        {
            Instantiate(glowstickPrefab, transform.position + transform.forward, Quaternion.identity);
        }
    }
}
