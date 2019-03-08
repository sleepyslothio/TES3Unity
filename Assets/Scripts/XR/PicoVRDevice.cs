#if PICOVR_SDK
using Pvr_UnitySDKAPI;
#endif
using UnityEngine;
using UnityEngine.XR;

namespace Demonixis.Toolbox.XR
{
#if !PICOVR_SDK
    public enum HeadDofNum
    {
        ThreeDof,
        SixDof
    }

    public enum HandDofNum
    {
        ThreeDof,
        SixDof
    }

    public enum RenderTextureAntiAliasing
    {
        X_1 = 1,
        X_2 = 2,
        X_4 = 4,
        X_8 = 8,
    }

    public enum RenderTextureDepth
    {
        BD_0 = 0,
        BD_16 = 16,
        BD_24 = 24,
    }

    public enum RenderTextureLevel
    {
        Normal,
        High
    }

#endif

    public enum HeadsetType
    {
        None = 0,
        Goblin,
        Neo,
        Goblin2
    }

    public class PicoVRDevice : XRDeviceBase
    {
        [Header("Tracking")]
        [SerializeField]
        private HeadDofNum m_HeadDofNum = HeadDofNum.ThreeDof;
        [SerializeField]
        private HandDofNum m_HandDofNum = HandDofNum.ThreeDof;
        [SerializeField]
        private RenderTextureDepth m_RenderTextureDepth = RenderTextureDepth.BD_24;
        [SerializeField]
        private RenderTextureLevel m_RenderTextureLevel = RenderTextureLevel.Normal;
        [SerializeField]
        private RenderTextureAntiAliasing m_RenderTextureAntiAliasing = RenderTextureAntiAliasing.X_2;
        [SerializeField]
        private bool m_ReplaceTrackedPoseDrivers = true;
        [Tooltip("Allow HDR on Pico Goblin v1 or not.")]
        [SerializeField]
        private bool m_AllowHDROnFirstGen = false;

        [Header("Prefabs")]
        [SerializeField]
        private GameObject[] m_CameraChildren = null;
        [SerializeField]
        private GameObject[] m_SDKChildren = null;

#if UNITY_EDITOR
        [SerializeField]
        private bool m_DisableInEditor = false;

        public bool DisableInEditor => m_DisableInEditor;
#endif

        public override bool IsAvailable
        {
            get
            {
#if PICOVR_SDK
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

        public override XRDeviceType VRDeviceType => XRDeviceType.PicoVR;

        public override float RenderScale { get; set; } = 1.0f;

        public override int EyeTextureWidth => Screen.width;

        public override int EyeTextureHeight => Screen.height;

        public override void Recenter()
        {
#if PICOVR_SDK

#endif
        }

        public static HeadsetType GetHeadsetType()
        {
#if PICOVR_SDK
#if UNITY_EDITOR
            if (FindObjectOfType<Pvr_ControllerManager>() == null)
                return HeadsetType.Goblin2;
#endif

            var id = Pvr_ControllerManager.controllerlink.GetDeviceType();

            if (id == 1)
                return HeadsetType.Goblin;
            else if (id == 2)
                return HeadsetType.Neo;
            else if (id == 3)
                return HeadsetType.Goblin2;

            return HeadsetType.None;
#else
            return HeadsetType.None;
#endif
        }

        public override void SetActive(bool isEnabled)
        {
#if PICOVR_SDK
            var camera = GetComponentInChildren<Camera>();
            var camTransform = camera.transform;
            var camGo = camera.gameObject;
            camera.name = "Head";

            var root = camera.transform.parent;
            var rootGo = root.gameObject;

            rootGo.SetActive(false);

            foreach (var item in m_SDKChildren)
            {
                var go = Instantiate(item, root);
                go.name = item.name;
            }

            foreach (var item in m_CameraChildren)
            {
                var go = Instantiate(item, camTransform);
                go.name = item.name;
            }

            rootGo.AddComponent<Pvr_ControllerManager>();

            var sdk = rootGo.AddComponent<Pvr_UnitySDKManager>();
            sdk.DefaultRenderTexture = true;
            sdk.HeadDofNum = m_HeadDofNum;
            sdk.HandDofNum = m_HandDofNum;
            sdk.DefaultRange = true;
            sdk.SixDofRecenter = true;
            sdk.MovingRatios = 1;
            sdk.RtBitDepth = m_RenderTextureDepth;
            sdk.RtAntiAlising = m_RenderTextureAntiAliasing;
            sdk.RtLevel = m_RenderTextureLevel;

            var head = camGo.AddComponent<Pvr_UnitySDKHeadTrack>();

            CreateEye(true, camera);
            CreateEye(false, camera);

            var eye = camGo.AddComponent<Pvr_UnitySDKEyeManager>();
            eye.isfirst = true;

            rootGo.SetActive(true);

            if (m_ReplaceTrackedPoseDrivers)
                ReplaceTrackedPoseDrivers();
#endif
        }

#if PICOVR_SDK

        private void CreateEye(bool left, Camera parent)
        {
            var go = new GameObject($"{(left ? "Left" : "Right")}Eye");
            var cam = go.AddComponent<Camera>();
            cam.clearFlags = CameraClearFlags.Skybox;
            cam.backgroundColor = new Color(0, 0, 0, 0.5f);
            cam.depth = left ? 3 : 4;
            cam.allowHDR = m_AllowHDROnFirstGen ? parent.allowHDR : false;
            cam.allowMSAA = parent.allowMSAA;
            cam.farClipPlane = parent.farClipPlane;
            cam.nearClipPlane = parent.nearClipPlane;
            cam.useOcclusionCulling = parent.useOcclusionCulling;
            cam.allowDynamicResolution = parent.allowDynamicResolution;
            cam.enabled = left;

            var tr = go.transform;
            tr.parent = parent.transform;
            tr.localPosition = Vector3.zero;
            tr.localRotation = Quaternion.identity;

            var eye = go.AddComponent<Pvr_UnitySDKEye>();
            eye.eye = left ? Eye.LeftEye : Eye.RightEye;
            eye.isFadeUSing = true;
        }

        public void ReplaceTrackedPoseDrivers()
        {
            var drivers = GetComponentsInChildren<MotionController>();

            GameObject left = null;
            GameObject right = null;

            for (var i = 0; i < drivers.Length; i++)
            {
                drivers[i].enabled = false;

                if (drivers[i].LeftHand)
                    left = drivers[i].gameObject;
                else
                    right = drivers[i].gameObject;
            }

            if (left == null && right == null)
                return;

            var wait = Pvr_ControllerManager.Instance;

            var controllers = gameObject.AddComponent<Pvr_Controller>();
            controllers.controller0 = right;
            controllers.controller1 = left;
        }

#endif

        public override void SetTrackingSpaceType(TrackingSpaceType type, Transform headTransform, float height)
        {
#if PICOVR_SDK
            type = TrackingSpaceType.Stationary;
            var position = headTransform.localPosition;
            headTransform.localPosition = new Vector3(position.x, type == TrackingSpaceType.RoomScale ? 0 : height, position.z);
#endif
        }
    }
}
