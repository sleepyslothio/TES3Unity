using Demonixis.Toolbox.XR;
using TESUnity.Components.VR;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.XR;

namespace TESUnity.Components
{
    public sealed class Spectator : MonoBehaviour
    {
        private void Start()
        {
            if (!XRManager.IsXREnabled())
            {
                enabled = false;
                return;
            }
            // Setup RoomScale/Sitted mode.
            XRManager.SetTrackingOriginMode(TrackingOriginModeFlags.Device, true);
            var menuComponent = FindObjectOfType<MenuComponent>();

            Debug.Assert(menuComponent != null);

            var canvas = menuComponent.GetComponentInParent<Canvas>();
            GUIUtils.SetCanvasToWorldSpace(canvas, null, 2.5f, 0.015f, 1.7f);

            var laser = GetComponentInChildren<LaserPointer>(true);
            laser.enabled = true;

            var standaloneInputModule = EventSystem.current.GetComponent<StandaloneInputModule>();
            Destroy(standaloneInputModule);

            var lightGo = new GameObject("Light");
            var light = lightGo.AddComponent<Light>();
            light.shadows = LightShadows.None;
            light.type = LightType.Directional;
            light.transform.rotation = Quaternion.Euler(45.0f, 225.0f, 0.0f);
        }
    }
}