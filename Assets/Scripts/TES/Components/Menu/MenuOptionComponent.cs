using Demonixis.Toolbox.UI;
using UnityEngine;
using UnityEngine.UI;

namespace TES3Unity.Components
{
    public class MenuOptionComponent : MonoBehaviour
    {
        private GameSettings m_Settings = null;

        [SerializeField]
        private Toggle m_AudioToggle = null;
        [SerializeField]
        private UISelectorWidget m_CellRadiusDd = null;
        [SerializeField]
        private UISelectorWidget m_CellDistanceDd = null;
        [SerializeField]
        private UISelectorWidget m_CellRadiusLoad = null;
        [SerializeField]
        private UISelectorWidget m_PostProcessDd = null;
        [SerializeField]
        private UISelectorWidget m_RenderPath = null;
        [SerializeField]
        private UISelectorWidget m_SRPQuality = null;
        [SerializeField]
        private Toggle m_GenerateNormalMapsToggle = null;
        [SerializeField]
        private Toggle m_AnimateLightsToggle = null;
        [SerializeField]
        private Toggle m_SunShadowsToggle = null;
        [SerializeField]
        private Toggle m_LightShadowsToggle = null;
        [SerializeField]
        private Toggle m_ExteriorLightsToggle = null;
        [SerializeField]
        private Toggle m_DayNightCycleToggle = null;
        [SerializeField]
        private UISelectorWidget m_CameraFarClipDropdown = null;
        [SerializeField]
        private Toggle m_FollowHeadToggle = null;
        [SerializeField]
        private Toggle m_RoomScaleToggle = null;
        [SerializeField]
        private UISelectorWidget m_RenderScaleDd = null;

        private void Awake()
        {
            m_Settings = GameSettings.Get();

            m_AudioToggle.isOn = m_Settings.MusicEnabled;
            m_AudioToggle.onValueChanged.AddListener(SetAudio);

            var values = new[] { "1", "2", "3", "4" };
            m_CellRadiusDd.Setup(ref values, m_Settings.CellRadius.ToString(), SetCellRadius);
            m_CellDistanceDd.Setup(ref values, m_Settings.CellDetailRadius.ToString(), SetCellDistance);
            m_CellRadiusLoad.Setup(ref values, m_Settings.CellRadiusOnLoad.ToString(), SetCellRadiusLoad);

            m_PostProcessDd.Setup<PostProcessingQuality>((int)m_Settings.PostProcessingQuality, SetPostProcessing);
            m_RenderPath.Setup<RendererMode>((int)m_Settings.RendererMode, SetRenderType);
            m_SRPQuality.Setup<SRPQuality>((int)m_Settings.SRPQuality, SetSRPQuality);

            m_GenerateNormalMapsToggle.isOn = m_Settings.GenerateNormalMaps;
            m_GenerateNormalMapsToggle.onValueChanged.AddListener(SetGenerateNormalMaps);

            m_AnimateLightsToggle.isOn = m_Settings.AnimateLights;
            m_AnimateLightsToggle.onValueChanged.AddListener(SetAnimateLights);

            m_SunShadowsToggle.isOn = m_Settings.SunShadows;
            m_SunShadowsToggle.onValueChanged.AddListener(SetSunShadow);

            m_LightShadowsToggle.isOn = m_Settings.LightShadows;
            m_LightShadowsToggle.onValueChanged.AddListener(SetLightShadows);

            m_ExteriorLightsToggle.isOn = m_Settings.ExteriorLights;
            m_ExteriorLightsToggle.onValueChanged.AddListener(SetExteriorLights);

            values = new[] { "50", "100", "150", "250", "500", "1000" };
            m_CameraFarClipDropdown.Setup(ref values, m_Settings.CameraFarClip.ToString(), SetCameraFarClip);

            m_DayNightCycleToggle.isOn = m_Settings.DayNightCycle;
            m_DayNightCycleToggle.onValueChanged.AddListener(SetDayNightCycle);

            m_FollowHeadToggle.isOn = m_Settings.FollowHead;
            m_FollowHeadToggle.onValueChanged.AddListener(SetVRFollowHead);

            m_RoomScaleToggle.isOn = m_Settings.RoomScale;
            m_RoomScaleToggle.onValueChanged.AddListener(SetRoomScale);

            values = new[] { "50", "60", "70", "80", "90", "100" };
            m_RenderScaleDd.Setup(ref values, m_Settings.RenderScale.ToString(), SetRenderScale);
        }

        private int GetRenderScaleIndex()
        {
            var value = (int)(m_Settings.RenderScale * 10.0f);

            if (value == 10)
                return 0;
            else if (value == 9)
                return 1;
            else if (value == 8)
                return 2;
            else if (value == 7)
                return 3;
            else if (value == 6)
                return 4;

            return 5;
        }

        private int GetCameraFarClip()
        {
            var value = m_Settings.CameraFarClip;

            if (value == 1000)
                return 0;
            else if (value == 500)
                return 1;
            else if (value == 250)
                return 2;
            else if (value == 150)
                return 3;
            else if (value == 100)
                return 4;

            return 5;
        }

        public void ShowMenu()
        {
            GameSettings.Save();
            var menu = GetComponent<MenuComponent>();
            menu.ShowOptions(false);
        }

        public void SetAudio(bool isOn)
        {
            m_Settings.MusicEnabled = isOn;
        }

        public void SetPostProcessing(int index)
        {
            m_Settings.PostProcessingQuality = (PostProcessingQuality)index;
        }

        public void SetRenderType(int index)
        {
            m_Settings.RendererMode = (RendererMode)index;
        }

        public void SetSRPQuality(int index)
        {
            m_Settings.SRPQuality = (SRPQuality)index;
        }

        public void SetGenerateNormalMaps(bool isOn)
        {
            m_Settings.GenerateNormalMaps = isOn;
        }

        public void SetAnimateLights(bool isOn)
        {
            m_Settings.AnimateLights = isOn;
        }

        public void SetSunShadow(bool isOn)
        {
            m_Settings.SunShadows = isOn;
        }

        public void SetDayNightCycle(bool isOn)
        {
            m_Settings.DayNightCycle = isOn;
        }

        public void SetVRFollowHead(bool isOn)
        {
            m_Settings.FollowHead = isOn;
        }

        public void SetRenderScale(string value)
        {
            if (float.TryParse(value, out float result))
                m_Settings.RenderScale = result;
            else
                Debug.LogWarning($"Can't parse the value {value}");
        }

        public void SetCellRadius(string value)
        {
            if (int.TryParse(value, out int result))
                m_Settings.CellRadius = result;
            else
                Debug.LogWarning($"Can't parse the value {value}");
        }

        public void SetCellDistance(string value)
        {
            if (int.TryParse(value, out int result))
                m_Settings.CellDetailRadius = result;
            else
                Debug.LogWarning($"Can't parse the value {value}");
        }

        public void SetCellRadiusLoad(string value)
        {
            if (int.TryParse(value, out int result))
                m_Settings.CellRadiusOnLoad = result;
            else
                Debug.LogWarning($"Can't parse the value {value}");
        }

        private void SetLightShadows(bool isOn)
        {
            m_Settings.LightShadows = isOn;
        }

        private void SetExteriorLights(bool isOn)
        {
            m_Settings.ExteriorLights = isOn;
        }

        private void SetCameraFarClip(string value)
        {
            if (float.TryParse(value, out float result))
                m_Settings.CameraFarClip = result;
            else
                Debug.LogWarning($"Can't parse the value {value}");
        }

        private void SetRoomScale(bool isOn)
        {
            m_Settings.RoomScale = isOn;
        }
    }
}