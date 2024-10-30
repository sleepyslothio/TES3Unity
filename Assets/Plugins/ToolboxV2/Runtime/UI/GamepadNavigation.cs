using Demonixis.ToolboxV2.Utils;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Demonixis.ToolboxV2
{
    public enum NavigationInputType
    {
        Gamepad = 0, Mouse
    }

    [RequireComponent(typeof(Canvas))]
    public class GamepadNavigation : MonoBehaviour
    {
        private const string MainButtonTag = "MainButton";
        private bool m_IsActive = true;

        [SerializeField]
        private float m_CheckInterval = 0.35f;

        [SerializeField]
        private InputActionAsset m_InputAsset = null;

        public NavigationInputType NavigationInputType { get; private set; }

        public event Action<NavigationInputType> InputModeChanged = null;

        private void Start()
        {
            if (EventSystem.current == null)
            {
                Debug.LogError("EventSystem is null. GamepadNavigation will be destroyed.");
                Destroy(this);
            }
            else
            {
                if (PlatformUtility.IsDesktopPlatform())
                {
                    StartCoroutine(CheckForMouseOrGamepad());
                }

                StartCoroutine(CheckButtons());
            }
        }

        private IEnumerator CheckForMouseOrGamepad()
        {
            var actionMap = m_InputAsset.FindActionMap("Main");
            actionMap.Enable();

            var mouseEvaluateAction = actionMap["Mouse"];
            var gamepadEvaluateAction = actionMap["Gamepad"];

            while (true)
            {
                var mouseCurrentlyActive = IsInputActive(mouseEvaluateAction, true);
                var gamepadCurrentlyActive = IsInputActive(gamepadEvaluateAction, false);
                var previousInputType = NavigationInputType;

                if (!mouseCurrentlyActive && previousInputType == NavigationInputType.Gamepad)
                {
                    gamepadCurrentlyActive = true;
                }

                m_IsActive = gamepadCurrentlyActive;

                NavigationInputType = m_IsActive ? NavigationInputType.Gamepad : NavigationInputType.Mouse;

                if (previousInputType != NavigationInputType)
                {
                    InputModeChanged?.Invoke(NavigationInputType);
                }

                yield return CoroutineFactory.WaitForSecondsUnscaled(m_CheckInterval);
            }
        }

        private static bool IsInputActive(InputAction action, bool mouse)
        {
            var value = action.ReadValue<Vector2>();
            var magnitude = value.sqrMagnitude;
            return magnitude > 0.05f;
        }

        private IEnumerator CheckButtons()
        {
            var canvas = GetComponent<Canvas>();
            var eventSystem = EventSystem.current;

            Selectable button = null;
            Selectable[] buttons = null;
            GameObject current = null;

            while (true)
            {
                if (m_IsActive)
                {
                    current = eventSystem.currentSelectedGameObject;

                    if (current == null || !current.activeInHierarchy || !current.activeSelf)
                    {
                        buttons = canvas.GetComponentsInChildren<Selectable>();

                        if (buttons != null && buttons.Length > 0)
                        {
                            button = GetMainButton(buttons);
                            eventSystem.SetSelectedGameObject(button.gameObject);

#if UNITY_EDITOR
                            //Debug.Log($"Use the {button} button as main button");
#endif
                        }
                    }
                }

                yield return CoroutineFactory.WaitForSecondsUnscaled(m_CheckInterval);
            }
        }

        private Selectable GetMainButton(Selectable[] buttons)
        {
            Selectable notMainInteractable = null;

            foreach (var button in buttons)
            {
                if (button.CompareTag(MainButtonTag) && button.interactable)
                {
                    return button;
                }

                if (notMainInteractable == null && button.interactable)
                {
                    notMainInteractable = button;
                }
            }

            return notMainInteractable;
        }
    }
}