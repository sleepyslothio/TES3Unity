/// GameVRSettings
/// Last Modified Date: 03/02/2018
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.XR;

namespace Demonixis.Toolbox.XR
{
    /// <summary>
    /// Defines the type of SDK.
    /// </summary>
    public enum XRDeviceType
    {
        None = 0,
        UnityXR,
        OSVR,
        GoogleVR,
        WaveVR,
        PicoVR
    }

    /// <summary>
    /// The GameVRSettings is responsible to check available VR devices and select the one with the higher priority.
    /// It's also used to Recenter the view.
    /// </summary>
    public sealed class XRManager : MonoBehaviour
    {
        #region Private Fields

        private static XRManager s_Instance = null;
        private XRDeviceBase m_ActiveDevice = null;
        private float m_BaseHeight = 0;
        private bool m_VRChecked = false;

        #endregion

        [SerializeField]
        private Transform m_HeadTransform = null;
        [SerializeField]
        private TrackingSpaceType m_TrackingSpaceType = TrackingSpaceType.RoomScale;
        [SerializeField]
#pragma warning disable CS0628 // Nouveau membre protégé déclaré dans la classe sealed
        protected bool m_ForceSeatedOnMobile = true;
#pragma warning restore CS0628 // Nouveau membre protégé déclaré dans la classe sealed

        public static XRManager Instance
        {
            get
            {
                if (s_Instance == null)
                    s_Instance = FindObjectOfType<XRManager>();

                return s_Instance;
            }
        }

        public bool ForceSeatedOnMobile => m_ForceSeatedOnMobile;

        public static bool UnityNativeXR { get; private set; }

        public TrackingSpaceType TrackingSpaceType
        {
            get { return m_TrackingSpaceType; }
            set
            {
                m_TrackingSpaceType = value;
                if (m_ActiveDevice == null)
                    return;

                m_ActiveDevice.SetTrackingSpaceType(m_TrackingSpaceType, m_HeadTransform, m_BaseHeight);
                m_ActiveDevice.Recenter();
            }
        }

        #region Instance Methods

        private void Awake()
        {
            if (s_Instance != null && s_Instance != this)
            {
                Debug.LogWarning("Only one VRManager can be used on the scene");
                Destroy(this);
                return;
            }

            s_Instance = this;

            StartCoroutine(CheckVRDevices());
        }

        private void OnDestroy()
        {
            m_ActiveDevice = null;
        }

        private IEnumerator CheckVRDevices()
        {
            var endOfFrame = new WaitForEndOfFrame();
            var camera = Camera.main;

            while (camera == null)
            {
                camera = Camera.main;
                yield return endOfFrame;
            }

            GetVRDevice();
        }

        /// <summary>
        /// Gets the type of VR device currently connected. It takes the first VR device which have the higher priority.
        /// </summary>
        /// <returns></returns>
        public XRDeviceType GetVRDevice()
        {
            if (m_VRChecked)
                return m_ActiveDevice != null ? m_ActiveDevice.VRDeviceType : XRDeviceType.None;

            // Gets all managers and enable only the first connected device.
            var vrManagers = GetComponents<XRDeviceBase>();
            var count = vrManagers.Length;

            m_ActiveDevice = null;

            if (m_HeadTransform == null)
                m_HeadTransform = transform;

            m_BaseHeight = m_HeadTransform.localPosition.y * (Camera.main.transform.localScale.y);

            if (count > 0)
            {
                Array.Sort(vrManagers);

                for (var i = 0; i < count; i++)
                {
                    if (vrManagers[i].IsAvailable && m_ActiveDevice == null)
                    {
                        m_ActiveDevice = vrManagers[i];
                        m_ActiveDevice.SetActive(true);
                        m_ActiveDevice.SetTrackingSpaceType(m_TrackingSpaceType, m_HeadTransform, m_BaseHeight);
                        UnityNativeXR = m_ActiveDevice.VRDeviceType == XRDeviceType.UnityXR;

                        break;
                    }
                    else
                        vrManagers[i].Dispose();
                }
            }

            m_VRChecked = true;

            return m_ActiveDevice != null ? m_ActiveDevice.VRDeviceType : XRDeviceType.None;
        }

        public void DestroyAll()
        {
            var devices = GetComponents<XRDeviceBase>();
            for (var i = 0; i < devices.Length; i++)
                Destroy(devices[i]);

            Destroy(this);
        }

        #endregion

        #region Static Fields

        /// <summary>
        /// Recenter the view of the active manager.
        /// </summary>
        public static void Recenter()
        {
            var instance = Instance;
            if (instance == null || instance.m_ActiveDevice == null)
                return;

            instance.m_ActiveDevice.Recenter();
        }

        /// <summary>
        /// Indicates if the VR mode is enabled.
        /// </summary>
        public static bool Enabled
        {
            get
            {
                var instance = Instance;
                if (instance == null || instance.m_ActiveDevice == null)
                    return instance.GetVRDevice() != XRDeviceType.None;

                return Instance.m_ActiveDevice != null || XRSettings.enabled;
            }
        }

        public static XRDeviceType Type => GetActiveDevice()?.VRDeviceType ?? XRDeviceType.None;

        /// <summary>
        /// Gets the current active VR device.
        /// </summary>
        public static XRDeviceBase GetActiveDevice()
        {
            var instance = Instance;
            if (instance == null)
                return null;

            return instance.m_ActiveDevice;
        }

        #endregion
    }
}