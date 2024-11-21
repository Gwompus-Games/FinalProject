using FMOD.Studio;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlashlightTool : ToolsParent
{
    private Light _light;
    private ToolController _controller;
    private EventInstance flashlightSound;

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
        flashlightSound = AudioManager.Instance.CreateEventInstance(FMODEvents.Instance.flashlight);
    }

    public override void UseTool()
    {
        _light.enabled = !_light.enabled;
        //FMOD
        //PLAYBACK_STATE ps;
        //flashlightSound.getPlaybackState(out ps);
        //if(ps.Equals(PLAYBACK_STATE.STOPPED))
        flashlightSound.start();

        if (_controller.debugMode)
        {
            if (_light.enabled)
            {
                Debug.Log("Flashlight turned on!");
            }
            else
            {
                Debug.Log("Flashlight turned off!");
            }
            
        }
    }

    public override void CancelUseTool()
    {
        
    }
}
