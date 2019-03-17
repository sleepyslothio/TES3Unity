using UnityEngine;
using UnityEngine.UI;

namespace TESUnity.Components
{
    public class MenuOptionComponent : MonoBehaviour
    {
        private GameSettings m_Settings = null;

        [SerializeField]
        private Toggle m_AudioToggle = null;
        [SerializeField]
        private Dropdown m_CellRadiusDd = null;
        [SerializeField]
        private Dropdown m_CellDistanceDd = null;
        [SerializeField]
        private Dropdown m_PostProcessDd = null;
        [SerializeField]
        private Dropdown m_MaterialDd = null;
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
        private Dropdown m_CameraFarClipDropdown = null;
        [SerializeField]
        private Toggle m_FollowHeadToggle = null;
        [SerializeField]
        private Toggle m_RoomScaleToggle = null;
        [SerializeField]
        private Dropdown m_RenderScaleDd = null;

        private void Awake()
        {
            m_Settings = GameSettings.Get();

            m_AudioToggle.isOn = m_Settings.Audio;
            m_AudioToggle.onValueChanged.AddListener(SetAudio);

            m_CellRadiusDd.value = m_Settings.CellRadius;
            m_CellRadiusDd.onValueChanged.AddListener(SetCellRadius);

            m_CellDistanceDd.value = m_Settings.CellDistance;
            m_CellDistanceDd.onValueChanged.AddListener(SetCellDistance);

            m_PostProcessDd.value = (int)m_Settings.PostProcessing;
            m_PostProcessDd.onValueChanged.AddListener(SetPostProcessing);

            m_MaterialDd.value = (int)m_Settings.Material;
            m_MaterialDd.onValueChanged.AddListener(SetMaterialQuality);

            m_GenerateNormalMapsToggle.isOn = m_Settings.GenerateNormalMaps;
            m_GenerateNormalMapsToggle.onValueChanged.AddListener(SetGenerateNormalMaps);

            m_AnimateLightsToggle.isOn = m_Settings.AnimateLights;
            m_AnimateLightsToggle.onValueChanged.AddListener(SetAnimateLights);

            m_SunShadowsToggle.isOn = m_Settings.SunShadows;
            m_SunShadowsToggle.onValueChanged.AddListener(SetSunShadow);

            m_LightShadowsToggle.isOn = m_Settings.LightShadows;
            m_LightShadowsToggle.onValueChanged.AddListener(SetLightShadows);

            m_ExteriorLightsToggle.isOn = m_Settings.ExteriorLight;
            m_ExteriorLightsToggle.onValueChanged.AddListener(SetExteriorLights);

            m_CameraFarClipDropdown.value = GetCameraFarClip();
            m_CameraFarClipDropdown.onValueChanged.AddListener(SetCameraFarClip);

            m_FollowHeadToggle.isOn = m_Settings.VRFollowHead;
            m_FollowHeadToggle.onValueChanged.AddListener(SetVRFollowHead);

            m_RoomScaleToggle.isOn = m_Settings.VRFollowHead;
            m_RoomScaleToggle.onValueChanged.AddListener(SetRoomScale);

            m_RenderScaleDd.value = GetRenderScaleIndex();
            m_RenderScaleDd.onValueChanged.AddListener(SetRenderScale);
        }

        private int GetRenderScaleIndex()
        {
            var value = (int)(m_Settings.RenderScale * 10.0f);

            if (value == 10)
                return 5;
            else if (value == 9)
                return 4;
            else if (value == 8)
                return 3;
            else if (value == 7)
                return 2;
            else if (value == 6)
                return 1;

            return 0;
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
            m_Settings.Audio = isOn;
        }

        public void SetPostProcessing(int level)
        {
            m_Settings.PostProcessing = (PostProcessingQuality)level;
        }

        public void SetMaterialQuality(int level)
        {
            m_Settings.Material = (MWMaterialType)level;
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

        public void SetVRFollowHead(bool isOn)
        {
            m_Settings.VRFollowHead = isOn;
        }

        public void SetRenderScale(int level)
        {
            // Index[0] = 50 so RenderScale = Index + 50 / 100
            m_Settings.RenderScale = ((float)level + 50.0f) / 100.0f;
        }

        public void SetCellRadius(int radius)
        {
            m_Settings.CellRadius = radius;
        }

        public void SetCellDistance(int distance)
        {
            m_Settings.CellDistance = distance;
        }

        private void SetLightShadows(bool isOn)
        {
            m_Settings.LightShadows = isOn;
        }

        private void SetExteriorLights(bool isOn)
        {
            m_Settings.ExteriorLight = isOn;
        }

        private void SetCameraFarClip(int level)
        {
            var value = 1000;

            if (level == 1)
                value = 500;
            else if (level == 2)
                value = 250;
            else if (level == 3)
                value = 150;
            else if (level == 4)
                value = 100;
            else
                value = 50;

            m_Settings.CameraFarClip = value;
        }

        private void SetRoomScale(bool isOn)
        {
            m_Settings.VRRoomScale = isOn;
        }
    }
}