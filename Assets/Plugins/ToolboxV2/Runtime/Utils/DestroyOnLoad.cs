using UnityEngine;

namespace Demonixis.ToolboxV2
{
    public sealed class DestroyOnLoad : MonoBehaviour
    {
        [SerializeField]
        private MonoBehaviour[] m_ScriptsToDestroy = null;
        [SerializeField]
        private bool m_DestroyObject = true;

        private void Awake()
        {
            if (m_ScriptsToDestroy != null && m_ScriptsToDestroy.Length > 0)
            {
                for (var i = 0; i < m_ScriptsToDestroy.Length; i++)
                {
                    Destroy(m_ScriptsToDestroy[i]);
                }
            }

            if (m_DestroyObject)
            {
                Destroy(gameObject);
            }
            else
            {
                Destroy(this);
            }
        }
    }
}
