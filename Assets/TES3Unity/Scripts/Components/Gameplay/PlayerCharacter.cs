using System;
using Demonixis.ToolboxV2.Inputs;
using Demonixis.ToolboxV2.XR;
using TES3Unity.Components;
using TES3Unity.Components.Records;
using TES3Unity.UI;
using UnityEngine;
using UnityEngine.InputSystem;

namespace TES3Unity
{
    public enum HandMode
    {
        Hidden = 0, Attack, Magic, XR
    }

    public class PlayerCharacter : MonoBehaviour
    {
        private const float MaxInteractDistance = 3;
        private const int MaxRaycastHit = 32;

        private PlayerInventory _inventory;
        private readonly RaycastHit[] _raycastHitBuffer = new RaycastHit[MaxRaycastHit];
        private InputAction _useAction;
        private HandMode _handMode = HandMode.Hidden;
        private bool _xrEnabled;

        [SerializeField] private GameObject _uiManagerPrefab;

        public Transform LeftHandContainer { get; private set; }
        public Transform RightHandContainer { get; private set; }
        public Transform LeftHandModel { get; private set; }
        public Transform RightHandModel { get; private set; }
        public Transform LeftHandSocket { get; private set; }
        public Transform RightHandSocket { get; private set; }
        public Transform RayCastTarget { get; private set; }

        public event Action<RecordComponent, bool> InteractiveTextChanged;
        public event Action<RecordComponent> RaycastedComponent;

        private void OnEnable() => BindEventListeners();
        private void OnDisable() => UnbindEventListeners();

        private void Start()
        {
            _xrEnabled = XRManager.IsXREnabled();

            if (_xrEnabled)
            {
                var uiManager = GetComponentInChildren<UIManager>(true);
                uiManager.Setup(gameObject);
                uiManager.Crosshair.Enabled = false;
            }
            else
            {
                var uiManagerGo = Instantiate(_uiManagerPrefab);
                var uiManager = uiManagerGo.GetComponent<UIManager>();
                uiManager.Setup(gameObject);
            }

            var cameras = GetComponentsInChildren<Camera>(true);
            foreach (var target in cameras)
            {
                if (target.CompareTag("MainCamera") && target.enabled)
                    RayCastTarget = target.transform;
            }

            if (RayCastTarget == null)
                throw new UnityException("Missing RaycastTarget");
            
            LeftHandContainer = transform.FindChildRecursiveExact("LeftHandAnchor");
            LeftHandModel = LeftHandContainer.FindChildRecursiveExact("LeftHandModel");
            RightHandContainer = transform.FindChildRecursiveExact("RightHandAnchor");
            RightHandModel = RightHandContainer.FindChildRecursiveExact("RightHandModel");
            
            // TODO: use the NPCFactory and add a 1.st person skin
            var hands = PlayerSkin.AddHands(LeftHandModel, RightHandModel);
            LeftHandSocket = hands.Item1;
            RightHandSocket = hands.Item2;

            if (!_xrEnabled)
            {
                var cameraTransform = RayCastTarget.transform;
                LeftHandContainer.parent = cameraTransform;
                RightHandContainer.parent = cameraTransform;

                LeftHandContainer.localPosition = new Vector3(-0.2f, -0.2f, 0.4f);
                LeftHandContainer.localRotation = Quaternion.identity;
                RightHandContainer.localPosition = new Vector3(0.2f, -0.2f, 0.4f);
                RightHandContainer.localRotation = Quaternion.identity;
            }
            else
            {
                RayCastTarget = RightHandContainer;
            }

            ToggleHands(); // Disabled by default

            _inventory = GetComponent<PlayerInventory>();

            var actionMap = InputSystemManager.Enable("Gameplay");
            _useAction = actionMap["Use"];
        }

        private void BindEventListeners()
        {
            var actionMap = InputSystemManager.Enable("Gameplay");
            actionMap["ReadyWeapon"].started += OnReadyWeapon;
            actionMap["ReadyMagic"].started += OnReadyMagic;
        }

        private void UnbindEventListeners()
        {
            var actionMap = InputSystemManager.Disable("Gameplay");
            actionMap["ReadyWeapon"].started -= OnReadyWeapon;
            actionMap["ReadyMagic"].started -= OnReadyMagic;
        }

        private void OnReadyWeapon(InputAction.CallbackContext context)
        {
            var status = ToggleHands();
            _handMode = status ? HandMode.Attack : HandMode.Hidden;

            if (_xrEnabled)
                _handMode = HandMode.XR;
        }

        private void OnReadyMagic(InputAction.CallbackContext context)
        {
            var status = ToggleHands();
            _handMode = status ? HandMode.Magic : HandMode.Hidden;

            if (_xrEnabled)
                _handMode = HandMode.XR;
        }

        private void LateUpdate()
        {
            CastInteractRay();
        }

        private bool ToggleHands()
        {
            if (_xrEnabled)
            {
                return true;
            }

            var active = !LeftHandModel.gameObject.activeSelf;
            LeftHandModel.gameObject.SetActive(active);
            RightHandModel.gameObject.SetActive(active);
            return active;
        }

        public void CastInteractRay()
        {
            // Cast a ray to see what the camera is looking at.
            var ray = new Ray(RayCastTarget.position, RayCastTarget.forward);
            var raycastHitCount = Physics.RaycastNonAlloc(ray, _raycastHitBuffer, MaxInteractDistance);

            if (raycastHitCount > 0)
            {
                for (int i = 0; i < raycastHitCount; i++)
                {
                    var hitInfo = _raycastHitBuffer[i];
                    var component = hitInfo.collider.GetComponentInParent<RecordComponent>();

                    if (component != null)
                    {
                        if (string.IsNullOrEmpty(component.objData.name))
                        {
                            return;
                        }

                        InteractiveTextChanged?.Invoke(component, true);
                        RaycastedComponent?.Invoke(component);
                        
                        if (_useAction.phase == InputActionPhase.Performed)
                        {
                            if (component is Door)
                            {
                                Tes3Engine.Instance.OpenDoor((Door)component);
                            }
                            else if (component.usable)
                            {
                                component.Interact();
                            }
                            else if (component.pickable)
                            {
                                _inventory.AddItem(component);
                            }
                        }

                        break;
                    }

                    //deactivate text if no interactable [ DOORS ONLY - REQUIRES EXPANSION ] is found
                    InteractiveTextChanged?.Invoke(null, false);
                }
            }
            else
            {
                //deactivate text if nothing is raycasted against
                InteractiveTextChanged?.Invoke(null, false);
            }
        }
    }
}
