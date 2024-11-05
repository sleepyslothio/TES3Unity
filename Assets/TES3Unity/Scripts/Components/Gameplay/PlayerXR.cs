using Demonixis.ToolboxV2.Inputs;
using Demonixis.ToolboxV2.XR;
using TES3Unity.UI;
using UnityEngine;
using UnityEngine.InputSystem;
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
        private TrackingOriginModeFlags _trackingSpaceType = TrackingOriginModeFlags.Device;

        [Header("General")]
        [SerializeField] private bool _spectator;
        
        [Header("Locomotion & Interactions")]
        [SerializeField] private Teleporter _teleporter;
        [SerializeField] private IUILaserPointer _laserPointer;
        
        [Header("Rig")]
        [SerializeField] private Transform[] _handOffsets;
        [SerializeField] private Transform _trackingSpace;
        [SerializeField] private Transform _cameraTransform;
        [SerializeField] private float _headHeight = 1.55f;
        [SerializeField] private bool _updateRoot = true;

        [Header("UI")] [SerializeField] private Transform _hudPivot;
        [SerializeField] private float _uiSmoothPosition = 2.5f;
        [SerializeField] private float _uiSmooothRotation = 5.0f;
        [SerializeField] private Transform _pivotCanvas;

        public GameObject HudPivot => _hudPivot.gameObject;

        private void Start()
        {
            _laserPointer.IsActive = _spectator;

            // Tracking Space Type
            XRManager.SetTrackingOriginMode(_trackingSpaceType, true);

            var bEyeSpace = _trackingSpaceType == TrackingOriginModeFlags.Device;
            _trackingSpace.localPosition = new Vector3(0, bEyeSpace ? _headHeight : 0, 0);

            // Setup the camera
            if (_cameraTransform.TryGetComponent(out Camera targetCamera))
                targetCamera.nearClipPlane = 0.01f;

            if (XRManager.IsOpenXREnabled())
            {
                foreach (var ctrl in _handOffsets)
                    ctrl.localRotation = Quaternion.Euler(45, 0, 0);
            }

            RecenterOrientationAndPosition();

            if (_spectator)
            {
                var menuUi = GUIUtils.MainCanvas;
                var menuCanvas = menuUi.GetComponent<Canvas>();
                GUIUtils.SetCanvasToWorldSpace(menuCanvas, null, 2.5f, 0.015f, 1.7f);
                return;
            }
            
            var uiManager = _pivotCanvas.GetComponentInChildren<UIManager>();
            uiManager.WindowOpenChanged += OnUIWindowsOpened;
        }

        private void OnEnable()
        {
            var actionMap = InputSystemManager.Enable("XR");
            actionMap["Recenter"].started += OnRecenter;

            if (!GameSettings.Get().vrTeleportation) return;
            actionMap["Teleportation"].started += OnTeleportationInput;
            actionMap["Teleportation"].canceled += OnTeleportationInput;
        }

        private void OnDisable()
        {
            var actionMap = InputSystemManager.Disable("XR");
            actionMap["Recenter"].started -= OnRecenter;

            if (!GameSettings.Get().vrTeleportation) return;
            actionMap["Teleportation"].started -= OnTeleportationInput;
            actionMap["Teleportation"].canceled -= OnTeleportationInput;
        }

        private void OnRecenter(InputAction.CallbackContext context)
        {
            RecenterOrientationAndPosition();
        }

        private void OnUIWindowsOpened(UIWindow window, bool open)
        {
            _laserPointer.IsActive = open;
        }

        private void OnTeleportationInput(InputAction.CallbackContext context)
        {
            if (context.phase == InputActionPhase.Started)
                _teleporter.InputIsPressed();
            else if (context.phase == InputActionPhase.Canceled)
                _teleporter.InputWasJustReleased();
        }

        private void LateUpdate()
        {
            if (_spectator) return;
            
            // Update HUD pivot
            var cameraLocalRotation = _cameraTransform.localEulerAngles;
            var hudLocalRotation = _hudPivot.localEulerAngles;
            hudLocalRotation.y = cameraLocalRotation.y;
            
            _hudPivot.localPosition = Vector3.Lerp(_hudPivot.localPosition, _cameraTransform.localPosition,
                Time.deltaTime * _uiSmoothPosition);

            _hudPivot.localRotation = Quaternion.Slerp(_hudPivot.localRotation, Quaternion.Euler(hudLocalRotation),
                Time.deltaTime * _uiSmooothRotation);

            // Update Main UI pivot
            var cameraRotation = _cameraTransform.eulerAngles;
            var pivotRotation = _pivotCanvas.eulerAngles;
            pivotRotation.y = cameraRotation.y;
            _pivotCanvas.rotation = Quaternion.Slerp(_pivotCanvas.rotation, Quaternion.Euler(pivotRotation), Time.deltaTime * _uiSmooothRotation);
            
            // Update root position if needed.
            if (!_updateRoot) return;
            
            var prevPos = _trackingSpace.position;
            var prevRot = _trackingSpace.rotation;

            var position = _cameraTransform.position;
            transform.position = new Vector3(
                position.x,
                transform.position.y,
                position.z);

            transform.rotation = Quaternion.Euler(0, _cameraTransform.rotation.eulerAngles.y, 0);

            _trackingSpace.position = prevPos;
            _trackingSpace.rotation = prevRot;
        }

        /// <summary>
        /// Recenter the Main UI.
        /// </summary>
        private void RecenterUI(bool onlyPosition = false)
        {
            if (_pivotCanvas == null || _spectator) return;

            //if (!onlyPosition)
            {
                var pivotRot = _pivotCanvas.localRotation;
                pivotRot.y = _cameraTransform.localRotation.y;
                _pivotCanvas.localRotation = pivotRot;
            }

            /*var camPosition = _cameraTransform.position;
            var targetPosition = _pivotCanvas.position;
            targetPosition.y = camPosition.y;
            _pivotCanvas.position = targetPosition;*/
        }

        /// <summary>
        /// Reset the orientation and the position of the HMD with a delay of 0.1ms.
        /// </summary>
        private void RecenterOrientationAndPosition()
        {
            InternalRecenter();
            RecenterUI();
        }

        private void InternalRecenter()
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