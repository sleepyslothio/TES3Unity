using Demonixis.Toolbox.XR;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

namespace TESUnity.Components
{
    [RequireComponent(typeof(PostProcessLayer))]
    public sealed class PostProcessManager : MonoBehaviour
    {
        private IEnumerator Start()
        {
            yield return new WaitForEndOfFrame();
            UpdateEffects();
        }

        public void UpdateEffects()
        {
            var settings = TESManager.instance;
            var layer = GetComponent<PostProcessLayer>();
            var volume = FindObjectOfType<PostProcessVolume>();
            var profile = volume.profile;
            var xrEnabled = XRManager.Enabled;
            var config = GameSettings.Get();
            var quality = config.PostProcessingQuality;
            var antiAliasing = config.AntiAliasingMode;

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
