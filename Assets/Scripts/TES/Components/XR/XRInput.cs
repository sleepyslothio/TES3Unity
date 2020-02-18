using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

namespace Demonixis.Toolbox.XR
{
    #region Enumerations

    public enum XRButton
    {
        Primary,
        Menu,
        Trigger,
        Grip,
        Thumbstick,
        ThumbstickUp,
        ThumbstickDown,
        ThumbstickLeft,
        ThumbstickRight,
        Touchpad
    }

    public enum XRAxis
    {
        Trigger,
        Grip,
        ThumbstickX,
        ThumbstickY,
        TouchpadX,
        TouchpadY
    }

    public enum XRAxis2D
    {
        Thumbstick,
        Touchpad
    }

    #endregion

    public static class XRInput
    {
        private static CoroutineExecuter Executer = null;
        private static List<XRNodeState> XRNodeStats = new List<XRNodeState>();
        public static float DeadZone { get; set; } = 0.25f;

        public static bool IsConnected()
        {
            var left = false;
            var right = false;

            XRNodeStats.Clear();
            InputTracking.GetNodeStates(XRNodeStats);

            foreach (var state in XRNodeStats)
            {
                if (state.nodeType == XRNode.LeftHand)
                    left = state.tracked;

                else if (state.nodeType == XRNode.RightHand)
                    right = state.tracked;
            }

            return left || right;
        }

        public static void GetConnectedControllers(ref bool left, ref bool right)
        {
            XRNodeStats.Clear();
            InputTracking.GetNodeStates(XRNodeStats);

            foreach (var state in XRNodeStats)
            {
                if (state.nodeType == XRNode.LeftHand)
                    left = state.tracked;

                else if (state.nodeType == XRNode.RightHand)
                    right = state.tracked;
            }
        }

        #region Methods to get button states

        /// <summary>
        /// Indicates whether a button is pressed.
        /// </summary>
        /// <param name="button">The button.</param>
        /// <param name="left">Left or Right controller.</param>
        /// <returns>Returns true if pressed otherwise it returns false.</returns>
        public static bool GetButton(XRButton button, bool left)
        {
            if (button == XRButton.Primary)
                return Input.GetKey(left ? "joystick button 2" : "joystick button 0");
            else if (button == XRButton.Menu)
                return Input.GetKey(left ? "joystick button 6" : "joystick button 7");
            else if (button == XRButton.Trigger)
                return Input.GetKey(left ? "joystick button 14" : "joystick button 15");
            else if (button == XRButton.Grip)
                return Input.GetKey(left ? "joystick button 4" : "joystick button 5");
            else if (button == XRButton.Thumbstick)
                return Input.GetKey(left ? "joystick button 8" : "joystick button 9");
            else if (button == XRButton.Touchpad)
                return Input.GetKey(left ? "joystick button 16" : "joystick button 17");
            else if (button == XRButton.ThumbstickUp)
                return GetAxis(XRAxis.ThumbstickY, left) > DeadZone;
            else if (button == XRButton.ThumbstickDown)
                return GetAxis(XRAxis.ThumbstickY, left) < -DeadZone;
            else if (button == XRButton.ThumbstickLeft)
                return GetAxis(XRAxis.ThumbstickX, left) < -DeadZone;
            else if (button == XRButton.ThumbstickRight)
                return GetAxis(XRAxis.ThumbstickX, left) > DeadZone;

            return false;
        }

        /// <summary>
        /// Indicates whether a button was pressed.
        /// </summary>
        /// <param name="button">The button.</param>
        /// <param name="left">Left or Right controller.</param>
        /// <returns>Returns true if pressed otherwise it returns false.</returns>
        public static bool GetButtonDown(XRButton button, bool left)
        {
            if (button == XRButton.Primary)
                return Input.GetKeyDown(left ? "joystick button 2" : "joystick button 0");
            else if (button == XRButton.Menu)
                return Input.GetKeyDown(left ? "joystick button 6" : "joystick button 7");
            else if (button == XRButton.Trigger)
                return Input.GetKeyDown(left ? "joystick button 14" : "joystick button 15");
            else if (button == XRButton.Grip)
                return Input.GetKeyDown(left ? "joystick button 4" : "joystick button 5");
            else if (button == XRButton.Thumbstick)
                return Input.GetKeyDown(left ? "joystick button 8" : "joystick button 9");
            else if (button == XRButton.Touchpad)
                return Input.GetKeyDown(left ? "joystick button 16" : "joystick button 17");
            else if (button == XRButton.ThumbstickUp)
                return GetAxis(XRAxis.ThumbstickY, left) > DeadZone && GetButtonDown(XRButton.Thumbstick, left);
            else if (button == XRButton.ThumbstickDown)
                return GetAxis(XRAxis.ThumbstickY, left) < -DeadZone && GetButtonDown(XRButton.Thumbstick, left);
            else if (button == XRButton.ThumbstickLeft)
                return GetAxis(XRAxis.ThumbstickX, left) < -DeadZone && GetButtonDown(XRButton.Thumbstick, left);
            else if (button == XRButton.ThumbstickRight)
                return GetAxis(XRAxis.ThumbstickX, left) > DeadZone && GetButtonDown(XRButton.Thumbstick, left);

            return false;
        }

