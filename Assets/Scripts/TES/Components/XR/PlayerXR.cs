using Demonixis.Toolbox.XR;
using Demonixis.UniversalXR;
using System.Collections;
using TESUnity.Inputs;
using TESUnity.UI;
using UnityEngine;
using UnityEngine.InputSystem.XR;
using UnityEngine.XR;

namespace TESUnity.Components.VR
{
    /// <summary>
    /// This component is responsible to enable the VR feature and deal with VR SDKs.
    /// VR SDKs allows us to provide more support (moving controller, teleportation, etc.)
    /// To enable a VR SDKs, please read the README.md file located in the Vendors folder.
    /// </summary>
    public class PlayerXR : MonoBehaviour
    {
        private Transform _camTransform = null;
        private Transform _transform = null;
        private RectTransform _canvas = null;
        private Transform _pivotCanvas = null;
        private Transform m_HUD = null;
        private bool m_FollowHead = false;
        private bool m_RoomScale = false;

        [SerializeField]
        private Canvas _mainCanvas = null;
        [SerializeField]
        private GameObject m_LeftHandPrefab = null;
        [SerializeField]
        private GameObject m_RightHandPrefab = null;
        [SerializeField]
        private Transform m_LeftHand = null;
        [SerializeField]
        private Transform m_RightHand = null;

        /// <summary>
        /// Intialize the VR support for the player.
        /// - The HUD and UI will use a WorldSpace Canvas
        /// - The HUD canvas is not recommanded, it's usefull for small informations
        /// - The UI is for all other UIs: Menu, Life, etc.
        /// </summary>
        private IEnumerator Start()
        {
            if (!XRManager.IsXREnabled())
            {
                enabled = false;
                yield break;
            }

            var hands = GetComponentsInChildren<TrackedPoseDriver>(true);
            foreach (var hand in hands)
            {
                hand.transform.parent = _camTransform.parent;
            }

            var settings = GameSettings.Get();

            // Setup RoomScale/Sitted mode.
            var trackingSpaceType = m_RoomScale ? TrackingOriginModeFlags.Floor : TrackingOriginModeFlags.Device;

            XRManager.SetTrackingOriginMode(trackingSpaceType, true);

            TryAddOculusSupport(this, new[] { m_LeftHand, m_RightHand }, new[] { m_LeftHandPrefab, m_RightHandPrefab });

            var teleporters = GetComponentsInChildren<Teleporter>();
            foreach (var tp in teleporters)
            {
                tp.enabled = true;
            }

#if UNITY_ANDROID || UNITY_IOS
            QualitySettings.SetQualityLevel(1, false);
#endif

            m_RoomScale = settings.RoomScale;
            m_FollowHead = settings.FollowHead;

            yield return new WaitForEndOfFrame();

            var renderScale = settings.RenderScale;

            if (renderScale > 0 && renderScale <= 2)
                XRSettings.renderViewportScale = renderScale;

            var uiManager = FindObjectOfType<UIManager>();

            if (uiManager != null)
            {
                if (_mainCanvas == null)
                    _mainCanvas = uiManager.GetComponent<Canvas>();

                if (_mainCanvas == null)
                    throw new UnityException("The Main Canvas Is Null");

                uiManager.Crosshair.Enabled = false;
            }

            _canvas = _mainCanvas.GetComponent<RectTransform>();
            _pivotCanvas = _canvas.parent;
            m_HUD = _canvas.Find("HUD");

            // Put the Canvas in WorldSpace and Attach it to the camera.
            _camTransform = Camera.main.GetComponent<Transform>();
            _transform = transform;

            // Add a pivot to the UI. It'll help to rotate it in the inverse direction of the camera.
            var uiPivot = new GameObject("UI Pivot");
            _pivotCanvas = uiPivot.GetComponent<Transform>();
            _pivotCanvas.parent = transform;
            _pivotCanvas.localPosition = Vector3.zero;
            _pivotCanvas.localRotation = Quaternion.identity;
            _pivotCanvas.localScale = Vector3.one;
            GUIUtils.SetCanvasToWorldSpace(_canvas.GetComponent<Canvas>(), _pivotCanvas, 1.0f, 0.002f);

            // Setup the camera
            Camera.main.nearClipPlane = 0.1f;

            RecenterOrientationAndPosition();
        }

