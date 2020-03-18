using Demonixis.Toolbox.XR;
using System.Collections;
using TES3Unity.Inputs;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.XR;
using Wacki;

namespace TES3Unity.Components.XR
{
    public class PlayerXRBase : MonoBehaviour
    {
        [SerializeField]
        private bool m_Spectator = false;

        public Transform CameraTransform { get; protected set; }
        public IUILaserPointer LaserPointer { get; protected set; }

        protected virtual IEnumerator Start()
        {
            if (!XRManager.IsXREnabled())
            {
                enabled = false;
                yield break;
            }

            var cameras = GetComponentsInChildren<Camera>();
            foreach (var camera in cameras)
            {
                if (camera.CompareTag("MainCamera"))
                {
                    CameraTransform = camera.transform;
                }
            }

            // Using the correct EventSystem
            var eventSystem = FindObjectOfType<EventSystem>();
            if (eventSystem != null)
            {
                Destroy(eventSystem.gameObject);
            }

            var uiActionMap = InputManager.Enable("UI");
            var handNode = GetXRAttachNode(false);
            var laserPointer = GameObjectUtils.Create("LaserPointer", handNode);
            
            LaserPointer = laserPointer.AddComponent<IUILaserPointer>();
            LaserPointer.PressAction = uiActionMap["Validate"];
            LaserPointer.IsActive = m_Spectator;

            GameObjectUtils.CreateEventSystem<LaserPointerInputModule>();

            if (m_Spectator)
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

            if (settings.RoomScale)
            {
                mode = TrackingOriginModeFlags.Floor;
                var trackingSpace = transform.FindChildRecursiveExact("TrackingSpace");
                trackingSpace.localPosition = Vector3.zero;
            }

            XRManager.SetTrackingOriginMode(mode, true);

            // RenderScale
            var renderScale = settings.RenderScale;
            if (renderScale > 0 && renderScale <= 2)
            {
                XRSettings.renderViewportScale = renderScale;
            }
        }

        public Transform GetXRAttachNode(bool left)
        {
            var hand = transform.FindChildRecursiveExact($"{(left ? "Left" : "Right")}Hand");
            var xr = hand.Find("XR");
            return xr ?? hand;
        }
    }
}
