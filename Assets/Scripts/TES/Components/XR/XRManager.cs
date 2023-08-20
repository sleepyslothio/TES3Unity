#if (UNITY_STANDALONE_WIN || UNITY_ANDROID) && !OCULUS_DISABLED
#define OCULUS_SUPPORTED
#endif
#if (UNITY_STANDALONE_WIN || UNITY_WSA || UNITY_ANDROID) && !OPENXR_DISABLED
#define OPENXR_SUPPORTED
#endif

using System.Collections;
using UnityEngine;
using UnityEngine.XR;
using System.Collections.Generic;
using UnityEngine.XR.Management;
using System;

namespace Demonixis.ToolboxV2.XR
{
    #region Enumerations

    public enum XRVendor
    {
        None = 0,
        Oculus,
        WindowsMR,
        OpenVR,
        Sony,
        Pico,
        WaveVR,
        Unknown
    }

    public enum XRHeadset
    {
        None = 0,
        OculusRiftCV1,
        OculusRiftS,
        OculusQuest,
        OculusQuest2,
        OculusQuest3,
        OculusQuestPro,
        OculusGo,
        HTCVive,
        ValveIndex,
        WindowsMR,
        PSVR,
        PSVR2,
        PicoNeo3,
        PicoNeo4,
        ViveFocus3,
        ViveXRElite,
        Unknown
    }

    #endregion

    public static class XRManager
    {
        private static bool? _XREnabled;
        private static readonly List<InputDevice> _inputDevices = new List<InputDevice>();
        public static bool Enabled => IsXREnabled(false);
        public static XRVendor Vendor { get; private set; }
        public static XRHeadset Headset { get; private set; }

        public static bool IsOpenXREnabled()
        {
#if OPENXR_SUPPORTED
            var xrLoader = GetXRLoader();
            if (xrLoader == null) return false;
            return xrLoader is UnityEngine.XR.OpenXR.OpenXRLoader;
#else
            return false;
#endif      
        }

        public static XRLoader GetXRLoader()
        {
            var settings = XRGeneralSettings.Instance;
            if (settings == null) return null;

            var manager = settings.Manager;
            return manager != null ? manager.activeLoader : null;
        }

        public static XRInputSubsystem GetXRInput()
        {
            var xrLoader = GetXRLoader();
            if (xrLoader == null) return null;
            return xrLoader.GetLoadedSubsystem<XRInputSubsystem>();
        }

        public static XRDisplaySubsystem GetDisplay()
        {
            var xrLoader = GetXRLoader();
            if (xrLoader == null) return null;
            return xrLoader.GetLoadedSubsystem<XRDisplaySubsystem>();
        }

        public static bool IsControllerConnected(bool left)
        {
            var input = GetXRInput();
            input.TryGetInputDevices(_inputDevices);

            foreach (var device in _inputDevices)
            {
                if (!device.isValid) continue;
                if (device.characteristics == InputDeviceCharacteristics.Left && left) return true;
                if (device.characteristics == InputDeviceCharacteristics.Right && !left) return true;
            }

            return false;
        }

        public static void SetRenderScale(float scale)
        {
            XRSettings.renderViewportScale = Mathf.Clamp(scale, 0.5f, 1.0f);
        }

