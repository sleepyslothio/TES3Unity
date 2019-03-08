/// OSVRDevice
/// Version: 4

using UnityEngine;
using UnityEngine.XR;
#if OSVR_SDK
using OSVR.Unity;
using System;
using System.Collections;
#endif

namespace Demonixis.Toolbox.XR
{
    /// <summary>
    /// OSVRDevice - Manages all aspect of the VR from this singleton.
    /// </summary>
    public sealed class OSVRDevice : XRDeviceBase
    {
#if OSVR_ENABLED
        private const bool AutoStartServer = false;
        private static ClientKit s_ClientKit = null;
        private DisplayController m_DisplayController = null;
        private OsvrUnityNativeVR m_UnityNativeVR = null;
        private GameObject m_DummyCamera = null;
        private Vector3 m_OriginalHeadPosition = Vector3.zero;
        private bool m_HasError = false;
#endif

        [SerializeField]
        private Transform m_HeadNode = null;
        [SerializeField]
        private Vector3 m_HeadFixAxis = Vector3.up;

#if OSVR_ENABLED
        public override bool IsAvailable
        {
            get
            {
                if (m_HasError)
                    return false;

                try
                {
                    var clientKit = GetClientKit();
                    return clientKit != null && clientKit.context != null && clientKit.context.CheckStatus();
                }
                catch (Exception)
                {
                    m_HasError = true;
                }

                return false;
            }
        }

        public override XRDeviceType VRDeviceType { get { return XRDeviceType.OSVR; } }

        public bool IsLegacyIntegration { get { return m_UnityNativeVR != null; } }

        public override float RenderScale
        {
            get
            {
                if (m_UnityNativeVR)
                    return XRSettings.eyeTextureResolutionScale;

                return 1.0f;
            }
            set { }
        }

        public override int EyeTextureWidth
        {
            get
            {
                if (m_UnityNativeVR)
                    return XRSettings.eyeTextureWidth;

                return Screen.width;
            }
        }

        public override int EyeTextureHeight
        {
            get
            {
                if (m_UnityNativeVR)
                    return XRSettings.eyeTextureHeight;

                return Screen.height;
            }
        }

        private void Awake()
        {
            if (m_HeadNode == null)
                m_HeadNode = transform;

            m_OriginalHeadPosition = m_HeadNode.localPosition;
        }

        public override void Dispose()
        {
            Destroy(m_UnityNativeVR);

            if (m_DisplayController != null)
                SetActive(false);
        }

        public static ClientKit GetClientKit()
        {
            if (s_ClientKit == null)
            {
                var go = new GameObject("ClientKit");
                go.SetActive(false);

                s_ClientKit = go.AddComponent<ClientKit>();
                s_ClientKit.AppID = Application.identifier;
#if UNITY_STANDALONE_WIN
                s_ClientKit.autoStartServer = false;
#endif

                go.SetActive(true);
            }

            return s_ClientKit;
        }

        public override void Recenter()
        {
            var clientKit = GetClientKit();

            if (clientKit == null)
                return;

            if (m_DisplayController != null && m_DisplayController.UseRenderManager)
                m_DisplayController.RenderManager.SetRoomRotationUsingHead();
            else
                clientKit.context.SetRoomRotationUsingHead();
        }

        public override void SetActive(bool isEnabled)
        {
            var clientKit = GetClientKit();
            var camera = Camera.main;

            if (clientKit == null || camera == null)
                return;

            if (clientKit.context == null || !clientKit.context.CheckStatus())
                return;

            var setupLegacy = System.Environment.CommandLine.Contains("--osvr-legacy");

#if !UNITY_STANDAONE_WIN
            setupLegacy = true;
#endif

            if (setupLegacy)
                SetupLegacySupport(camera, isEnabled);
            else
                SetupUnityXRSupport(camera, isEnabled);
        }

