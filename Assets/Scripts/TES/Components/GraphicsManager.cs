using Demonixis.Toolbox.XR;
using System;
using System.Collections;
using UnityEngine;
#if HDRP_ENABLED
using UnityEngine.Experimental.Rendering.HDPipeline;
#endif
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityStandardAssets.Water;

namespace TESUnity.Components
{
#if HDRP_ENABLED
    using Bloom = UnityEngine.Rendering.PostProcessing.Bloom;
    using MotionBlur = UnityEngine.Rendering.PostProcessing.MotionBlur;
    using Vignette = UnityEngine.Rendering.PostProcessing.Vignette;
#endif

    public sealed class GraphicsManager : MonoBehaviour
    {
        private void Awake()
        {
            var config = GameSettings.Get();
            var rendererMode = config.RendererMode;
            var target = config.SRPQuality.ToString();

#if UNITY_ANDROID
            rendererMode = RendererMode.UniversalRP
            target = "Mobile";
#endif

            if (rendererMode == RendererMode.UniversalRP)
            {
                var lwrpAsset = Resources.Load<UniversalRenderPipelineAsset>($"Rendering/UniversalRP/URP-{target}");
                lwrpAsset.renderScale = config.RenderScale;
                GraphicsSettings.renderPipelineAsset = lwrpAsset;
            }

#if HDRP_ENABLED
            if (rendererMode == RendererMode.HDRP)
            {
                GraphicsSettings.renderPipelineAsset = Resources.Load<HDRenderPipelineAsset>($"Rendering/HDRP/HDRPAsset-{target}");

                var volumeSettings = Resources.Load<GameObject>("Rendering/HDRP/HDRP-Volume");
                Instantiate(volumeSettings);
            }
#endif
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
