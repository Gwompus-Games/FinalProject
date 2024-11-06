using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class FadeOutElement : MonoBehaviour
{
    private Image _imageToFade;
    [SerializeField] [Range(0f, 1f)] private float _startingAlpha;
    [SerializeField] [Range(0f, 1f)] private float _endingAlpha;
    [SerializeField] private float _fadeTimeInSeconds = 5f;
    [SerializeField] AnimationCurve _fadeCurve;
    private float _fadeTimeTotal = 0f;
    private bool _fadeFinished = false;

    private void Awake()
    {
        _imageToFade = GetComponent<Image>();
        _imageToFade.enabled = true;
    }

    private void Start()
    {
        Color colour = _imageToFade.color;
        colour.a = _startingAlpha;
        _imageToFade.color = colour;
        _fadeTimeTotal = 0f;
        _fadeFinished = false;
    }

    private void Update()
    {
        if (_fadeFinished)
        {
            return;
        }

        float lerpAmount = _fadeTimeTotal / _fadeTimeInSeconds;
        float alpha = Mathf.Lerp(_startingAlpha, _endingAlpha, _fadeCurve.Evaluate(lerpAmount));
        Color colour = _imageToFade.color;
        colour.a = alpha;
        _imageToFade.color = colour;

        _fadeTimeTotal += Time.deltaTime;

        if (_fadeTimeTotal > _fadeTimeInSeconds)
        {
            FinishFade();
        }
    }

    private void FinishFade()
    {
        _fadeFinished = true;
        Destroy(gameObject);
    }
}
