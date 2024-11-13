using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class DeathVisualComponent : DeathComponent
{
    private CanvasGroup _canvasGroup;

    private void Awake()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
    }

    public override void StartDeathComponent()
    {
        SetAlpha(1.0f);
    }

    public override void UpdateDeathComponent(float normalizedTime)
    {
        
    }

    public override void EndDeathComponent()
    {
        SetAlpha(0);
    }

    public void SetAlpha(float alpha)
    {
        if (_canvasGroup == null)
        {
            return;
        }
        alpha = Mathf.Clamp(alpha, 0f, 1f);
        _canvasGroup.alpha = alpha;
    }

    public float GetAlpha()
    {
        return _canvasGroup.alpha;
    }
}