        public static bool IsXREnabled(bool force = false)
        {
            if (_XREnabled.HasValue && !force)
                return _XREnabled.Value;

            var loader = GetXRLoader();

            _XREnabled = loader != null;

#if OPENXR_SUPPORTED
            if (loader is UnityEngine.XR.OpenXR.OpenXRLoader)
            {
                Vendor = ParseVendor(UnityEngine.XR.OpenXR.OpenXRRuntime.name);
                Headset = ParseHeadset(UnityEngine.XR.OpenXR.OpenXRRuntime.name);
            }
#endif

#if OCULUS_SUPPORTED
            if (loader is Unity.XR.Oculus.OculusLoader)
            {
                Vendor = XRVendor.Oculus;

                switch (Unity.XR.Oculus.Utils.GetSystemHeadsetType())
                {
                    case Unity.XR.Oculus.SystemHeadset.Oculus_Quest:
                    case Unity.XR.Oculus.SystemHeadset.Oculus_Link_Quest:
                        Headset = XRHeadset.OculusQuest;
                        break;
                    case Unity.XR.Oculus.SystemHeadset.Oculus_Link_Quest_2:
                    case Unity.XR.Oculus.SystemHeadset.Oculus_Quest_2:
                        Headset = XRHeadset.OculusQuest2;
                        break;
                    case Unity.XR.Oculus.SystemHeadset.Rift_CV1:
                        Headset = XRHeadset.OculusRiftCV1;
                        break;
                    case Unity.XR.Oculus.SystemHeadset.Rift_S:
                        Headset = XRHeadset.OculusRiftS;
                        break;
                    default:
                        Headset = XRHeadset.OculusQuest2;
                        break;
                }
            }
#endif

#if PICO_LEGACYXR_SUPPORT
            if (loader is Unity.XR.PXR.PXR_Loader)
            {
                Vendor = XRVendor.Pico;

                var controller = Unity.XR.PXR.PXR_Input.GetActiveController();

                switch (controller)
                {
                    case Unity.XR.PXR.PXR_Input.ControllerDevice.Neo3:
                        Headset = XRHeadset.PicoNeo3;
                        break;
                    case Unity.XR.PXR.PXR_Input.ControllerDevice.NewController:
                        Headset = XRHeadset.PicoNeo4;
                        break;
                    default:
                        Headset = XRHeadset.PicoNeo3;
                        break;
                }
            }
#endif

            return _XREnabled.Value;
        }

        public static void TryInitialize()
        {
            var manager = XRGeneralSettings.Instance.Manager;

            if (manager.activeLoader != null) return;

            manager.InitializeLoaderSync();
            manager.StartSubsystems();
        }

        public static void TryShutdown()
        {
            var manager = XRGeneralSettings.Instance.Manager;

            if (manager.activeLoader == null) return;

            manager.DeinitializeLoader();
            manager.StopSubsystems();
        }

        private static bool InArray(string word, params string[] terms)
        {
            foreach (var term in terms)
            {
                if (word.Contains(term))
                    return true;
            }

            return false;
        }

        private static bool InArrayAnd(string word, params string[] terms)
        {
            var counter = 0;
            foreach (var term in terms)
            {
                if (word.Contains(term))
                    counter++;
            }

            return counter == terms.Length;
        }

        public static XRVendor ParseVendor(string name)
        {
            if (string.IsNullOrEmpty(name))
                return XRVendor.None;

            var mobile = Application.isMobilePlatform;
            name = name.ToLower();

            if (name.Contains("oculus"))
            {
                return XRVendor.Oculus;
            }
            else if (InArray(name, "windows", "holographic"))
            {
                return XRVendor.WindowsMR;
            }
            else if (mobile && InArray(name, "focus", "vive"))
            {
                return XRVendor.WaveVR;
            }
            else if (!mobile && InArray(name, "valve", "vive", "steam", "openvr"))
            {
                return XRVendor.OpenVR;
            }
            else if (InArray(name, "sony", "psvr"))
            {
                return XRVendor.Sony;
            }
            else if (InArray(name, "pico"))
            {
                return XRVendor.Pico;
            }

            return XRVendor.Unknown;
        }

