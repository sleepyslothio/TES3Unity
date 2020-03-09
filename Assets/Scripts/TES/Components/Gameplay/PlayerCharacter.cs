using Demonixis.Toolbox.XR;
using System;
using TES3Unity.Components;
using TES3Unity.Components.Records;
using TES3Unity.Inputs;
using UnityEngine;
using UnityEngine.InputSystem;

namespace TES3Unity
{
    public class PlayerCharacter : MonoBehaviour
    {
        public const float maxInteractDistance = 3;

        private PlayerInventory m_PlayerInventory = null;
        private RaycastHit[] m_InteractRaycastHitBuffer = new RaycastHit[32];
        private InputAction m_UseAction;
        private Transform m_LeftHandAnchor = null;
        private Transform m_RightHandAnchor = null;

        [SerializeField]
        private Transform m_LeftHand = null;
        [SerializeField]
        private Transform m_RightHand = null;

        public Transform LeftHandContainer => m_LeftHand;
        public Transform RightHandContainer => m_RightHand;
        public Transform LeftHand => m_LeftHandAnchor;
        public Transform RightHand => m_RightHandAnchor;
        public Transform RayCastTarget { get; private set; }

        public event Action<RecordComponent, bool> InteractiveTextChanged = null;
        public event Action<RecordComponent> RaycastedComponent = null;

        private void Start()
        {
            var xrEnabled = XRManager.IsXREnabled();
            var camera = GetComponentInChildren<Camera>();

            RayCastTarget = camera.transform;

            if (xrEnabled)
                RayCastTarget = m_RightHand;

            // Loading hands.
            var nifManager = TES3Manager.Instance.Engine.nifManager; // FIXME

            var race = "nord";
            var gender = "m";
            var hands1st = $"b_n_{race}_{gender}_hands.1st";

            var hands = nifManager.InstantiateNIF($"meshes\\b\\{hands1st}.NIF");

            var meshColliders = hands.GetComponentsInChildren<MeshCollider>(true);
            foreach (var collider in meshColliders)
            {
                Destroy(collider);
            }

            m_LeftHandAnchor = CreateHand(hands, true);
            m_RightHandAnchor = CreateHand(hands, false);

            if (!xrEnabled)
            {
                m_LeftHand.localPosition = new Vector3(-0.2f, -0.2f, 0.4f);
                m_LeftHand.localRotation = Quaternion.Euler(0, 0, -75);
                m_RightHand.localPosition = new Vector3(0.2f, -0.2f, 0.4f);
                m_RightHand.localRotation = Quaternion.Euler(0, 0, 75);
            }

            Destroy(hands.gameObject);

            m_PlayerInventory = GetComponent<PlayerInventory>();

            var gameplayActionMap = InputManager.GetActionMap("Gameplay");
            gameplayActionMap.Enable();

            m_UseAction = gameplayActionMap["Use"];
        }

        private Transform CreateHand(GameObject hands, bool left)
        {
            var path = "Bip01 Pelvis/Bip01 Spine/Bip01 Spine1/Bip01 Spine2/Bip01 Neck/Bip01 {0} Clavicle/Bip01 {0} UpperArm/{1} Hand";

            var hand = hands.transform.Find(string.Format(path, left ? "L" : "R", left ? "Left" : "Right"));
            hand.gameObject.isStatic = false;
            hand.parent = left ? m_LeftHand : m_RightHand;
            hand.localPosition = Vector3.zero;
            hand.localRotation = Quaternion.Euler(left ? -180.0f : 180.0f, 90.0f, 0.0f);

            var anchor = new GameObject($"{(left ? "Left" : "Right")}HandAnchor");
            var anchorTransform = anchor.transform;
            anchorTransform.parent = hand;
            anchorTransform.localPosition = new Vector3(left ? 0.03f : -0.03f, 0, 0);
            anchorTransform.localRotation = Quaternion.Euler(0, left ? 180 : -180, 0);

            return anchorTransform;
        }

        private void Update()
        {
            CastInteractRay();
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
                                m_PlayerInventory.Add(component);
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
