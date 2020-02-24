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
            var headset = XRManager.GetXRHeadset();
            var settings = GameSettings.Get();

            m_OculusGoController.SetActive(headset == XRHeadset.OculusGo && !settings.HandTracking);
            m_OculusQuestAndSController.SetActive(headset != XRHeadset.OculusGo && !settings.HandTracking);
        }
    }
}