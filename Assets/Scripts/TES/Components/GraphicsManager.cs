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
using UnityEngine.Rendering.PostProcessing;
using UnityStandardAssets.Water;

namespace TESUnity.Components
{
#if HDRP_ENABLED
    using Bloom = UnityEngine.Rendering.PostProcessing.Bloom;
    using MotionBlur = UnityEngine.Rendering.PostProcessing.MotionBlur;
    using Vignette = UnityEngine.Rendering.PostProcessing.Vignette;
#endif

    [RequireComponent(typeof(PostProcessLayer), typeof(Camera))]
    public sealed class GraphicsManager : MonoBehaviour
    {
        private void Awake()
        {
            var config = GameSettings.Get();
            var renderPath = config.RendererMode;

#if !HDRP_ENABLED && !LWRP_ENABLED
            if (renderPath == RendererMode.LightweightRP)
                renderPath = RendererMode.Forward;
            else if (renderPath == RendererMode.HDRP)
                renderPath = RendererMode.Deferred;
#endif

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
                    var lwrpAsset = Resources.Load<LightweightRenderPipelineAsset>($"Rendering/LWRP/LightweightAsset-{target}");
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
            var settings = TESManager.instance;
            var layer = GetComponent<PostProcessLayer>();
            var volume = FindObjectOfType<PostProcessVolume>();
            var profile = volume.profile;
            var xrEnabled = XRManager.IsXREnabled();
            var config = GameSettings.Get();
            var quality = config.PostProcessingQuality;
            var antiAliasing = config.AntiAliasingMode;

            // The post process stack is broken with the new XR Stack
            if (XRManager.IsXREnabled())
            {
                layer.enabled = false;
                return;
            }

            if (quality != PostProcessingQuality.None)
            {
                var asset = Resources.Load<PostProcessProfile>($"Rendering/Effects/PostProcessVolume-{quality}");
                volume.profile = asset;

                if (xrEnabled)
                {
                    UpdateEffect<Bloom>(profile, (bloom) =>
                    {
                        bloom.dirtTexture.value = null;
                        bloom.dirtIntensity.value = 0;
                        bloom.fastMode.value = true;
                    });

                    DisableEffect<ScreenSpaceReflections>(profile);
                    DisableEffect<Vignette>(profile);
                    DisableEffect<MotionBlur>(profile);
                }
            }
            else
            {
                volume.enabled = false;
                layer.enabled = false;
            }

            if (antiAliasing == AntiAliasingMode.None)
                SetMSAA(layer, 0);
            else if (antiAliasing == AntiAliasingMode.MSAA2X)
                SetMSAA(layer, 2);
            else if (antiAliasing == AntiAliasingMode.MSAA4X)
                SetMSAA(layer, 4);
            else if (antiAliasing == AntiAliasingMode.MSAA8X)
                SetMSAA(layer, 8);
            else if (antiAliasing == AntiAliasingMode.SMAA)
                SetPostProcessAA(layer, PostProcessLayer.Antialiasing.SubpixelMorphologicalAntialiasing);
            else if (antiAliasing == AntiAliasingMode.FXAA)
                SetPostProcessAA(layer, PostProcessLayer.Antialiasing.FastApproximateAntialiasing);
            else if (antiAliasing == AntiAliasingMode.TAA)
                SetPostProcessAA(layer, PostProcessLayer.Antialiasing.TemporalAntialiasing);
        }

        private void SetMSAA(PostProcessLayer layer, int level)
        {
            layer.antialiasingMode = PostProcessLayer.Antialiasing.None;
            QualitySettings.antiAliasing = level;
        }

        private void SetPostProcessAA(PostProcessLayer layer, PostProcessLayer.Antialiasing level)
        {
            layer.antialiasingMode = level;
            QualitySettings.antiAliasing = 0;

            if (GameSettings.IsMobile() && level == PostProcessLayer.Antialiasing.FastApproximateAntialiasing)
                layer.fastApproximateAntialiasing.fastMode = true;
        }

        private void SetEffectEnabled<T>(bool isEnabled) where T : MonoBehaviour
        {
            var component = GetComponent<T>();
            if (component != null)
                component.enabled = isEnabled;
            else
                Debug.LogWarningFormat("The component {0} doesn't exists", typeof(T));
        }

        private static void UpdateEffect<T>(PostProcessProfile profile, Action<T> callback) where T : PostProcessEffectSettings
        {
            T outParam;
            if (profile.TryGetSettings<T>(out outParam))
                callback(outParam);
        }

        public static void SetPostProcessEffectEnabled<T>(PostProcessProfile profile, bool isEnabled) where T : PostProcessEffectSettings
        {
            UpdateEffect<T>(profile, (e) => e.enabled.value = isEnabled);
        }

        private static void DisableEffect<T>(PostProcessProfile profile) where T : PostProcessEffectSettings
        {
            if (!profile.HasSettings<T>())
                return;

            profile.RemoveSettings<T>();
        }
    }
}
