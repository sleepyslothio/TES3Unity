using TESUnity;
using UnityEngine;

namespace Demonixis.Toolbox.XR
{
    public class XRController : MonoBehaviour
    {
        [SerializeField]
        private GameObject m_OculusGoController = null;
        [SerializeField]
        private GameObject m_OculusQuestAndSController = null;

        private void Start()
        {
            var settings = GameSettings.Get();

            if (settings.HandTracking)
            {
                gameObject.SetActive(false);
            }
        }
    }
}