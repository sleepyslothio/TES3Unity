using Demonixis.Toolbox.XR;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

namespace TESUnity.Inputs
{
    public static class InputManager
    {
        private struct XRButtonMapping
        {
            public XRButton Button { get; set; }
            public bool LeftHand { get; set; }

            public XRButtonMapping(XRButton button, bool left)
            {
                Button = button;
                LeftHand = left;
            }
        }

        private static Dictionary<string, XRButtonMapping> m_XRMapping = null;

        public static float GetAxis(string axis)
        {
            var result = Input.GetAxis(axis);

            if (XRSettings.enabled)
            {
                if (axis == "Horizontal")
                    result += XRInput.GetAxis(XRAxis.ThumbstickX, true);
                else if (axis == "Vertical")
                    result += XRInput.GetAxis(XRAxis.ThumbstickY, true);
                else if (axis == "Mouse X")
                    result += XRInput.GetAxis(XRAxis.ThumbstickX, false);
                else if (axis == "Mouse Y")
                    result += XRInput.GetAxis(XRAxis.ThumbstickY, false);

                // Deadzone
                if (Mathf.Abs(result) < 0.15f)
                    result = 0.0f;
            }

            return result;
        }

        public static bool GetButton(string button)
        {
            var result = Input.GetButton(button);

            if (XRSettings.enabled)
            {
                if (m_XRMapping == null)
                    InitializeMapping();

                if (m_XRMapping.ContainsKey(button))
                {
                    var mapping = m_XRMapping[button];
                    result |= XRInput.GetButton(mapping.Button, mapping.LeftHand);
                }
            }

            return result;
        }

        public static bool GetButtonUp(string button)
        {
            var result = Input.GetButtonUp(button);

            if (XRSettings.enabled)
            {
                if (m_XRMapping == null)
                    InitializeMapping();

                if (m_XRMapping.ContainsKey(button))
                {
                    var mapping = m_XRMapping[button];
                    result |= XRInput.GetButtonUp(mapping.Button, mapping.LeftHand);
                }
            }

            return result;
        }

        public static bool GetButtonDown(string button)
        {
            var result = Input.GetButtonDown(button);

            if (XRSettings.enabled)
            {
                if (m_XRMapping == null)
                    InitializeMapping();

                if (m_XRMapping.ContainsKey(button))
                {
                    var mapping = m_XRMapping[button];
                    result |= XRInput.GetButtonDown(mapping.Button, mapping.LeftHand);
                }
            }

            return result;
        }

        private static void InitializeMapping()
        {
            m_XRMapping = new Dictionary<string, XRButtonMapping>()
            {
                { "Jump", new XRButtonMapping(XRButton.Thumbstick, true) },
                { "Light", new XRButtonMapping(XRButton.Thumbstick, false) },
                { "Run", new XRButtonMapping(XRButton.Grip, true) },
                { "Slow", new XRButtonMapping(XRButton.Grip, false) },
                { "Attack", new XRButtonMapping(XRButton.Trigger, false) },
                { "Recenter", new XRButtonMapping(XRButton.Menu, false) },
                { "Use", new XRButtonMapping(XRButton.Trigger, true) },
                { "Menu", new XRButtonMapping(XRButton.Menu, true) }
            };
        }
    }
}
