using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlashlightTool : ToolsParent
{
    private Light _light;

    protected override void Awake()
    {
        base.Awake();
        _light = GetComponentInChildren<Light>();
        if (_light == null)
        {
            GameObject lightGO = new GameObject("Spot Light", typeof(Light));
        }
    }

    protected override void Start()
    {

    }

    public override void UseTool()
    {
        _light.enabled = !_light.enabled;
    }
}
