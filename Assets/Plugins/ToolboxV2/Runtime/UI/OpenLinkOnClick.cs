using UnityEngine;
using UnityEngine.UI;

namespace Demonixis.ToolboxV2.UI
{
    [RequireComponent(typeof(Button))]
    public sealed class OpenLinkOnClick : MonoBehaviour
    {
        private Button m_Button = null;

        [SerializeField]
        private string m_URL = null;

        private void Start()
        {
            m_Button = GetComponent(typeof(Button)) as Button;
            m_Button.onClick.AddListener(OpenURL);
        }

        private void OnDestroy()
        {
            if (m_Button != null)
            {
                m_Button.onClick.RemoveListener(OpenURL);
            }
        }

        public void OpenURL()
        {
            Application.OpenURL(m_URL);
        }
    }
}