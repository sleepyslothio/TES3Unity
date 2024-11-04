using System;
using Demonixis.ToolboxV2.Inputs;
using Demonixis.ToolboxV2.XR;
using TES3Unity.Components;
using TES3Unity.Components.Records;
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
        public const float maxInteractDistance = 3;

        private PlayerInventory m_PlayerInventory;
        private RaycastHit[] m_InteractRaycastHitBuffer = new RaycastHit[32];
        private InputAction m_UseAction;
        private HandMode m_HandMode = HandMode.Hidden;
        private bool m_XREnabled;

        public Transform LeftHandContainer { get; private set; }
        public Transform RightHandContainer { get; private set; }
        public Transform LeftHandModel { get; private set; }
        public Transform RightHandModel { get; private set; }
        public Transform LeftHandSocket { get; private set; }
        public Transform RightHandSocket { get; private set; }
        public Transform RayCastTarget { get; private set; }

        public event Action<RecordComponent, bool> InteractiveTextChanged;
        public event Action<RecordComponent> RaycastedComponent;

        private void Start()
        {
            m_XREnabled = XRManager.IsXREnabled();
            var camera = GetComponentInChildren<Camera>();

            RayCastTarget = camera.transform;

            LeftHandContainer = transform.FindChildRecursiveExact("LeftHandAnchor");
            LeftHandModel = LeftHandContainer.Find("HandModel");
            RightHandContainer = transform.FindChildRecursiveExact("RightHandAnchor");
            RightHandModel = RightHandContainer.Find("HandModel");
            
            // TODO: use the NPCFactory and add a 1.st person skin
            var hands = PlayerSkin.AddHands(LeftHandModel, RightHandModel);
            LeftHandSocket = hands.Item1;
            RightHandSocket = hands.Item2;

            if (!m_XREnabled)
            {
                var cameraTransform = camera.transform;
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

            m_PlayerInventory = GetComponent<PlayerInventory>();

            var gameplayActionMap = InputSystemManager.Enable("Gameplay");
            gameplayActionMap["ReadyWeapon"].started += c =>
            {
                var status = ToggleHands();
                m_HandMode = status ? HandMode.Attack : HandMode.Hidden;

                if (m_XREnabled)
                {
                    m_HandMode = HandMode.XR;
                }
            };

            gameplayActionMap["ReadyMagic"].started += c =>
            {
                var status = ToggleHands();
                m_HandMode = status ? HandMode.Magic : HandMode.Hidden;

                if (m_XREnabled)
                {
                    m_HandMode = HandMode.XR;
                }
            };

            m_UseAction = gameplayActionMap["Use"];
        }

        private void Update()
        {
            CastInteractRay();
        }

        private bool ToggleHands()
        {
            if (m_XREnabled)
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
            var raycastHitCount = Physics.RaycastNonAlloc(ray, m_InteractRaycastHitBuffer, maxInteractDistance);

            if (raycastHitCount > 0)
            {
                for (int i = 0; i < raycastHitCount; i++)
                {
                    var hitInfo = m_InteractRaycastHitBuffer[i];
                    var component = hitInfo.collider.GetComponentInParent<RecordComponent>();

                    if (component != null)
                    {
                        if (string.IsNullOrEmpty(component.objData.name))
                        {
                            return;
                        }

                        InteractiveTextChanged?.Invoke(component, true);
                        RaycastedComponent?.Invoke(component);
                        
                        if (m_UseAction.phase == InputActionPhase.Performed)
                        {
                            if (component is Door)
                            {
                                TES3Engine.Instance.OpenDoor((Door)component);
                            }
                            else if (component.usable)
                            {
                                component.Interact();
                            }
                            else if (component.pickable)
                            {
                                m_PlayerInventory.AddItem(component);
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
