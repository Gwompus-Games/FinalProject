using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flashlight : MonoBehaviour
{
    private Light lightSource;
    public GameObject glowstickPrefab;
    public int numberOfGlowsticks = 10;

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
        if (Input.GetKeyDown(KeyCode.G) && numberOfGlowsticks > 0)
        {
            Instantiate(glowstickPrefab, transform.position + transform.forward, Quaternion.identity);
            numberOfGlowsticks--;
        }
    }
}
