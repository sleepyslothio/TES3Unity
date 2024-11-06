using Demonixis.ToolboxV2;
using UnityEngine;

namespace Demonixis.ToolboxV2.Graphics
{
    public class TerrainSwitcher : MonoBehaviour
    {
        [SerializeField]
        private GameObject m_TerrainHQ = null;
        [SerializeField]
        private GameObject m_TerrainLQ = null;

        private void Awake()
        {
            var lq = PlatformUtility.IsMobilePlatform() || PlatformUtility.IsXboxOneUWP();
            m_TerrainHQ.SetActive(!lq);
            m_TerrainLQ.SetActive(lq);
        }
    }
}
