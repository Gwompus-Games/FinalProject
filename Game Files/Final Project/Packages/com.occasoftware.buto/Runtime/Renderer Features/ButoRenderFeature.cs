using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace OccaSoftware.Buto.Runtime
{
    public class ButoRenderFeature : ScriptableRendererFeature
    {
        [System.Serializable]
        public class Settings
        {
            public RenderPassEvent renderPassEvent = RenderPassEvent.BeforeRenderingTransparents;
        }

        public Settings settings = new Settings();

        RenderFogPass renderFogPass;

        private Camera camera;

        public override void Create()
        {
            renderFogPass = new RenderFogPass();
            renderFogPass.renderPassEvent = settings.renderPassEvent;
        }

        private void OnDisable()
        {
            Shader.DisableKeyword("Buto");
            Shader.SetGlobalFloat(Params.ButoIsEnabled.Id, 0f);
        }

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            renderFogPass.RegisterStackComponent();
            renderFogPass.SetupMaterials(renderingData.cameraData.camera);

            Shader.SetGlobalFloat(Params.ButoIsEnabled.Id, 0f);
            Shader.DisableKeyword("Buto");
            if (renderingData.cameraData.camera.cameraType == CameraType.Reflection)
                return;

#if UNITY_EDITOR 
            bool isSceneCamera = renderingData.cameraData.camera.cameraType == CameraType.SceneView;

            if (isSceneCamera && UnityEditor.SceneView.currentDrawingSceneView != null && UnityEditor.SceneView.currentDrawingSceneView.sceneViewState != null)
            {
                bool fogEnabled = UnityEditor.SceneView.currentDrawingSceneView.sceneViewState.fogEnabled;

                bool isDrawingTextured = UnityEditor.SceneView.currentDrawingSceneView.cameraMode.drawMode == UnityEditor.DrawCameraMode.Textured;
                if (!fogEnabled || !isDrawingTextured)
                    return;
            }

            if (renderingData.cameraData.camera.cameraType == CameraType.Preview)
                return;
#endif

            if (!renderingData.cameraData.postProcessEnabled)
                return;


            if (!renderFogPass.RegisterStackComponent())
                return;

            if (renderingData.cameraData.camera.TryGetComponent<DisableButoRendering>(out _))
                return;

            renderer.EnqueuePass(renderFogPass);
            Shader.SetGlobalFloat(Params.ButoIsEnabled.Id, 1f);
            Shader.EnableKeyword("Buto");
        }

#if !UNITY_6000_4_OR_NEWER
        public override void SetupRenderPasses(ScriptableRenderer renderer, in RenderingData renderingData)
        {
            renderFogPass.ConfigureInput(ScriptableRenderPassInput.Color | ScriptableRenderPassInput.Depth | ScriptableRenderPassInput.Motion);
        }
#endif

        protected override void Dispose(bool disposing)
        {
            OnDisable();
            renderFogPass?.Dispose();
            renderFogPass = null;
            base.Dispose(disposing);
        }
    }
}
