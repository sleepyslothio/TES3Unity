using UnityEngine;
#if HDRP_ENABLED
using UnityEngine.Rendering.HighDefinition;
#endif
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace TESUnity.Components
{
    public sealed class GraphicsManager : MonoBehaviour
    {


        private bool m_UniversalRP = true;

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

            m_UniversalRP = config.RendererMode == RendererMode.UniversalRP;

#if !HDRP_ENABLED || UNITY_ANDROID
            m_UniversalRP = true; // Double check
#endif
            var assetPath = GetPipelineAssetPath(m_UniversalRP);
            var volumePath = GetVolumePath(m_UniversalRP);

            if (m_UniversalRP)
            {
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
            }
#if HDRP_ENABLED
            else
            {
                // Setup the HDRP Asset.
                var hdrpAsset = Resources.Load<HDRenderPipelineAsset>($"{assetPath}/HDRPAsset-{target}");
                GraphicsSettings.renderPipelineAsset = hdrpAsset;

                // Instantiate HDRP Volume (Post Processing + Environment)
                var postProcessingProfile = Resources.Load<VolumeProfile>($"{volumePath}/PostProcess-Profile");
                var environmentProfile = Resources.Load<VolumeProfile>($"{volumePath}/Environment-Profile");
                CreateVolume(postProcessingProfile, environmentProfile);
            }
#endif
        }

        private void Start()
        {
            var config = GameSettings.Get();
            var camera = Camera.main;

            // 1. Setup Camera
            camera.farClipPlane = config.CameraFarClip;

            if (m_UniversalRP)
            {
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
#if HDRP_ENABLED
            else
            {
            }
#endif
        }

        private static void CreateVolume(params VolumeProfile[] profiles)
        {
            var volumeGo = new GameObject("Volume");
            volumeGo.transform.localPosition = Vector3.zero;

            Volume volume = null;

            foreach (var profile in profiles)
            {
                volume = volumeGo.AddComponent<Volume>();
                volume.isGlobal = true;
                volume.sharedProfile = profile;
            }
        }

        public static string GetPipelineAssetPath(bool universalRP)
        {
            return $"Rendering/{(universalRP ? "UniversalRP" : "HDRP")}/PipelineAssets";
        }

        public static string GetVolumePath(bool universalRP)
        {
            return $"Rendering/{(universalRP ? "UniversalRP" : "HDRP")}/Volumes";
        }
    }
}
