using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DVCFadeOut : DeathVisualComponent
{
    [SerializeField] AnimationCurve fadeCurve;

    public override void StartDeathComponent()
    {
        SetAlpha(0);
    }

    public override void UpdateDeathComponent(float normalizedTime)
    {
        float fade = fadeCurve.Evaluate(normalizedTime);
        SetAlpha(fade);
    }

    public override void EndDeathComponent()
    {
        SetAlpha(1);
    }
}
