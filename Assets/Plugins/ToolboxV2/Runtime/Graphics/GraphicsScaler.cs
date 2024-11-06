#define SSR_SUPPORTED_
#define SSGI_SUPPORTED_
#define HBAO_SUPPORTED_
#define BEAUTIFY_SUPPORTED_
#define USE_SHADER_SWITCHER_

using System;
using Demonixis.ToolboxV2.Utils;
using Demonixis.ToolboxV2.XR;
#if HBAO_SUPPORTED
using HorizonBasedAmbientOcclusion.Universal;
#endif
#if SSR_SUPPORTED
using ShinySSRR;
#endif
#if SSGI_SUPPORTED
using RadiantGI.Universal;
#endif
using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.XR;

namespace Demonixis.ToolboxV2.Graphics
{
    public class GraphicsScaler : MonoBehaviour
    {
        private List<ScriptableRendererFeature> _disabledRenderFeatures = new();

        public static bool BypassVRActivation = false;

#if UNITY_EDITOR
        [Header("Editor Only")] [SerializeField]
        private bool _startInVR;
#endif

        protected void PreInitialization(bool startInVR, bool lqShader)
        {
            var cmd = CommandLineParser.Get();

#if UNITY_EDITOR
            if (_startInVR)
                startInVR = true;
#endif

            if (startInVR)
                cmd.AddKey("xr", $"openxr");

            if (cmd.GetString("xr") != string.Empty)
                startInVR = true;

#if UNITY_STANDALONE_WIN
            if (startInVR && !BypassVRActivation)
                XRManager.TryInitialize();
#endif

#if USE_SHADER_SWITCHER
            if (lqShader && TryGetComponent(out ShaderSwitcher switcher))
                switcher.SwitchShaders();
#endif
        }

        protected virtual void OnDestroy()
        {
            if (_disabledRenderFeatures == null || _disabledRenderFeatures.Count <= 0) return;
            foreach (var renderFeature in _disabledRenderFeatures)
                renderFeature.SetActive(true);
        }

        private void OnApplicationQuit()
        {
#if UNITY_STANDALONE_WIN
            XRManager.TryShutdown();
#endif
        }

        protected IEnumerator ReloadSettingsCoroutine(GraphicsSettingsData graphicsSettingsData,
            XRSettingsData xRSettings)
        {
            SetupQualityLevel((int)graphicsSettingsData.SRPQuality);
            SetupRefreshRate((int)graphicsSettingsData.RefreshRate);

            yield return new WaitForEndOfFrame();

            var profile = GetUniqueVolumeProfile();
            SetupPostProcessing(profile, graphicsSettingsData.PostProcessingQuality);
            SetupBeautify(profile, ref graphicsSettingsData);
            SetupShadows(ref graphicsSettingsData);
            SetupTerrains(graphicsSettingsData.TerrainQuality);

            // Find Main Camera
            var camera = Camera.main;
            while (camera == null)
            {
                camera = Camera.main;
                yield return null;
            }

            // Post Processing
            SetupMainCamera(camera, ref graphicsSettingsData);

            // SRP Asset
            var asset = GetUniqueRenderAsset();
            SetupRenderAsset(asset, profile, ref graphicsSettingsData);

            SetupRenderFeatures(asset, profile, ref graphicsSettingsData, out List<string> disableRenderFeatures);

            foreach (var renderFeature in disableRenderFeatures)
                DisableRenderFeature(asset, renderFeature);

            TrySetupOculus(ref xRSettings);

            OnSettingsInitialized(asset, profile);
        }

        protected virtual void OnSettingsInitialized(UniversalRenderPipelineAsset asset, VolumeProfile profile)
        {
        }

