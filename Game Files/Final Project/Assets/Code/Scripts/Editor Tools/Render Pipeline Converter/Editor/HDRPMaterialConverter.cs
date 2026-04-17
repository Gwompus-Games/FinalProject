using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class HDRPMaterialConverter : EditorWindow
{
    private enum TargetPipeline
    {
        BuiltIn,
        URP
    }

    private TargetPipeline targetPipeline = TargetPipeline.URP;
    private bool logDetails = true;
    private bool preserveTransparency = true;
    private bool preserveEmission = true;
    private bool dryRun = false;

    [MenuItem("Tools/Materials/Convert Selected HDRP Materials")]
    public static void ShowWindow()
    {
        var window = GetWindow<HDRPMaterialConverter>("HDRP Material Converter");
        window.minSize = new Vector2(420, 240);
    }

    private void OnGUI()
    {
        EditorGUILayout.LabelField("Convert Selected HDRP Materials", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        EditorGUILayout.HelpBox(
            "Select one or more Material assets in the Project window, then convert them to Built-in Standard or URP Lit.\n\n" +
            "This tool preserves common data where possible: color, albedo, normals, metallic/smoothness, emission, tiling/offset, and basic transparency.",
            MessageType.Info);

        targetPipeline = (TargetPipeline)EditorGUILayout.EnumPopup("Target Pipeline", targetPipeline);
        preserveTransparency = EditorGUILayout.Toggle("Preserve Transparency", preserveTransparency);
        preserveEmission = EditorGUILayout.Toggle("Preserve Emission", preserveEmission);
        logDetails = EditorGUILayout.Toggle("Verbose Log", logDetails);
        dryRun = EditorGUILayout.Toggle("Dry Run Only", dryRun);

        EditorGUILayout.Space();

        using (new EditorGUI.DisabledScope(Selection.objects == null || Selection.objects.Length == 0))
        {
            if (GUILayout.Button("Convert Selected Materials", GUILayout.Height(32)))
            {
                ConvertSelection();
            }
        }

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Selected Materials", EditorStyles.boldLabel);

        var materials = GetSelectedMaterials();
        if (materials.Count == 0)
        {
            EditorGUILayout.LabelField("No material assets selected.");
        }
        else
        {
            foreach (var mat in materials)
            {
                EditorGUILayout.LabelField("• " + mat.name);
            }
        }
    }

    private void ConvertSelection()
    {
        var materials = GetSelectedMaterials();

        if (materials.Count == 0)
        {
            EditorUtility.DisplayDialog("No Materials Selected", "Please select one or more material assets in the Project window.", "OK");
            return;
        }

        int converted = 0;
        int skipped = 0;

        Undo.RecordObjects(materials.ToArray(), "Convert HDRP Materials");

        foreach (var mat in materials)
        {
            if (mat == null)
            {
                skipped++;
                continue;
            }

            if (!LooksLikeHDRPMaterial(mat))
            {
                Log($"Skipping '{mat.name}' because it does not appear to use a supported HDRP shader.");
                skipped++;
                continue;
            }

            if (dryRun)
            {
                Log($"[Dry Run] Would convert '{mat.name}' from '{mat.shader.name}' to '{GetTargetShaderName()}'.");
                converted++;
                continue;
            }

            if (ConvertMaterial(mat, targetPipeline))
            {
                EditorUtility.SetDirty(mat);
                converted++;
            }
            else
            {
                skipped++;
            }
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        EditorUtility.DisplayDialog(
            "Material Conversion Complete",
            $"Converted: {converted}\nSkipped: {skipped}\nTarget: {GetTargetShaderName()}",
            "OK");
    }

    private List<Material> GetSelectedMaterials()
    {
        var result = new List<Material>();

        foreach (var obj in Selection.objects)
        {
            if (obj is Material mat)
            {
                result.Add(mat);
            }
        }

        return result;
    }

    private string GetTargetShaderName()
    {
        return targetPipeline == TargetPipeline.URP
            ? "Universal Render Pipeline/Lit"
            : "Standard";
    }

    private bool LooksLikeHDRPMaterial(Material mat)
    {
        if (mat == null || mat.shader == null)
            return false;

        string shaderName = mat.shader.name;

        return shaderName.StartsWith("HDRP/")
               || shaderName.Contains("High Definition Render Pipeline")
               || shaderName.Contains("HDRenderPipeline")
               || shaderName.Contains("Lit")
               || mat.HasProperty("_BaseColorMap")
               || mat.HasProperty("_BaseColor");
    }

    private bool ConvertMaterial(Material sourceMat, TargetPipeline target)
    {
        Shader targetShader = Shader.Find(target == TargetPipeline.URP
            ? "Universal Render Pipeline/Lit"
            : "Standard");

        if (targetShader == null)
        {
            Debug.LogError($"Target shader not found: {GetTargetShaderName()}");
            return false;
        }

        // Capture HDRP values before swapping shader.
        var data = ExtractSourceData(sourceMat);

        string oldShader = sourceMat.shader != null ? sourceMat.shader.name : "<null>";

        sourceMat.shader = targetShader;

        if (target == TargetPipeline.URP)
            ApplyToURP(sourceMat, data);
        else
            ApplyToBuiltIn(sourceMat, data);

        Log($"Converted '{sourceMat.name}' from '{oldShader}' to '{sourceMat.shader.name}'");
        return true;
    }

    private class MaterialData
    {
        public Color baseColor = Color.white;
        public Texture baseMap;
        public Vector2 baseMapScale = Vector2.one;
        public Vector2 baseMapOffset = Vector2.zero;

        public Texture normalMap;
        public float normalScale = 1f;

        public Texture emissionMap;
        public Color emissionColor = Color.black;
        public bool emissionEnabled;

        public Texture metallicMap;
        public float metallic = 0f;
        public float smoothness = 0.5f;

        public float alpha = 1f;
        public bool transparent;

        public Texture occlusionMap;
        public float occlusionStrength = 1f;

        public Texture detailMask;
    }

    private MaterialData ExtractSourceData(Material mat)
    {
        var data = new MaterialData();

        // Base color
        if (mat.HasProperty("_BaseColor"))
            data.baseColor = mat.GetColor("_BaseColor");
        else if (mat.HasProperty("_Color"))
            data.baseColor = mat.GetColor("_Color");

        data.alpha = data.baseColor.a;

        // Base texture
        if (mat.HasProperty("_BaseColorMap"))
        {
            data.baseMap = mat.GetTexture("_BaseColorMap");
            data.baseMapScale = mat.GetTextureScale("_BaseColorMap");
            data.baseMapOffset = mat.GetTextureOffset("_BaseColorMap");
        }
        else if (mat.HasProperty("_BaseMap"))
        {
            data.baseMap = mat.GetTexture("_BaseMap");
            data.baseMapScale = mat.GetTextureScale("_BaseMap");
            data.baseMapOffset = mat.GetTextureOffset("_BaseMap");
        }
        else if (mat.HasProperty("_MainTex"))
        {
            data.baseMap = mat.GetTexture("_MainTex");
            data.baseMapScale = mat.GetTextureScale("_MainTex");
            data.baseMapOffset = mat.GetTextureOffset("_MainTex");
        }

        // Normal
        if (mat.HasProperty("_NormalMap"))
            data.normalMap = mat.GetTexture("_NormalMap");
        else if (mat.HasProperty("_BumpMap"))
            data.normalMap = mat.GetTexture("_BumpMap");

        if (mat.HasProperty("_NormalScale"))
            data.normalScale = mat.GetFloat("_NormalScale");
        else if (mat.HasProperty("_BumpScale"))
            data.normalScale = mat.GetFloat("_BumpScale");

        // Metallic / smoothness
        if (mat.HasProperty("_Metallic"))
            data.metallic = mat.GetFloat("_Metallic");

        if (mat.HasProperty("_MetallicGlossMap"))
            data.metallicMap = mat.GetTexture("_MetallicGlossMap");
        else if (mat.HasProperty("_MetallicMap"))
            data.metallicMap = mat.GetTexture("_MetallicMap");
        else if (mat.HasProperty("_MaskMap"))
            data.metallicMap = mat.GetTexture("_MaskMap"); // approximation only

        if (mat.HasProperty("_Smoothness"))
            data.smoothness = mat.GetFloat("_Smoothness");

        // Emission
        if (mat.HasProperty("_EmissionColor"))
        {
            data.emissionColor = mat.GetColor("_EmissionColor");
            data.emissionEnabled = data.emissionColor.maxColorComponent > 0.0001f;
        }

        if (mat.HasProperty("_EmissionMap"))
        {
            data.emissionMap = mat.GetTexture("_EmissionMap");
            if (data.emissionMap != null)
                data.emissionEnabled = true;
        }

        // Occlusion (best effort)
        if (mat.HasProperty("_OcclusionMap"))
            data.occlusionMap = mat.GetTexture("_OcclusionMap");
        else if (mat.HasProperty("_MaskMap"))
            data.occlusionMap = mat.GetTexture("_MaskMap"); // approximation only

        if (mat.HasProperty("_OcclusionStrength"))
            data.occlusionStrength = mat.GetFloat("_OcclusionStrength");

        // Transparency inference
        data.transparent = InferTransparency(mat, data);

        // Detail mask if present
        if (mat.HasProperty("_DetailMask"))
            data.detailMask = mat.GetTexture("_DetailMask");

        return data;
    }

    private bool InferTransparency(Material mat, MaterialData data)
    {
        if (!preserveTransparency)
            return false;

        // HDRP usually has _SurfaceType: 0 = Opaque, 1 = Transparent
        if (mat.HasProperty("_SurfaceType"))
        {
            float surfaceType = mat.GetFloat("_SurfaceType");
            if (surfaceType > 0.5f)
                return true;
        }

        // Fallback: alpha below 1 suggests transparency
        if (data.alpha < 0.999f)
            return true;

        string renderTypeTag = mat.GetTag("RenderType", false, "");
        if (!string.IsNullOrEmpty(renderTypeTag) && renderTypeTag.ToLower().Contains("transparent"))
            return true;

        return false;
    }

    private void ApplyToURP(Material mat, MaterialData data)
    {
        // Base color + map
        SetColorIfExists(mat, "_BaseColor", data.baseColor);
        SetTextureIfExists(mat, "_BaseMap", data.baseMap);
        SetTexSTIfExists(mat, "_BaseMap", data.baseMapScale, data.baseMapOffset);

        // Normal
        SetTextureIfExists(mat, "_BumpMap", data.normalMap);
        SetFloatIfExists(mat, "_BumpScale", data.normalScale);
        if (data.normalMap != null)
            mat.EnableKeyword("_NORMALMAP");

        // Metallic / smoothness
        SetFloatIfExists(mat, "_Metallic", data.metallic);
        SetFloatIfExists(mat, "_Smoothness", data.smoothness);
        SetTextureIfExists(mat, "_MetallicGlossMap", data.metallicMap);
        if (data.metallicMap != null)
            mat.EnableKeyword("_METALLICSPECGLOSSMAP");

        // Occlusion
        SetTextureIfExists(mat, "_OcclusionMap", data.occlusionMap);
        SetFloatIfExists(mat, "_OcclusionStrength", data.occlusionStrength);
        if (data.occlusionMap != null)
            mat.EnableKeyword("_OCCLUSIONMAP");

        // Emission
        if (preserveEmission)
        {
            SetColorIfExists(mat, "_EmissionColor", data.emissionColor);
            SetTextureIfExists(mat, "_EmissionMap", data.emissionMap);

            if (data.emissionEnabled || data.emissionMap != null)
            {
                mat.EnableKeyword("_EMISSION");
                MaterialGlobalIlluminationFlags flags = mat.globalIlluminationFlags;
                flags &= ~MaterialGlobalIlluminationFlags.EmissiveIsBlack;
                mat.globalIlluminationFlags = flags;
            }
        }

        // Transparency
        if (data.transparent)
            ConfigureURPTransparent(mat);
        else
            ConfigureURPOpaque(mat);
    }

    private void ApplyToBuiltIn(Material mat, MaterialData data)
    {
        // Base color + map
        SetColorIfExists(mat, "_Color", data.baseColor);
        SetTextureIfExists(mat, "_MainTex", data.baseMap);
        SetTexSTIfExists(mat, "_MainTex", data.baseMapScale, data.baseMapOffset);

        // Normal
        SetTextureIfExists(mat, "_BumpMap", data.normalMap);
        SetFloatIfExists(mat, "_BumpScale", data.normalScale);
        if (data.normalMap != null)
            mat.EnableKeyword("_NORMALMAP");

        // Metallic / smoothness
        SetFloatIfExists(mat, "_Metallic", data.metallic);
        SetFloatIfExists(mat, "_Glossiness", data.smoothness);
        SetTextureIfExists(mat, "_MetallicGlossMap", data.metallicMap);
        if (data.metallicMap != null)
            mat.EnableKeyword("_METALLICGLOSSMAP");

        // Occlusion
        SetTextureIfExists(mat, "_OcclusionMap", data.occlusionMap);
        SetFloatIfExists(mat, "_OcclusionStrength", data.occlusionStrength);

        // Emission
        if (preserveEmission)
        {
            SetColorIfExists(mat, "_EmissionColor", data.emissionColor);
            SetTextureIfExists(mat, "_EmissionMap", data.emissionMap);

            if (data.emissionEnabled || data.emissionMap != null)
            {
                mat.EnableKeyword("_EMISSION");
                MaterialGlobalIlluminationFlags flags = mat.globalIlluminationFlags;
                flags &= ~MaterialGlobalIlluminationFlags.EmissiveIsBlack;
                mat.globalIlluminationFlags = flags;
            }
        }

        // Transparency
        if (data.transparent)
            ConfigureStandardTransparent(mat);
        else
            ConfigureStandardOpaque(mat);
    }

    private void ConfigureURPTransparent(Material mat)
    {
        // These values match URP Lit conventions closely enough for editor conversion.
        SetFloatIfExists(mat, "_Surface", 1f); // Transparent
        SetFloatIfExists(mat, "_Blend", 0f);   // Alpha
        SetFloatIfExists(mat, "_AlphaClip", 0f);

        mat.SetOverrideTag("RenderType", "Transparent");
        mat.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;

        mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        mat.SetInt("_ZWrite", 0);

        mat.DisableKeyword("_ALPHATEST_ON");
        mat.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
    }

    private void ConfigureURPOpaque(Material mat)
    {
        SetFloatIfExists(mat, "_Surface", 0f); // Opaque
        SetFloatIfExists(mat, "_AlphaClip", 0f);

        mat.SetOverrideTag("RenderType", "Opaque");
        mat.renderQueue = -1;

        mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
        mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
        mat.SetInt("_ZWrite", 1);

        mat.DisableKeyword("_SURFACE_TYPE_TRANSPARENT");
        mat.DisableKeyword("_ALPHATEST_ON");
    }

    private void ConfigureStandardTransparent(Material mat)
    {
        // Built-in Standard transparent mode.
        if (mat.HasProperty("_Mode"))
            mat.SetFloat("_Mode", 3f); // Transparent

        mat.SetOverrideTag("RenderType", "Transparent");
        mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        mat.SetInt("_ZWrite", 0);

        mat.DisableKeyword("_ALPHATEST_ON");
        mat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
        mat.EnableKeyword("_ALPHABLEND_ON");

        mat.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;
    }

    private void ConfigureStandardOpaque(Material mat)
    {
        if (mat.HasProperty("_Mode"))
            mat.SetFloat("_Mode", 0f); // Opaque

        mat.SetOverrideTag("RenderType", "");
        mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
        mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
        mat.SetInt("_ZWrite", 1);

        mat.DisableKeyword("_ALPHATEST_ON");
        mat.DisableKeyword("_ALPHABLEND_ON");
        mat.DisableKeyword("_ALPHAPREMULTIPLY_ON");

        mat.renderQueue = -1;
    }

    private void SetColorIfExists(Material mat, string propertyName, Color value)
    {
        if (mat.HasProperty(propertyName))
            mat.SetColor(propertyName, value);
    }

    private void SetFloatIfExists(Material mat, string propertyName, float value)
    {
        if (mat.HasProperty(propertyName))
            mat.SetFloat(propertyName, value);
    }

    private void SetTextureIfExists(Material mat, string propertyName, Texture value)
    {
        if (mat.HasProperty(propertyName))
            mat.SetTexture(propertyName, value);
    }

    private void SetTexSTIfExists(Material mat, string propertyName, Vector2 scale, Vector2 offset)
    {
        if (mat.HasProperty(propertyName))
        {
            mat.SetTextureScale(propertyName, scale);
            mat.SetTextureOffset(propertyName, offset);
        }
    }

    private void Log(string message)
    {
        if (logDetails)
            Debug.Log("[HDRP Material Converter] " + message);
    }
}