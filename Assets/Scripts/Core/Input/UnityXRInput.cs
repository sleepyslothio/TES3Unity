using Demonixis.Toolbox.XR;
using System.Collections.Generic;

namespace TESUnity.Inputs
{
    public class UnityXRInput : IInputProvider
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

        private static Dictionary<MWButton, XRButtonMapping> m_XRMapping = null;

        public bool TryInitialize()
        {
            return XRManager.Enabled;
        }

        public float GetAxis(MWAxis axis)
        {
            var result = 0.0f;

            if (axis == MWAxis.Horizontal)
                result += XRInput.GetAxis(XRAxis.ThumbstickX, true);
            else if (axis == MWAxis.Vertical)
                result += XRInput.GetAxis(XRAxis.ThumbstickY, true);
            else if (axis == MWAxis.MouseX)
                result += XRInput.GetAxis(XRAxis.ThumbstickX, false);
            else if (axis == MWAxis.MouseY)
                result += XRInput.GetAxis(XRAxis.ThumbstickY, false);

            return result;
        }

        public bool GetButton(MWButton button)
        {
            if (m_XRMapping == null)
                InitializeMapping();

            if (m_XRMapping.ContainsKey(button))
            {
                var mapping = m_XRMapping[button];
                return XRInput.GetButton(mapping.Button, mapping.LeftHand);
            }

            return false;
        }

        public bool GetButtonDown(MWButton button)
        {
            if (m_XRMapping == null)
                InitializeMapping();

            if (m_XRMapping.ContainsKey(button))
            {
                var mapping = m_XRMapping[button];
                return XRInput.GetButtonDown(mapping.Button, mapping.LeftHand);
            }

            return false;
        }

        public bool GetButtonUp(MWButton button)
        {
            if (m_XRMapping == null)
                InitializeMapping();

            if (m_XRMapping.ContainsKey(button))
            {
                var mapping = m_XRMapping[button];
                return XRInput.GetButtonUp(mapping.Button, mapping.LeftHand);
            }

            return false;
        }

        private static void InitializeMapping()
        {
            m_XRMapping = new Dictionary<MWButton, XRButtonMapping>()
            {
                { MWButton.Jump, new XRButtonMapping(XRButton.Thumbstick, true) },
                { MWButton.Light, new XRButtonMapping(XRButton.Thumbstick, false) },
                { MWButton.Run, new XRButtonMapping(XRButton.Grip, true) },
                { MWButton.Teleport, new XRButtonMapping(XRButton.Grip, false) },
                { MWButton.Recenter, new XRButtonMapping(XRButton.Menu, false) },
                { MWButton.Use, new XRButtonMapping(XRButton.Trigger, false) },
                { MWButton.Menu, new XRButtonMapping(XRButton.Menu, true) }
            };
        }
    }
}
