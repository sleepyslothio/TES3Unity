using System;
using System.Collections;
using TES3Unity.Components.Records;
using TES3Unity.Inputs;
using UnityEngine;
using UnityEngine.InputSystem;

namespace TES3Unity.UI
{
    public enum UIWindowType
    {
        None = 0, Book, Scroll, Inventory, Menu, Rest
    }

    [RequireComponent(typeof(Canvas))]
    public class UIManager : MonoBehaviour
    {
        private InputActionMap m_UIActionMap = null;
        private UIWindow m_CurrentWindow = null;

        [Header("HUD Elements")]
        [SerializeField]
        private UICrosshair _crosshair = null;
        [SerializeField]
        private UIInteractiveText _interactiveText = null;

        [Header("UI Elements")]
        [SerializeField]
        private UIBook _book = null;
        [SerializeField]
        private UIScroll _scroll = null;
        [SerializeField]
        private UIInventory m_Inventory = null;
        [SerializeField]
        private UIMenu m_Menu = null;
        [SerializeField]
        private UIRest m_Rest = null;

        public UIBook Book => _book;
        public UIInteractiveText InteractiveText => _interactiveText;
        public UIScroll Scroll => _scroll;
        public UICrosshair Crosshair => _crosshair;

        public event Action<UIWindow, bool> WindowOpenChanged = null;

        public IEnumerator Start()
        {
            var playerTag = "Player";
            var player = GameObject.FindWithTag(playerTag);

            m_UIActionMap = InputManager.GetActionMap("UI");
            m_UIActionMap["Validate"].started += (c) => m_CurrentWindow?.OnValidateClicked();
            m_UIActionMap["Back"].started += (c) => m_CurrentWindow?.OnBackClicked();
            m_UIActionMap["Next"].started += (c) => m_CurrentWindow?.OnNextClicked();
            m_UIActionMap["Previous"].started += (c) => m_CurrentWindow?.OnPreviousClicked();

            while (player == null)
            {
                player = GameObject.FindWithTag(playerTag);
                yield return null;
            }

            var windows = GetComponentsInChildren<UIWindow>();
            foreach (var window in windows)
            {
                window.CloseRequest += OnWindowCloseRequest;
            }

            var playerCharacter = player.GetComponent<PlayerCharacter>();
            playerCharacter.InteractiveTextChanged += OnInteractiveTextChanged;
        }

        private void OnInteractiveTextChanged(RecordComponent component, bool visible)
        {
            if (visible)
            {
                var data = component.objData;
                _interactiveText.Show(GUIUtils.CreateSprite(data.icon), data.interactionPrefix, data.name, data.value, data.weight);
            }
            else
            {
                _interactiveText.Close();
            }
        }

        public bool OpenWindow(UIWindowType type, out UIWindow window)
        {
            window = null;

            // One window at a time.
            CloseWindow();

            if (type == UIWindowType.Book)
            {
                m_CurrentWindow = _book;
            }
            else if (type == UIWindowType.Scroll)
            {
                m_CurrentWindow = _scroll;
            }
            else if (type == UIWindowType.Inventory)
            {
                m_CurrentWindow = m_Inventory;
            }
            else if (type == UIWindowType.Menu)
            {
                m_CurrentWindow = m_Menu;
            }
            else if (type == UIWindowType.Rest)
            {
                m_CurrentWindow = m_Rest;
            }

            if (m_CurrentWindow != null)
            {
                m_UIActionMap.Enable();
                m_CurrentWindow.SetVisible(true);
                window = m_CurrentWindow;
                WindowOpenChanged?.Invoke(m_CurrentWindow, true);
                return true;
            }

            m_UIActionMap.Disable();

            return false;
        }

        public void CloseWindow()
        {
            if (m_CurrentWindow == null)
            {
                return;
            }

            m_CurrentWindow.OnCloseRequest();
            m_CurrentWindow.SetVisible(false);
            WindowOpenChanged?.Invoke(m_CurrentWindow, false);
        }

        private void OnWindowCloseRequest(UIWindow window)
        {
            if (m_CurrentWindow != window)
            {
                Debug.Log($"The current windows {m_CurrentWindow?.name ?? "NULL"} is different than {window.name}");
                window.OnCloseRequest();
            }
            else
            {
                CloseWindow();
            }
        }
    }
}
