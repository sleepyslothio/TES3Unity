using Demonixis.ToolboxV2.Inputs;
using Demonixis.ToolboxV2.XR;
using UnityEngine;
using UnityEngine.InputSystem;

namespace TES3Unity
{
    public class PlayerController : MonoBehaviour
    {
        private enum MovementSpeedMode
        {
            Walk,
            Run,
            Slow
        }

        private Transform _cameraTransform;
        private Transform _transform;
        private CapsuleCollider _capsule;
        private Rigidbody _rigidbody;
        private InputAction _leftStickAction;
        private InputAction _rightStickAction;
        private MovementSpeedMode _movementSpeedMode = MovementSpeedMode.Walk;
        private bool _paused = false;
        private bool _grounded;
        private bool _flying;
        private bool _xrEnabled;

        #region Editor Fields

        [Header("Movement Settings")] public float slowSpeed = 3;
        public float normalSpeed = 5;
        public float fastSpeed = 10;
        public float flightSpeedMultiplier = 3;
        public float airborneForceMultiplier = 5;
        public float lookSensitivity = 1.0f;
        public float mouseSensitivity = 0.25f;
        public float minVerticalAngle = -90;
        public float maxVerticalAngle = 90;

        #endregion

        #region Public Fields

        private bool IsFlying
        {
            get => _flying;
            set
            {
                _flying = value;
                _rigidbody.useGravity = !_flying;
            }
        }

        public bool Paused => _paused;

        #endregion

        private void Start()
        {
            _transform = GetComponent<Transform>();
            _cameraTransform = Camera.main.GetComponent<Transform>();
            _capsule = GetComponent<CapsuleCollider>();
            _rigidbody = GetComponent<Rigidbody>();

#if !UNITY_STANDALONE && !UNITY_EDITOR
            Cursor.lockState = CursorLockMode.None;
#endif

            var actionMap = InputSystemManager.GetActionMap("Movement");
            _leftStickAction = actionMap["LeftAxis"];
            _rightStickAction = actionMap["RightAxis"];
        }

        private void BindEventListeners()
        {
            var actionMap = InputSystemManager.GetActionMap("Movement");
            actionMap.Enable();
            actionMap["Run"].started += OnRun;
            actionMap["Run"].canceled += OnRun;
            actionMap["Fly"].started += OnFly;
            actionMap["Jump"].started += OnJump;
            actionMap["Crouch"].started += OnCrouch;
            actionMap["Crouch"].canceled += OnCrouch;
        }

        private void UnbindEventListeners()
        {
            var actionMap = InputSystemManager.GetActionMap("Movement");
            actionMap.Disable();
            actionMap["Run"].started -= OnRun;
            actionMap["Run"].canceled -= OnRun;
            actionMap["Fly"].started -= OnFly;
            actionMap["Jump"].started -= OnJump;
            actionMap["Crouch"].started -= OnCrouch;
            actionMap["Crouch"].canceled -= OnCrouch;
        }

        private void OnRun(InputAction.CallbackContext context)
        {
            if (context.phase == InputActionPhase.Started)
                SetMovementType(MovementSpeedMode.Run);
            else if (context.phase == InputActionPhase.Canceled)
                SetMovementType(MovementSpeedMode.Walk);
        }

        private void OnFly(InputAction.CallbackContext context)
        {
            IsFlying = !IsFlying;
        }
        
        private void OnJump(InputAction.CallbackContext context)
        {
            Jump();
        }
        
        private void OnCrouch(InputAction.CallbackContext context)
        {
            if (context.phase == InputActionPhase.Started)
                Crouch(true);
            else if (context.phase == InputActionPhase.Canceled)
                Crouch(false);
        }

        private void OnEnable() => BindEventListeners();
        private void OnDisable() => UnbindEventListeners();

        private void Update()
        {
            if (_paused) return;
            RotatePlayer();
        }

        private void FixedUpdate()
        {
            _grounded = CalculateIsGrounded();

            if (_grounded || IsFlying)
            {
                SetVelocity();
            }
            else if (!_grounded || !IsFlying)
            {
                ApplyAirborneForce();
            }
        }

        private void SetMovementType(MovementSpeedMode mode)
        {
            _movementSpeedMode = mode switch
            {
                MovementSpeedMode.Slow when _movementSpeedMode == MovementSpeedMode.Slow => MovementSpeedMode.Walk,
                MovementSpeedMode.Run when _movementSpeedMode != MovementSpeedMode.Slow => MovementSpeedMode.Run,
                _ => MovementSpeedMode.Walk
            };
        }

        private void Crouch(bool crouch)
        {
            SetMovementType(crouch ? MovementSpeedMode.Slow : MovementSpeedMode.Walk);
        }

        private void Jump()
        {
            if (!_grounded || _flying) return;
            
            var newVelocity = _rigidbody.linearVelocity;
            newVelocity.y = 5;

            _rigidbody.linearVelocity = newVelocity;
        }

        private void RotatePlayer()
        {
            var eulerAngles = new Vector3(_cameraTransform.localEulerAngles.x, _transform.localEulerAngles.y, 0);

            // Make eulerAngles.x range from -180 to 180 so we can clamp it between a negative and positive angle.
            if (eulerAngles.x > 180)
                eulerAngles.x -= 360;

            var deltaMouse = lookSensitivity * _rightStickAction.ReadValue<Vector2>();
            eulerAngles.x = Mathf.Clamp(eulerAngles.x - deltaMouse.y, minVerticalAngle, maxVerticalAngle);
            eulerAngles.y = Mathf.Repeat(eulerAngles.y + deltaMouse.x, 360);

            if (!_xrEnabled)
                _cameraTransform.localEulerAngles = new Vector3(eulerAngles.x, 0, 0);

            _transform.localEulerAngles = new Vector3(0, eulerAngles.y, 0);
        }

        private void SetVelocity()
        {
            Vector3 velocity;

            if (!IsFlying)
            {
                velocity = _transform.TransformVector(CalculateLocalVelocity());
                velocity.y = _rigidbody.linearVelocity.y;
            }
            else
            {
                velocity = _cameraTransform.TransformVector(CalculateLocalVelocity());
            }

            _rigidbody.linearVelocity = velocity;
        }

        private void ApplyAirborneForce()
        {
            var forceDirection = _transform.TransformVector(CalculateLocalMovementDirection());
            forceDirection.y = 0;
            forceDirection.Normalize();

            var force = airborneForceMultiplier * _rigidbody.mass * forceDirection;

            _rigidbody.AddForce(force);
        }

        private Vector3 CalculateLocalMovementDirection()
        {
            var leftAxis = _leftStickAction.ReadValue<Vector2>();
            var direction = new Vector3(leftAxis.x, 0.0f, leftAxis.y);
            return direction.normalized;
        }

        private float CalculateSpeed()
        {
            var speed = _movementSpeedMode switch
            {
                MovementSpeedMode.Run => fastSpeed,
                MovementSpeedMode.Slow => slowSpeed,
                _ => normalSpeed
            };

            if (IsFlying)
            {
                speed *= flightSpeedMultiplier;
            }

            return speed;
        }

        private Vector3 CalculateLocalVelocity()
        {
            return CalculateSpeed() * CalculateLocalMovementDirection();
        }

        private bool CalculateIsGrounded()
        {
            var playerCenter = _transform.position + _capsule.center;
            var castedSphereRadius = 0.8f * _capsule.radius;
            var sphereCastDistance = (_capsule.height / 2);

            return Physics.SphereCast(new Ray(playerCenter, -_transform.up), castedSphereRadius, sphereCastDistance);
        }
    }
}