#define STEAMVR_SDK_
#if UNITY_STANDALONE && STEAMVR_SDK
#define STEAMVR_INPUT
#endif

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
#if UNITY_WSA
using UnityEngine.XR.WSA.Input;
#endif

#if STEAMVR_INPUT
using Valve.VR;
#endif

namespace Demonixis.Toolbox.XR
{
    public enum XRButton
    {
        Menu,
        Button1,
        Button2,
        Button3,
        Button4,
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

    public enum XRVendor
    {
        None = 0, Oculus, OpenVR, WindowsMR
    }

    public sealed class XRInput : MonoBehaviour
    {
        private static XRInput s_Instance = null;
        private Vector2 m_TmpVector = Vector2.zero;
        private List<XRNodeState> m_XRNodeStates = new List<XRNodeState>();
        private XRButton[] m_Buttons = null;
        private bool m_Running = true;
        private bool[] m_AxisStates = null;

#if STEAMVR_INPUT
        private int m_SteamVRLeftIndex;
        private int m_SteamVRRightIndex;
#endif

        [SerializeField]
        private float m_DeadZone = 0.25f;

        public static XRInput Instance
        {
            get
            {
                if (s_Instance == null)
                {
                    var gameObject = new GameObject("XRInput");
                    s_Instance = gameObject.AddComponent<XRInput>();
                }

                return s_Instance;
            }
        }

        public bool UseNativeIntegration { get; set; } = false;

        public XRVendor InputVendor { get; private set; }

        public bool IsConnected { get; private set; }

        public float DeadZone
        {
            get { return m_DeadZone; }
            set
            {
                m_DeadZone = value;

                if (m_DeadZone < 0)
                    m_DeadZone = 0.0f;
                else if (m_DeadZone >= 1.0f)
                    m_DeadZone = 1.0f;
            }
        }

        private delegate float GetAxisFunction(string axis);

        public void Awake()
        {
            if (s_Instance != null && s_Instance != this)
            {
                Destroy(this);
                return;
            }

            var vendor = XRSettings.loadedDeviceName.ToLower();

#if UNITY_WSA
            InputVendor = XRVendor.WindowsMR;
#else

#if STEAMVR_INPUT
            var trackingSystemName = SteamVR.instance.hmd_TrackingSystemName.ToLower();
#endif

            if (vendor == "openvr")
            {
                InputVendor = XRVendor.OpenVR;

#if STEAMVR_INPUT
                if (trackingSystemName == "oculus")
                    InputVendor = XRVendor.Oculus;
#endif
            }
            else if (vendor == "oculus")
                InputVendor = XRVendor.Oculus;
#endif

#if UNITY_2018_2 && STEAMVR_INPUT
            if (trackingSystemName == "holographic")
                UseNativeIntegration = false;
#endif

            m_Buttons = new XRButton[]
            {
                XRButton.Trigger, XRButton.Grip,
                XRButton.ThumbstickUp, XRButton.ThumbstickDown,
                XRButton.ThumbstickLeft, XRButton.ThumbstickRight
            };

            m_AxisStates = new bool[m_Buttons.Length * 2];

            StartCoroutine(UpdateAxisToButton());

            CheckConnected();
        }

#if STEAMVR_INPUT

        private bool CheckSteamVRIndex()
        {
            if (InputVendor == XRVendor.OpenVR && !UseNativeIntegration)
            {
                m_SteamVRLeftIndex = SteamVR_Controller.GetDeviceIndex(SteamVR_Controller.DeviceRelation.Leftmost);
                m_SteamVRRightIndex = SteamVR_Controller.GetDeviceIndex(SteamVR_Controller.DeviceRelation.Rightmost);

                return m_SteamVRLeftIndex > -1 && m_SteamVRRightIndex > -1;
            }

            return false;
        }

#endif

        private void OnDestroy()
        {
            m_Running = false;
        }

