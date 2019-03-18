using System;

namespace TESUnity.Inputs
{
    public sealed class WaveVRInput : IInputProvider
    {
        public float GetAxis(MWAxis axis)
        {
            throw new NotImplementedException();
        }

        public bool GetButton(MWButton button)
        {
            throw new NotImplementedException();
        }

        public bool GetButtonDown(MWButton button)
        {
            throw new NotImplementedException();
        }

        public bool GetButtonUp(MWButton button)
        {
            throw new NotImplementedException();
        }

        public bool TryInitialize()
        {
#if WAVEVR_SDK
            return true;
#else
            return false;
#endif
        }
    }
}
