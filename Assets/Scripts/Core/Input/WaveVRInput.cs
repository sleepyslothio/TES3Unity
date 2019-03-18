using System;
#if WAVEVR_SDK
using Demonixis.Toolbox.XR;
using UnityEngine;
using wvr;
#endif

namespace TESUnity.Inputs
{
    public sealed class WaveVRInput : IInputProvider
    {
#if WAVEVR_SDK
        private bool m_IsLeftHanded = false;
        private bool m_Is6DOF = false;
#endif

        public float GetAxis(MWAxis axis)
        {
#if WAVEVR_SDK
            var controller = GetController(true);

            if (axis == MWAxis.Vertical)
            {
                if (controller.GetPress(WVR_InputId.WVR_InputId_Alias1_DPad_Up))
                    return 1.0f;
                else if (controller.GetPress(WVR_InputId.WVR_InputId_Alias1_DPad_Down))
                    return -1.0f;
            }
#endif
            return 0.0f;
        }

        public bool GetButton(MWButton button)
        {
#if WAVEVR_SDK
            if (button == MWButton.Use)
            {
                var pred1 = GetController(false).GetPress(WVR_InputId.WVR_InputId_Alias1_Digital_Trigger);
                var pred2 = GetController(false).GetPress(WVR_InputId.WVR_InputId_Alias1_Trigger);
                return pred1 || pred2;
            }
            else if (button == MWButton.Menu)
                return GetController(false).GetPress(WVR_InputId.WVR_InputId_Alias1_Menu);
            else if (button == MWButton.Teleport)
                return GetController(false).GetPress(WVR_InputId.WVR_InputId_Alias1_Grip);
#endif
            return false;
        }

        public bool GetButtonDown(MWButton button)
        {
#if WAVEVR_SDK
            if (button == MWButton.Use)
            {
                var pred1 = GetController(false).GetPressDown(WVR_InputId.WVR_InputId_Alias1_Digital_Trigger);
                var pred2 = GetController(false).GetPressDown(WVR_InputId.WVR_InputId_Alias1_Trigger);
                return pred1 || pred2;
            }
            else if (button == MWButton.Menu)
                return GetController(false).GetPressDown(WVR_InputId.WVR_InputId_Alias1_Menu);
            else if (button == MWButton.Teleport)
                return GetController(false).GetPressDown(WVR_InputId.WVR_InputId_Alias1_Grip);
#endif
            return false;
        }

        public bool GetButtonUp(MWButton button)
        {
#if WAVEVR_SDK
            if (button == MWButton.Use)
            {
                var pred1 = GetController(false).GetPressUp(WVR_InputId.WVR_InputId_Alias1_Digital_Trigger);
                var pred2 = GetController(false).GetPressUp(WVR_InputId.WVR_InputId_Alias1_Trigger);
                return pred1 || pred2;
            }
            else if (button == MWButton.Menu)
                return GetController(false).GetPressUp(WVR_InputId.WVR_InputId_Alias1_Menu);
            else if (button == MWButton.Teleport)
                return GetController(false).GetPressUp(WVR_InputId.WVR_InputId_Alias1_Grip);
#endif
            return false;
        }

        public bool TryInitialize()
        {
#if WAVEVR_SDK
#if UNITY_EDITOR
            var waveVRDevice = GameObject.FindObjectOfType<WaveVRDevice>();
            if (waveVRDevice.DisableInEditor)
                return false;
#endif
            m_IsLeftHanded = WaveVR_Controller.IsLeftHanded;

#if !UNITY_EDITOR
            var dofLeft = Interop.WVR_GetDegreeOfFreedom(WVR_DeviceType.WVR_DeviceType_Controller_Left);
            var dofRight = Interop.WVR_GetDegreeOfFreedom(WVR_DeviceType.WVR_DeviceType_Controller_Right);
            m_Is6DOF = dofLeft == WVR_NumDoF.WVR_NumDoF_6DoF || dofRight == WVR_NumDoF.WVR_NumDoF_6DoF;
#endif

            return true;
#else
            return false;
#endif
        }

#if WAVEVR_SDK
        private WaveVR_Controller.Device GetController(bool left)
        {
            var isLeftHand = m_IsLeftHanded;

            if (m_Is6DOF)
                isLeftHand = left;

            var controller = isLeftHand ? WVR_DeviceType.WVR_DeviceType_Controller_Left : WVR_DeviceType.WVR_DeviceType_Controller_Right;

            return WaveVR_Controller.Input(controller);
        }
#endif
    }
}
