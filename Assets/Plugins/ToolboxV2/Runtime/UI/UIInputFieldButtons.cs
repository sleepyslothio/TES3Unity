using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Demonixis.ToolboxV2.UI
{
    public sealed class UIInputFieldButtons : MonoBehaviour
    {
        private InputField m_InputField = null;
        private TMP_InputField m_TmpInputField;
        private bool m_UseTMP;
        private float m_Value = 0;

        [SerializeField] private float m_Increment = 0.1f;
        [SerializeField] private bool m_LessThan0;

        private void Awake()
        {
            m_TmpInputField = GetComponentInChildren<TMP_InputField>();
            m_UseTMP = m_TmpInputField != null;

            if (m_UseTMP)
            {
                m_TmpInputField.onValueChanged.AddListener(v => TryParseValue());
                m_TmpInputField.interactable = false; // PlatformUtility.IsDesktopPlatform();
            }
            else
            {
                m_InputField = GetComponentInChildren<InputField>();
                m_InputField.onValueChanged.AddListener(v => TryParseValue());
                m_InputField.interactable = false; // PlatformUtility.IsDesktopPlatform();
            }

            TryParseValue();

            var buttons = GetComponentsInChildren<Button>(true);
            if (buttons.Length == 2)
            {
                foreach (var button in buttons)
                {
                    if (button.name.Contains("+"))
                    {
                        button.onClick.AddListener(IncreaseValue);
                    }
                    else
                    {
                        button.onClick.AddListener(DecreaseValue);
                    }
                }
            }
        }

        private string GetText()
        {
            if (m_TmpInputField != null)
                return m_TmpInputField.text;
            if (m_InputField != null)
                return m_InputField.text;

            return string.Empty;
        }

        private void TryParseValue()
        {
            if (float.TryParse(GetText(), out float value))
            {
                m_Value = value;
            }
        }

        public void IncreaseValue()
        {
            var rawValue = m_Value + m_Increment;
            
            if (!m_LessThan0 && rawValue < 0)
                rawValue = 0;
            
            SetValue(rawValue.ToString());
        }

        public void DecreaseValue()
        {
            var rawValue = m_Value - m_Increment;
            
            if (!m_LessThan0 && rawValue < 0)
                rawValue = 0;
   
            SetValue(rawValue.ToString());
        }

        private void SetValue(string value)
        {
            if (m_TmpInputField != null)
                m_TmpInputField.text = value;
            else if (m_InputField != null)
                m_InputField.text = value;
        }
    }
}