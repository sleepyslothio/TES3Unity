using System.Collections;
using UnityEngine;
using UnityEngine.XR;

namespace Demonixis.Toolbox.XR
{
    /// <summary>
    /// The UnityVRDevice is an abstract device that uses the UnityEngine.VR implementation.
    /// </summary>
    public class WaveVRDevice : XRDeviceBase
    {
        [SerializeField]
        private bool m_ReplaceTrackedPoseDriver = false;
        [SerializeField]
        private bool m_ControllerFollowsHead = false;
#if UNITY_EDITOR
        [SerializeField]
        private bool m_DisableInEditor = false;

        public bool DisableInEditor => m_DisableInEditor;
#endif

#if WAVEVR_SDK
        private WaveVR_Render m_Render = null;
#endif

        public override bool IsAvailable
        {
            get
            {
#if WAVEVR_SDK
#if UNITY_EDITOR
                if (m_DisableInEditor)
                    return false;
#endif
                return true;
#else
                return false;
#endif
            }
        }

        public override XRDeviceType VRDeviceType => XRDeviceType.WaveVR;

        public override float RenderScale { get; set; } = 1.0f;

        public override int EyeTextureWidth
        {
#if WAVEVR_SDK
            get => (int)(m_Render?.sceneWidth ?? Screen.width);
#else
            get => Screen.width;
#endif
        }

        public override int EyeTextureHeight
        {
#if WAVEVR_SDK
            get => (int)(m_Render?.sceneHeight ?? Screen.height);
#else
            get => Screen.height;
#endif
        }

        public override bool IsDualCamera => true;

        public override void Recenter()
        {
        }

        public override void SetActive(bool isEnabled)
        {
#if WAVEVR_SDK
            var camera = GetComponentInChildren<Camera>(true);
            var camGo = camera.gameObject;

            // Ears
            Destroy(camGo.GetComponent<AudioListener>());

            // Renderer
            m_Render = camGo.AddComponent<WaveVR_Render>();

            // Tracker
            var deviceTracker = camGo.AddComponent<WaveVR_DevicePoseTracker>();
            deviceTracker.type = wvr.WVR_DeviceType.WVR_DeviceType_HMD;

            if (m_ReplaceTrackedPoseDriver)
            {
                var drivers = GetComponentsInChildren<MotionController>();
                foreach (var driver in drivers)
                {
                    AddControllerSupport(driver.transform, driver.LeftHand, m_ControllerFollowsHead);
                    driver.enabled = false;
                }
            }
#endif
        }

#if WAVEVR_SDK
        public static void AddControllerSupport(Transform target, bool left, bool followHead)
        {
            var tracker = target.gameObject.AddComponent<WaveVR_ControllerPoseTracker>();
            tracker.Type = left ? wvr.WVR_DeviceType.WVR_DeviceType_Controller_Left : wvr.WVR_DeviceType.WVR_DeviceType_Controller_Right;
            tracker.FollowHead = followHead;
        }
#endif

        public override void SetTrackingSpaceType(TrackingSpaceType type, Transform headTransform, float height)
        {
#if WAVEVR_SDK
#if UNITY_EDITOR
            type = TrackingSpaceType.Stationary;
#endif
            var renderer = GetComponentInChildren<WaveVR_Render>();

            if (type == TrackingSpaceType.Stationary)
                renderer.origin = wvr.WVR_PoseOriginModel.WVR_PoseOriginModel_OriginOnHead;
            else
                renderer.origin = wvr.WVR_PoseOriginModel.WVR_PoseOriginModel_OriginOnGround;

            var position = headTransform.localPosition;
            headTransform.localPosition = new Vector3(position.x, type == TrackingSpaceType.RoomScale ? 0 : height, position.z);
#endif
        }

#if WAVEVR_SDK
        private void OnApplicationPause(bool pause) => Pause(pause);

        public void Pause(bool pause)
        {
            if (pause)
                WaveVR_Utils.IssueEngineEvent(WaveVR_Utils.EngineEventID.UNITY_APPLICATION_PAUSE);
            else
                WaveVR_Utils.IssueEngineEvent(WaveVR_Utils.EngineEventID.UNITY_APPLICATION_RESUME);
        }
#endif
    }
}
