using UnityEngine;

namespace Demonixis.ToolboxV2.Utils
{
    [RequireComponent(typeof(Light))]
    public sealed class BlinkLight : MonoBehaviour
    {
        private float m_ElapsedTime = 0.0f;
        private Light m_Light = null;
        private float m_OriginalIntensity = 1.0f;
#if UNITY_EDITOR
        private bool m_LastPreviewState = true;
#endif

        [SerializeField]
        private AnimationCurve m_LightCurve = null;
        [SerializeField]
        private float m_Duration = 10.0f;
        [SerializeField]
        private float m_IntensityScale = 1.0f;
        [SerializeField]
        private bool m_RandomStart = true;

#if UNITY_EDITOR
        [Header("Editor")]
        [SerializeField]
        private bool m_Preview = true;
#endif

        private void Start()
        {
            m_Light = GetComponent<Light>();
            m_OriginalIntensity = m_Light.intensity;

            if (m_RandomStart)
            {
                m_ElapsedTime = Random.Range(0, m_Duration);
            }

#if UNITY_EDITOR
            m_LastPreviewState = m_Preview;
#endif
        }

        private void Update()
        {
#if UNITY_EDITOR
            if (!UnityEditor.EditorApplication.isPlaying && !m_Preview)
            {
                return;
            }
#endif
            if (m_LightCurve == null || m_LightCurve.length == 0)
            {
                Debug.LogWarning("The LightCurve is not set.");
                enabled = false;
                return;
            }

            if (m_Light == null)
            {
                return;
            }

            m_ElapsedTime += Time.deltaTime;

            if (m_ElapsedTime > m_Duration)
            {
                m_ElapsedTime = 0.0f;
            }

            var eval = m_LightCurve.Evaluate(m_ElapsedTime / m_Duration) / (1.0f / m_IntensityScale);
            m_Light.intensity = m_OriginalIntensity * eval;
        }

#if UNITY_EDITOR
        private void OnRenderObject()
        {
            if (!UnityEditor.EditorApplication.isPlaying && !m_Preview)
            {
                return;
            }

            if (m_Light == null)
            {
                m_Light = GetComponent<Light>();
            }

            if (m_LastPreviewState != m_Preview)
            {
                m_LastPreviewState = m_Preview;
                m_ElapsedTime = 0;
                m_Light.intensity = m_OriginalIntensity;
            }

            Update();
        }
#endif
    }
}