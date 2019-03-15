using UnityEngine;

namespace TESUnity.Inputs
{
    public class DesktopInput : IInputProvider
    {
        public bool TryInitialize() => true;
        public float GetAxis(MWAxis axis) => Input.GetAxis(axis.ToString());
        public bool GetButton(MWButton button) => Input.GetButton(button.ToString());
        public bool GetButtonDown(MWButton button) => Input.GetButtonDown(button.ToString());
        public bool GetButtonUp(MWButton button) => Input.GetButtonUp(button.ToString());
    }
}
