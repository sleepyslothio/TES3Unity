using UnityEngine;
using UnityEngine.Rendering;

namespace Demonixis.ToolboxV2.Graphics
{
    public class VolumeProfileSwitcher : MonoBehaviour
    {
        [SerializeField]
        private VolumeProfile m_DesktopProfile = null;
        [SerializeField]
        private VolumeProfile m_MobileProfile = null;

        private void Awake()
        {
            var volume = GetComponent<Volume>();
            volume.sharedProfile = PlatformUtility.IsMobilePlatform() ? m_MobileProfile : m_DesktopProfile;
        }

        public void SetDesktopProfile()
        {
            var volume = GetComponent<Volume>();
            volume.sharedProfile = m_DesktopProfile;
        }

        public void SetMobileProfile()
        {
            var volume = GetComponent<Volume>();
            volume.sharedProfile = m_MobileProfile;
        }
    }
}