        public bool CheckConnected(bool forceCheck = false)
        {
            if (IsConnected && !forceCheck)
                return IsConnected;

            m_XRNodeStates.Clear();
            InputTracking.GetNodeStates(m_XRNodeStates);

            var left = false;
            var right = false;

            foreach (var state in m_XRNodeStates)
            {
                if (state.nodeType == XRNode.LeftHand)
                    left = state.tracked;

                else if (state.nodeType == XRNode.RightHand)
                    right = state.tracked;
            }

            IsConnected = left || right;

            return IsConnected;
        }

        private IEnumerator UpdateAxisToButton()
        {
            var endOfFrame = new WaitForEndOfFrame();
            var index = 0;

            while (m_Running)
            {
                index = 0;

                for (var i = 0; i < m_Buttons.Length; i++)
                {
                    m_AxisStates[index] = GetButton(m_Buttons[i], true);
                    m_AxisStates[index + 1] = GetButton(m_Buttons[i], false);
                    index += 2;
                }

                yield return endOfFrame;
            }
        }

        /// <summary>
        /// Gets the position of a specific node.
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public Vector3 GetLocalPosition(XRNode node)
        {
            return InputTracking.GetLocalPosition(node);
        }

        /// <summary>
        /// Gets the rotation of a specific node.
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public Quaternion GetLocalRotation(XRNode node)
        {
            return InputTracking.GetLocalRotation(node);
        }


        #region Methods to get button states

        /// <summary>
        /// Indicates whether a button is pressed.
        /// </summary>
        /// <param name="button">The button.</param>
        /// <param name="left">Left or Right controller.</param>
        /// <returns>Returns true if pressed otherwise it returns false.</returns>
        public bool GetButton(XRButton button, bool left)
        {
#if STEAMVR_INPUT
            if (CheckSteamVRIndex())
            {
                var device = SteamVR_Controller.Input(left ? m_SteamVRLeftIndex : m_SteamVRRightIndex);

                if (button == XRButton.Grip)
                    return device.GetPress(EVRButtonId.k_EButton_Grip);
                else if (button == XRButton.Menu)
                    return device.GetPress(EVRButtonId.k_EButton_ApplicationMenu);
                else if (button == XRButton.Trigger)
                    return device.GetPress(EVRButtonId.k_EButton_SteamVR_Trigger);
                else if (button == XRButton.Thumbstick)
                    return device.GetPress(EVRButtonId.k_EButton_SteamVR_Touchpad);
            }
#endif

            if (InputVendor == XRVendor.Oculus)
            {
                if (button == XRButton.Button1)
                    return Input.GetKey("joystick button 0");

                else if (button == XRButton.Button2)
                    return Input.GetKey("joystick button 1");

                else if (button == XRButton.Button3)
                    return Input.GetKey("joystick button 2");

                else if (button == XRButton.Button4)
                    return Input.GetKey("joystick button 3");
            }

            if (button == XRButton.Menu)
            {
                if (InputVendor == XRVendor.OpenVR)
                    return Input.GetKey(left ? "joystick button 2" : "joystick button 0");
                else if (InputVendor == XRVendor.WindowsMR)
                    return Input.GetKey(left ? "joystick button 6" : "joystick button 7");

                return Input.GetKey("joystick button 7");
            }

            if (InputVendor == XRVendor.Oculus)
            {
                if (button == XRButton.Trigger)
                    return GetRawAxis(XRAxis.Trigger, left) > m_DeadZone;

                else if (button == XRButton.Grip)
                    return GetRawAxis(XRAxis.Grip, left) > m_DeadZone;
            }
            else
            {
                if (button == XRButton.Trigger)
                    return Input.GetKey(left ? "joystick button 14" : "joystick button 15");

                else if (button == XRButton.Grip)
                    return Input.GetKey(left ? "joystick button 4" : "joystick button 5");
            }

            if (button == XRButton.Thumbstick)
                return Input.GetKey(left ? "joystick button 8" : "joystick button 9");

            else if (button == XRButton.Touchpad)
                return Input.GetKey(left ? "joystick button 16" : "joystick button 17");

            if (button == XRButton.ThumbstickUp)
                return GetRawAxis(XRAxis.ThumbstickY, left) > m_DeadZone;

            else if (button == XRButton.ThumbstickDown)
                return GetRawAxis(XRAxis.ThumbstickY, left) < m_DeadZone * -1.0f;

            else if (button == XRButton.ThumbstickLeft)
                return GetRawAxis(XRAxis.ThumbstickX, left) < m_DeadZone * -1.0f;

            else if (button == XRButton.ThumbstickRight)
                return GetRawAxis(XRAxis.ThumbstickX, left) > m_DeadZone;

            return false;
        }

