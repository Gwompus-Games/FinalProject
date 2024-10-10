using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

[System.Serializable]
public class UnderwaterSettings
{
    public Material material;
    public Color color = Color.blue;
    public float fogDensity = 1f;
    [Range(0, 1)]
    public float alpha = 0.5f;
    public float refraction = 0.1f;
    public Texture normalmap;
    public Vector4 UV = new Vector4(1, 1, 0.2f, 0.1f);
}

public class Underwater : CustomPass
{
    public UnderwaterSettings settings = new UnderwaterSettings();

    protected override void Setup(ScriptableRenderContext renderContext, CommandBuffer cmd)
    {
        // Setup code if needed
    }

    protected override void Execute(CustomPassContext ctx)
    {
        if (settings.material == null)
            return;

        // Set material properties
        settings.material.SetFloat("_FogDensity", settings.fogDensity);
        settings.material.SetFloat("_Alpha", settings.alpha);
        settings.material.SetColor("_Color", settings.color);
        settings.material.SetTexture("_NormalMap", settings.normalmap);
        settings.material.SetFloat("_Refraction", settings.refraction);
        settings.material.SetVector("_NormalUV", settings.UV);

        // Setup temporary render texture
        RTHandle tempRT = RTHandles.Alloc(ctx.cameraColorBuffer);

        // Execute the effect
        CoreUtils.SetRenderTarget(ctx.cmd, tempRT);
        CoreUtils.DrawFullScreen(ctx.cmd, settings.material, shaderPassId: 0);
        CoreUtils.SetRenderTarget(ctx.cmd, ctx.cameraColorBuffer);
        CoreUtils.DrawFullScreen(ctx.cmd, settings.material, tempRT, shaderPassId: 1);

        // Release temporary render texture
        RTHandles.Release(tempRT);
    }

    protected override void Cleanup()
    {
        // Cleanup code if needed
    }
}