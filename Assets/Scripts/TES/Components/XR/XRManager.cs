using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Management;

namespace Demonixis.Toolbox.XR
{
    public enum XRVendor
    {
        None = 0, Oculus, WindowsMR, SteamVR
    }

    public static class XRManager
    {
        public static bool IsXREnabled()
        {
            var loader = XRGeneralSettings.Instance?.Manager?.activeLoader;
            return loader != null;
        }

        public static XRVendor GetXRVendor(bool logName = true)
        {
            var xrLoader = XRGeneralSettings.Instance?.Manager?.activeLoader;
            var name = xrLoader?.name.ToLower() ?? string.Empty;

            Debug.Log(name);

            if (name.Contains("oculus"))
            {
                return XRVendor.Oculus;
            }
            else if (name.Contains("windows"))
            {
                return XRVendor.WindowsMR;
            }
            else if (name.Contains("steamvr") || name.Contains("openvr"))
            {
                return XRVendor.SteamVR;
            }

            return XRVendor.None;
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
