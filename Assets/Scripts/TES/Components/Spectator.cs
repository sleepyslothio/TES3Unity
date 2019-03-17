using Demonixis.Toolbox.XR;
using TESUnity.Inputs;
using UnityEngine;
using UnityEngine.XR;

namespace TESUnity.Components
{
    public class Spectator : MonoBehaviour
    {
        [SerializeField]
        private MenuComponent m_Menu = null;

        private void Start()
        {
            if (!XRManager.Enabled)
            {
                enabled = false;
                return;
            }
            // Setup RoomScale/Sitted mode.
            XRDevice.SetTrackingSpaceType(TrackingSpaceType.Stationary);

            var canvas = m_Menu.GetComponentInParent<Canvas>();
            GUIUtils.SetCanvasToWorldSpace(canvas, null, 0, 0.015f, 1.7f);
        }

        private void Update()
        {
            if (InputManager.GetButtonDown(MWButton.Use))
                m_Menu.LoadWorld();
            else if (InputManager.GetButtonDown(MWButton.Menu))
                m_Menu.Quit();
        }
    }
}