        /// <summary>
        /// Indicates whether a button was released.
        /// </summary>
        /// <param name="button">The button.</param>
        /// <param name="left">Left or Right controller.</param>
        /// <returns>Returns true if pressed otherwise it returns false.</returns>
        public static bool GetButtonUp(XRButton button, bool left)
        {
            if (button == XRButton.Primary)
                return Input.GetKeyUp(left ? "joystick button 2" : "joystick button 0");
            else if (button == XRButton.Menu)
                return Input.GetKeyUp(left ? "joystick button 6" : "joystick button 7");
            else if (button == XRButton.Trigger)
                return Input.GetKeyUp(left ? "joystick button 14" : "joystick button 15");
            else if (button == XRButton.Grip)
                return Input.GetKeyUp(left ? "joystick button 4" : "joystick button 5");
            else if (button == XRButton.Thumbstick)
                return Input.GetKeyUp(left ? "joystick button 8" : "joystick button 9");
            else if (button == XRButton.Touchpad)
                return Input.GetKeyUp(left ? "joystick button 16" : "joystick button 17");
            else if (button == XRButton.ThumbstickUp)
                return GetAxis(XRAxis.ThumbstickY, left) > DeadZone && GetButtonUp(XRButton.Thumbstick, left);
            else if (button == XRButton.ThumbstickDown)
                return GetAxis(XRAxis.ThumbstickY, left) < -DeadZone && GetButtonUp(XRButton.Thumbstick, left);
            else if (button == XRButton.ThumbstickLeft)
                return GetAxis(XRAxis.ThumbstickX, left) < -DeadZone && GetButtonUp(XRButton.Thumbstick, left);
            else if (button == XRButton.ThumbstickRight)
                return GetAxis(XRAxis.ThumbstickX, left) > DeadZone && GetButtonUp(XRButton.Thumbstick, left);

            return false;
        }

        #endregion

        #region Helpers

        /// <summary>
        /// Indicates if the menu button is pressed. On some platforms the menu button is binded on XRButton.Menu, 
        /// on other platforms it's binded on XRButton.Primary.
        /// </summary>
        /// <param name="left"></param>
        /// <returns></returns>
        public static bool GetMenuDown(bool left)
        {
            return GetButtonDown(XRButton.Menu, left) || GetButtonDown(XRButton.Primary, left);
        }

        /// <summary>
        /// Indicates if the button is pressed on the left or right controller.
        /// </summary>
        /// <param name="button">The button.</param>
        /// <returns>Returns true if the button is pressed on the left or right controller.</returns>
        public static bool GetAnyButton(XRButton button)
        {
            return GetButton(button, false) || GetButton(button, true);
        }

        /// <summary>
        /// Indicates if the button is pressed on both controllers.
        /// </summary>
        /// <param name="button">The button.</param>
        /// <returns>Returns true if the button is pressed on both left and right controllers.</returns>
        public static bool GetBothButtons(XRButton button)
        {
            return GetButton(button, false) && GetButton(button, true);
        }

        /// <summary>
        /// Indicates if the button was pressed on the left or right controller.
        /// </summary>
        /// <param name="button">The button.</param>
        /// <returns>Returns true if the button was pressed on the left or right controller.</returns>
        public static bool GetAnyButtonDown(XRButton button)
        {
            return GetButtonDown(button, false) || GetButtonDown(button, true);
        }

        /// <summary>
        /// Indicates if the button was pressed on both controllers.
        /// </summary>
        /// <param name="button">The button.</param>
        /// <returns>Returns true if the button was pressed on both left and right controllers.</returns>
        public static bool GetBothButtonsDown(XRButton button)
        {
            return GetButtonDown(button, false) && GetButtonDown(button, true);
        }

