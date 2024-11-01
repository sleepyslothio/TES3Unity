using System.Collections;
using Demonixis.ToolboxV2.Inputs;
using Demonixis.ToolboxV2.XR;
using TES3Unity.UI;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XR;
using UnityEngine.XR;
using Wacki;

namespace TES3Unity.Components.XR
{
    /// <summary>
    /// This component is responsible to enable the VR feature and deal with VR SDKs.
    /// VR SDKs allows us to provide more support (moving controller, teleportation, etc.)
    /// To enable a VR SDKs, please read the README.md file located in the Vendors folder.
    /// </summary>
    public sealed class PlayerXR : MonoBehaviour
    {
        private Transform _transform;
        private RectTransform _canvas;
        private Transform _pivotCanvas;
        private Transform _hud;
        private InputActionMap _xrActionMap;
        private bool _followHead;
        private bool _roomScale;
        private Canvas _mainCanvas;
        private TrackingOriginModeFlags _trackingSpaceType;

        [SerializeField] private bool _spectator;
        [SerializeField] private Teleporter _teleporter;
        [SerializeField] private Transform _cameraTransform;
        [SerializeField] private Transform _trackingSpace;
        [SerializeField] private IUILaserPointer _laserPointer;
        [SerializeField] private float _headHeight = 1.55f;

        private void OnEnable() => _xrActionMap?.Enable();
        private void OnDisable() => _xrActionMap?.Disable();

        private IEnumerator Start()
        {
            if (!XRManager.IsXREnabled())
            {
                enabled = false;
                _laserPointer.SetActive(false);
                yield break;
            }

            var cameras = GetComponentsInChildren<Camera>();
            foreach (var target in cameras)
            {
                if (target.CompareTag("MainCamera"))
                    _cameraTransform = target.transform;
            }

            _transform = transform;
            _laserPointer.IsActive = _spectator;

            if (_spectator)
            {
                var mainUI = GUIUtils.MainCanvas;
                var canvas = mainUI.GetComponent<Canvas>();
                GUIUtils.SetCanvasToWorldSpace(canvas, null, 2.5f, 0.015f, 1.7f);
            }

            // Wait that everything is initialized.
            yield return new WaitForEndOfFrame();

            // Tracking Space Type
            var settings = GameSettings.Get();
            var mode = TrackingOriginModeFlags.Device;

            if (!settings.vrSeated)
            {
                mode = TrackingOriginModeFlags.Floor;
                var trackingSpace = transform.FindChildRecursiveExact("TrackingSpace");
                trackingSpace.localPosition = Vector3.zero;
            }

            XRManager.SetTrackingOriginMode(mode, true);


            var trackedPoseDriversNew = GetComponentsInChildren<TrackedPoseDriver>(true);
            foreach (var driver in trackedPoseDriversNew)
            {
                driver.enabled = true;
            }

            _xrActionMap = InputSystemManager.GetActionMap("XR");
            _xrActionMap.Enable();
            _xrActionMap["Recenter"].started += c => RecenterOrientationAndPosition();

            _roomScale = !settings.vrSeated;
            _followHead = settings.VRFollowHead;

            if (settings.VRTeleportation)
                _teleporter.SetHand(false);

            var uiManager = FindFirstObjectByType<UIManager>();
            if (uiManager != null)
            {
                if (_mainCanvas == null)
                {
                    _mainCanvas = uiManager.GetComponent<Canvas>();
                    _canvas = _mainCanvas.GetComponent<RectTransform>();
                    _pivotCanvas = _canvas.parent;
                    _hud = _canvas.Find("HUD");

                    // Add a pivot to the UI. It'll help to rotate it in the inverse direction of the camera.
                    var uiPivot = GameObjectUtils.Create("UI Pivot", transform);
                    _pivotCanvas = uiPivot.transform;
                    GUIUtils.SetCanvasToWorldSpace(_canvas.GetComponent<Canvas>(), _pivotCanvas, 1.5f, 0.002f);
                }

                if (_mainCanvas == null)
                {
                    throw new UnityException("The Main Canvas Is Null");
                }

                uiManager.Crosshair.Enabled = false;
                uiManager.WindowOpenChanged += OnUIWindowsOpened;
            }

            // Setup the camera
            Camera.main.nearClipPlane = 0.01f;

            RecenterOrientationAndPosition();
        }

        private void OnUIWindowsOpened(UIWindow window, bool open)
        {
            _laserPointer.IsActive = open;
        }

        private void Update()
        {
            if (_pivotCanvas == null)
            {
                return;
            }

            RecenterUI();

            var centerEye = _cameraTransform;
            var root = centerEye.parent;
            var prevPos = root.position;
            var prevRot = root.rotation;

            if (_followHead)
            {
                //m_Transform.rotation = Quaternion.Euler(0.0f, centerEye.rotation.eulerAngles.y, 0.0f);
                //root.rotation = prevRot;
            }

            if (_roomScale)
            {
                //_transform.position = new Vector3(centerEye.position.x, 0.0f, centerEye.position.z);
                //root.position = prevPos;
            }
        }

        /// <summary>
        /// Recenter the Main UI.
        /// </summary>
        private void RecenterUI(bool onlyPosition = false)
        {
            if (!onlyPosition)
            {
                var pivotRot = _pivotCanvas.localRotation;
                pivotRot.y = _cameraTransform.localRotation.y;
                _pivotCanvas.localRotation = pivotRot;
            }

            var camPosition = _cameraTransform.position;
            var targetPosition = _pivotCanvas.position;
            targetPosition.y = camPosition.y;
            _pivotCanvas.position = targetPosition;
        }

        /// <summary>
        /// Reset the orientation and the position of the HMD with a delay of 0.1ms.
        /// </summary>
        public void RecenterOrientationAndPosition()
        {
            InternalRecenter();
            RecenterUI();
        }

        public Transform GetXRAttachNode(bool left)
        {
            var hand = transform.FindChildRecursiveExact($"{(left ? "Left" : "Right")}Hand");
            var xr = hand.Find("XR");
            return xr ?? hand;
        }

        public void InternalRecenter()
        {
#if OCULUS_BUILD
            if (OVRManager.display != null)
            {
                OVRManager.display.RecenterPose();
                return;
            }
#endif

#if !UNITY_VISIONOS
            XRManager.Recenter();
#endif

            var bEyeSpace = _trackingSpaceType == TrackingOriginModeFlags.Device;
            var headHeight = bEyeSpace ? _headHeight : 0;
            var trackingLoc = -_cameraTransform.localPosition;
            trackingLoc.y += headHeight;

            _trackingSpace.localPosition = trackingLoc;
        }
    }
}