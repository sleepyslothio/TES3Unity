using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Demonixis.ToolboxV2.UI
{
    [RequireComponent(typeof(Button))]
    public sealed class UIGamepadButton : MonoBehaviour
    {
        private Button m_Button = null;

        [SerializeField]
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
            m_BackAction.started += (c) =>
            {
                m_Button.onClick.Invoke();
            };
        }

        public void ForceDisable()
        {
            m_BackAction?.Disable();
        }
    }
}