        public static void TrySetupOculus(ref XRSettingsData xRSettings)
        {
            if (XRManager.Vendor != XRVendor.Meta) return;
#if UNITY_XR_SUPPORTED
            PatchOculusXR(ref xRSettings);
#endif
#if OCULUS_BUILD
            if (OVRManager.OVRManagerinitialized)
            {
                OVRManager.useDynamicFoveatedRendering = xRSettings.VRFFRLevel > 0;
                OVRManager.foveatedRenderingLevel = (OVRManager.FoveatedRenderingLevel)xRSettings.VRFFRLevel;

                if (OVRManager.display != null)
                {
                    var refreshRate = xRSettings.GetRefreshRate();
                    foreach (var f in OVRManager.display.displayFrequenciesAvailable)
                    {
                        if ((int)f != refreshRate) continue;
                        OVRManager.display.displayFrequency = f;
                        break;
                    }
                }

                Debug.Log(
                    $"Oculus Setup - FFR: {xRSettings.VRFFRLevel}, Frequency: {OVRManager.display?.displayFrequency ?? 72}, AppSpaceWarp: {xRSettings.VRAppSpaceWarp}");
            }
            else
                return;

            if (xRSettings.VRAppSpaceWarp)
            {
                var cam = Camera.main;
                if (cam != null)
                    cam.depthTextureMode = DepthTextureMode.MotionVectors;
            }

#if UNITY_ANDROID
            //OVRManager.SetSpaceWarp(xRSettings.VRAppSpaceWarp);
#endif

            var ovrManager = OVRManager.instance;
            if (ovrManager == null) return;
            ovrManager.enableDynamicResolution = xRSettings.VRDynamicResolution;
            ovrManager.minDynamicResolutionScale = 0.7f;
            ovrManager.maxDynamicResolutionScale = 1.2f;
            Debug.Log(
                $"OVRManager Dynamic Resolution: {ovrManager.enableDynamicResolution} => {ovrManager.minDynamicResolutionScale}/{ovrManager.maxDynamicResolutionScale}");
#endif
        }

#if UNITY_ANDROID
        private static void PatchOculusXR(ref XRSettingsData xRSettings)
        {
#if UNITY_XR_SUPPORTED
            Unity.XR.Oculus.Utils.useDynamicFoveatedRendering = xRSettings.VRFFRLevel > 0;
            Unity.XR.Oculus.Utils.foveatedRenderingLevel = (int)xRSettings.VRFFRLevel;
            Unity.XR.Oculus.Performance.TrySetDisplayRefreshRate(xRSettings.GetRefreshRate());
            Unity.XR.Oculus.Performance.TryGetDisplayRefreshRate(out float rate);
            Debug.Log(
                $"Oculus Setup - FFR: {xRSettings.VRFFRLevel}, Frequency: {rate}, AppSpaceWarp: {xRSettings.VRAppSpaceWarp}");
#endif
        }
#endif

        public void SetRenderScale(float scale)
        {
            var asset = GetCurrentRenderAsset();
            asset.renderScale = Mathf.Clamp(scale, 0.5f, 2.0f);
        }

        public static void SetupQualityLevel(int level)
        {
            QualitySettings.SetQualityLevel(level);
        }

        private static void SetupPostProcessing(VolumeProfile profile, PostProcessingQuality quality)
        {
            var xrEnabled = XRManager.Enabled;
            var mobile = PlatformUtility.IsMobilePlatform();

            if (xrEnabled || mobile || quality == PostProcessingQuality.Low)
            {
                profile.DisableEffect<MotionBlur>();
                profile.DisableEffect<Vignette>();
                profile.DisableEffect<LensDistortion>();
                profile.DisableEffect<FilmGrain>();
                profile.DisableEffect<ChromaticAberration>();
#if UNITY_2023_OR_NEWER || UNITY_6000_0_OR_NEWER
                profile.DisableEffect<ScreenSpaceLensFlare>();
#endif
                profile.DisableEffect<FilmGrain>();

                profile.UpdateEffect<Bloom>((b) =>
                {
                    b.dirtIntensity.overrideState = true;
                    b.dirtIntensity.Override(0);
                    b.dirtTexture.overrideState = true;
                    b.dirtTexture.Override(null);
                    b.highQualityFiltering.overrideState = true;
                    b.highQualityFiltering.Override(false);
                });
            }

            if (quality == PostProcessingQuality.Low)
            {
                profile.UpdateEffect<Bloom>((b) =>
                {
                    b.highQualityFiltering.overrideState = true;
                    b.highQualityFiltering.Override(false);
                    b.maxIterations.overrideState = true;
                    b.maxIterations.Override(2);
                });
            }

            if (quality == PostProcessingQuality.Medium)
            {
                profile.UpdateEffect<Bloom>((b) =>
                {
                    b.highQualityFiltering.overrideState = true;
                    b.highQualityFiltering.Override(false);
                    b.maxIterations.overrideState = true;
                    b.maxIterations.Override(4);
                });

                profile.UpdateEffect<MotionBlur>((m) =>
                {
                    var mbQuality = mobile ? MotionBlurQuality.Low : MotionBlurQuality.Medium;
                    m.quality.overrideState = true;
                    m.quality.Override(mbQuality);
                });

                profile.DisableEffect<LensDistortion>();
                profile.DisableEffect<FilmGrain>();
                profile.DisableEffect<ChromaticAberration>();
            }
        }

