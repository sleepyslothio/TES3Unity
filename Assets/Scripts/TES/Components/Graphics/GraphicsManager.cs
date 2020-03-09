using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using System.Collections;

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

#if UNITY_ANDROID
            target = "Mobile";
#endif
            // Setup the UniversalRP Asset.
            var lwrpAsset = Resources.Load<UniversalRenderPipelineAsset>($"{assetPath}/URPAsset-{target}");
            lwrpAsset.renderScale = config.RenderScale;
            GraphicsSettings.renderPipelineAsset = lwrpAsset;

            // Instantiate URP Volume
            var profile = Resources.Load<VolumeProfile>($"{volumePath}/PostProcess-Profile");
            CreateVolume(profile);

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

            var additionalData = camera.GetComponent<UniversalAdditionalCameraData>();
            if (additionalData == null)
            {
                additionalData = camera.gameObject.AddComponent<UniversalAdditionalCameraData>();
            }

            additionalData.renderPostProcessing = config.PostProcessingQuality != PostProcessingQuality.None;

            switch (config.AntiAliasingMode)
            {
                case AntiAliasingMode.None:
                case AntiAliasingMode.MSAA:
                    additionalData.antialiasing = AntialiasingMode.None;
                    break;
                case AntiAliasingMode.FXAA:
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

        private static void CreateVolume(params VolumeProfile[] profiles)
        {
            GameObject volumeGo = null;
            Volume volume = null;

            foreach (var profile in profiles)
            {
                volumeGo = new GameObject($"{(profile.name.Replace("-Profile", "-Volume"))}");
                volumeGo.transform.localPosition = Vector3.zero;

                volume = volumeGo.AddComponent<Volume>();
                volume.isGlobal = true;
                volume.sharedProfile = profile;
            }
        }
    }
}
