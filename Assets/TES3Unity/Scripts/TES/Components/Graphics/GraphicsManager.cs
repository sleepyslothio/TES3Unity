using Demonixis.Toolbox.XR;
using Demonixis.ToolboxV2.XR;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace TES3Unity.Components
{
    public sealed class GraphicsManager : MonoBehaviour
    {
#if UNITY_EDITOR
        private void Awake()
        {
            // We need to call this component now because SRP settings is very early
            // And we want to be sure it's called before SRP settings.
            var settingsOverride = FindObjectOfType<GameSettingsOverride>();
            settingsOverride?.ApplyEditorSettingsOverride();
        }
#endif

        private IEnumerator Start()
        {
            var config = GameSettings.Get();
            var camera = Camera.main;
            var wait = new WaitForSeconds(1);

            // Setup the Quality level
            var qualityIndex = 0; // PC

            if (config.SRPQuality == SRPQuality.High)
            {
                qualityIndex = 1;
            }

            QualitySettings.SetQualityLevel(qualityIndex);

            var asset = (UniversalRenderPipelineAsset)GraphicsSettings.defaultRenderPipeline;

            var renderScale = config.VRRenderScale / 100.0f;
            if (renderScale >= 0.5f && renderScale <= 2.0f)
            {
                asset.renderScale = renderScale;
            }
            asset.msaaSampleCount = config.AntiAliasingMode != AntialiasingMode.None ? (int)MsaaQuality.Disabled : (int)MsaaQuality._4x;

            // Instantiate URP Volume
            var volume = FindObjectOfType<Volume>();
            if (volume != null)
            {
                var profile = volume.profile;
                SetupPostProcessing(config, profile);
            }

            // Skybox
            if (config.SRPQuality == SRPQuality.Low || GameSettings.IsMobile())
            {
                var skyboxMaterial = RenderSettings.skybox;
                if (skyboxMaterial != null)
                {
                    skyboxMaterial.DisableKeyword("_SUNDISK_HIGH_QUALITY");
                    skyboxMaterial.EnableKeyword("_SUNDISK_SIMPLE");
                }
            }

            while (camera == null)
            {
                camera = Camera.main;
                yield return wait;
            }

            // 1. Setup Camera
            camera.farClipPlane = config.CameraFarClip;

            var data = camera.GetComponent<UniversalAdditionalCameraData>();
            data.renderPostProcessing = config.PostProcessingQuality != PostProcessingQuality.None;
            data.antialiasing = config.AntiAliasingMode;

#if UNITY_ANDROID
            SetupOculus(config);
#endif
        }

        private void SetupOculus(GameSettings settings)
        {
#if OCULUS_XR
            Unity.XR.Oculus.Utils.useDynamicFoveatedRendering = settings.VRFFRLevel > 0;
            Unity.XR.Oculus.Utils.foveatedRenderingLevel = settings.VRFFRLevel;
            Unity.XR.Oculus.Performance.TrySetDisplayRefreshRate((float)settings.VRRefreshRate);

	        if (settings.VRAppSpaceWarp)
		        Camera.main.depthTextureMode = DepthTextureMode.MotionVectors;

	        OVRManager.SetSpaceWarp(settings.VRAppSpaceWarp);

            Unity.XR.Oculus.Performance.TryGetDisplayRefreshRate(out float rate);
            Debug.Log(
                $"Oculus Setup - FFR: {settings.VRFFRLevel}, Frequency: {settings.VRRefreshRate}, AppSpaceWarp: {settings.VRAppSpaceWarp}");
#endif
        }

        public void SetupPostProcessing(GameSettings config, VolumeProfile profile)
        {
            var xrEnabled = XRManager.Enabled;
            var mobile = GameSettings.IsMobile();

            if (config.SRPQuality == SRPQuality.Low)
            {
                profile.DisableEffect<Tonemapping>();
                profile.DisableEffect<ColorAdjustments>();
                profile.DisableEffect<WhiteBalance>();
                profile.DisableEffect<FilmGrain>();
                profile.DisableEffect<ScreenSpaceLensFlare>();
            }

            if (xrEnabled || mobile || config.PostProcessingQuality == PostProcessingQuality.Low)
            {
                profile.DisableEffect<MotionBlur>();
                profile.DisableEffect<Vignette>();
                profile.DisableEffect<LensDistortion>();
                profile.DisableEffect<FilmGrain>();
                profile.DisableEffect<ChromaticAberration>();
                profile.DisableEffect<FilmGrain>();
                profile.DisableEffect<ScreenSpaceLensFlare>();
            }

            if (xrEnabled)
            {
                Debug.LogWarning("Bloom is disabled because of an issue with HDR lighting.");

                profile.UpdateEffect<Bloom>((b) =>
                {
                    b.dirtIntensity.overrideState = true;
                    b.dirtIntensity.Override(0);
                    b.dirtTexture.overrideState = true;
                    b.dirtTexture.Override(null);
                });
            }

            if (config.PostProcessingQuality == PostProcessingQuality.Medium)
            {
                profile.UpdateEffect<Bloom>((b) =>
                {
                    b.highQualityFiltering.overrideState = true;
                    b.highQualityFiltering.Override(false);
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
    }
}
