﻿using Demonixis.Toolbox.XR;
using Demonixis.ToolboxV2.XR;
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
                LaserPointer.SetActive(false);
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

            GameObjectUtils.CreateEventSystem<LaserPointerInputModule>();
            var uiActionMap = InputManager.Enable("UI");

            LaserPointer = GetComponentInChildren<IUILaserPointer>(true);
            if (LaserPointer == null)
            {
                var handNode = GetXRAttachNode(false);
                var laserPointer = GameObjectUtils.Create("LaserPointer", handNode);
                LaserPointer = laserPointer.AddComponent<IUILaserPointer>();
                LaserPointer.PressAction = uiActionMap["Validate"];
            }

            LaserPointer.IsActive = m_Spectator;

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

            if (settings.VRRoomScale)
            {
                mode = TrackingOriginModeFlags.Floor;
                var trackingSpace = transform.FindChildRecursiveExact("TrackingSpace");
                trackingSpace.localPosition = Vector3.zero;
            }

            XRManager.SetTrackingOriginMode(mode, true);
        }

        public Transform GetXRAttachNode(bool left)
        {
            var hand = transform.FindChildRecursiveExact($"{(left ? "Left" : "Right")}Hand");
            var xr = hand.Find("XR");
            return xr ?? hand;
        }
    }
}
