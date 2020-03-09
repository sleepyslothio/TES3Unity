using UnityEngine;
using UnityEngine.UI;

namespace TES3Unity.UI
{
    public class UIRest : MonoBehaviour
    {
        [SerializeField]
        private GameObject m_Container = null;
        [SerializeField]
        private Slider m_RestTime = null;
        [SerializeField]
        private Text m_RestText = null;
        [SerializeField]
        private Text m_Date = null;

        public void SetVisible(bool visible, string date)
        {
            if (visible)
            {
                UpdateRest(0, date);
            }

            m_Container.SetActive(visible);
        }

        public void UpdateRest(int hours, string date)
        {
            m_RestTime.value = hours;
            m_RestText.text = $"{hours} hour{(hours > 1 ? "s" : "")}";
            m_Date.text = date;
        }
    }
}
