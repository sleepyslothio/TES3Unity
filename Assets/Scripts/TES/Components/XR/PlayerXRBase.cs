using Demonixis.Toolbox.XR;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.XR;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit.UI;

namespace TESUnity.Components.XR
{
    public class PlayerXRBase : MonoBehaviour
    {
        public Transform CameraTransform { get; protected set; }

        protected virtual void Start()
        {
            if (!XRManager.IsXREnabled())
            {
                return;
            }

            var cameras = GetComponentsInChildren<Camera>();
            foreach (var camera in cameras)
            {
                if (camera.CompareTag("MainCamera"))
                {
                    CameraTransform = camera.transform;
                }
            }

            Debug.Assert(CameraTransform != null);

            // Detech hands from the head node.
            var hands = GetComponentsInChildren<TrackedPoseDriver>(true);
            foreach (var hand in hands)
            {
                hand.transform.parent = CameraTransform.parent;
            }

            // Setup RoomScale/Sitted mode.
            var settings = GameSettings.Get();
            var trackingSpaceType = settings.RoomScale ? TrackingOriginModeFlags.Floor : TrackingOriginModeFlags.Device;

            XRManager.SetTrackingOriginMode(trackingSpaceType, true);

            // RenderScale
            var renderScale = settings.RenderScale;

            if (renderScale > 0 && renderScale <= 2)
                XRSettings.renderViewportScale = renderScale;

            // Using the correct EventSystem
            var eventSystem = FindObjectOfType<EventSystem>();
            if (eventSystem != null)
            {
                Destroy(eventSystem.gameObject);
            }

            GameObjectUtils.CreateEventSystem<XRUIInputModule>();

            // Allow to interact with the UI
            var mainUI = GUIUtils.MainCanvas;
            mainUI.AddComponent<TrackedDeviceGraphicRaycaster>();
        }
    }
}
