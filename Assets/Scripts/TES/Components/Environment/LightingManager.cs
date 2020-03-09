using System.Collections;
using TES3Unity.ESM.Records;
using UnityEngine;
using UnityEngine.Rendering;

namespace TES3Unity.Components
{
    public sealed class LightingManager : MonoBehaviour
    {
        private Light m_Sun = null;
        private Transform m_Transform = null;
        private Quaternion m_OriginalOrientation;
        private Color m_DefaultSunLightColor;
        private Color32 m_DefaultAmbientColor = new Color32(137, 140, 160, 255);
        private bool m_DayNightCycle = false;
        

        [SerializeField]
        private float m_RotationTime = 0.5f;

        private void Start()
        {
            var config = GameSettings.Get();

            m_Sun = GetComponent<Light>();
            m_Sun.shadows = config.SunShadows ? LightShadows.Soft : LightShadows.None;
            m_DefaultSunLightColor = m_Sun.color;

            m_Transform = transform;
            m_OriginalOrientation = m_Transform.rotation;

            RenderSettings.sun = GetComponent<Light>();

            m_DayNightCycle = config.DayNightCycle;

            var engine = TES3Engine.Instance;
            engine.CurrentCellChanged += OnCurrentCellChanged;
            OnCurrentCellChanged(engine.CurrentCell);

#if UNITY_ANDROID
            RenderSettings.ambientIntensity = 4;
#endif
        }

        private void Update()
        {
            if (m_DayNightCycle)
            {
                m_Transform.Rotate(m_RotationTime * Time.deltaTime, 0.0f, 0.0f);
            }
        }

        private void OnCurrentCellChanged(CELLRecord cell)
        {
            if (cell == null)
            {
                return;
            }

            var ambientColor = m_DefaultAmbientColor;
            var sunColor = m_DefaultSunLightColor;
            var ambientData = cell.AMBI;

            if (ambientData != null)
            {
                ambientColor = ColorUtils.B8G8R8ToColor32(ambientData.ambientColor);
                sunColor = ColorUtils.B8G8R8ToColor32(ambientData.sunlightColor);
            }

            RenderSettings.ambientLight = ambientColor;
            RenderSettings.ambientMode = cell.isInterior ? AmbientMode.Flat : AmbientMode.Skybox;
            RenderSettings.ambientIntensity = TES3Manager.Instance.ambientIntensity;

            m_Sun.enabled = !cell.isInterior;
            m_Sun.color = sunColor;
        }
    }
}
