using UnityEngine;
using UnityEngine.Rendering;
using OccaSoftware.Buto.Runtime;

namespace OccaSoftware.Buto.Demo
{
    [AddComponentMenu("Buto/Demo/Animate Buto Settings")]
    public class AnimateButoSetting : MonoBehaviour
    {
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
#if UNITY_2023_1_OR_NEWER
                Volume[] volumes = FindObjectsByType<Volume>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
#else
                Volume[] volumes = FindObjectsOfType<Volume>();
#endif

                foreach (Volume volume in volumes)
                {
                    if (volume.profile != null && volume.profile.TryGet(out ButoVolumetricFog volumetricFog))
                    {
                        volumetricFog.baseHeight.overrideState = true;
                        volumetricFog.baseHeight.value = volumetricFog.baseHeight.value + 1;
                        Debug.Log(volumetricFog.baseHeight.value);
                    }
                }
            }
        }
    }
}
