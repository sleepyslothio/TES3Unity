using UnityEngine;

namespace Demonixis.ToolboxV2.Graphics
{
    public sealed class RealtimeLightingManager : MonoBehaviour
    {
        [SerializeField] private Material[] m_EmissiveMaterials = null;
        [SerializeField] private bool m_RefreshReflectionProbes = false;

        [ContextMenu("Set Emissive Lights On")]
        public void SetLightsOn()
        {
            SetEmissiveLightsOn(true);
        }

        [ContextMenu("Set Emissive Lights Off")]
        public void SetLightsOff()
        {
            SetEmissiveLightsOn(false);
        }

        public void SetEmissiveLightsOn(bool on)
        {
            foreach (var material in m_EmissiveMaterials)
            {
                if (on)
                {
                    material.EnableKeyword("_EMISSION");
                }
                else
                {
                    material.DisableKeyword("_EMISSION");
                }
            }

            if (!m_RefreshReflectionProbes) return;

            var reflectionProbes = FindObjectsByType<ReflectionProbe>(FindObjectsSortMode.None);
            foreach (var reflectionProbe in reflectionProbes)
            {
                reflectionProbe.RenderProbe();
            }
        }
    }
}
