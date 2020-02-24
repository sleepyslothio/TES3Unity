using UnityEngine;
#if HDRP_ENABLED
using UnityEngine.Rendering.HDPipeline;
#endif
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace TESUnity.Components
{
    public sealed class GraphicsManager : MonoBehaviour
    {
        private void Awake()
        {
#if UNITY_EDITOR
            // We need to call this component now because SRP settings is very early
            // And we want to be sure it's called before SRP settings.
            var settingsOverride = FindObjectOfType<GameSettingsOverride>();
            settingsOverride?.ApplyEditorSettingsOverride();
#endif
            var config = GameSettings.Get();
            var hdrp = config.RendererMode == RendererMode.HDRP;
            var target = config.SRPQuality.ToString();

#if UNITY_ANDROID
            rendererMode = RendererMode.UniversalRP
            target = "Mobile";
#endif
#if !HDRP_ENABLED
            hdrp = false; // Double check
#endif

            if (!hdrp)
            {
                var lwrpAsset = Resources.Load<UniversalRenderPipelineAsset>($"Rendering/UniversalRP/URP-{target}");
                lwrpAsset.renderScale = config.RenderScale;
                GraphicsSettings.renderPipelineAsset = lwrpAsset;

                // Instantiate URP Volume
            }
            else
            {
#if HDRP_ENABLED
                GraphicsSettings.renderPipelineAsset = Resources.Load<HDRenderPipelineAsset>($"Rendering/HDRP/HDRPAsset-{target}");
                // Instantiate HDRP Volume (Post Processing + Environment)
#endif
            }
        }

        private void Start()
        {
            var config = GameSettings.Get();
            var camera = Camera.main;

            // 1. Setup Camera
            camera.farClipPlane = config.CameraFarClip;

            var additionalData = camera.GetComponent<UniversalAdditionalCameraData>();
            additionalData.renderPostProcessing = config.PostProcessingQuality != PostProcessingQuality.None;

            switch (config.AntiAliasingMode)
            {
                case AntiAliasingMode.None:
                case AntiAliasingMode.MSAA:
                    additionalData.antialiasing = AntialiasingMode.None;
                    break;
                case AntiAliasingMode.FXAA:
                case AntiAliasingMode.TAA:
                    additionalData.antialiasing = AntialiasingMode.FastApproximateAntialiasing;
                    break;
                case AntiAliasingMode.SMAA:
                    additionalData.antialiasing = AntialiasingMode.SubpixelMorphologicalAntiAliasing;
                    break;
            }

            var urpAsset = GraphicsSettings.renderPipelineAsset as UniversalRenderPipelineAsset;
            if (urpAsset != null)
            {
                if (config.AntiAliasingMode != AntiAliasingMode.MSAA)
                {
                    urpAsset.msaaSampleCount = 0;
                }
            }
        }
    }
}
