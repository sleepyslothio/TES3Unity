using System;
using Demonixis.ToolboxV2.Graphics;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

namespace TES3Unity.Components
{
    public class MenuOptionComponent : MonoBehaviour
    {
        private GameSettings _settings;

        [Header("Audio")] [SerializeField] private Toggle m_AudioToggle;

        [Header("General")] [SerializeField] private Dropdown m_CellRadiusDd;
        [SerializeField] private Dropdown m_CellDistanceDd;
        [SerializeField] private Dropdown m_CellRadiusLoad;

        [Header("Graphics")] [SerializeField] private Dropdown m_PostProcessDd;
        [SerializeField] private Dropdown m_SRPQuality;
        [SerializeField] private Toggle m_ShaderQuality;
        [SerializeField] private Dropdown m_AntiAliasing;
        [SerializeField] private Dropdown m_CameraFarClipDropdown;
        [SerializeField] private Dropdown m_RenderScaleDd;
        [SerializeField] private Toggle m_GenerateNormalMapsToggle;
        [SerializeField] private Toggle m_AnimateLightsToggle;
        [SerializeField] private Toggle m_SunShadowsToggle;
        [SerializeField] private Toggle m_LightShadowsToggle;
        [SerializeField] private Toggle m_ExteriorLightsToggle;
        [SerializeField] private Toggle m_DayNightCycleToggle;

        [Header("XR")] [SerializeField] private Toggle m_FollowHeadToggle;
        [SerializeField] private Toggle m_RoomScaleToggle;
        [SerializeField] private Toggle m_Teleportation;
        [SerializeField] private Toggle m_VRAsyncSpaceWarp;
        [SerializeField] private Dropdown m_VRFFRLevel;
        [SerializeField] private Dropdown m_VRFrequency;

