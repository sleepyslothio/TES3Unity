using TESUnity.Inputs;
using Wacki;

namespace TESUnity.Components.XR
{
    public sealed class LaserPointer : IUILaserPointer
    {
        public override bool ButtonDown() => InputManager.GetButtonDown(MWButton.Use);
        public override bool ButtonUp() => InputManager.GetButtonUp(MWButton.Use);
    }
}
