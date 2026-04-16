using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace OccaSoftware.Buto.Runtime
{
    public class RenderingUtilsHelper : MonoBehaviour
    {
        public static bool ReAllocateIfNeeded(
               ref RTHandle handle,
               in RenderTextureDescriptor descriptor,
               FilterMode filterMode = FilterMode.Point,
               TextureWrapMode wrapMode = TextureWrapMode.Repeat,
               bool isShadowMap = false,
               int anisoLevel = 1,
               float mipMapBias = 0,
               string name = "")
        {
#if UNITY_2023_3_OR_NEWER
            return RenderingUtils.ReAllocateHandleIfNeeded(ref handle, descriptor, filterMode, wrapMode, anisoLevel, mipMapBias, name);
#else
            return RenderingUtils.ReAllocateIfNeeded(ref handle, descriptor, filterMode, wrapMode, isShadowMap, anisoLevel, mipMapBias, name);
#endif

        }
    }
}
