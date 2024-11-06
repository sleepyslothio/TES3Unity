using Demonixis.ToolboxV2.XR;
using UnityEngine;
using UnityEngine.Rendering;

namespace Demonixis.ToolboxV2.Graphics
{
    public class VolumeProfileSwitcher : MonoBehaviour
    {
        [SerializeField] private VolumeProfile _desktopProfile;
        [SerializeField] private VolumeProfile _desktopProfileVr;
        [SerializeField] private VolumeProfile _mobileProfile;
        [SerializeField] private VolumeProfile _mobileProfileVr;

        private void Awake()
        {
            var xr = XRManager.Enabled;
            var mobileProfile = xr ? _mobileProfileVr : _mobileProfile;
            var desktopProfile = xr ? _desktopProfileVr : _desktopProfile;
            
            var volume = GetComponent<Volume>();
            volume.sharedProfile = PlatformUtility.IsMobilePlatform() ? mobileProfile : desktopProfile;
        }

        public void SetDesktopProfile()
        {
            var volume = GetComponent<Volume>();
            volume.sharedProfile = _desktopProfile;
        }

        public void SetMobileProfile()
        {
            var volume = GetComponent<Volume>();
            volume.sharedProfile = _mobileProfile;
        }
    }
}