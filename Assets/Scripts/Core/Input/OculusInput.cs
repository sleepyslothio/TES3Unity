using Demonixis.Toolbox.XR;

namespace TESUnity.Inputs
{
    public class OculusInput : IInputProvider
    {
        public delegate bool GetAxisDelegate(OVRInput.Button virtualMask, OVRInput.Controller controllerMask = OVRInput.Controller.Active);
        private bool m_6DOFControllers = false;

        public bool TryInitialize()
        {
#if OCULUS_SDK
            if (UnityXRDevice.IsOculus)
            {
                m_6DOFControllers = UnityXRDevice.GetVRHeadsetModel() == VRHeadsetModel.OculusQuest;
                return true;
            }
#endif
            return false;
        }

        public float GetAxis(MWAxis axis)
        {
            var result = 0.0f;
#if OCULUS_SDK
            var leftValue = OVRInput.Get(OVRInput.Axis2D.PrimaryTouchpad, GetController(false));

            if (axis == MWAxis.Vertical)
                result = leftValue.y;
            else if (axis == MWAxis.Horizontal)
                result = leftValue.x;
            else if (axis == MWAxis.MouseX)
                return OVRInput.Get(OVRInput.Axis2D.PrimaryTouchpad, GetController(true)).x;

#endif
            return result;
        }

        private bool GetButtonState(GetAxisDelegate inputFunction, MWButton button)
        {
#if OCULUS_SDK
            if (button == MWButton.Use)
            {
                return inputFunction(OVRInput.Button.PrimaryIndexTrigger, GetController(true));
            }
            else if (button == MWButton.Menu)
            {
                return inputFunction(OVRInput.Button.Back, GetController(false)) ||
                    inputFunction(OVRInput.Button.Start, GetController(false));
            }
            else if (button == MWButton.Jump)
            {
                inputFunction(OVRInput.Button.One, GetController(false));
            }
#endif
            return false;
        }

        public bool GetButton(MWButton button)
        {
#if OCULUS_SDK
            return GetButtonState(OVRInput.Get, button);
#else
            return false;
#endif
        }

        public bool GetButtonDown(MWButton button)
        {
#if OCULUS_SDK
            return GetButtonState(OVRInput.GetDown, button);
#else
            return false;
#endif
        }

        public bool GetButtonUp(MWButton button)
        {
#if OCULUS_SDK
            return GetButtonState(OVRInput.GetUp, button);
#else
            return false;
#endif
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