        private void Update()
        {
            if (_pivotCanvas == null)
                return;

            // At any time, the user might want to reset the orientation and position.
            if (InputManager.GetButtonDown(MWButton.Recenter))
                RecenterOrientationAndPosition();

            RecenterUI();

            var centerEye = _camTransform;
            var root = centerEye.parent;
            var prevPos = root.position;
            var prevRot = root.rotation;

            if (m_FollowHead)
            {
                _transform.rotation = Quaternion.Euler(0.0f, centerEye.rotation.eulerAngles.y, 0.0f);
                root.rotation = prevRot;
            }

            if (m_RoomScale)
            {
                //_transform.position = new Vector3(centerEye.position.x, 0.0f, centerEye.position.z);
                root.position = prevPos;
            }
        }

        public void ShowUICursor(bool visible)
        {
            // TODO: Add hand selector for the Touchs and the Vive.
            var uiCursor = GetComponentInChildren<VRGazeUI>(true);
            uiCursor.SetActive(visible);
        }

        /// <summary>
        /// Recenter the Main UI.
        /// </summary>
        private void RecenterUI(bool onlyPosition = false)
        {
            if (!onlyPosition)
            {
                var pivotRot = _pivotCanvas.localRotation;
                pivotRot.y = _camTransform.localRotation.y;
                _pivotCanvas.localRotation = pivotRot;
            }

            var camPosition = _camTransform.position;
            var targetPosition = _pivotCanvas.position;
            targetPosition.y = camPosition.y;
            _pivotCanvas.position = targetPosition;
        }

        /// <summary>
        /// Reset the orientation and the position of the HMD with a delay of 0.1ms.
        /// </summary>
        public void RecenterOrientationAndPosition()
        {
            XRManager.Recenter();
            RecenterUI();
        }

        /// <summary>
        /// Sent by the PlayerComponent when the pause method is called.
        /// </summary>
        /// <param name="paused">Boolean: Indicates if the player is paused.</param>
        private void OnPlayerPause(bool paused)
        {
            if (paused)
                RecenterUI();
        }

        public static void TryAddOculusSupport(MonoBehaviour target, Transform[] hands, GameObject[] handPrefabs)
        {
            if (XRManager.GetXRVendor() != XRVendor.Oculus)
            {
                return;
            }

            var settings = GameSettings.Get();
            var manager = target.gameObject.AddComponent<OVRManager>();
            var cameraRig = target.gameObject.AddComponent<OVRCameraRig>();

            target.StartCoroutine(SetupOculusManager());

            IEnumerator SetupOculusManager()
            {
                yield return null;
                manager.trackingOriginType = settings.RoomScale ? OVRManager.TrackingOrigin.FloorLevel : OVRManager.TrackingOrigin.EyeLevel;
            }

            if (settings.HandTracking)
            {
                var leftHand = AddHandSupport(true);
                var rightHand = AddHandSupport(false);

                InputManager.AddInput(new OculusHandTrackingInput(leftHand, rightHand));

                OVRHand AddHandSupport(bool left)
                {
                    var parent = left ? hands[0] : hands[1];
                    var hand = Instantiate(left ? handPrefabs[0] : handPrefabs[1], parent);

                    var teleporter = parent.GetComponentInChildren<Teleporter>(true);
                    if (teleporter != null)
                    {
                        teleporter.SetHand(hand.GetComponent<OVRHand>());
                    }

                    return hand.GetComponent<OVRHand>();
                }
            }
        }
    }
}