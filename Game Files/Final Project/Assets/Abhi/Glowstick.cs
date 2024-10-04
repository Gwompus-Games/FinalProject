using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Glowstick : MonoBehaviour
{
    public Color[] colors;
    private void Awake()
    {
        transform.Rotate(90, Random.Range(0, 360), 0);
        GetComponentInChildren<Light>().color = colors[Random.Range(0,colors.Length)];
        GetComponentInChildren<Light>().intensity *= Random.Range(0.5f, 1);
    }
}
