using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Demonixis.ToolboxV2.UI
{
    //[RequireComponent(typeof(Text))]
    public sealed class TranslateText : MonoBehaviour
    {
        private string m_OriginalKey = null;
        private bool m_UseTMP;

        public string key;

        public string TranslationKey
        {
            get
            {
                if (string.IsNullOrEmpty(m_OriginalKey))
                {
                    m_OriginalKey = key != string.Empty ? key : GetText();
                }

                return m_OriginalKey;
            }
        }

        private void SetText(string inputStr)
        {
            if (TryGetComponent(out Text text))
                text.text = inputStr;
            else if (TryGetComponent(out TextMeshProUGUI tmp))
                tmp.text = inputStr;
        }

        private string GetText()
        {
            if (TryGetComponent(out Text text))
                return text.text;
            if (TryGetComponent(out TextMeshProUGUI tmp))
                return tmp.text;

            return string.Empty;
        }

        private void Start()
        {
            Translate();
        }

        public void Translate()
        {
            if (string.IsNullOrEmpty(m_OriginalKey))
            {
                m_OriginalKey = key != string.Empty ? key : GetText();
            }

            SetText(Translator.Get(m_OriginalKey));
        }

        public static void ForceRefreshTranslations()
        {
            var allCanvas = FindObjectsByType<Canvas>(FindObjectsSortMode.None);

            foreach (var canvas in allCanvas)
            {
                var translateTexts = canvas.GetComponentsInChildren<TranslateText>(true);

                foreach (var text in translateTexts)
                {
                    text.Translate();
                }
            }
        }
    }
}