        private static void SetupBeautify(VolumeProfile profile, ref GraphicsSettingsData graphicsSettingsData)
        {
#if BEAUTIFY_SUPPORTED
            var xrEnabled = XRManager.Enabled;
            var mobile = PlatformUtility.IsMobilePlatform();
            var quality = graphicsSettingsData.PostProcessingQuality;
            var bloom = graphicsSettingsData.Bloom;

            profile.UpdateEffect<Beautify.Universal.Beautify>(b =>
            {
                if (quality == PostProcessingQuality.Low)
                {
                    b.turboMode.overrideState = true;
                    b.turboMode.Override(true);
                    b.eyeAdaptation.overrideState = true;
                    b.eyeAdaptation.Override(false);
                }

                if (quality != PostProcessingQuality.High || mobile)
                {
                    b.bloomAntiflicker.overrideState = true;
                    b.bloomAntiflicker.Override(false);
                    b.anamorphicFlaresAntiflicker.overrideState = true;
                    b.anamorphicFlaresAntiflicker.Override(false);
                }

                if (xrEnabled)
                {
                    b.lensDirtIntensity.overrideState = true;
                    b.lensDirtIntensity.Override(0);
                    b.vignettingOuterRing.overrideState = true;
                    b.vignettingOuterRing.Override(0);
                    b.vignettingInnerRing.overrideState = true;
                    b.vignettingInnerRing.Override(0);
                }

                if (!bloom)
                {
                    b.bloomIntensity.overrideState = true;
                    b.bloomIntensity.Override(0);
                    b.bloomThreshold.overrideState = true;
                    b.bloomThreshold.Override(10);
                    b.anamorphicFlaresAntiflicker.overrideState = true;
                    b.anamorphicFlaresAntiflicker.Override(false);
                }
            });
#endif
        }


        public static void SetupShadows(ref GraphicsSettingsData graphicsSettingsData)
        {
            var lights = FindObjectsByType<Light>(FindObjectsSortMode.None);
            var directionalShadow = graphicsSettingsData.DirectionalShadows;
            var additionShadow = graphicsSettingsData.AdditionalShadows;
            var softShadows = graphicsSettingsData.SoftShadows ? LightShadows.Soft : LightShadows.Hard;

            foreach (var light in lights)
            {
                if (light.type == LightType.Directional)
                    light.shadows = directionalShadow ? softShadows : LightShadows.None;
                else
                    light.shadows = additionShadow ? softShadows : LightShadows.None;
            }
        }

        private static void SetupRenderAsset(
            UniversalRenderPipelineAsset asset,
            VolumeProfile profile,
            ref GraphicsSettingsData graphicsSettingsData)
        {
            if (!asset.supportsHDR)
            {
                profile.DisableEffect<ColorAdjustments>();
                profile.DisableEffect<Tonemapping>();
            }

            asset.msaaSampleCount = (int)graphicsSettingsData.GetMsaaQuality();
            asset.shadowCascadeCount = graphicsSettingsData.GetShadowCascadeCount();
            asset.shadowDistance = graphicsSettingsData.ShadowDistance;

            var renderScale = Mathf.Clamp(graphicsSettingsData.RenderScale, 0.5f, 2.0f);
            if (XRManager.Enabled)
            {
                XRManager.SetRenderScale(renderScale);
            }
            else
                asset.renderScale = renderScale;
        }

        public static void SetupRefreshRate(int refreshRate)
        {
            if (!XRManager.Enabled)
                Application.targetFrameRate = refreshRate;
        }

        private static void SetupMainCamera(Camera targetCamera, ref GraphicsSettingsData graphicsSettingsData)
        {
            targetCamera.nearClipPlane = 0.1f; // Mathf.Clamp(graphicsSettingsData.nearClip, 0.1f, 0.3f);
            targetCamera.farClipPlane = graphicsSettingsData.farClip;

            if (targetCamera.TryGetComponent(out UniversalAdditionalCameraData data))
            {
                data.renderPostProcessing = graphicsSettingsData.PostProcessingQuality != PostProcessingQuality.None;
                if (data.dithering)
                    data.dithering = data.renderPostProcessing;

                data.renderShadows = graphicsSettingsData.DirectionalShadows;
                data.antialiasing = graphicsSettingsData.GetUrpAntiAliasing();
            }
            else
            {
                Debug.LogWarning("Camera Data not found");
            }
        }

