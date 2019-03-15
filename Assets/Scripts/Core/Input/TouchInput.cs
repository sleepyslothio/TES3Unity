#if UNITY_ANDROID || UNITY_IOS
#define UNITY_MOBILE
#endif

using Demonixis.Toolbox.XR;
using UnityEngine;

namespace TESUnity.Inputs
{
    public class TouchInput : IInputProvider
    {
        private LeftJoystick LeftJoystick = null;
        private RightJoystick RightJoystick = null;

        public bool TryInitialize()
        {
            var valid = !XRManager.Enabled;

#if !UNITY_MOBILE
            valid = false;
#endif

            var prefab = Resources.Load<GameObject>("Prefabs/TouchJoysticks");
            var go = GameObject.Instantiate(prefab);
            LeftJoystick = go.GetComponentInChildren<LeftJoystick>();
            RightJoystick = go.GetComponentInChildren<RightJoystick>();

            return true;
        }

        public float GetAxis(MWAxis axis)
        {
            var result = 0.0f;

            if (axis == MWAxis.Horizontal)
                result += LeftJoystick.GetInputDirection().x;
            else if (axis == MWAxis.Vertical)
                result += LeftJoystick.GetInputDirection().y;
            else if (axis == MWAxis.MouseX)
                result += RightJoystick.GetInputDirection().x;
            else if (axis == MWAxis.MouseY)
                result += RightJoystick.GetInputDirection().y;

            return result;
        }

        public bool GetButton(MWButton button)
        {
            if (button == MWButton.Use)
                return Input.touchCount > 0 && Input.touches[0].phase == TouchPhase.Moved || Input.touches[0].phase == TouchPhase.Stationary;

            return false;
        }

        public bool GetButtonDown(MWButton button)
        {
            if (button == MWButton.Use)
                return Input.touchCount > 0 && Input.touches[0].phase == TouchPhase.Began;

            return false;
        }

        public bool GetButtonUp(MWButton button)
        {
            if (button == MWButton.Use)
                return Input.touchCount > 0 && Input.touches[0].phase == TouchPhase.Ended;

            return false;
        }
    }
}
