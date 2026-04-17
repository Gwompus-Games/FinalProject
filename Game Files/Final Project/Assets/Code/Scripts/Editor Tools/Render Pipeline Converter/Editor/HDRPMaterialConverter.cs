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

    private enum ScopeMode
    {
        Selected,
        AllHDRPInProject
    }

    private TargetPipeline targetPipeline = TargetPipeline.URP;
    private ScopeMode scopeMode = ScopeMode.Selected;
    private bool logDetails = true;
    private bool preserveTransparency = true;
    private bool preserveEmission = true;
    private bool dryRun = false;

    private readonly List<Material> previewMaterials = new List<Material>();
    private Vector2 scrollPos;

    [MenuItem("Tools/Materials/Convert Selected HDRP Materials")]
    public static void ShowWindowForSelected()
    {
        var window = GetWindow<HDRPMaterialConverter>("HDRP Material Converter");
        window.minSize = new Vector2(460, 320);
        window.Initialize(ScopeMode.Selected);
    }

    [MenuItem("Tools/Materials/Convert All HDRP Materials")]
    public static void ShowWindowForAll()
    {
        var window = GetWindow<HDRPMaterialConverter>("HDRP Material Converter");
        window.minSize = new Vector2(460, 320);
        window.Initialize(ScopeMode.AllHDRPInProject);
    }

    private void Initialize(ScopeMode mode)
    {
        scopeMode = mode;
        RefreshPreviewList();
    }

    private void OnFocus()
    {
        RefreshPreviewList();
    }

    private void OnSelectionChange()
    {
        if (scopeMode == ScopeMode.Selected)
        {
            RefreshPreviewList();
            Repaint();
        }
    }

    private void OnGUI()
    {
        EditorGUILayout.LabelField("Convert HDRP Materials", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        EditorGUILayout.HelpBox(
            "Convert HDRP materials to Built-in Standard or URP Lit.\n\n" +
            "Use the menu item you opened this window with to choose whether to process the currently selected materials or every supported HDRP material in the project.",
            MessageType.Info);

        using (new EditorGUI.DisabledScope(true))
        {
            EditorGUILayout.EnumPopup("Scope", scopeMode);
        }

        targetPipeline = (TargetPipeline)EditorGUILayout.EnumPopup(new GUIContent("Target Pipeline", "Select the shader type to convert materials to (URP Lit or Built-in Standard)."), targetPipeline);
        preserveTransparency = EditorGUILayout.Toggle(new GUIContent("Preserve Transparency", "Attempts to keep materials transparent based on HDRP settings and alpha values."), preserveTransparency);
        preserveEmission = EditorGUILayout.Toggle(new GUIContent("Preserve Emission", "Preserves emission color and maps so glowing materials remain emissive."), preserveEmission);
        logDetails = EditorGUILayout.Toggle(new GUIContent("Verbose Log", "Outputs detailed conversion info to the Console for debugging."), logDetails);
        dryRun = EditorGUILayout.Toggle(new GUIContent("Dry Run Only", "Simulates conversion without modifying any materials."), dryRun);

        EditorGUILayout.HelpBox("Tip: Use 'Dry Run Only' before converting large batches to preview changes safely.", MessageType.None);

        EditorGUILayout.Space();

        const float actionButtonHeight = 24f;
        using (new EditorGUILayout.HorizontalScope())
        {
            if (GUILayout.Button("Refresh List", GUILayout.Height(actionButtonHeight)))
            {
                RefreshPreviewList();
            }

            using (new EditorGUI.DisabledScope(previewMaterials.Count == 0))
            {
                if (GUILayout.Button(scopeMode == ScopeMode.Selected ? "Convert Selected Materials" : "Convert All Listed Materials", GUILayout.Height(actionButtonHeight)))
                {
                    ConvertCurrentList();
                }
            }
        }

        EditorGUILayout.Space();
        EditorGUILayout.LabelField($"Materials To Convert ({previewMaterials.Count})", EditorStyles.boldLabel);

        if (previewMaterials.Count == 0)
        {
            EditorGUILayout.LabelField(scopeMode == ScopeMode.Selected
                ? "No supported HDRP materials found in the current selection."
                : "No supported HDRP materials found in the project.");
            return;
        }

        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
        foreach (var mat in previewMaterials)
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                EditorGUILayout.ObjectField(mat, typeof(Material), false);
                EditorGUILayout.LabelField(mat.shader != null ? mat.shader.name : "<no shader>");
            }
        }
        EditorGUILayout.EndScrollView();
    }

    private void RefreshPreviewList()
    {
        previewMaterials.Clear();

        if (scopeMode == ScopeMode.Selected)
        {
            foreach (var mat in GetSelectedMaterials())
            {
                if (mat != null && LooksLikeHDRPMaterial(mat) && CanConvertMaterial(mat))
                    previewMaterials.Add(mat);
            }
        }
        else
        {
            foreach (var mat in GetAllConvertibleHDRPMaterialsInProject())
            {
                if (mat != null)
                    previewMaterials.Add(mat);
            }
        }
    }

    private void ConvertCurrentList()
    {
        if (previewMaterials.Count == 0)
        {
            EditorUtility.DisplayDialog("No Materials Found", "There are no supported HDRP materials to convert.", "OK");
            return;
        }

        int converted = 0;
        int skipped = 0;

        Undo.RecordObjects(previewMaterials.ToArray(), "Convert HDRP Materials");

        foreach (var mat in previewMaterials)
        {
            if (mat == null)
            {
                skipped++;
                continue;
            }

            if (!LooksLikeHDRPMaterial(mat) || !CanConvertMaterial(mat))
            {
                Log($"Skipping '{mat.name}' because it no longer appears to be a supported HDRP material.");
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
        RefreshPreviewList();

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
                result.Add(mat);
        }

        return result;
    }

    private List<Material> GetAllConvertibleHDRPMaterialsInProject()
    {
        var result = new List<Material>();
        string[] guids = AssetDatabase.FindAssets("t:Material", new[] { "Assets" });

        foreach (var guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            if (!path.StartsWith("Assets/"))
                continue;

            var mat = AssetDatabase.LoadAssetAtPath<Material>(path);
            if (mat == null)
                continue;

            if (!LooksLikeHDRPMaterial(mat))
                continue;

            if (!CanConvertMaterial(mat))
                continue;

            result.Add(mat);
        }

        result.Sort((a, b) => string.CompareOrdinal(a.name, b.name));
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
        return shaderName.StartsWith("HDRP/");
    }

    private bool CanConvertMaterial(Material mat)
    {
        if (mat == null || mat.shader == null)
            return false;

        // Intentionally limited to common Lit-style HDRP materials this tool can map reasonably well.
        string shaderName = mat.shader.name;

        if (shaderName.Contains("Unlit"))
            return false;

        return true;
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

        if (mat.HasProperty("_BaseColor"))
            data.baseColor = mat.GetColor("_BaseColor");
        else if (mat.HasProperty("_Color"))
            data.baseColor = mat.GetColor("_Color");

        data.alpha = data.baseColor.a;

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

        if (mat.HasProperty("_NormalMap"))
            data.normalMap = mat.GetTexture("_NormalMap");
        else if (mat.HasProperty("_BumpMap"))
            data.normalMap = mat.GetTexture("_BumpMap");

        if (mat.HasProperty("_NormalScale"))
            data.normalScale = mat.GetFloat("_NormalScale");
        else if (mat.HasProperty("_BumpScale"))
            data.normalScale = mat.GetFloat("_BumpScale");

        if (mat.HasProperty("_Metallic"))
            data.metallic = mat.GetFloat("_Metallic");

        if (mat.HasProperty("_MetallicGlossMap"))
            data.metallicMap = mat.GetTexture("_MetallicGlossMap");
        else if (mat.HasProperty("_MetallicMap"))
            data.metallicMap = mat.GetTexture("_MetallicMap");
        else if (mat.HasProperty("_MaskMap"))
            data.metallicMap = mat.GetTexture("_MaskMap");

        if (mat.HasProperty("_Smoothness"))
            data.smoothness = mat.GetFloat("_Smoothness");

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

        if (mat.HasProperty("_OcclusionMap"))
            data.occlusionMap = mat.GetTexture("_OcclusionMap");
        else if (mat.HasProperty("_MaskMap"))
            data.occlusionMap = mat.GetTexture("_MaskMap");

        if (mat.HasProperty("_OcclusionStrength"))
            data.occlusionStrength = mat.GetFloat("_OcclusionStrength");

        data.transparent = InferTransparency(mat, data);

        if (mat.HasProperty("_DetailMask"))
            data.detailMask = mat.GetTexture("_DetailMask");

        return data;
    }

    private bool InferTransparency(Material mat, MaterialData data)
    {
        if (!preserveTransparency)
            return false;

        if (mat.HasProperty("_SurfaceType"))
        {
            float surfaceType = mat.GetFloat("_SurfaceType");
            if (surfaceType > 0.5f)
                return true;
        }

        if (data.alpha < 0.999f)
            return true;

        string renderTypeTag = mat.GetTag("RenderType", false, "");
        if (!string.IsNullOrEmpty(renderTypeTag) && renderTypeTag.ToLower().Contains("transparent"))
            return true;

        return false;
    }

    private void ApplyToURP(Material mat, MaterialData data)
    {
        SetColorIfExists(mat, "_BaseColor", data.baseColor);
        SetTextureIfExists(mat, "_BaseMap", data.baseMap);
        SetTexSTIfExists(mat, "_BaseMap", data.baseMapScale, data.baseMapOffset);

        SetTextureIfExists(mat, "_BumpMap", data.normalMap);
        SetFloatIfExists(mat, "_BumpScale", data.normalScale);
        if (data.normalMap != null)
            mat.EnableKeyword("_NORMALMAP");

        SetFloatIfExists(mat, "_Metallic", data.metallic);
        SetFloatIfExists(mat, "_Smoothness", data.smoothness);
        SetTextureIfExists(mat, "_MetallicGlossMap", data.metallicMap);
        if (data.metallicMap != null)
            mat.EnableKeyword("_METALLICSPECGLOSSMAP");

        SetTextureIfExists(mat, "_OcclusionMap", data.occlusionMap);
        SetFloatIfExists(mat, "_OcclusionStrength", data.occlusionStrength);
        if (data.occlusionMap != null)
            mat.EnableKeyword("_OCCLUSIONMAP");

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

        if (data.transparent)
            ConfigureURPTransparent(mat);
        else
            ConfigureURPOpaque(mat);
    }

    private void ApplyToBuiltIn(Material mat, MaterialData data)
    {
        SetColorIfExists(mat, "_Color", data.baseColor);
        SetTextureIfExists(mat, "_MainTex", data.baseMap);
        SetTexSTIfExists(mat, "_MainTex", data.baseMapScale, data.baseMapOffset);

        SetTextureIfExists(mat, "_BumpMap", data.normalMap);
        SetFloatIfExists(mat, "_BumpScale", data.normalScale);
        if (data.normalMap != null)
            mat.EnableKeyword("_NORMALMAP");

        SetFloatIfExists(mat, "_Metallic", data.metallic);
        SetFloatIfExists(mat, "_Glossiness", data.smoothness);
        SetTextureIfExists(mat, "_MetallicGlossMap", data.metallicMap);
        if (data.metallicMap != null)
            mat.EnableKeyword("_METALLICGLOSSMAP");

        SetTextureIfExists(mat, "_OcclusionMap", data.occlusionMap);
        SetFloatIfExists(mat, "_OcclusionStrength", data.occlusionStrength);

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

        if (data.transparent)
            ConfigureStandardTransparent(mat);
        else
            ConfigureStandardOpaque(mat);
    }

    private void ConfigureURPTransparent(Material mat)
    {
        SetFloatIfExists(mat, "_Surface", 1f);
        SetFloatIfExists(mat, "_Blend", 0f);
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
        SetFloatIfExists(mat, "_Surface", 0f);
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
        if (mat.HasProperty("_Mode"))
            mat.SetFloat("_Mode", 3f);

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
            mat.SetFloat("_Mode", 0f);

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
