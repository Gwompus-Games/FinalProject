using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WI_Glowstick : WorldItem
{
    public Color? chosenColour { get; private set; }
    public bool used { get; private set; }
    private Light _light;

    protected override void Awake()
    {
        base.Awake();
        _light = GetComponentInChildren<Light>();
        _light.enabled = false;
    }

    protected override void Start()
    {
        base.Start();
        GlowstickSO glowstickData = _itemData as GlowstickSO;
        if (glowstickData == null)
        {
            throw new System.Exception("Non glowstick data assigned for glowstick!");
        }
        if (chosenColour == null)
        {
            AssignColour(glowstickData.possibleColours[Random.Range(0, glowstickData.possibleColours.Length)]);
        }
    }

    public void AssignColour(Color cColour)
    {
        if (chosenColour != null)
        {
            return;
        }
        if (used)
        {
            return;
        }
        chosenColour = cColour;
        _light.color = (Color)chosenColour;
    }

    public void UseGlowstick(Color cColour)
    {
        chosenColour = null;
        _light.enabled = true;
        AssignColour(cColour);
        used = true;
    }

    public override void EnablePopup()
    {
        if (used)
        {
            return;
        }
        base.EnablePopup();
    }

    public override void Interact()
    {
        if (used)
        {
            return;
        }
        base.Interact();
    }
}
