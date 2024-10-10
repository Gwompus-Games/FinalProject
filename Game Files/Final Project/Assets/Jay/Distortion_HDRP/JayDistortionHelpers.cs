using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using System;

namespace Jay_Parameters 
{
    public enum maskChannelMode
    {
        alphaChannel,
        redChannel
    }
    [Serializable]
    public sealed class maskChannelModeParameter : VolumeParameter<maskChannelMode> { };
}
