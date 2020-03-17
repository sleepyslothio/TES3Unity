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
        [SerializeField]
        private GameObject m_RayInteractionPrefab = null;

        public Transform CameraTransform { get; protected set; }

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

            var rayGo = Instantiate(m_RayInteractionPrefab);
            rayGo.transform.parent = GetXRAttachNode(false);
            rayGo.transform.localPosition = Vector3.zero;
            rayGo.transform.localRotation = Quaternion.identity;

            var ray = rayGo.GetComponent<IUILaserPointer>();
            ray.PressAction = InputManager.GetAction("UI", "Validate");

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
            var mode = settings.RoomScale ? TrackingOriginModeFlags.Floor : TrackingOriginModeFlags.Device;

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
            var hand = transform.FindChildRecursiveExact($"{(left ? "Left" : "Right")}HandAnchor");
            return hand.Find("XR");
        }
    }
}
