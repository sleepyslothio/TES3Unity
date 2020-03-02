using UnityEngine;

namespace TESUnity.Inputs
{
    public class DesktopInput : IInputProvider
    {
        public bool TryInitialize() => true;

        public float GetAxis(MWAxis axis)
        {
            if (axis == MWAxis.Horizontal)
                return Input.GetAxis("Horizontal");
            else if (axis == MWAxis.Vertical)
                return Input.GetAxis("Vertical");
            else if (axis == MWAxis.MouseX)
                return Input.GetAxis("Mouse X");
            else if (axis == MWAxis.MouseY)
                return Input.GetAxis("Mouse Y");

            return 0.0f;
        }

        public bool Get(MWButton button) => Input.GetButton(button.ToString());
        public bool GetDown(MWButton button) => Input.GetButtonDown(button.ToString());
        public bool GetUp(MWButton button) => Input.GetButtonUp(button.ToString());
    }
}
