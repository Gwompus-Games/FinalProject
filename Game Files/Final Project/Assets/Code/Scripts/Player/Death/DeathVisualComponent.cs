using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class DeathVisualComponent : MonoBehaviour
{
    private CanvasGroup _canvasGroup;

    private void Awake()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
    }

    public void SetAlpha(float alpha)
    {
        alpha = Mathf.Clamp(alpha, 0f, 1f);
        _canvasGroup.alpha = alpha;
    }

    public float GetAlpha()
    {
        return _canvasGroup.alpha;
    }
}