        private void SetupUnityXRSupport(Camera camera, bool isEnabled)
        {
            if (isEnabled && m_UnityNativeVR == null)
            {
                // OSVR doesn't support deferred renderer for now.
                if (camera.renderingPath != RenderingPath.Forward)
                    QualitySettings.antiAliasing = 0;

                m_UnityNativeVR = camera.gameObject.AddComponent<OsvrUnityNativeVR>();

                // Recenter and mirror mode.
                StartCoroutine(FinishSetup());
            }
            else if (m_UnityNativeVR != null)
            {
                Destroy<OsvrMirrorDisplay>();
                Destroy<OsvrUnityNativeVR>();
                Destroy<OsvrRenderManager>(false);
                Destroy(m_DummyCamera);
                m_UnityNativeVR = null;
            }
        }

        private void SetupLegacySupport(Camera camera, bool isEnabled)
        {
            if (isEnabled && m_DisplayController == null)
            {
                // OSVR doesn't support deferred renderer for now.
                if (camera.renderingPath != RenderingPath.Forward)
                    QualitySettings.antiAliasing = 0;

                m_DisplayController = camera.transform.parent.gameObject.AddComponent<DisplayController>();
                camera.gameObject.AddComponent<VRViewer>();

                // Recenter and mirror mode.
                StartCoroutine(FinishSetup());
            }
            else if (m_DisplayController != null)
            {
                Destroy(m_DisplayController);
                Destroy<OsvrMirrorDisplay>();
                Destroy<VREye>();
                Destroy<VRSurface>();
                Destroy<VRViewer>();
                Destroy<OsvrRenderManager>(false);
                m_DisplayController = null;
            }
        }

        private IEnumerator FinishSetup()
        {
            yield return new WaitForEndOfFrame();

            var osvrMirror = (OsvrMirrorDisplay)null;

            if ((m_DisplayController != null && m_DisplayController.UseRenderManager) || m_UnityNativeVR != null)
                osvrMirror = gameObject.AddComponent<OsvrMirrorDisplay>();

            if (m_UnityNativeVR != null && osvrMirror != null)
            {
                osvrMirror.MirrorCamera = Camera.main;

                m_DummyCamera = new GameObject("OsvrDummyCamera");

                var cam = m_DummyCamera.AddComponent<Camera>();
                cam.clearFlags = CameraClearFlags.Nothing;
                cam.stereoTargetEye = StereoTargetEyeMask.None;
                cam.useOcclusionCulling = false;
                cam.allowMSAA = false;
                cam.allowHDR = false;
                cam.cullingMask = 0;
            }

            yield return new WaitForEndOfFrame();

            StartCoroutine(RecenterAndFixOffsetCoroutine());
        }

        public override void SetTrackingSpaceType(TrackingSpaceType type, Transform headTransform, float height)
        {
        }

        private IEnumerator RecenterAndFixOffsetCoroutine()
        {
            yield return new WaitForEndOfFrame();

            Recenter();

            var targetPosition = transform.position;

            if (targetPosition == Vector3.zero)
                yield break;

            if (m_HeadFixAxis.x < 1)
                targetPosition.x = 0.0f;

            if (m_HeadFixAxis.y < 1)
                targetPosition.y = 0.0f;

            if (m_HeadFixAxis.z < 1)
                targetPosition.z = 0.0f;

            m_HeadNode.localPosition = m_OriginalHeadPosition - targetPosition;
        }
#else
        public override bool IsAvailable { get { return false; } }
        public override XRDeviceType VRDeviceType { get { return XRDeviceType.None; } }
        public override float RenderScale { get; set; }
        public override int EyeTextureWidth { get { return 0; } }
        public override int EyeTextureHeight { get { return 0; } }
        public override void Recenter() { }
        public override void SetActive(bool isEnabled) { }
        public override void SetTrackingSpaceType(TrackingSpaceType type, Transform headTransform, float height)
        {
        }
#endif
    }
}