        private void Awake()
        {
            _settings = GameSettings.Get();

            // Cell Distance
            SetupUShortDropdown(m_CellRadiusDd, GameSettings.CellDistanceValues, _settings.CellRadius,
                i => _settings.CellRadius = GameSettings.CellDistanceValues[i]);
            SetupUShortDropdown(m_CellDistanceDd, GameSettings.CellDistanceValues, _settings.CellDetailRadius,
                i => _settings.CellDetailRadius = GameSettings.CellDistanceValues[i]);
            SetupUShortDropdown(m_CellRadiusLoad, GameSettings.CellDistanceValues, _settings.CellRadiusOnLoad,
                i => _settings.CellRadiusOnLoad = GameSettings.CellDistanceValues[i]);

            // Post Processing
            SetupDropdown<PostProcessingQuality>(m_PostProcessDd, (int)_settings.postProcessingQuality,
                i => _settings.postProcessingQuality = (PostProcessingQuality)i);

            // SRP
            SetupDropdown<GraphicsQuality2>(m_SRPQuality, (int)_settings.srpQuality,
                i => _settings.srpQuality = (GraphicsQuality2)i);

            // Shader
            //m_ShaderQuality.SetIsOnWithoutNotify(_settings.lowQualityShader);
            //m_ShaderQuality.onValueChanged.AddListener(b => _settings.lowQualityShader = b);
            
            // AntiAliasing
            SetupDropdown<AntiAliasingProxy>(m_AntiAliasing, (int)_settings.antiAliasing,
                i => _settings.antiAliasing = (AntiAliasingProxy)i);

            // Camera Far Clip
            //SetupFloatDropdown(m_CameraFarClipDropdown, GameSettings.CameraFarClipValues, _settings.CameraFarClip,
                //i => _settings.farClip = GameSettings.CameraFarClipValues[i]);

            // RenderScale
            //SetupUShortDropdown(m_RenderScaleDd, GameSettings.RenderScaleValues, _settings.VRRenderScale,
                //i => _settings.VRRenderScale = GameSettings.RenderScaleValues[i]);

            m_AudioToggle.isOn = _settings.MusicEnabled;
            m_AudioToggle.onValueChanged.AddListener(b => _settings.MusicEnabled = b);

            //m_GenerateNormalMapsToggle.isOn = _settings.GenerateNormalMaps;
            //m_GenerateNormalMapsToggle.onValueChanged.AddListener(b => _settings.GenerateNormalMaps = b);

            m_AnimateLightsToggle.isOn = _settings.animateLights;
            m_AnimateLightsToggle.onValueChanged.AddListener(b => _settings.animateLights = b);

            m_SunShadowsToggle.isOn = _settings.directionalShadows;
            m_SunShadowsToggle.onValueChanged.AddListener(b => _settings.directionalShadows = b);

            m_LightShadowsToggle.isOn = _settings.additionalShadows;
            m_LightShadowsToggle.onValueChanged.AddListener(b => _settings.additionalShadows = b);

            m_ExteriorLightsToggle.isOn = _settings.exteriorLights;
            m_ExteriorLightsToggle.onValueChanged.AddListener(b => _settings.exteriorLights = b);

            m_DayNightCycleToggle.isOn = _settings.DayNightCycle;
            m_DayNightCycleToggle.onValueChanged.AddListener(b => _settings.DayNightCycle = b);

            m_FollowHeadToggle.isOn = _settings.vrFollowHead;
            m_FollowHeadToggle.onValueChanged.AddListener(b => _settings.vrFollowHead = b);

            m_RoomScaleToggle.isOn = !_settings.vrSeated;
            m_RoomScaleToggle.onValueChanged.AddListener(b => _settings.vrSeated = !b);

            m_Teleportation.isOn = _settings.vrTeleportation;
            m_Teleportation.onValueChanged.AddListener(b => _settings.vrTeleportation = b);

            m_VRAsyncSpaceWarp.isOn = _settings.vrAsyncSpaceWarp;
            m_VRAsyncSpaceWarp.onValueChanged.AddListener(b => _settings.vrAsyncSpaceWarp = b);

            //SetupUShortDropdown(m_VRFFRLevel, GameSettings.VRFFRLevels, _settings.VRFFRLevel,
               // i => _settings.VRFFRLevel = GameSettings.VRFFRLevels[i]);
            //SetupUShortDropdown(m_VRFrequency, GameSettings.VRRefreshRates, _settings.VRRefreshRate,
                //i => _settings.VRRefreshRate = GameSettings.VRRefreshRates[i]);
        }

        private void SetupDropdown<T>(Dropdown dropdown, int value, UnityAction<int> callback)
        {
            var names = Enum.GetNames(typeof(T));

            dropdown.options.Clear();

            for (var i = 0; i < names.Length; i++)
            {
                dropdown.options.Add(new Dropdown.OptionData
                {
                    text = names[i]
                });
            }

            dropdown.value = value;
            dropdown.onValueChanged.AddListener(callback);
        }

        private void SetupFloatDropdown(Dropdown dropdown, float[] array, float value, UnityAction<int> callback)
        {
            dropdown.options.Clear();

            var valueIndex = 0;

            for (var i = 0; i < array.Length; i++)
            {
                dropdown.options.Add(new Dropdown.OptionData
                {
                    text = array[i].ToString()
                });

                if (Mathf.Approximately(array[i], value))
                {
                    valueIndex = i;
                }
            }

            dropdown.value = valueIndex;
            dropdown.onValueChanged.AddListener(callback);
        }

        private void SetupUShortDropdown(Dropdown dropdown, ushort[] array, ushort value, UnityAction<int> callback)
        {
            dropdown.options.Clear();

            var valueIndex = 0;

            for (var i = 0; i < array.Length; i++)
            {
                dropdown.options.Add(new Dropdown.OptionData
                {
                    text = array[i].ToString()
                });

                if (array[i] == value)
                {
                    valueIndex = i;
                }
            }

            dropdown.value = valueIndex;
            dropdown.onValueChanged.AddListener(callback);
        }

        public void ShowMenu()
        {
            GameSettings.Get().Save();
            var menu = GetComponent<MenuComponent>();
            menu.ShowOptions(false);
        }
    }
}