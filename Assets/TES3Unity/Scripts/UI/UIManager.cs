using System;
using System.Collections;
using Demonixis.ToolboxV2.Inputs;
using Demonixis.ToolboxV2.XR;
using TES3Unity.Components.Records;
using TES3Unity.Components.XR;
using UnityEngine;
using UnityEngine.InputSystem;

namespace TES3Unity.UI
{
    public enum UIWindowType
    {
        None = 0,
        Book,
        Scroll,
        Inventory,
        Menu,
        Rest,
        Journal
    }

    [RequireComponent(typeof(Canvas))]
    public class UIManager : MonoBehaviour
    {
        private InputActionMap _uiActionMap;
        private InputActionMap _gameplayActionMap;
        private UIWindow _currentWindow;
        private UIWindowType _currentWindowType = UIWindowType.None;
        private bool _xrEnabled;
        private bool _eventBound;

        [Header("HUD Elements")] [SerializeField]
        private UICrosshair _crosshair;

        [SerializeField] private UIInteractiveText _interactiveText;

        [Header("UI Elements")] [SerializeField]
        private UIBook _book;

        [SerializeField] private UIScroll _scroll;
        [SerializeField] private UIInventory _inventory;
        [SerializeField] private UIMenu _menu;
        [SerializeField] private UIRest _rest;
        [SerializeField] private UIJournal _journal;

        public UIBook Book => _book;
        public UIInteractiveText InteractiveText => _interactiveText;
        public UIScroll Scroll => _scroll;
        public UICrosshair Crosshair => _crosshair;

        public event Action<UIWindow, bool> WindowOpenChanged;

        public void Setup(GameObject player)
        {
#if UNITY_STANDALONE
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
#endif
            
            _xrEnabled = XRManager.Enabled;
            _uiActionMap = InputSystemManager.GetActionMap("UI");
            _gameplayActionMap = InputSystemManager.GetActionMap("Gameplay");

            if (_xrEnabled)
            {
                var playerXr = player.GetComponent<PlayerXR>();
                var hudPivot = playerXr.HudPivot;
                _crosshair = hudPivot.GetComponentInChildren<UICrosshair>(true);
                _interactiveText = hudPivot.GetComponentInChildren<UIInteractiveText>(true);
            }

            BindEventListeners();

            var windows = GetComponentsInChildren<UIWindow>();
            foreach (var window in windows)
                window.CloseRequest += c => CloseWindow();

            var playerCharacter = player.GetComponent<PlayerCharacter>();
            playerCharacter.InteractiveTextChanged += OnInteractiveTextChanged;
        }

        private void OnDestroy()
        {
            UnbindEventListeners();
        }
        
        private void BindEventListeners()
        {
            if (_eventBound) return;

            _uiActionMap["Validate"].started += OnInputValidate;
            _uiActionMap["Back"].started += OnInputBack;
            _uiActionMap["Next"].started += OnInputNext;
            _uiActionMap["Previous"].started += OnInputPrevious;

            _gameplayActionMap["Rest"].started += OnGameplayRest;
            _gameplayActionMap["Journal"].started += OnGameplayJournal;
            _gameplayActionMap["Inventory"].started += OnGameplayInventory;
            _gameplayActionMap["Menu"].started += OnGameplayMenu;

            _eventBound = true;
        }

        private void UnbindEventListeners()
        {
            if (!_eventBound) return;

            _uiActionMap["Validate"].started -= OnInputValidate;
            _uiActionMap["Back"].started -= OnInputBack;
            _uiActionMap["Next"].started -= OnInputNext;
            _uiActionMap["Previous"].started -= OnInputPrevious;

            _gameplayActionMap["Rest"].started += OnGameplayRest;
            _gameplayActionMap["Journal"].started += OnGameplayJournal;
            _gameplayActionMap["Inventory"].started += OnGameplayInventory;
            _gameplayActionMap["Menu"].started += OnGameplayMenu;

            _eventBound = false;
        }

        private void OnInputValidate(InputAction.CallbackContext context)
        {
            _currentWindow?.OnValidateClicked();
        }

        private void OnInputBack(InputAction.CallbackContext context)
        {
            _currentWindow?.OnBackClicked();
        }

        private void OnInputNext(InputAction.CallbackContext context)
        {
            _currentWindow?.OnNextClicked();
        }

        private void OnInputPrevious(InputAction.CallbackContext context)
        {
            _currentWindow?.OnPreviousClicked();
        }

        private void OnGameplayRest(InputAction.CallbackContext context)
        {
            OpenWindow(UIWindowType.Rest);
        }

        private void OnGameplayJournal(InputAction.CallbackContext context)
        {
            OpenWindow(UIWindowType.Journal);
        }

        private void OnGameplayInventory(InputAction.CallbackContext context)
        {
            OpenWindow(UIWindowType.Inventory);
        }

        private void OnGameplayMenu(InputAction.CallbackContext context)
        {
            OpenWindow(UIWindowType.Menu);
        }

        private void OnInteractiveTextChanged(RecordComponent component, bool visible)
        {
            if (visible)
            {
                var data = component.objData;
                _interactiveText.Show(GUIUtils.CreateSprite(data.icon), data.interactionPrefix, data.name, data.value,
                    data.weight);
            }
            else
            {
                _interactiveText.Close();
            }
        }

        public UIWindow OpenWindow(UIWindowType type)
        {
            // Toggle
            if (type != UIWindowType.None && type == _currentWindowType)
            {
                CloseWindow();
                SetActionMapEnabled(false);
                return null;
            }

            // One window at a time.
            CloseWindow();

            if (type == UIWindowType.Book)
            {
                _currentWindow = _book;
            }
            else if (type == UIWindowType.Scroll)
            {
                _currentWindow = _scroll;
            }
            else if (type == UIWindowType.Inventory)
            {
                _currentWindow = _inventory;
            }
            else if (type == UIWindowType.Menu)
            {
                _currentWindow = _menu;
            }
            else if (type == UIWindowType.Rest)
            {
                _currentWindow = _rest;
            }
            else if (type == UIWindowType.Journal)
            {
                _currentWindow = _journal;
            }

            if (_currentWindow != null)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;

                _uiActionMap.Enable();
                _currentWindow.SetVisible(true);
                _currentWindowType = type;
                WindowOpenChanged?.Invoke(_currentWindow, true);
            }

            SetActionMapEnabled(_currentWindow != null);

            return _currentWindow;
        }

        public void CloseWindow()
        {
            if (_currentWindow == null)
            {
                return;
            }

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            _currentWindow.OnCloseRequest();
            _currentWindow.SetVisible(false);
            _currentWindowType = UIWindowType.None;
            SetActionMapEnabled(false);
            WindowOpenChanged?.Invoke(_currentWindow, false);
        }

        private void SetActionMapEnabled(bool ui)
        {
            if (ui)
            {
                InputSystemManager.Disable("Movement");
                _uiActionMap.Enable();
            }
            else
            {
                InputSystemManager.Enable("Movement");
                _uiActionMap.Disable();
            }

            if (!_xrEnabled)
                _crosshair.Enabled = !ui;
        }
    }
}