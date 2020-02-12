using Demonixis.Toolbox.XR;
using TESUnity;
using TESUnity.Inputs;

namespace TESUnity.Inputs
{
    public class OculusHandTrackingInput : IInputProvider
    {
        private OVRHand m_LeftHand = null;
        private OVRHand m_RightHand = null;

        public OculusHandTrackingInput(OVRHand left, OVRHand right)
        {
            m_LeftHand = left;
            m_RightHand = right;
        }

        public bool Get(MWButton button)
        {
            if (button == MWButton.Use)
            {
                return m_RightHand.GetFingerIsPinching(OVRHand.HandFinger.Index);
            }
            else if (button == MWButton.Light)
            {
                return m_RightHand.GetFingerIsPinching(OVRHand.HandFinger.Pinky);
            }

            return false;
        }

        public float GetAxis(MWAxis axis)
        {
            return 0;
        }

        public bool GetDown(MWButton button)
        {
            if (button == MWButton.Use)
            {
                return m_RightHand.GetFingerIsPinching(OVRHand.HandFinger.Index);
            }
            else if (button == MWButton.Light)
            {
                return m_RightHand.GetFingerIsPinching(OVRHand.HandFinger.Pinky);
            }

            return false;
        }

        public bool GetUp(MWButton button)
        {
            return false;
        }

        public bool TryInitialize()
        {
#if !UNITY_STANDALONE && !UNITY_ANDROID
            return false;
#else
            return XRManager.GetXRVendor() == XRVendor.Oculus && GameSettings.Get().HandTracking;
#endif
        }
    }
}
