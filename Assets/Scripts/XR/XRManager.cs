using UnityEngine.XR;
using UnityEngine.XR.Management;

namespace Demonixis.Toolbox.XR
{
    public static class XRManager
    {
        public static bool IsXREnabled()
        {
            var loader = XRGeneralSettings.Instance?.Manager?.activeLoader;
            return loader != null;
        }

        public static void SetTrackingOriginMode(TrackingOriginModeFlags origin, bool recenter)
        {
            var xrLoader = XRGeneralSettings.Instance?.Manager?.activeLoader;
            var xrInput = xrLoader?.GetLoadedSubsystem<XRInputSubsystem>();
            xrInput?.TrySetTrackingOriginMode(origin);

            if (recenter)
            {
                xrInput?.TryRecenter();
            }
        }

        public static void Recenter()
        {
            var xrLoader = XRGeneralSettings.Instance?.Manager?.activeLoader;
            var xrInput = xrLoader?.GetLoadedSubsystem<XRInputSubsystem>();
            xrInput?.TryRecenter();
        }
    }
}
