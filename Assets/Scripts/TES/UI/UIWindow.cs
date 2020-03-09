using UnityEngine;

namespace TES3Unity.UI
{
    public class UIWindow : MonoBehaviour
    {
        [SerializeField]
        private GameObject m_Container = null;

        public virtual void SetVisible(bool visible)
        {
            m_Container.SetActive(visible);
        }
    }
}