        /// <summary>
        /// Indicates whether a button was pressed.
        /// </summary>
        /// <param name="button">The button.</param>
        /// <param name="left">Left or Right controller.</param>
        /// <returns>Returns true if pressed otherwise it returns false.</returns>
        public bool GetButtonDown(XRButton button, bool left)
        {
#if STEAMVR_INPUT
            if (CheckSteamVRIndex())
            {
                var device = SteamVR_Controller.Input(left ? m_SteamVRLeftIndex : m_SteamVRRightIndex);

                if (button == XRButton.Grip)
                    return device.GetPressDown(EVRButtonId.k_EButton_Grip);
                else if (button == XRButton.Menu)
                    return device.GetPressDown(EVRButtonId.k_EButton_ApplicationMenu);
                else if (button == XRButton.Trigger)
                    return device.GetPressDown(EVRButtonId.k_EButton_SteamVR_Trigger);
                else if (button == XRButton.Thumbstick)
                    return device.GetPressDown(EVRButtonId.k_EButton_SteamVR_Touchpad);
            }
#endif

            if (InputVendor == XRVendor.Oculus)
            {
                if (button == XRButton.Button1)
                    return Input.GetKeyDown("joystick button 0");

                else if (button == XRButton.Button2)
                    return Input.GetKeyDown("joystick button 1");

                else if (button == XRButton.Button3)
                    return Input.GetKeyDown("joystick button 2");

                else if (button == XRButton.Button4)
                    return Input.GetKeyDown("joystick button 3");
            }

            if (button == XRButton.Menu)
            {
                if (InputVendor == XRVendor.OpenVR)
                    return Input.GetKeyDown(left ? "joystick button 2" : "joystick button 0");
                else if (InputVendor == XRVendor.WindowsMR)
                    return Input.GetKeyDown(left ? "joystick button 6" : "joystick button 7");

                return Input.GetKeyDown("joystick button 7");
            }

            if (InputVendor != XRVendor.Oculus)
            {
                if (button == XRButton.Trigger)
                    return Input.GetKeyDown(left ? "joystick button 14" : "joystick button 15");

                else if (button == XRButton.Grip)
                    return Input.GetKeyDown(left ? "joystick button 4" : "joystick button 5");
            }

            if (button == XRButton.Thumbstick)
                return Input.GetKeyDown(left ? "joystick button 8" : "joystick button 9");

            else if (button == XRButton.Touchpad)
                return Input.GetKeyDown(left ? "joystick button 16" : "joystick button 17");

            // Simulate other buttons using previous states.
            var index = 0;
            for (var i = 0; i < m_Buttons.Length; i++)
            {
                if (m_Buttons[i] != button)
                {
                    index += 2;
                    continue;
                }

                var prev = m_AxisStates[left ? index : index + 1];
                var now = GetButton(m_Buttons[i], left);

                return now && !prev;
            }

            return false;
        }

