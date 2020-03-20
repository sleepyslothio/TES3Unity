using Demonixis.Toolbox.XR;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace TES3Unity.Components
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
            var target = config.SRPQuality.ToString();

            var assetPath = $"Rendering/UniversalRP/PipelineAssets";
            var volumePath = $"Rendering/UniversalRP/Volumes";

            if (GameSettings.IsMobile())
            {
                target = "Mobile";
            }

            // Setup the Quality level
            var qualityIndex = 3; // High

            if (target == "Mobile")
            {
                qualityIndex = 0;
            }
            else if (config.SRPQuality == SRPQuality.Low)
            {
                qualityIndex = 1;
            }
            else if (config.SRPQuality == SRPQuality.Medium)
            {
                qualityIndex = 2;
            }

            QualitySettings.SetQualityLevel(qualityIndex);

            // Setup the UniversalRP Asset.
            var lwrpAsset = Resources.Load<UniversalRenderPipelineAsset>($"{assetPath}/URPAsset-{target}");
            var instanceAsset = Instantiate(lwrpAsset);

            var renderScale = (float)config.RenderScale / 100.0f;

            if (renderScale >= 0.5f && renderScale <= 2.0f && !XRManager.Enabled)
            {
                instanceAsset.renderScale = renderScale;
            }

            if (config.AntiAliasingMode != AntiAliasingMode.MSAA)
            {
                instanceAsset.msaaSampleCount = 0;
            }

            GraphicsSettings.renderPipelineAsset = instanceAsset;

            // Instantiate URP Volume
            var profile = Resources.Load<VolumeProfile>($"{volumePath}/PostProcess-Profile");
            var volumeGo = new GameObject($"{(profile.name.Replace("-Profile", "-Volume"))}");
            volumeGo.transform.localPosition = Vector3.zero;

            var instanceProfile = Instantiate(profile);

            if (XRManager.Enabled || config.PostProcessingQuality == PostProcessingQuality.Medium)
            {
                instanceProfile.UpdateEffect<Bloom>((b) =>
                {
                    b.dirtIntensity.overrideState = true;
                    b.dirtIntensity.Override(0);
                    b.highQualityFiltering.overrideState = true;
                    b.highQualityFiltering.Override(false);
                });

                instanceProfile.DisableEffect<MotionBlur>();
                instanceProfile.DisableEffect<Vignette>();

#if !UNITY_ANDROID
                // Apply Fixed Foveated Rendering on Qo/Quest
                var tes3 = TES3Engine.Instance;
                if (tes3 != null && XRManager.GetXRVendor() == XRVendor.Oculus)
                {
                    tes3.CurrentCellChanged += Instance_CurrentCellChanged;
                }
#endif
            }

            if (config.PostProcessingQuality == PostProcessingQuality.Low)
            {
                instanceProfile.DisableEffect<Bloom>();
            }

            var volume = volumeGo.AddComponent<Volume>();
            volume.isGlobal = true;
            volume.sharedProfile = instanceProfile;

            var skyboxMaterial = new Material(Shader.Find("Skybox/Procedural"));
            var lowQualitySkybox = config.SRPQuality != SRPQuality.Low;
#if UNITY_ANDROID
            lowQualitySkybox = true;
#endif

            if (lowQualitySkybox)
            {
                skyboxMaterial.DisableKeyword("_SUNDISK_HIGH_QUALITY");
                skyboxMaterial.EnableKeyword("_SUNDISK_SIMPLE");
            }

            RenderSettings.skybox = skyboxMaterial;
        }

        private IEnumerator Start()
        {
            var config = GameSettings.Get();
            var camera = Camera.main;
            var wait = new WaitForSeconds(1);

            while (camera == null)
            {
                camera = Camera.main;
                yield return wait;
            }

            // 1. Setup Camera
            camera.farClipPlane = config.CameraFarClip;

            var data = camera.GetComponent<UniversalAdditionalCameraData>();
            data.renderPostProcessing = config.PostProcessingQuality != PostProcessingQuality.None;

            switch (config.AntiAliasingMode)
            {
                case AntiAliasingMode.None:
                case AntiAliasingMode.MSAA:
                    data.antialiasing = AntialiasingMode.None;
                    break;
                case AntiAliasingMode.FXAA:
                    data.antialiasing = AntialiasingMode.FastApproximateAntialiasing;
                    break;
                case AntiAliasingMode.SMAA:
                    data.antialiasing = AntialiasingMode.SubpixelMorphologicalAntiAliasing;
                    break;
            }
        }

#if !UNITY_ANDROID
        private void Instance_CurrentCellChanged(ESM.Records.CELLRecord obj)
        {
            var level = obj.isInterior ? 1 : 3;
            Unity.XR.Oculus.Utils.SetFoveationLevel(level);
        }
#endif
    }
}
