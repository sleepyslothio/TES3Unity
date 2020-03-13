using Demonixis.Toolbox.XR;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.UI;

namespace TES3Unity.Components.XR
{
    using XRIController = UnityEngine.XR.Interaction.Toolkit.XRController;

    public class PlayerXRBase : MonoBehaviour
    {
        [SerializeField]
        private bool m_Spectator = false;

        public Transform CameraTransform { get; protected set; }

        protected virtual void Awake()
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

            var settings = GameSettings.Get();
  
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

            if (m_Spectator)
            {
                var canvas = mainUI.GetComponent<Canvas>();
                GUIUtils.SetCanvasToWorldSpace(canvas, null, 2.5f, 0.015f, 1.7f);
            }

            CreateLocomotionSystem(gameObject);
        }

        public static void CreateInteractionSystem()
        {
            GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/XR Interaction Manager"));
        }

        public static void CreateLocomotionSystem(GameObject player)
        {
            if (FindObjectOfType<LocomotionSystem>() != null)
            {
                return;
            }

            var settings = GameSettings.Get();
            var trackingSpaceType = settings.RoomScale ? TrackingOriginModeFlags.Floor : TrackingOriginModeFlags.Device;
            var xrRig = player.GetComponent<XRRig>();
            xrRig.TrackingOriginMode = trackingSpaceType;

            var xrControllers = player.GetComponentsInChildren<XRIController>();
            XRIController rightController = null;

            foreach (var controller in xrControllers)
            {
                if (controller.name.ToLower().Contains("right"))
                {
                    rightController = controller;
                }
            }

            var gameObject = new GameObject("Locomotion System");

            var locomotionSystem = gameObject.AddComponent<LocomotionSystem>();
            locomotionSystem.xrRig = xrRig;

            var teleportion = gameObject.AddComponent<TeleportationProvider>();
            teleportion.system = locomotionSystem;

            var snapTurn = gameObject.AddComponent<SnapTurnProvider>();
            snapTurn.system = locomotionSystem;
            snapTurn.controllers.AddRange(xrControllers);
        }
    }
}
