using Demonixis.ToolboxV2.XR;
using UnityEngine;

namespace Demonixis.ToolboxV2
{
    public enum GamePlatform
    {
        Desktop = 0, Mobile, Console, Web
    }

    public class PlatformUtility : MonoBehaviour
    {
        public static bool IsDesktopPlatform()
        {
#if UNITY_STANDALONE
            return true;
#elif UNITY_WSA
            return SystemInfo.deviceType == DeviceType.Desktop;
#else
            return false;
#endif
        }

        public static bool IsWindowsPlatform()
        {
#if UNITY_STANDALONE_WIN
            return false;
#else
            return true;
#endif
        }

        public static bool IsWebPlatform()
        {
#if UNITY_WEBGL
            return true;
#else
            return false;
#endif
        }

        public static bool IsMobilePlatform()
        {
#if UNITY_ANDROID || UNITY_IOS || UNITY_VISIONOS 
            return true;
#elif UNITY_WSA
            return false;
#else
            return SystemInfo.deviceType == DeviceType.Handheld;
#endif
        }
        
        public static bool IsMobileFlatPlatform()
        {
            var xr = XRManager.Enabled;
#if UNITY_ANDROID || UNITY_IOS || UNITY_VISIONOS 
            return !xr;
#elif UNITY_WSA
            return false;
#else
            return !xr && SystemInfo.deviceType == DeviceType.Handheld;
#endif
        }

        public static bool IsMobileVR()
        {
            return IsMobilePlatform() && XRManager.Enabled;
        }

        public static bool IsConsolePlatform()
        {
#if UNITY_XBOXONE || UNITY_PS4 || UNITY_PS5 
            return true;
#elif UNITY_WSA
            var infos = SystemInfo.deviceName.ToLower();
            var isConsole = infos.Contains("xbox");
            return isConsole || SystemInfo.deviceType == DeviceType.Console;
#else
            return SystemInfo.deviceType == DeviceType.Console;
#endif
        }

        public static bool IsXboxOneUWP()
        {
#if  UNITY_WSA
            return IsConsolePlatform();
#else
            return false;
#endif
        }
    }
}