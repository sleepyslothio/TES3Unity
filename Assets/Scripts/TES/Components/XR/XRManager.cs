using Unity.XR.Oculus;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Management;

namespace Demonixis.Toolbox.XR
{
    public enum XRVendor
    {
        None = 0, Oculus, WindowsMR, SteamVR
    }

    public enum XRHeadset
    {
        None = 0, OculusGo, OculusQuest, OculusRift, WindowsMR, HTCVive
    }

    public static class XRManager
    {
        private static bool? XREnabled;

        public static XRLoader GetXRLoader()
        {
            return XRGeneralSettings.Instance?.Manager?.activeLoader;
        }

        public static XRInputSubsystem GetXRInput()
        {
            var xrLoader = GetXRLoader();
            return xrLoader?.GetLoadedSubsystem<XRInputSubsystem>();
        }

        public static bool IsXREnabled(bool force = false)
        {
            if (XREnabled.HasValue && !force)
            {
                return XREnabled.Value;
            }

            var loader = GetXRLoader();
            XREnabled = loader != null;

            return XREnabled.Value;
        }

        public static XRHeadset GetXRHeadset()
        {
            var vendor = GetXRVendor();

            if (vendor != XRVendor.None)
            {
                if (vendor == XRVendor.Oculus)
                {
#if UNITY_ANDROID
                    var oculusLoader =(OculusLoader)GetXRLoader();
                    if (oculusLoader.GetSettings().V2Signing)
                    {
                        return XRHeadset.OculusQuest;
                    }

                    return XRHeadset.OculusGo;
#else
                    return XRHeadset.OculusRift;
#endif
                }
                else if (vendor == XRVendor.SteamVR)
                {
                    return XRHeadset.HTCVive;
                }
                else if (vendor == XRVendor.WindowsMR)
                {
                    return XRHeadset.WindowsMR;
                }
            }

            return XRHeadset.None;
        }

        public static XRVendor GetXRVendor(bool logName = true)
        {
            var xrLoader = GetXRLoader();
            var name = xrLoader?.name.ToLower() ?? string.Empty;

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
            var xrInput = GetXRInput();
            xrInput?.TrySetTrackingOriginMode(origin);

            if (recenter)
            {
                xrInput?.TryRecenter();
            }
        }

        public static void Recenter()
        {
            var xrInput = GetXRInput();
            xrInput?.TryRecenter();
        }
    }
}
