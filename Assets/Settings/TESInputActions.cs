// GENERATED AUTOMATICALLY FROM 'Assets/Settings/TESInputActions.inputactions'

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

namespace TESUnity.Inputs
{
    public class @TESInputActions : IInputActionCollection, IDisposable
    {
        public InputActionAsset asset { get; }
        public @TESInputActions()
        {
            asset = InputActionAsset.FromJson(@"{
    ""name"": ""TESInputActions"",
    ""maps"": [
        {
            ""name"": ""Movement"",
            ""id"": ""aa830bd8-1c99-4cb4-97cf-0ab41abcae44"",
            ""actions"": [
                {
                    ""name"": ""LeftAxis"",
                    ""type"": ""Value"",
                    ""id"": ""b49aec66-5454-4471-86f0-42634077039e"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""RightAxis"",
                    ""type"": ""Value"",
                    ""id"": ""cb9934d3-824f-44f7-bf47-6e532f962d33"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""98931166-ac08-4642-ab75-350f051298b7"",
                    ""path"": ""<Gamepad>/leftStick"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""LeftAxis"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""2D Vector"",
                    ""id"": ""8400fc66-4fa8-4747-9183-4b47a543b56f"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""LeftAxis"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""f0379b5d-8d57-44e2-a321-34be34c4f91c"",
                    ""path"": ""<Keyboard>/w"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""LeftAxis"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""2f97bdac-bdc5-4ac1-a95e-2d7e2984cc50"",
                    ""path"": ""<Keyboard>/s"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""LeftAxis"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""d68e5106-ec44-4a81-b185-bfe5076b56fd"",
                    ""path"": ""<Keyboard>/a"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""LeftAxis"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""42270a1e-cfb8-4a5b-934d-bdd110f412cf"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""LeftAxis"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""5f94773a-c3d8-4c60-893d-400f45447ab4"",
                    ""path"": ""<XRController>{LeftHand}/joystick"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""LeftAxis"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""5454ff91-bec3-470a-8394-dad549c012fa"",
                    ""path"": ""<Gamepad>/rightStick"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""RightAxis"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""d01909e5-b777-492e-8924-33fdbb0fdfad"",
                    ""path"": ""<Mouse>/delta"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""RightAxis"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""c0f64eb0-dc29-4311-a843-55fd8ce87f96"",
                    ""path"": ""<XRController>{RightHand}/joystick"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""RightAxis"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        },
        {
            ""name"": ""GameActions"",
            ""id"": ""dd349e84-d489-4e7d-a451-43d03f4c0b90"",
            ""actions"": [
                {
                    ""name"": ""Jump"",
                    ""type"": ""Button"",
                    ""id"": ""25d25b42-ad8a-4d98-a8dc-746157eb585f"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Light"",
                    ""type"": ""Button"",
                    ""id"": ""5c942f72-20c8-454e-9d83-7488003d597e"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Attack"",
                    ""type"": ""Button"",
                    ""id"": ""3b0b2f82-8450-4092-93da-14cdb6544c59"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Use"",
                    ""type"": ""Button"",
                    ""id"": ""763ec881-7f4b-4780-b96c-3182e8284acf"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""c072404e-29ec-4058-be27-9f10bf6f8b21"",
                    ""path"": ""<Keyboard>/e"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Jump"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""b73ac0d2-79c5-4dd9-9ae3-26ddd502eedb"",
                    ""path"": ""<Gamepad>/buttonNorth"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Jump"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""c3931055-22ff-4e2b-8a59-bca35d86d990"",
                    ""path"": ""<Keyboard>/f"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Light"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""50db29ae-7974-439a-bd5b-f359697eeb02"",
                    ""path"": ""<Mouse>/leftButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Attack"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""3bb93f54-ea8f-4b0c-af6b-8c1c7230e743"",
                    ""path"": ""<Gamepad>/rightTrigger"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Attack"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""c6ac3af0-a3fa-483c-947e-ee426ec37d5c"",
                    ""path"": ""<Keyboard>/space"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Use"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""62eb21d8-cc70-4f5d-9a13-c22e1c078410"",
                    ""path"": ""<Gamepad>/buttonSouth"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Use"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        },
        {
            ""name"": ""XR"",
            ""id"": ""2936608f-02c1-4b73-935c-aee7e1d2f416"",
            ""actions"": [
                {
                    ""name"": ""Recenter"",
                    ""type"": ""Button"",
                    ""id"": ""473b398b-df24-40e5-8865-7c98489c26de"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Teleport"",
                    ""type"": ""Button"",
                    ""id"": ""e14999c7-1905-4e39-a199-9bbd08df5fe5"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""2f68e72b-f1ab-4142-b592-de0666f81624"",
                    ""path"": """",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Recenter"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""289e944e-aa2c-46b7-beaa-4447d72ecf49"",
                    ""path"": """",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Teleport"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        },
        {
            ""name"": ""UI"",
            ""id"": ""e86cee8f-c7ed-4a81-a54d-0ad65c905775"",
            ""actions"": [
                {
                    ""name"": ""Menu"",
                    ""type"": ""Button"",
                    ""id"": ""13c79a8c-f03e-442f-805c-3795ed82141d"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""771d000e-fe00-46c1-a63c-0b1302a09964"",
                    ""path"": ""<Keyboard>/escape"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Menu"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""22026baa-193c-40ff-9a1d-d62ff597d064"",
                    ""path"": ""<Gamepad>/start"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Menu"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""db1c3b16-fb7e-4de3-af64-97d5f87f69f8"",
                    ""path"": """",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Menu"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": []
}");
            // Movement
            m_Movement = asset.FindActionMap("Movement", throwIfNotFound: true);
            m_Movement_LeftAxis = m_Movement.FindAction("LeftAxis", throwIfNotFound: true);
            m_Movement_RightAxis = m_Movement.FindAction("RightAxis", throwIfNotFound: true);
            // GameActions
            m_GameActions = asset.FindActionMap("GameActions", throwIfNotFound: true);
            m_GameActions_Jump = m_GameActions.FindAction("Jump", throwIfNotFound: true);
            m_GameActions_Light = m_GameActions.FindAction("Light", throwIfNotFound: true);
            m_GameActions_Attack = m_GameActions.FindAction("Attack", throwIfNotFound: true);
            m_GameActions_Use = m_GameActions.FindAction("Use", throwIfNotFound: true);
            // XR
            m_XR = asset.FindActionMap("XR", throwIfNotFound: true);
            m_XR_Recenter = m_XR.FindAction("Recenter", throwIfNotFound: true);
            m_XR_Teleport = m_XR.FindAction("Teleport", throwIfNotFound: true);
            // UI
            m_UI = asset.FindActionMap("UI", throwIfNotFound: true);
            m_UI_Menu = m_UI.FindAction("Menu", throwIfNotFound: true);
        }

        public void Dispose()
        {
            UnityEngine.Object.Destroy(asset);
        }

        public InputBinding? bindingMask
        {
            get => asset.bindingMask;
            set => asset.bindingMask = value;
        }

        public ReadOnlyArray<InputDevice>? devices
        {
            get => asset.devices;
            set => asset.devices = value;
        }

        public ReadOnlyArray<InputControlScheme> controlSchemes => asset.controlSchemes;

        public bool Contains(InputAction action)
        {
            return asset.Contains(action);
        }

        public IEnumerator<InputAction> GetEnumerator()
        {
            return asset.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Enable()
        {
            asset.Enable();
        }

        public void Disable()
        {
            asset.Disable();
        }

        // Movement
        private readonly InputActionMap m_Movement;
        private IMovementActions m_MovementActionsCallbackInterface;
        private readonly InputAction m_Movement_LeftAxis;
        private readonly InputAction m_Movement_RightAxis;
        public struct MovementActions
        {
            private @TESInputActions m_Wrapper;
            public MovementActions(@TESInputActions wrapper) { m_Wrapper = wrapper; }
            public InputAction @LeftAxis => m_Wrapper.m_Movement_LeftAxis;
            public InputAction @RightAxis => m_Wrapper.m_Movement_RightAxis;
            public InputActionMap Get() { return m_Wrapper.m_Movement; }
            public void Enable() { Get().Enable(); }
            public void Disable() { Get().Disable(); }
            public bool enabled => Get().enabled;
            public static implicit operator InputActionMap(MovementActions set) { return set.Get(); }
            public void SetCallbacks(IMovementActions instance)
            {
                if (m_Wrapper.m_MovementActionsCallbackInterface != null)
                {
                    @LeftAxis.started -= m_Wrapper.m_MovementActionsCallbackInterface.OnLeftAxis;
                    @LeftAxis.performed -= m_Wrapper.m_MovementActionsCallbackInterface.OnLeftAxis;
                    @LeftAxis.canceled -= m_Wrapper.m_MovementActionsCallbackInterface.OnLeftAxis;
                    @RightAxis.started -= m_Wrapper.m_MovementActionsCallbackInterface.OnRightAxis;
                    @RightAxis.performed -= m_Wrapper.m_MovementActionsCallbackInterface.OnRightAxis;
                    @RightAxis.canceled -= m_Wrapper.m_MovementActionsCallbackInterface.OnRightAxis;
                }
                m_Wrapper.m_MovementActionsCallbackInterface = instance;
                if (instance != null)
                {
                    @LeftAxis.started += instance.OnLeftAxis;
                    @LeftAxis.performed += instance.OnLeftAxis;
                    @LeftAxis.canceled += instance.OnLeftAxis;
                    @RightAxis.started += instance.OnRightAxis;
                    @RightAxis.performed += instance.OnRightAxis;
                    @RightAxis.canceled += instance.OnRightAxis;
                }
            }
        }
        public MovementActions @Movement => new MovementActions(this);

        // GameActions
        private readonly InputActionMap m_GameActions;
        private IGameActionsActions m_GameActionsActionsCallbackInterface;
        private readonly InputAction m_GameActions_Jump;
        private readonly InputAction m_GameActions_Light;
        private readonly InputAction m_GameActions_Attack;
        private readonly InputAction m_GameActions_Use;
        public struct GameActionsActions
        {
            private @TESInputActions m_Wrapper;
            public GameActionsActions(@TESInputActions wrapper) { m_Wrapper = wrapper; }
            public InputAction @Jump => m_Wrapper.m_GameActions_Jump;
            public InputAction @Light => m_Wrapper.m_GameActions_Light;
            public InputAction @Attack => m_Wrapper.m_GameActions_Attack;
            public InputAction @Use => m_Wrapper.m_GameActions_Use;
            public InputActionMap Get() { return m_Wrapper.m_GameActions; }
            public void Enable() { Get().Enable(); }
            public void Disable() { Get().Disable(); }
            public bool enabled => Get().enabled;
            public static implicit operator InputActionMap(GameActionsActions set) { return set.Get(); }
            public void SetCallbacks(IGameActionsActions instance)
            {
                if (m_Wrapper.m_GameActionsActionsCallbackInterface != null)
                {
                    @Jump.started -= m_Wrapper.m_GameActionsActionsCallbackInterface.OnJump;
                    @Jump.performed -= m_Wrapper.m_GameActionsActionsCallbackInterface.OnJump;
                    @Jump.canceled -= m_Wrapper.m_GameActionsActionsCallbackInterface.OnJump;
                    @Light.started -= m_Wrapper.m_GameActionsActionsCallbackInterface.OnLight;
                    @Light.performed -= m_Wrapper.m_GameActionsActionsCallbackInterface.OnLight;
                    @Light.canceled -= m_Wrapper.m_GameActionsActionsCallbackInterface.OnLight;
                    @Attack.started -= m_Wrapper.m_GameActionsActionsCallbackInterface.OnAttack;
                    @Attack.performed -= m_Wrapper.m_GameActionsActionsCallbackInterface.OnAttack;
                    @Attack.canceled -= m_Wrapper.m_GameActionsActionsCallbackInterface.OnAttack;
                    @Use.started -= m_Wrapper.m_GameActionsActionsCallbackInterface.OnUse;
                    @Use.performed -= m_Wrapper.m_GameActionsActionsCallbackInterface.OnUse;
                    @Use.canceled -= m_Wrapper.m_GameActionsActionsCallbackInterface.OnUse;
                }
                m_Wrapper.m_GameActionsActionsCallbackInterface = instance;
                if (instance != null)
                {
                    @Jump.started += instance.OnJump;
                    @Jump.performed += instance.OnJump;
                    @Jump.canceled += instance.OnJump;
                    @Light.started += instance.OnLight;
                    @Light.performed += instance.OnLight;
                    @Light.canceled += instance.OnLight;
                    @Attack.started += instance.OnAttack;
                    @Attack.performed += instance.OnAttack;
                    @Attack.canceled += instance.OnAttack;
                    @Use.started += instance.OnUse;
                    @Use.performed += instance.OnUse;
                    @Use.canceled += instance.OnUse;
                }
            }
        }
        public GameActionsActions @GameActions => new GameActionsActions(this);

        // XR
        private readonly InputActionMap m_XR;
        private IXRActions m_XRActionsCallbackInterface;
        private readonly InputAction m_XR_Recenter;
        private readonly InputAction m_XR_Teleport;
        public struct XRActions
        {
            private @TESInputActions m_Wrapper;
            public XRActions(@TESInputActions wrapper) { m_Wrapper = wrapper; }
            public InputAction @Recenter => m_Wrapper.m_XR_Recenter;
            public InputAction @Teleport => m_Wrapper.m_XR_Teleport;
            public InputActionMap Get() { return m_Wrapper.m_XR; }
            public void Enable() { Get().Enable(); }
            public void Disable() { Get().Disable(); }
            public bool enabled => Get().enabled;
            public static implicit operator InputActionMap(XRActions set) { return set.Get(); }
            public void SetCallbacks(IXRActions instance)
            {
                if (m_Wrapper.m_XRActionsCallbackInterface != null)
                {
                    @Recenter.started -= m_Wrapper.m_XRActionsCallbackInterface.OnRecenter;
                    @Recenter.performed -= m_Wrapper.m_XRActionsCallbackInterface.OnRecenter;
                    @Recenter.canceled -= m_Wrapper.m_XRActionsCallbackInterface.OnRecenter;
                    @Teleport.started -= m_Wrapper.m_XRActionsCallbackInterface.OnTeleport;
                    @Teleport.performed -= m_Wrapper.m_XRActionsCallbackInterface.OnTeleport;
                    @Teleport.canceled -= m_Wrapper.m_XRActionsCallbackInterface.OnTeleport;
                }
                m_Wrapper.m_XRActionsCallbackInterface = instance;
                if (instance != null)
                {
                    @Recenter.started += instance.OnRecenter;
                    @Recenter.performed += instance.OnRecenter;
                    @Recenter.canceled += instance.OnRecenter;
                    @Teleport.started += instance.OnTeleport;
                    @Teleport.performed += instance.OnTeleport;
                    @Teleport.canceled += instance.OnTeleport;
                }
            }
        }
        public XRActions @XR => new XRActions(this);

        // UI
        private readonly InputActionMap m_UI;
        private IUIActions m_UIActionsCallbackInterface;
        private readonly InputAction m_UI_Menu;
        public struct UIActions
        {
            private @TESInputActions m_Wrapper;
            public UIActions(@TESInputActions wrapper) { m_Wrapper = wrapper; }
            public InputAction @Menu => m_Wrapper.m_UI_Menu;
            public InputActionMap Get() { return m_Wrapper.m_UI; }
            public void Enable() { Get().Enable(); }
            public void Disable() { Get().Disable(); }
            public bool enabled => Get().enabled;
            public static implicit operator InputActionMap(UIActions set) { return set.Get(); }
            public void SetCallbacks(IUIActions instance)
            {
                if (m_Wrapper.m_UIActionsCallbackInterface != null)
                {
                    @Menu.started -= m_Wrapper.m_UIActionsCallbackInterface.OnMenu;
                    @Menu.performed -= m_Wrapper.m_UIActionsCallbackInterface.OnMenu;
                    @Menu.canceled -= m_Wrapper.m_UIActionsCallbackInterface.OnMenu;
                }
                m_Wrapper.m_UIActionsCallbackInterface = instance;
                if (instance != null)
                {
                    @Menu.started += instance.OnMenu;
                    @Menu.performed += instance.OnMenu;
                    @Menu.canceled += instance.OnMenu;
                }
            }
        }
        public UIActions @UI => new UIActions(this);
        public interface IMovementActions
        {
            void OnLeftAxis(InputAction.CallbackContext context);
            void OnRightAxis(InputAction.CallbackContext context);
        }
        public interface IGameActionsActions
        {
            void OnJump(InputAction.CallbackContext context);
            void OnLight(InputAction.CallbackContext context);
            void OnAttack(InputAction.CallbackContext context);
            void OnUse(InputAction.CallbackContext context);
        }
        public interface IXRActions
        {
            void OnRecenter(InputAction.CallbackContext context);
            void OnTeleport(InputAction.CallbackContext context);
        }
        public interface IUIActions
        {
            void OnMenu(InputAction.CallbackContext context);
        }
    }
}
