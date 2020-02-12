using Demonixis.Toolbox.XR;
using UnityEngine;

namespace TESUnity.Inputs
{
    public class OculusInput : IInputProvider
    {
#if OCULUS_SDK
        public delegate bool GetAxisDelegate(OVRInput.Button virtualMask, OVRInput.Controller controllerMask = OVRInput.Controller.Active);
#endif
        private bool m_6DOFControllers = false;

        public bool TryInitialize()
        {
#if OCULUS_SDK
            //var model = UnityXRDevice.GetVRHeadsetModel();
            //m_6DOFControllers = model == VRHeadsetModel.OculusQuest;
            //Debug.Log($"[TESUnity] Oculus Input Initialized. 6DoF:{m_6DOFControllers}");
            m_6DOFControllers = true; // FIXME
            return OVRManager.loadedXRDevice == OVRManager.XRDevice.Oculus;
            return true;
#else
            return false;
#endif
        }

        public float GetAxis(MWAxis axis)
        {
#if OCULUS_SDK
            if (m_6DOFControllers)
            {
                var value = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick, LeftController);

                if (axis == MWAxis.Vertical)
                    return value.y;
                else if (axis == MWAxis.Horizontal)
                    return value.x;
                else if (axis == MWAxis.MouseX)
                    return OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick, RightController).x;
            }
            else
            {
                var leftValue = OVRInput.Get(OVRInput.Axis2D.PrimaryTouchpad, LeftController);
                if (axis == MWAxis.Vertical)
                    return leftValue.y;
                else if (axis == MWAxis.Horizontal)
                    return leftValue.x;
            }
#endif
            return 0.0f;
        }

#if OCULUS_SDK
        private bool GetButtonState(GetAxisDelegate inputFunction, MWButton button)
        {
            if (button == MWButton.Use)
            {
                return inputFunction(OVRInput.Button.PrimaryIndexTrigger, RightController);
            }
            else if (button == MWButton.Menu)
            {
                return inputFunction(OVRInput.Button.Back, LeftController) ||
                    inputFunction(OVRInput.Button.Start, LeftController);
            }
            else if (button == MWButton.Jump)
            {
                return inputFunction(OVRInput.Button.Two, RightController);
            }
            else if (button == MWButton.Run)
            {
                return inputFunction(OVRInput.Button.PrimaryHandTrigger, LeftController);
            }
            else if (button == MWButton.Recenter)
            {
                return inputFunction(OVRInput.Button.PrimaryThumbstick, LeftController) &&
                    inputFunction(OVRInput.Button.PrimaryThumbstick, RightController);
            }
            else if (button == MWButton.Teleport)
            {
                return inputFunction(OVRInput.Button.PrimaryHandTrigger, LeftController);
            }

            return false;
        }
#endif

        public bool Get(MWButton button)
        {
#if OCULUS_SDK
            return GetButtonState(OVRInput.Get, button);
#else
            return false;
#endif
        }

        public bool GetDown(MWButton button)
        {
#if OCULUS_SDK
            return GetButtonState(OVRInput.GetDown, button);
#else
            return false;
#endif
        }

        public bool GetUp(MWButton button)
        {
#if OCULUS_SDK
            return GetButtonState(OVRInput.GetUp, button);
#else
            return false;
#endif
        }

#if OCULUS_SDK

        public OVRInput.Controller LeftController => m_6DOFControllers ? OVRInput.Controller.LTouch : OVRInput.Controller.Remote;
        public OVRInput.Controller RightController => m_6DOFControllers ? OVRInput.Controller.RTouch : OVRInput.Controller.Remote;

        private OVRInput.Controller GetController(bool left)
        {
            if (m_6DOFControllers)
                return left ? OVRInput.Controller.LTouch : OVRInput.Controller.RTouch;

            return OVRInput.Controller.Active;
        }
#endif
    }
}
