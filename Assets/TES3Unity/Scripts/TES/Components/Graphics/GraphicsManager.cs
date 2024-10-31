using System;
using System.Collections;
using System.Collections.Generic;
using Demonixis.ToolboxV2;
using Demonixis.ToolboxV2.Graphics;
using Demonixis.ToolboxV2.Utils;
using Demonixis.ToolboxV2.XR;
using UnityEngine;
#if HDRP_ENABLED
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;
using UnityEngine.Rendering.Universal;
using ScreenSpaceAmbientOcclusion = UnityEngine.Rendering.HighDefinition.ScreenSpaceAmbientOcclusion;
#endif

namespace TES3Unity
{
    public sealed class GraphicsManager : GraphicsScaler
    {
        private const string TreeContainerName = "Trees";

        private void Awake()
        {
            var settings = GameSettings.Get();
            PreInitialization(settings.startInVr, false);
        }

        private void Start()
        {
            UpdateSettings();
        }

        public void UpdateSettings()
        {
            var settings = GameSettings.Get();
            var settingsData = settings.GetGraphicsSettingsData();
            var xrSettingsData = settings.GetXRSettingsData();

#if HDRP_ENABLED
            StartCoroutine(SetupHdrp(settingsData, xrSettingsData));
#else
            StartCoroutine(ReloadSettingsCoroutine(settingsData, xrSettingsData));
#endif

            var trees = GameObject.Find(TreeContainerName);
            if (trees == null) return;
            trees.SetActive(settings.terrainQuality != GraphicsQuality.Low);
        }

#if HDRP_ENABLED
        private IEnumerator SetupHdrp(GraphicsSettingsData settingsData, XRSettingsData xrSettingsData)
        {
            SetupQualityLevel((int)settingsData.SRPQuality);
            TrySetupOculus(ref xrSettingsData);
            //SetupShadows(ref settingsData);
            SetupRefreshRate(settingsData.RefreshRate);
            SetupPostProcessing(settingsData);

            yield return CoroutineFactory.WaitForSeconds(5.0f);

            var cameras = FindObjectsByType<Camera>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            foreach (var target in cameras)
            {
                if (target.CompareTag("MainCamera"))
                    SetupHdrpMainCamera(target, ref settingsData);
            }
        }

        private static void SetupHdrpMainCamera(Camera targetCamera, ref GraphicsSettingsData graphicsSettingsData)
        {
            targetCamera.nearClipPlane = graphicsSettingsData.nearClip;
            targetCamera.farClipPlane = graphicsSettingsData.farClip;

            if (targetCamera.TryGetComponent(out HDAdditionalCameraData data))
            {
                data.antialiasing = graphicsSettingsData.GetHdrpAntiAliasing();
            }
            else
            {
                Debug.LogWarning("Camera Data not found");
            }
        }

        private static void SetupPostProcessing(GraphicsSettingsData graphicsSettingsData)
        {
            var volumes = FindObjectsByType<Volume>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            foreach (var volume in volumes)
            {
                // Don't do anything 
                if (volume.name.ToLower().Contains("sky")) continue;

                if (graphicsSettingsData.PostProcessingQuality == PostProcessingQuality.None)
                    volume.enabled = false;
                else
                    SetupVolumeProfile(volume.profile, graphicsSettingsData);
            }
        }

        private static void SetupVolumeProfile(VolumeProfile profile, GraphicsSettingsData graphicsSettingsData)
        {
            profile.UpdateEffect<UnityEngine.Rendering.HighDefinition.Bloom>(bloom =>
                bloom.active = graphicsSettingsData.Bloom);
            profile.UpdateEffect<ScreenSpaceAmbientOcclusion>(ao => ao.active = graphicsSettingsData.SSAO);
            profile.UpdateEffect<ScreenSpaceReflection>(ssr => ssr.active = graphicsSettingsData.SSR);
            profile.UpdateEffect<GlobalIllumination>(ssgi => ssgi.active = graphicsSettingsData.SSGI);
        }
#endif
    }
}