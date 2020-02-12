using Demonixis.Toolbox.XR;
using TESUnity.Inputs;
using TESUnity.UI;
using UnityEngine;

namespace TESUnity
{
    public class PlayerComponent : MonoBehaviour
    {
        private Transform m_CameraTransform;
        private Transform m_Transform;
        private CapsuleCollider m_CapsuleCollider;
        private Rigidbody m_Rigidbody;
        private UICrosshair m_Crosshair;
        private bool m_Paused = false;
        private bool m_IsGrounded = false;
        private bool m_IsFlying = false;
        private bool m_XREnabled = false;

        #region Editor Fields

        [Header("Movement Settings")]
        public float slowSpeed = 3;
        public float normalSpeed = 5;
        public float fastSpeed = 10;
        public float flightSpeedMultiplier = 3;
        public float airborneForceMultiplier = 5;
        public float mouseSensitivity = 3;
        public float minVerticalAngle = -90;
        public float maxVerticalAngle = 90;

        [Header("Misc")]
        public Light lantern;
        public Transform leftHand;
        public Transform rightHand;

        #endregion

        #region Public Fields

        public bool isFlying
        {
            get => m_IsFlying;
            set
            {
                m_IsFlying = value;

                if (!m_IsFlying)
                    m_Rigidbody.useGravity = true;
                else
                    m_Rigidbody.useGravity = false;
            }
        }

        public bool Paused => m_Paused;

        public Transform RayCastTarget { get; private set; }

        #endregion

        private void Start()
        {
            m_Transform = GetComponent<Transform>();
            m_CameraTransform = Camera.main.GetComponent<Transform>();
            m_CapsuleCollider = GetComponent<CapsuleCollider>();
            m_Rigidbody = GetComponent<Rigidbody>();

            // Setup the camera
            var config = GameSettings.Get();

            m_Crosshair = FindObjectOfType<UICrosshair>();

#if !UNITY_STANDALONE && !UNITY_EDITOR
            Cursor.lockState = CursorLockMode.None;
#endif
            m_XREnabled = XRManager.IsXREnabled();

            RayCastTarget = m_CameraTransform;

            if (m_XREnabled)
                RayCastTarget = m_Transform.Find("Head/Right Hand");
        }

        private void Update()
        {
            if (m_Paused)
                return;

            if (!m_XREnabled)
                Rotate();

            if (Input.GetKeyDown(KeyCode.Tab) || Input.touchCount == 3)
                isFlying = !isFlying;

            if (m_IsGrounded && !isFlying && InputManager.GetButtonDown(MWButton.Jump))
            {
                var newVelocity = m_Rigidbody.velocity;
                newVelocity.y = 5;

                m_Rigidbody.velocity = newVelocity;
            }

            if (InputManager.GetButtonDown(MWButton.Light))
                lantern.enabled = !lantern.enabled;
        }

        private void FixedUpdate()
        {
            m_IsGrounded = CalculateIsGrounded();

            if (m_IsGrounded || isFlying)
                SetVelocity();
            else if (!m_IsGrounded || !isFlying)
                ApplyAirborneForce();
        }

        private void Rotate()
        {
            if (Cursor.lockState != CursorLockMode.Locked)
            {
                if (Input.GetMouseButtonDown(0))
                    Cursor.lockState = CursorLockMode.Locked;
                else
                    return;
            }
            else
            {
                if (Input.GetKeyDown(KeyCode.BackQuote))
                {
                    Cursor.lockState = CursorLockMode.None;
                    Cursor.visible = true;
                }
            }

            var eulerAngles = new Vector3(m_CameraTransform.localEulerAngles.x, m_Transform.localEulerAngles.y, 0);

            // Make eulerAngles.x range from -180 to 180 so we can clamp it between a negative and positive angle.
            if (eulerAngles.x > 180)
                eulerAngles.x = eulerAngles.x - 360;

            var deltaMouse = mouseSensitivity * (new Vector2(InputManager.GetAxis(MWAxis.MouseX), InputManager.GetAxis(MWAxis.MouseY)));

            eulerAngles.x = Mathf.Clamp(eulerAngles.x - deltaMouse.y, minVerticalAngle, maxVerticalAngle);
            eulerAngles.y = Mathf.Repeat(eulerAngles.y + deltaMouse.x, 360);

            m_CameraTransform.localEulerAngles = new Vector3(eulerAngles.x, 0, 0);

            m_Transform.localEulerAngles = new Vector3(0, eulerAngles.y, 0);
        }

        private void SetVelocity()
        {
            Vector3 velocity;

            if (!isFlying)
            {
                velocity = m_Transform.TransformVector(CalculateLocalVelocity());
                velocity.y = m_Rigidbody.velocity.y;
            }
            else
                velocity = m_CameraTransform.TransformVector(CalculateLocalVelocity());

            m_Rigidbody.velocity = velocity;
        }

        private void ApplyAirborneForce()
        {
            var forceDirection = m_Transform.TransformVector(CalculateLocalMovementDirection());
            forceDirection.y = 0;
            forceDirection.Normalize();

            var force = airborneForceMultiplier * m_Rigidbody.mass * forceDirection;

            m_Rigidbody.AddForce(force);
        }

        private Vector3 CalculateLocalMovementDirection()
        {
            // Calculate the local movement direction.
            var direction = new Vector3(InputManager.GetAxis(MWAxis.Horizontal), 0.0f, InputManager.GetAxis(MWAxis.Vertical));

            // A small hack for French Keyboard...
            if (Application.systemLanguage == SystemLanguage.French)
            {
                // Cancel Qwerty
                if (Input.GetKeyDown(KeyCode.W))
                    direction.z = 0;
                else if (Input.GetKeyDown(KeyCode.A))
                    direction.x = 0;

                // Use Azerty
                if (Input.GetKey(KeyCode.Z))
                    direction.z = 1;
                else if (Input.GetKey(KeyCode.S))
                    direction.z = -1;

                if (Input.GetKey(KeyCode.Q))
                    direction.x = -1;
                else if (Input.GetKey(KeyCode.D))
                    direction.x = 1;
            }

            return direction.normalized;
        }

        private float CalculateSpeed()
        {
            var speed = normalSpeed;

            if (InputManager.GetButton(MWButton.Run))
                speed = fastSpeed;

            else if (InputManager.GetButton(MWButton.Slow))
                speed = slowSpeed;

            if (isFlying)
                speed *= flightSpeedMultiplier;

            return speed;
        }

        private Vector3 CalculateLocalVelocity()
        {
            return CalculateSpeed() * CalculateLocalMovementDirection();
        }

        private bool CalculateIsGrounded()
        {
            var playerCenter = m_Transform.position + m_CapsuleCollider.center;
            var castedSphereRadius = 0.8f * m_CapsuleCollider.radius;
            var sphereCastDistance = (m_CapsuleCollider.height / 2);

            return Physics.SphereCast(new Ray(playerCenter, -m_Transform.up), castedSphereRadius, sphereCastDistance);
        }

        public void Pause(bool pause)
        {
            m_Paused = pause;
            m_Crosshair.SetActive(!m_Paused);

            Time.timeScale = pause ? 0.0f : 1.0f;

#if UNITY_STANDALONE
            Cursor.lockState = pause ? CursorLockMode.None : CursorLockMode.Locked;
            Cursor.visible = pause;
#endif
            // Used by the VR Component to enable/disable some features.
            SendMessage("OnPlayerPause", pause, SendMessageOptions.DontRequireReceiver);
        }
    }
}