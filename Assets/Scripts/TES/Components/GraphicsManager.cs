using Demonixis.Toolbox.XR;
using System;
using System.Collections;
using UnityEngine;
#if HDRP_ENABLED
using UnityEngine.Experimental.Rendering.HDPipeline;
#endif
using UnityEngine.Rendering;
#if LWRP_ENABLED
using UnityEngine.Rendering.LWRP;
#endif
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
            var renderPath = config.RendererMode;

            // Setup SRP if enabled.
            var srpEnabled = config.IsSRP();
            if (!srpEnabled)
                GraphicsSettings.renderPipelineAsset = null;

#if LWRP_ENABLED || HDRP_ENABLED
            if (srpEnabled)
            {
                var rendererMode = config.RendererMode;
                var target = config.SRPQuality.ToString();

#if LWRP_ENABLED
                if (rendererMode == RendererMode.LightweightRP)
                {
#if UNITY_ANDROID
					target = "Mobile";
#endif
                    var lwrpAsset = Resources.Load<UnityEngine.Rendering.Universal.UniversalRenderPipelineAsset>($"Rendering/LWRP/LightweightAsset-{target}");
                    lwrpAsset.renderScale = config.RenderScale;
                    GraphicsSettings.renderPipelineAsset = lwrpAsset;
                }
#endif
#if HDRP_ENABLED
                if (rendererMode == RendererMode.HDRP)
                {
                    GraphicsSettings.renderPipelineAsset = Resources.Load<HDRenderPipelineAsset>($"Rendering/HDRP/HDRPAsset-{target}");

                    var volumeSettings = Resources.Load<GameObject>("Rendering/HDRP/HDRP-Volume");
                    Instantiate(volumeSettings);
                }
#endif
                // Only this mode is compatible with SRP.
                config.WaterQuality = Water.WaterMode.Simple;
            }
#endif
        }

        private void Start()
        {
            var config = GameSettings.Get();
            var renderPath = config.RendererMode;
            var camera = GetComponent<Camera>();

            // 1. Setup Camera
            if (renderPath == RendererMode.Forward)
            {
                camera.renderingPath = RenderingPath.Forward;
                camera.allowMSAA = true;
            }
            else if (renderPath == RendererMode.Deferred)
            {
                camera.renderingPath = RenderingPath.DeferredShading;
                camera.allowMSAA = false;
            }

            camera.farClipPlane = config.CameraFarClip;
            camera.allowHDR = !GameSettings.IsMobile();

            // 2. Setup Post Processing.
            UpdateEffects();
        }

        public void UpdateEffects()
        {
        }
    }
}
