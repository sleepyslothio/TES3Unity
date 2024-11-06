using Demonixis.ToolboxV2.Inputs;
using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Demonixis.DVRSimulator.UI
{
    [RequireComponent(typeof(Button)), Obsolete]
    public sealed class UIBackButton : MonoBehaviour
    {
        private Button m_Button = null;
        private InputAction m_BackAction = null;

        private void OnEnable()
        {
            m_BackAction?.Enable();
        }

        private void OnDisable()
        {
            m_BackAction?.Disable();
        }

        private void Start()
        {
            m_Button = GetComponent<Button>();

            m_BackAction = InputSystemManager.GetAction("UI", "Back").Clone();
            m_BackAction.Enable();
            m_BackAction.started += (c) =>
            {
                m_Button.onClick.Invoke();
            };
        }
    }
}
