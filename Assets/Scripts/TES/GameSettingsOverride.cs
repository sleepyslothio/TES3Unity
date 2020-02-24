using UnityEngine;

namespace TESUnity.Components
{
    /// <summary>
    /// This class is used in editor mode only. It allows to override the GameSettings class.
    /// This is usefull to test custom parameters without the need to change them from the option menu.
    /// </summary>
    public sealed class GameSettingsOverride : MonoBehaviour
    {
        public bool Override = false;

        [Header("Global")]
        public bool PlayMusic = false;
        public bool KinematicRigidbodies = true;

        [Header("Optimizations")]
        public int CellRadius = 4;
        public int CellDetailRadius = 3;
        public int CellRadiusOnLoad = 2;

        [Header("Rendering")]
        public RendererMode RendererType = RendererMode.UniversalRP;
        public float CameraFarClip = 500.0f;
        public SRPQuality SRPQuality = SRPQuality.High;
        public float RenderScale = 1.0f;
        public bool WaterTransparency = false;

        [Header("Lighting")]
        public bool SunShadows = true;
        public bool LightShadows = true;
        public bool ExteriorCellLights = true;
        public bool AnimateLights = true;
        public bool DayNightCycle = false;
        public bool GenerateNormalMap = true;

        [Header("Effects")]
        public PostProcessingQuality PostProcessingQuality = PostProcessingQuality.High;
        public AntiAliasingMode AntiAliasing = AntiAliasingMode.TAA;

        [Header("VR")]
        public bool FollowHead = true;
        public bool RoomScale = true;
        public bool HandTracking = true;

        /// <summary>
        /// Apply overriden settings from the editor for testing purpose only.
        /// This method do nothing in a build.
        /// </summary>
        public void ApplyEditorSettingsOverride()
        {
#if UNITY_EDITOR
            if (!Override)
                return;

            var settings = GameSettings.Get();
            settings.AnimateLights = AnimateLights;
            settings.AntiAliasingMode = AntiAliasing;
            settings.MusicEnabled = PlayMusic;
            settings.CameraFarClip = CameraFarClip;
            settings.CellDetailRadius = CellDetailRadius;
            settings.CellRadius = CellRadius;
            settings.CellRadiusOnLoad = CellRadiusOnLoad;
            settings.DayNightCycle = DayNightCycle;
            settings.ExteriorLights = ExteriorCellLights;
            settings.FollowHead = FollowHead;
            settings.GenerateNormalMaps = GenerateNormalMap;
            settings.LightShadows = LightShadows;
            settings.PostProcessingQuality = PostProcessingQuality;
            settings.RendererMode = RendererType;
            settings.RenderScale = RenderScale;
            settings.RoomScale = RoomScale;
            settings.SRPQuality = SRPQuality;
            settings.SunShadows = SunShadows;
            settings.WaterTransparency = WaterTransparency;
            settings.KinematicRigidbody = KinematicRigidbodies;
            settings.HandTracking = HandTracking;
            settings.CheckSettings();
#endif
        }
    }
}
