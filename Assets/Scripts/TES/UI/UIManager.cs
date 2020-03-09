using System;
using System.Collections;
using TES3Unity.Components.Records;
using UnityEngine;

namespace TES3Unity.UI
{
    public enum UIWindowType
    {
        None = 0, Book, Scroll, Inventory, Menu, Rest
    }

    [RequireComponent(typeof(Canvas))]
    public class UIManager : MonoBehaviour
    {
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

        public UIBook Book => _book;
        public UIInteractiveText InteractiveText => _interactiveText;
        public UIScroll Scroll => _scroll;
        public UICrosshair Crosshair => _crosshair;

        public event Action<UIWindow, bool> WindowOpenChanged = null;

        public IEnumerator Start()
        {
            var player = GameObject.FindWithTag("Player");

            while (player == null)
            {
                player = GameObject.FindWithTag("Player");
                yield return null;
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

        public UIWindow OpenWindow(UIWindowType type)
        {
            if (m_CurrentWindow != null)
            {
                m_CurrentWindow.SetVisible(false);
                WindowOpenChanged?.Invoke(m_CurrentWindow, false);
            }

            if (type == UIWindowType.Book)
            {

            }
            else if (type == UIWindowType.Scroll)
            {

            }
            else if (type == UIWindowType.Inventory)
            {

            }
            else if (type == UIWindowType.Menu)
            {

            }
            else if (type == UIWindowType.Rest)
            {

            }

            if (m_CurrentWindow != null)
            {
                m_CurrentWindow.SetVisible(true);
                WindowOpenChanged?.Invoke(m_CurrentWindow, true);
            }

            return m_CurrentWindow;
        }
    }
}
