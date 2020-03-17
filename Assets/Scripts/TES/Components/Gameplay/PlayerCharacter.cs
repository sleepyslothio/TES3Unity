using Demonixis.Toolbox.XR;
using System;
using TES3Unity.Components;
using TES3Unity.Components.Records;
using TES3Unity.Inputs;
using UnityEngine;
using UnityEngine.InputSystem;

namespace TES3Unity
{
    public enum HandMode
    {
        Hidden = 0, Attack, Magic
    }

    public class PlayerCharacter : MonoBehaviour
    {
        public const float maxInteractDistance = 3;

        private PlayerInventory m_PlayerInventory = null;
        private RaycastHit[] m_InteractRaycastHitBuffer = new RaycastHit[32];
        private InputAction m_UseAction;
        private Transform m_LeftHandSocket = null;
        private Transform m_RightHandSocket = null;
        private HandMode m_HandMode = HandMode.Hidden;

        [SerializeField]
        private Transform m_LeftHand = null;
        [SerializeField]
        private Transform m_RightHand = null;

        public Transform LeftHandContainer => m_LeftHand;
        public Transform RightHandContainer => m_RightHand;
        public Transform LeftHand => m_LeftHandSocket;
        public Transform RightHand => m_RightHandSocket;
        public Transform RayCastTarget { get; private set; }

        public event Action<RecordComponent, bool> InteractiveTextChanged = null;
        public event Action<RecordComponent> RaycastedComponent = null;

        private void Start()
        {
            var xrEnabled = XRManager.IsXREnabled();
            var camera = GetComponentInChildren<Camera>();

            RayCastTarget = camera.transform;

            if (xrEnabled)
            {
                RayCastTarget = m_RightHand;
            }

            // TODO: use the NPCFactory and add a 1.st person skin
            var hands = PlayerSkin.AddHands(m_LeftHand, m_RightHand, xrEnabled);
            m_LeftHandSocket = hands.Item1;
            m_RightHandSocket = hands.Item2;
            ToggleHands(); // Disabled by default

            m_PlayerInventory = GetComponent<PlayerInventory>();

            var gameplayActionMap = InputManager.GetActionMap("Gameplay");
            gameplayActionMap.Enable();

            gameplayActionMap["ReadyWeapon"].started += (c) =>
            {
                var status = ToggleHands();
                m_HandMode = status ? HandMode.Attack : HandMode.Hidden;
            };

            gameplayActionMap["ReadyMagic"].started += (c) =>
            {
                var status = ToggleHands();
                m_HandMode = status ? HandMode.Magic : HandMode.Hidden;
            };

            m_UseAction = gameplayActionMap["Use"];
        }

        private void Update()
        {
            CastInteractRay();
        }

        private bool ToggleHands()
        {
            var active = !m_LeftHand.gameObject.activeSelf;
            m_LeftHand.gameObject.SetActive(active);
            m_RightHand.gameObject.SetActive(active);
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

                        if (m_UseAction.phase == InputActionPhase.Started)
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
                    else
                    {
                        //deactivate text if no interactable [ DOORS ONLY - REQUIRES EXPANSION ] is found
                        InteractiveTextChanged?.Invoke(null, false);
                    }
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
