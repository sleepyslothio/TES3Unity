using TES3Unity.Rendering;
using UnityEngine;

namespace TES3Unity.Components
{
    /// <summary>
    /// This class is used in editor mode only. It allows to override the GameSettings class.
    /// This is usefull to test custom parameters without the need to change them from the option menu.
    /// </summary>
    public sealed class GameSettingsOverride : MonoBehaviour
    {
        public bool Override = false;

        [Header("Player")]
        public PlayerData Player;

        [Header("Global")]
        public bool PlayMusic = false;
        public bool KinematicRigidbodies = true;

        [Header("Optimizations")]
        public ushort CellRadius = 4;
        public ushort CellDetailRadius = 3;
        public ushort CellRadiusOnLoad = 2;

        [Header("Rendering")]
        public float CameraFarClip = 500.0f;
        public SRPQuality SRPQuality = SRPQuality.High;
        public ushort RenderScale = 100;
        public ShaderType ShaderType = ShaderType.PBR;

        [Header("Lighting")]
        public bool SunShadows = true;
        public bool LightShadows = true;
        public bool ExteriorCellLights = true;
        public bool AnimateLights = true;
        public bool DayNightCycle = false;
        public bool GenerateNormalMap = true;

        [Header("Effects")]
        public PostProcessingQuality PostProcessingQuality = PostProcessingQuality.High;
        public AntiAliasingMode AntiAliasing = AntiAliasingMode.SMAA;

        [Header("VR")]
        public bool FollowHead = true;
        public bool RoomScale = true;
        public bool Teleportation = false;

        /// <summary>
        /// Apply overriden settings from the editor for testing purpose only.
        /// This method do nothing in a build.
        /// </summary>
        public void ApplyEditorSettingsOverride()
        {
#if UNITY_EDITOR
            if (!Override)
            {
                return;
            }

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
            settings.PonctualLightShadows = LightShadows;
            settings.PostProcessingQuality = PostProcessingQuality;
            settings.RenderScale = RenderScale;
            settings.RoomScale = RoomScale;
            settings.SRPQuality = SRPQuality;
            settings.SunShadows = SunShadows;
            settings.KinematicRigidbody = KinematicRigidbodies;
            settings.ShaderType = ShaderType;
            settings.Teleportation = Teleportation;
#endif
        }
    }
}
