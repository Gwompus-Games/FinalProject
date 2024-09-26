using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlashingLight : MonoBehaviour
{
    [Header("Light Settings")]
    [SerializeField] private Color offColor = Color.black;

    [Header("Timing Settings")]
    [SerializeField] private float minOffTime = 0.05f;
    [SerializeField] private float maxOffTime = 0.15f;
    [SerializeField] private float minDelay = 2f;
    [SerializeField] private float maxDelay = 5f;

    private Light lightComponent;
    private Color onColor;

    private void Awake()
    {
        lightComponent = GetComponent<Light>();
        onColor = lightComponent.color;
    }

    void Start()
    {
        StartCoroutine(Flicker());
    }

    IEnumerator Flicker()
    {
        lightComponent.color = onColor;
        yield return new WaitForSeconds(Random.Range(minDelay, maxDelay));

        lightComponent.color = offColor;
        yield return new WaitForSeconds(Random.Range(minOffTime, maxOffTime));

        StartCoroutine(Flicker());
    }
}
