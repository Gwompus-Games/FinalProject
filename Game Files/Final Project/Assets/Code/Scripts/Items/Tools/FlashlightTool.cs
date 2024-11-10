using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlashlightTool : ToolsParent
{
    private Light _light;
    private ToolController _controller;
    [SerializeField] private AudioSource _flashlightSource;
    [SerializeField] private AudioClip _flashlightClick;

    public override void Init()
    {
        base.Init();
        _light = GetComponentInChildren<Light>();
        if (_light == null)
        {
            GameObject lightGO = new GameObject("Spot Light", typeof(Light));
        }

        _myTool.rightHandPositionTransform = _handPositionController.GetRightHandTransform();
        _controller = GameManager.Instance.GetManagedComponent<ToolController>();
    }

    public override void CustomStart()
    {
        base.CustomStart();
    }

    public override void UseTool()
    {
        _light.enabled = !_light.enabled;
        if (_controller.debugMode)
        {
            if (_light.enabled)
            {
                Debug.Log("Flashlight turned on!");
                _flashlightSource.PlayOneShot(_flashlightClick);
            }
            else
            {
                Debug.Log("Flashlight turned off!");
                _flashlightSource.PlayOneShot(_flashlightClick);
            }
            
        }
    }

    public override void CancelUseTool()
    {
        
    }
}