        /// <summary>
        /// Indicates if the button was released on the left or right controllers.
        /// </summary>
        /// <param name="button">The button.</param>
        /// <returns>Returns true if the button was released on the left or right controller.</returns>
        public static bool GetAnyButtonUp(XRButton button)
        {
            return GetButtonUp(button, false) || GetButtonUp(button, true);
        }

        /// <summary>
        /// Indicates if the button was just released on both controllers.
        /// </summary>
        /// <param name="button">The button.</param>
        /// <returns>Returns true if the button was just released on both controllers.</returns>
        public static bool GetBothButtonsUp(XRButton button)
        {
            return GetButtonUp(button, false) && GetButtonUp(button, true);
        }

        #endregion

        #region Methods to get axis state

        /// <summary>
        /// Gets an axis value.
        /// </summary>
        /// <param name="axis">The axis.</param>
        /// <param name="left">Left or Right controller.</param>
        /// <returns>Returns the axis value.</returns>
        public static float GetAxis(XRAxis axis, bool left)
        {
            if (axis == XRAxis.Trigger)
                return Input.GetAxis(left ? "Axis 9" : "Axis 10");
            else if (axis == XRAxis.Grip)
                return Input.GetAxis(left ? "Axis 11" : "Axis 12");
            else if (axis == XRAxis.ThumbstickX)
                return Input.GetAxis(left ? "Axis 1" : "Axis 4");
            else if (axis == XRAxis.ThumbstickY)
                return Input.GetAxis(left ? "Axis 2" : "Axis 5");
            else if (axis == XRAxis.TouchpadX)
                return Input.GetAxis(left ? "Axis 17" : "Axis 19");
            else if (axis == XRAxis.TouchpadX)
                return Input.GetAxis(left ? "Axis 18" : "Axis 20");

            return 0.0f;
        }

        /// <summary>
        /// Gets two axis values.
        /// </summary>
        /// <param name="axis"></param>
        /// <param name="left">Left or Right controller.</param>
        /// <returns>Returns two axis values.</returns>
        public static Vector2 GetAxis2D(XRAxis2D axis, bool left)
        {
            if (axis == XRAxis2D.Thumbstick)
            {
                return new Vector2(
                    Input.GetAxis(left ? "Axis 1" : "Axis 4"),
                    Input.GetAxis(left ? "Axis 2" : "Axis 5"));
            }
            else if (axis == XRAxis2D.Touchpad)
            {
                return new Vector2(
                    Input.GetAxis(left ? "Axis 17" : "Axis 19"),
                    Input.GetAxis(left ? "Axis 18" : "Axis 20"));
            }

            return Vector2.zero;
        }

        #endregion

        #region Vibration

        public static void Vibrate(XRNode node, float amplitude = 0.5f, float seconds = 0.25f)
        {
            var device = InputDevices.GetDeviceAtXRNode(node);

            if (device.TryGetHapticCapabilities(out HapticCapabilities capabilities))
            {
                if (capabilities.supportsBuffer)
                {
                    byte[] buffer = { };

                    if (GenerateBuzzClip(seconds, node, ref buffer))
                        device.SendHapticBuffer(0, buffer);
                }
                else if (capabilities.supportsImpulse)
                    device.SendHapticImpulse(0, amplitude, seconds);

                StartCoroutine(StopVibrationCoroutine(device, seconds));
            }
        }

        private static Coroutine StartCoroutine(IEnumerator routine)
        {
            if (Executer == null)
            {
                var go = new GameObject("CoroutineExecuter");
                Executer = go.AddComponent<CoroutineExecuter>();
            }

            return Executer.StartCoroutine(routine);
        }

        private static IEnumerator StopVibrationCoroutine(InputDevice device, float delay)
        {
            yield return new WaitForSeconds(delay);
            device.StopHaptics();
        }

        public static bool GenerateBuzzClip(float seconds, XRNode node, ref byte[] clip)
        {
            var caps = new HapticCapabilities();
            var device = InputDevices.GetDeviceAtXRNode(node);
            var result = device.TryGetHapticCapabilities(out caps);

            if (result)
            {
                var clipCount = (int)(caps.bufferFrequencyHz * seconds);
                clip = new byte[clipCount];

                for (int i = 0; i < clipCount; i++)
                    clip[i] = byte.MaxValue;
            }

            return result;
        }

        #endregion
    }

    public class CoroutineExecuter : MonoBehaviour { }
}