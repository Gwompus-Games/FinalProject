using UnityEngine;

namespace OccaSoftware.Buto.Runtime
{
    public static class Params
    {
        public readonly struct Param
        {
            public Param(string property)
            {
                Property = property;
                Id = Shader.PropertyToID(property);
            }

            readonly public string Property;
            readonly public int Id;
        }

        public static Param MediaData = new Param("MediaData");
        public static Param MediaDataPrevious = new Param("MediaDataPrevious");
        public static Param LightingDataTex = new Param("LightingDataTex");
        public static Param LightingDataTexPrevious = new Param("LightingDataTexPrevious");
        public static Param IntegratorData = new Param("IntegratorData");
        public static Param _ScreenTexture = new Param("_ScreenTexture");

        public static Param ButoIsEnabled = new Param("_ButoIsEnabled");
        public static Param FrameId = new Param("_FrameId");

        public static Param MaxDistanceVolumetric = new Param("_MaxDistanceVolumetric");
        public static Param MaxDistanceNonVolumetric = new Param("_MaxDistanceNonVolumetric");
        public static Param FogMaskBlendMode = new Param("_FogMaskBlendMode");

        // TAA Param
        public static Param TemporalAaIntegrationRate = new Param("_IntegrationRate");

        public static Param FogDensity = new Param("_FogDensity");
        public static Param Anisotropy = new Param("_Anisotropy");
        public static Param LightIntensity = new Param("_LightIntensity");
        public static Param DensityInShadow = new Param("_DensityInShadow");
        public static Param DensityInLight = new Param("_DensityInLight");

        public static Param BaseHeight = new Param("_BaseHeight");
        public static Param AttenuationBoundarySize = new Param("_AttenuationBoundarySize");

        public static Param ColorRamp = new Param("_ColorRamp");
        public static Param SimpleColor = new Param("_SimpleColor");
        public static Param ColorInfluence = new Param("_ColorInfluence");

        public static Param NoiseTexture = new Param("_NoiseTexture");
        public static Param Octaves = new Param("_Octaves");
        public static Param Lacunarity = new Param("_Lacunarity");
        public static Param Gain = new Param("_Gain");
        public static Param NoiseTiling = new Param("_NoiseTiling");
        public static Param NoiseWindSpeed = new Param("_NoiseWindSpeed");
        public static Param NoiseIntensityMin = new Param("_NoiseIntensityMin");
        public static Param NoiseIntensityMax = new Param("_NoiseIntensityMax");

        public static Param LightCountButo = new Param("_LightCountButo");
        public static Param LightPosButo = new Param("_LightPosButo");
        public static Param LightIntensityButo = new Param("_LightIntensityButo");
        public static Param LightColorButo = new Param("_LightColorButo");
        public static Param LightDirectionButo = new Param("_LightDirectionButo");
        public static Param LightAngleButo = new Param("_LightAngleButo");

        // Volume Data
        public static Param VolumeCountButo = new Param("_VolumeCountButo");
        public static Param VolumePosition = new Param("_VolumePosition");
        public static Param VolumeSize = new Param("_VolumeSize");
        public static Param VolumeShape = new Param("_VolumeShape");
        public static Param VolumeIntensity = new Param("_VolumeIntensityButo");
        public static Param VolumeBlendMode = new Param("_VolumeBlendMode");
        public static Param VolumeBlendDistance = new Param("_VolumeBlendDistance");

        public static Param DirectionalLightingForward = new Param("_DirectionalLightingForward");
        public static Param DirectionalLightingBack = new Param("_DirectionalLightingBack");
        public static Param DirectionalLightingRatio = new Param("_DirectionalLightingRatio");

        // Ambient Lighting
        public static Param WorldColor = new Param("_WorldColor");

        public static Param fog_volume_size = new Param("fog_volume_size");
        public static Param fog_cell_size = new Param("fog_cell_size");
        public static Param fogNearSize = new Param("fogNearSize");
        public static Param fogFarSize = new Param("fogFarSize");
        public static Param cameraFarPlane = new Param("cameraFarPlane");
        public static Param _depth_ratio = new Param("_depth_ratio");
        public static Param toPreviousView = new Param("toPreviousView");
        public static Param os_CameraInvProjection = new Param("os_CameraInvProjection");
        public static Param os_CameraToWorld = new Param("os_CameraToWorld");
        public static Param os_WorldToCamera = new Param("os_WorldToCamera");
        public static Param os_WorldSpaceCameraPosition = new Param("os_WorldSpaceCameraPosition");

        public static Param _InverseNoiseScale = new Param("_InverseNoiseScale");
        public static Param _Inverse_AttenuationBoundarySize = new Param(
          "_Inverse_AttenuationBoundarySize"
        );
    }
}