        /// <summary>
        /// Indicates whether a button was released.
        /// </summary>
        /// <param name="button">The button.</param>
        /// <param name="left">Left or Right controller.</param>
        /// <returns>Returns true if pressed otherwise it returns false.</returns>
        public bool GetButtonUp(XRButton button, bool left)
        {
#if STEAMVR_INPUT
            if (CheckSteamVRIndex())
            {
                var device = SteamVR_Controller.Input(left ? m_SteamVRLeftIndex : m_SteamVRRightIndex);

                if (button == XRButton.Grip)
                    return device.GetPressUp(EVRButtonId.k_EButton_Grip);
                else if (button == XRButton.Menu)
                    return device.GetPressUp(EVRButtonId.k_EButton_ApplicationMenu);
                else if (button == XRButton.Trigger)
                    return device.GetPressUp(EVRButtonId.k_EButton_SteamVR_Trigger);
                else if (button == XRButton.Thumbstick)
                    return device.GetPressUp(EVRButtonId.k_EButton_SteamVR_Touchpad);
            }
#endif

            if (InputVendor == XRVendor.Oculus)
            {
                if (button == XRButton.Button1)
                    return Input.GetKeyUp("joystick button 0");

                else if (button == XRButton.Button2)
                    return Input.GetKeyUp("joystick button 1");

                else if (button == XRButton.Button3)
                    return Input.GetKeyUp("joystick button 2");

                else if (button == XRButton.Button4)
                    return Input.GetKeyUp("joystick button 3");
            }

            if (button == XRButton.Menu)
            {
                if (InputVendor == XRVendor.OpenVR)
                    return Input.GetKeyUp(left ? "joystick button 2" : "joystick button 0");
                else if (InputVendor == XRVendor.WindowsMR)
                    return Input.GetKeyUp(left ? "joystick button 6" : "joystick button 7");

                return Input.GetKeyUp("joystick button 7");
            }

            if (InputVendor != XRVendor.Oculus)
            {
                if (button == XRButton.Trigger)
                    return Input.GetKeyUp(left ? "joystick button 14" : "joystick button 15");

                else if (button == XRButton.Grip)
                    return Input.GetKeyUp(left ? "joystick button 4" : "joystick button 5");
            }

            else if (button == XRButton.Thumbstick)
                return Input.GetKeyUp(left ? "joystick button 8" : "joystick button 9");

            else if (button == XRButton.Touchpad)
                return Input.GetKeyUp(left ? "joystick button 16" : "joystick button 17");

            // Simulate other buttons using previous states.
            var index = 0;
            for (var i = 0; i < m_Buttons.Length; i++)
            {
                if (m_Buttons[i] != button)
                {
                    index += 2;
                    continue;
                }

                var prev = m_AxisStates[left ? index : index + 1];
                var now = GetButton(m_Buttons[i], left);

                return !now && prev;
            }

            return false;
        }

        /// <summary>
        /// Indicates if the button is pressed on the left or right controller.
        /// </summary>
        /// <param name="button">The button.</param>
        /// <returns>Returns true if the button is pressed on the left or right controller.</returns>
        public bool GetAnyButton(XRButton button)
        {
            return GetButton(button, false) || GetButton(button, true);
        }

        /// <summary>
        /// Indicates if the button is pressed on both controllers.
        /// </summary>
        /// <param name="button">The button.</param>
        /// <returns>Returns true if the button is pressed on both left and right controllers.</returns>
        public bool GetBothButtons(XRButton button)
        {
            return GetButton(button, false) && GetButton(button, true);
        }

        /// <summary>
        /// Indicates if the button was pressed on the left or right controller.
        /// </summary>
        /// <param name="button">The button.</param>
        /// <returns>Returns true if the button was pressed on the left or right controller.</returns>
        public bool GetAnyButtonDown(XRButton button)
        {
            return GetButtonDown(button, false) || GetButtonDown(button, true);
        }

        /// <summary>
        /// Indicates if the button was pressed on both controllers.
        /// </summary>
        /// <param name="button">The button.</param>
        /// <returns>Returns true if the button was pressed on both left and right controllers.</returns>
        public bool GetBothButtonsDown(XRButton button)
        {
            return GetButtonDown(button, false) && GetButtonDown(button, true);
        }

        /// <summary>
        /// Indicates if the button was released on the left or right controllers.
        /// </summary>
        /// <param name="button">The button.</param>
        /// <returns>Returns true if the button was released on the left or right controller.</returns>
        public bool GetAnyButtonUp(XRButton button)
        {
            return GetButtonUp(button, false) || GetButtonUp(button, true);
        }

