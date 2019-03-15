#define OCULUS_SDK // FIXME
using UnityEngine.XR;

namespace TESUnity.Inputs
{
    public class OculusInput : IInputProvider
    {
        private bool m_LeftHandEnabled = false;
        private bool m_6DOFControllers = false;

        public bool TryInitialize()
        {
#if OCULUS_SDK
            if (XRSettings.loadedDeviceName.ToLower() == "oculus")
            {
                var hand = OVRInput.GetDominantHand();
                m_LeftHandEnabled = hand == OVRInput.Handedness.LeftHanded;
                m_6DOFControllers = OVRInput.IsControllerConnected(OVRInput.Controller.LTouch) || OVRInput.IsControllerConnected(OVRInput.Controller.RTouch);
                return true;
            }
#endif
            return false;
        }

        public float GetAxis(MWAxis axis)
        {
            var result = 0.0f;
#if OCULUS_SDK
            var value = OVRInput.Get(OVRInput.Axis2D.PrimaryTouchpad);

            if (axis == MWAxis.Horizontal)
                result = value.x;
            else if (axis == MWAxis.Vertical)
                result = value.y;
#endif
            return result;
        }

        public bool GetButton(MWButton button)
        {
#if OCULUS_SDK
            if (button == MWButton.Use)
                return OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger, GetController(false));
            else if (button == MWButton.Menu)
                return OVRInput.Get(OVRInput.Button.Back, GetController(false));
            else if (button == MWButton.Teleport)
                OVRInput.Get(OVRInput.Button.PrimaryThumbstickUp, GetController(false));
            else if (button == MWButton.Light)
                OVRInput.Get(OVRInput.Button.PrimaryThumbstickDown, GetController(false));
#endif
            return false;
        }

        public bool GetButtonDown(MWButton button)
        {
#if OCULUS_SDK
            if (button == MWButton.Use)
                return OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger, GetController(false));
            else if (button == MWButton.Menu)
                return OVRInput.Get(OVRInput.Button.Back, GetController(false));
            else if (button == MWButton.Teleport)
                OVRInput.GetDown(OVRInput.Button.PrimaryThumbstickUp, GetController(false));
            else if (button == MWButton.Light)
                OVRInput.GetDown(OVRInput.Button.PrimaryThumbstickDown, GetController(false));
#endif
            return false;
        }

        public bool GetButtonUp(MWButton button)
        {
#if OCULUS_SDK
            if (button == MWButton.Use)
                return OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger, GetController(false));
            else if (button == MWButton.Menu)
                return OVRInput.Get(OVRInput.Button.Back, GetController(false));
            else if (button == MWButton.Teleport)
                OVRInput.GetUp(OVRInput.Button.PrimaryThumbstickUp, GetController(false));
            else if (button == MWButton.Light)
                OVRInput.GetUp(OVRInput.Button.PrimaryThumbstickDown, GetController(false));
#endif
            return false;
        }

#if OCULUS_SDK
        private OVRInput.Controller GetController(bool left)
        {
            if (m_6DOFControllers)
                return left ? OVRInput.Controller.LTouch : OVRInput.Controller.RTouch;

            return OVRInput.Controller.Active;
        }
#endif
    }
}
