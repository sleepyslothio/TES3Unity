using Demonixis.Toolbox.XR;
using System.Collections;
using TES3Unity.Inputs;
using TES3Unity.UI;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XR;

namespace TES3Unity.Components.XR
{
    /// <summary>
    /// This component is responsible to enable the VR feature and deal with VR SDKs.
    /// VR SDKs allows us to provide more support (moving controller, teleportation, etc.)
    /// To enable a VR SDKs, please read the README.md file located in the Vendors folder.
    /// </summary>
    public class PlayerXR : PlayerXRBase
    {
        private Transform m_Transform = null;
        private RectTransform m_Canvas = null;
        private Transform m_PivotCanvas = null;
        private Transform m_HUD = null;
        private InputActionMap m_XRActionMap = null;
        private bool m_FollowHead = false;
        private bool m_RoomScale = false;
        private Canvas m_MainCanvas = null;

        [SerializeField]
        private GameObject m_TeleportationPrefab = null;

        private void OnEnable() => m_XRActionMap?.Enable();
        private void OnDisable() => m_XRActionMap?.Disable();

        protected override IEnumerator Start()
        {
            if (!XRManager.IsXREnabled())
            {
                enabled = false;
                yield break;
            }

            StartCoroutine(base.Start());

            m_Transform = transform;

            var trackedPoseDriversNew = GetComponentsInChildren<TrackedPoseDriver>(true);
            foreach (var driver in trackedPoseDriversNew)
            {
                driver.enabled = true;
            }

            m_XRActionMap = InputManager.GetActionMap("XR");
            m_XRActionMap.Enable();
            m_XRActionMap["Recenter"].started += (c) => RecenterOrientationAndPosition();

            var settings = GameSettings.Get();
            m_RoomScale = settings.RoomScale;
            m_FollowHead = settings.FollowHead;

            if (settings.Teleportation)
            {
                var tpGo = Instantiate(m_TeleportationPrefab);
                tpGo.transform.parent = GetXRAttachNode(false);
                tpGo.transform.localPosition = Vector3.zero;
                tpGo.transform.localRotation = Quaternion.identity;

                var tp = tpGo.GetComponent<Teleporter>();
                tp.SetHand(false);
            }

            var uiManager = FindObjectOfType<UIManager>();

            if (uiManager != null)
            {
                if (m_MainCanvas == null)
                {
                    m_MainCanvas = uiManager.GetComponent<Canvas>();
                }

                if (m_MainCanvas == null)
                {
                    throw new UnityException("The Main Canvas Is Null");
                }

                uiManager.Crosshair.Enabled = false;
            }

            uiManager.WindowOpenChanged += OnUIWindowsOpened;

            m_Canvas = m_MainCanvas.GetComponent<RectTransform>();
            m_PivotCanvas = m_Canvas.parent;
            m_HUD = m_Canvas.Find("HUD");

            // Add a pivot to the UI. It'll help to rotate it in the inverse direction of the camera.
            var uiPivot = GameObjectUtils.Create("UI Pivot", transform);
            m_PivotCanvas = uiPivot.transform;
            GUIUtils.SetCanvasToWorldSpace(m_Canvas.GetComponent<Canvas>(), m_PivotCanvas, 1.5f, 0.002f);

            // Setup the camera
            Camera.main.nearClipPlane = 0.1f;

            RecenterOrientationAndPosition();
        }

        private void OnUIWindowsOpened(UIWindow window, bool open)
        {
            LaserPointer.IsActive = open;
        }

        private void Update()
        {
            if (m_PivotCanvas == null)
            {
                return;
            }

            RecenterUI();

            var centerEye = CameraTransform;
            var root = centerEye.parent;
            var prevPos = root.position;
            var prevRot = root.rotation;

            if (m_FollowHead)
            {
                //m_Transform.rotation = Quaternion.Euler(0.0f, centerEye.rotation.eulerAngles.y, 0.0f);
                //root.rotation = prevRot;
            }

            if (m_RoomScale)
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
                var pivotRot = m_PivotCanvas.localRotation;
                pivotRot.y = CameraTransform.localRotation.y;
                m_PivotCanvas.localRotation = pivotRot;
            }

            var camPosition = CameraTransform.position;
            var targetPosition = m_PivotCanvas.position;
            targetPosition.y = camPosition.y;
            m_PivotCanvas.position = targetPosition;
        }

        /// <summary>
        /// Reset the orientation and the position of the HMD with a delay of 0.1ms.
        /// </summary>
        public void RecenterOrientationAndPosition()
        {
            XRManager.Recenter();
            RecenterUI();
        }
    }
}