using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;
using System;

[Serializable, VolumeComponentMenu("Post-processing/Jay Distortion")]
public sealed class JayDistortion : CustomPostProcessVolumeComponent, IPostProcessComponent
{
    public BoolParameter enable = new BoolParameter(false);
    
    [Tooltip("Controls the strength of the distortion effect")]
    public ClampedFloatParameter distortionIntensity = new ClampedFloatParameter(0.5f, 0f, 1f);
    
    [Tooltip("Controls the speed of the distortion animation")]
    public FloatParameter distortionSpeed = new FloatParameter(1f);
    
    [Tooltip("Controls the scale of the distortion pattern")]
    public FloatParameter distortionScale = new FloatParameter(5f);
    
    [Tooltip("The texture to be distorted")]
    public TextureParameter mainTex = new TextureParameter(null);

    Material m_Material;

    public bool IsActive() => m_Material != null && enable.value;

    public override CustomPostProcessInjectionPoint injectionPoint => CustomPostProcessInjectionPoint.AfterPostProcess;

    const string kShaderName = "Jay/Distortion";

    public override void Setup()
    {
        if (Shader.Find(kShaderName) != null)
            m_Material = new Material(Shader.Find(kShaderName));
        else
            Debug.LogError($"Unable to find shader '{kShaderName}'. Post Process Volume Jay Distortion is unable to load.");
    }

    public override void Render(CommandBuffer cmd, HDCamera camera, RTHandle source, RTHandle destination)
    {
        if (m_Material == null)
            return;

        m_Material.SetFloat("_DistortionIntensity", distortionIntensity.value);
        m_Material.SetFloat("_DistortionSpeed", distortionSpeed.value);
        m_Material.SetFloat("_DistortionScale", distortionScale.value);

        if (mainTex.value != null)
        {
            m_Material.SetTexture("_MainTex", mainTex.value);
        }
        else
        {
            m_Material.SetTexture("_MainTex", source);
        }

        cmd.Blit(source, destination, m_Material, 0);
    }

    public override void Cleanup()
    {
        CoreUtils.Destroy(m_Material);
    }
}