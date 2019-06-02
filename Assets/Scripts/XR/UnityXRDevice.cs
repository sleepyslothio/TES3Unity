/// UnityVRDevice
/// Last Modified Date: 01/07/2017
using UnityEngine;
using UnityEngine.XR;

namespace Demonixis.Toolbox.XR
{
    public enum VRHeadsetModel
    {
        None = 0, OculusRift,
        OculusQuest, OculusGo,
        Vive, WindowsMR,
        PSVR, Other
    }

    /// <summary>
    /// The UnityVRDevice is an abstract device that uses the UnityEngine.VR implementation.
    /// </summary>
    public class UnityXRDevice : XRDeviceBase
    {
        #region Public Fields

#if OCULUS_SDK
        [SerializeField]
        private OVRManager.TiledMultiResLevel m_TiledMultiResLevel = OVRManager.TiledMultiResLevel.Off;
#endif
        public override float RenderScale
        {
            get => XRSettings.eyeTextureResolutionScale;
            set => XRSettings.eyeTextureResolutionScale = value;
        }

        public override int EyeTextureWidth => XRSettings.eyeTextureWidth;
        public override int EyeTextureHeight => XRSettings.eyeTextureHeight;
        public override XRDeviceType VRDeviceType => XRDeviceType.UnityXR;
        public override Vector3 HeadPosition => InputTracking.GetLocalPosition(XRNode.Head);
        public override bool IsAvailable => XRSettings.enabled;

        #endregion

        public static bool IsOculus => XRSettings.loadedDeviceName.ToLower() == "oculus";

        public static VRHeadsetModel GetVRHeadsetModel()
        {
            if (!XRSettings.enabled)
                return VRHeadsetModel.None;

#if UNITY_PS4
            return VRHeadsetModel.PSVR;
#endif

            var device = XRDevice.model.ToLower();

            if (device.Contains("oculus"))
            {
#if UNITY_ANDROID
                return device.Contains("quest") ? VRHeadsetModel.OculusQuest : VRHeadsetModel.OculusGo;
#else
                return VRHeadsetModel.OculusRift;
#endif
            }
            else if (device.Contains("vive"))
                return VRHeadsetModel.Vive;
            else if (device.Contains("windows"))
                return VRHeadsetModel.WindowsMR;

            return VRHeadsetModel.Other;
        }

        public override void Recenter() => InputTracking.Recenter();

        public override void SetActive(bool active)
        {
            if (XRSettings.enabled != active)
                XRSettings.enabled = active;

#if OCULUS_SDK
            if (IsOculus)
            {
                var manager = gameObject.AddComponent<OVRManager>();
                OVRManager.tiledMultiResLevel = m_TiledMultiResLevel;
            }
#endif

            Debug.Log($"[GSPVR] IsOculus: {IsOculus}");
        }

        public override void SetTrackingSpaceType(TrackingSpaceType type, Transform headTransform, float height)
        {
#if UNITY_ANDROID
            if (XRManager.Instance.ForceSeatedOnMobile && GetVRHeadsetModel() == VRHeadsetModel.OculusQuest)
                type = TrackingSpaceType.Stationary;
#endif
            XRDevice.SetTrackingSpaceType(type);

            var position = headTransform.localPosition;
            headTransform.localPosition = new Vector3(position.x, type == TrackingSpaceType.RoomScale ? 0 : height, position.z);

            InputTracking.Recenter();
        }
    }
}
