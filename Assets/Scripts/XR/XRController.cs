using UnityEngine;

namespace Demonixis.Toolbox.XR
{
    public class XRController : MonoBehaviour
    {
        [SerializeField]
        private GameObject m_DayDreamController = null;
        [SerializeField]
        private GameObject m_OculusGoController = null;
        [SerializeField]
        private GameObject m_OculusQuestAndSController = null;
        [SerializeField]
        private GameObject m_WaveVRController = null;

        private void Start()
        {
            var model = UnityXRDevice.GetVRHeadsetModel();
            SetControllerEnabled(model);
        }

        public void SetControllerEnabled(VRHeadsetModel model)
        {
#if WAVEVR_SDK
            m_WaveVRController.SetActive(true);
            model = VRHeadsetModel.None;
#endif
            m_DayDreamController.SetActive(model == VRHeadsetModel.Other);
            m_OculusGoController.SetActive(model == VRHeadsetModel.OculusGo);
            m_OculusQuestAndSController.SetActive(model == VRHeadsetModel.OculusQuest);
        }
    }
}