        /// <summary>
        /// Indicates if the button was just released on both controllers.
        /// </summary>
        /// <param name="button">The button.</param>
        /// <returns>Returns true if the button was just released on both controllers.</returns>
        public bool GetBothButtonsUp(XRButton button)
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
        public float GetAxis(XRAxis axis, bool left)
        {
            return GetAxis(Input.GetAxis, axis, left);
        }

        /// <summary>
        /// Gets a raw axis value.
        /// </summary>
        /// <param name="axis">The axis.</param>
        /// <param name="left">Left or Right controller.</param>
        /// <returns>Returns the axis value.</returns>
        public float GetRawAxis(XRAxis axis, bool left)
        {
            return GetAxis(Input.GetAxisRaw, axis, left);
        }

        /// <summary>
        /// Gets two axis values.
        /// </summary>
        /// <param name="axis"></param>
        /// <param name="left">Left or Right controller.</param>
        /// <returns>Returns two axis values.</returns>
        public Vector2 GetAxis2D(XRAxis2D axis, bool left)
        {
            return GetAxis2D(Input.GetAxis, axis, left);
        }

        /// <summary>
        /// Gets two raw axis values.
        /// </summary>
        /// <param name="axis"></param>
        /// <param name="left">Left or Right controller.</param>
        /// <returns>Returns two axis values.</returns>
        public Vector2 GetRawAxis2D(XRAxis2D axis, bool left)
        {
            return GetAxis2D(Input.GetAxis, axis, left);
        }

        private float GetAxis(GetAxisFunction axisFunction, XRAxis axis, bool left)
        {
            if (axis == XRAxis.Trigger)
                return axisFunction(left ? "Axis 9" : "Axis 10");

            else if (axis == XRAxis.Grip)
                return axisFunction(left ? "Axis 11" : "Axis 12");

            else if (axis == XRAxis.ThumbstickX || InputVendor != XRVendor.WindowsMR && axis == XRAxis.TouchpadX)
                return axisFunction(left ? "Axis 1" : "Axis 4");

            else if (axis == XRAxis.ThumbstickY || InputVendor != XRVendor.WindowsMR && axis == XRAxis.TouchpadY)
                return axisFunction(left ? "Axis 2" : "Axis 5");

            return 0.0f;
        }

        private Vector2 GetAxis2D(GetAxisFunction axisFunction, XRAxis2D axis, bool left)
        {
            m_TmpVector.x = 0;
            m_TmpVector.y = 0;

            if (axis == XRAxis2D.Thumbstick || InputVendor != XRVendor.WindowsMR && axis == XRAxis2D.Touchpad)
            {
                m_TmpVector.x = axisFunction(left ? "Axis 1" : "Axis 4");
                m_TmpVector.y = axisFunction(left ? "Axis 2" : "Axis 5");
            }
            else if (axis == XRAxis2D.Touchpad)
            {
                m_TmpVector.x = axisFunction(left ? "Axis 17" : "Axis 19");
                m_TmpVector.y = axisFunction(left ? "Axis 18" : "Axis 20");
            }

            return m_TmpVector;
        }

        #endregion

        #region Vibration

        public void Vibrate(bool left, float length = 0.25f, float strength = 0.5f)
        {
#if STEAMVR_INPUT
            if (CheckSteamVRIndex())
                StartCoroutine(LongVibration(left ? m_SteamVRLeftIndex : m_SteamVRRightIndex, length, strength));
#endif
        }

#if STEAMVR_INPUT

        private IEnumerator LongVibration(int index, float length, float strength)
        {
            for (float i = 0; i < length; i += Time.deltaTime)
            {
                SteamVR_Controller.Input(index).TriggerHapticPulse((ushort)Mathf.Lerp(0, 3999, strength * i));
                yield return null;
            }
        }

#endif

        #endregion
    }
}