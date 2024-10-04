using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Glowstick : MonoBehaviour, IHideable
{
    public Color[] colors;
    private Light lightSource;

    private void Awake()
    {
        lightSource = GetComponentInChildren<Light>();

        transform.Rotate(90, Random.Range(0, 360), 0);
        lightSource.color = colors[Random.Range(0,colors.Length)];
        lightSource.intensity *= Random.Range(0.5f, 1);
    }

    public void Show(bool isShowing)
    {
        lightSource.enabled = isShowing;
    }
}