        private static void SetupRenderFeatures(
            UniversalRenderPipelineAsset asset,
            VolumeProfile profile,
            ref GraphicsSettingsData graphicsSettingsData,
            out List<string> disableRenderFeatures)
        {
            disableRenderFeatures = new List<string>();

            var xrEnabled = XRManager.Enabled;
            var ssaoEnabled = graphicsSettingsData.SSAO;
            var ssrEnabled = graphicsSettingsData.SSR;
            var ssgiEnabled = !xrEnabled && graphicsSettingsData.SSGI;

            // Disable post processing
            if (!ssaoEnabled)
                disableRenderFeatures.Add("ScreenSpaceAmbientOcclusion");

            if (!graphicsSettingsData.Bloom)
                profile.DisableEffect<Bloom>();
#if UNITY_2023_OR_NEWER || UNITY_6000_0_OR_NEWER
            if (!graphicsSettingsData.LenseEffect)
                profile.DisableEffect<ScreenSpaceLensFlare>();
#endif

            if (!graphicsSettingsData.FilmGrain)
                profile.DisableEffect<FilmGrain>();

#if HBAO_SUPPORTED
            if (!ssaoEnabled)
            {
                disableRenderFeatures.Add("HBAORendererFeature");
                profile.DisableEffect<HBAO>();
            }
#endif

#if SSGI_SUPPORTED
            if (!ssgiEnabled)
            {
                disableRenderFeatures.Add("RadiantRenderFeature");
                profile.DisableEffect<RadiantGlobalIllumination>();
            }
#endif

#if SSR_SUPPORTED
            if (!ssrEnabled)
            {
                var reflections = FindObjectsByType<Reflections>(FindObjectsSortMode.None);
                foreach (var reflection in reflections)
                {
                    reflection.enabled = false;
                    Destroy(reflection);
                }

                disableRenderFeatures.Add("ShinySSRR");
                profile.DisableEffect<ShinyScreenSpaceRaytracedReflections>();
            }
#endif
        }

        private static void SetupTerrains(GraphicsQuality terrainQuality)
        {
            var terrains = FindObjectsByType<Terrain>(FindObjectsSortMode.None);

            if (terrainQuality == GraphicsQuality.High) return;

            var lowerTerrain = terrainQuality == GraphicsQuality.Low;

            foreach (var terrain in terrains)
            {
#if UNITY_ANDROID || UNITY_IOS
                terrain.heightmapPixelError = 200;
                terrain.basemapDistance = 500;
#endif

                terrain.detailObjectDensity = lowerTerrain ? 0 : 0.5f;
                terrain.treeDistance = Mathf.Min(lowerTerrain ? 15 : 30, terrain.treeDistance);
                terrain.heightmapPixelError = lowerTerrain ? 200 : 100;
                terrain.basemapDistance = lowerTerrain ? 50 : 100;

                if (lowerTerrain)
                {
                    terrain.shadowCastingMode = ShadowCastingMode.Off;
                }
            }
        }

        #region Private Functions

        public static VolumeProfile GetUniqueVolumeProfile()
        {
            var volume = FindFirstObjectByType<Volume>();

            if (volume == null)
            {
                return ScriptableObject.CreateInstance<VolumeProfile>();
            }

            return volume.profile;
        }

        private static UniversalRenderPipelineAsset GetUniqueRenderAsset()
        {
            var asset = (UniversalRenderPipelineAsset)GraphicsSettings.currentRenderPipeline;
            asset = Instantiate(asset);
            GraphicsSettings.defaultRenderPipeline = asset;
            return asset;
        }

        public static VolumeProfile GetCurrentVolumeProfile()
        {
            var volume = FindFirstObjectByType<Volume>();
            return volume.profile;
        }

        private static UniversalRenderPipelineAsset GetCurrentRenderAsset()
        {
            return (UniversalRenderPipelineAsset)GraphicsSettings.defaultRenderPipeline;
        }

        #endregion

        private void DisableRenderFeature(UniversalRenderPipelineAsset asset, string feature)
        {
            var renderFeature = asset.DisableRenderFeature(feature);

            if (renderFeature != null)
            {
                _disabledRenderFeatures.AddRange(renderFeature);
            }
        }
    }
}