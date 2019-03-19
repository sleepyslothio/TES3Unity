using UnityEngine;
using UnityStandardAssets.Water;

namespace TESUnity.Components
{
    /// <summary>
    /// This class is used in editor mode only. It allows to override the GameSettings class.
    /// This is usefull to test custom parameters without the need to change them from the option menu.
    /// </summary>
    public sealed class GameSettingsOverride : MonoBehaviour
    {
#if UNITY_EDITOR
        public bool Override = false;

        [Header("Global")]
        public bool playMusic = false;
        public Water.WaterMode waterQuality = Water.WaterMode.Simple;

        [Header("Optimizations")]
        public int cellRadius = 4;
        public int cellDetailRadius = 3;
        public int cellRadiusOnLoad = 2;

        [Header("Rendering")]
        public MWMaterialType materialType = MWMaterialType.Standard;
        public RendererType renderPath = RendererType.Forward;
        public float cameraFarClip = 500.0f;
        public SRPQuality srpQuality = SRPQuality.Medium;
        public float renderScale = 1.0f;

        [Header("Lighting")]
        public bool renderSunShadows = false;
        public bool renderLightShadows = false;
        public bool renderExteriorCellLights = false;
        public bool animateLights = false;
        public bool dayNightCycle = false;
        public bool generateNormalMap = true;

        [Header("Effects")]
        public PostProcessingQuality postProcessingQuality = PostProcessingQuality.High;
        public AntiAliasingMode antiAliasing = AntiAliasingMode.TAA;
        public bool waterBackSideTransparent = false;

        [Header("VR")]
        public bool FollowHead = true;
        public bool RoomScale = true;

        private void Awake()
        {
            if (!Override)
                return;

            var settings = GameSettings.Get();
            settings.AnimateLights = animateLights;
            settings.AntiAliasing = antiAliasing;
            settings.MusicEnabled = playMusic;
            settings.CameraFarClip = cameraFarClip;
            settings.CellDetailRadius = cellDetailRadius;
            settings.CellRadius = cellRadius;
            settings.CellRadiusOnLoad = cellRadiusOnLoad;
            settings.DayNightCycle = dayNightCycle;
            settings.ExteriorLights = renderExteriorCellLights;
            settings.FollowHead = FollowHead;
            settings.GenerateNormalMaps = generateNormalMap;
            settings.LightShadows = renderLightShadows;
            settings.MaterialType = materialType;
            settings.PostProcessing = postProcessingQuality;
            settings.RenderPath = renderPath;
            settings.RenderScale = renderScale;
            settings.RoomScale = RoomScale;
            settings.SRPQuality = srpQuality;
            settings.SunShadows = renderSunShadows;
            settings.WaterQuality = waterQuality;
        }
#endif
    }
}