        public static XRHeadset ParseHeadset(string name)
        {
            if (string.IsNullOrEmpty(name))
                return XRHeadset.None;

            var mobile = Application.isMobilePlatform;
            name = name.ToLower();

            if (name.Contains("oculus"))
            {
                if (InArray(name, "rift", "cv1"))
                {
                    return XRHeadset.OculusRiftCV1;
                }
                else if (InArray(name, "rift", "rift s", "rift-s"))
                {
                    return XRHeadset.OculusRiftS;
                }
                else if (InArray(name, "go"))
                {
                    return XRHeadset.OculusGo;
                }
                else if (name.Contains("quest"))
                {
                    if (name.Contains("pro"))
                        return XRHeadset.OculusQuestPro;

                    if (name.Contains("2"))
                        return XRHeadset.OculusQuest2;

                    if (name.Contains("3"))
                        return XRHeadset.OculusQuest3;

                    return XRHeadset.OculusQuest;
                }

                return XRHeadset.OculusQuest;
            }
            else if (InArray(name, "windows", "reverb"))
            {
                return XRHeadset.WindowsMR;
            }
            else if (name.Contains("vive"))
            {
                if (name.Contains("focus"))
                    return XRHeadset.ViveFocus3;

                if (InArray(name, "vive", "xr", "elite"))
                    return XRHeadset.ViveXRElite;

                return XRHeadset.HTCVive;
            }
            else if (InArray("valve", "index"))
            {
                return XRHeadset.ValveIndex;
            }
            else if (InArray("sony", "psvr"))
            {
                if (name.Contains("2"))
                    return XRHeadset.PSVR2;

                return XRHeadset.PSVR;
            }
            else if (name.Contains("pico"))
            {
                if (name.Contains("3"))
                    return XRHeadset.PicoNeo3;

                return XRHeadset.PicoNeo4;
            }

            return XRHeadset.Unknown;
        }

        public static IEnumerator GetXRInfos(Action<XRVendor, XRHeadset> callback)
        {
            var vendor = XRVendor.None;
            var headset = XRHeadset.None;

            var head = new List<InputDevice>();
            var left = new List<InputDevice>();
            var right = new List<InputDevice>();

            do
            {
                InputDevices.GetDevicesWithCharacteristics(InputDeviceCharacteristics.HeadMounted | InputDeviceCharacteristics.TrackedDevice, head);
                InputDevices.GetDevicesWithCharacteristics(InputDeviceCharacteristics.Controller | InputDeviceCharacteristics.Left | InputDeviceCharacteristics.TrackedDevice, left);
                InputDevices.GetDevicesWithCharacteristics(InputDeviceCharacteristics.Controller | InputDeviceCharacteristics.Right | InputDeviceCharacteristics.TrackedDevice, right);
                yield return null;
            }
            while (head.Count + left.Count + right.Count < 2);

            foreach (var h in head)
            {
                vendor = ParseVendor(h.name);
                headset = ParseHeadset(h.name);
            }

            Vendor = Vendor;
            Headset = headset;
            callback(Vendor, Headset);
        }

        public static void SetTrackingOriginMode(TrackingOriginModeFlags origin, bool recenter)
        {
            var xrInput = GetXRInput();
            if (xrInput == null) return;

            xrInput.TrySetTrackingOriginMode(origin);

            if (recenter)
                Recenter();
        }

        public static void Recenter()
        {
            var subsystems = new List<XRInputSubsystem>();
            SubsystemManager.GetInstances(subsystems);

            foreach (var subsystem in subsystems)
                subsystem.TryRecenter();
        }

        public static void Vibrate(MonoBehaviour target, XRNode node, float amplitude = 0.5f, float seconds = 0.25f)
        {
            if (!target.gameObject.activeSelf || !target.gameObject.activeInHierarchy) return;

            var device = InputDevices.GetDeviceAtXRNode(node);

            if (!device.TryGetHapticCapabilities(out HapticCapabilities capabilities)) return;

            if (capabilities.supportsBuffer)
            {
                byte[] buffer = { };

                if (GenerateBuzzClip(seconds, node, ref buffer))
                    device.SendHapticBuffer(0, buffer);
            }
            else if (capabilities.supportsImpulse)
                device.SendHapticImpulse(0, amplitude, seconds);

            target.StartCoroutine(StopVibrationCoroutine(device, seconds));
        }

        private static IEnumerator StopVibrationCoroutine(InputDevice device, float delay)
        {
            if (delay > 0)
                yield return new WaitForSeconds(delay);

            device.StopHaptics();
        }

        public static bool GenerateBuzzClip(float seconds, XRNode node, ref byte[] clip)
        {
            var device = InputDevices.GetDeviceAtXRNode(node);
            var result = device.TryGetHapticCapabilities(out HapticCapabilities caps);

            if (result)
            {
                var clipCount = (int)(caps.bufferFrequencyHz * seconds);
                clip = new byte[clipCount];

                for (int i = 0; i < clipCount; i++)
                {
                    clip[i] = byte.MaxValue;
                }
            }

            return result;
        }
    }
}