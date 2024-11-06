using Demonixis.ToolboxV2.XR;
using UnityEngine;

namespace Demonixis.ToolboxV2.Utils
{
    public class XRControllerHolder : MonoBehaviour
    {
        public enum XRControllerSupport
        {
            None = 0,
            OculusQuestAndRift,
            OculusQuest2,
            OculusRiftCV1,
            OculusQuestPro,
            Steam,
            Standard,
            WindowsMR
        }

        [Header("Oculus")]
        [SerializeField] private GameObject m_OculusQuestAndRiftController = null;
        [SerializeField] private GameObject m_OculusQuest2Controller = null;
        [SerializeField] private GameObject m_OculusRiftController = null;
        [SerializeField] private GameObject m_OculusQuestProController = null;

        [Header("Steam")]
        [SerializeField] private GameObject m_SteamController = null;

        [Header("Standard")]
        [SerializeField] private GameObject m_StandardController = null;

        [Header("Windows MR")]
        [SerializeField] private GameObject m_WindowsMRController = null;
        
        private void Start()
        {
            SetControllersVisible(XRControllerSupport.None);

            if (!XRManager.Enabled) return;

            StartCoroutine(XRManager.GetXRInfos((vendor, headset) =>
            {
                var controllerType = XRControllerSupport.None;

                switch (headset)
                {
                    case XRHeadset.WindowsMr:
                        controllerType = XRControllerSupport.WindowsMR;
                        break;
                    case XRHeadset.OculusQuest:
                    case XRHeadset.OculusRiftS:
                        controllerType = XRControllerSupport.OculusQuestAndRift;
                        break;
                    case XRHeadset.OculusQuest2:
                        controllerType = XRControllerSupport.OculusQuest2;
                        break;
                    case XRHeadset.OculusRiftCv1:
                        controllerType = XRControllerSupport.OculusRiftCV1;
                        break;
                    case XRHeadset.HtcVive:
                    case XRHeadset.ValveIndex:
                        controllerType = XRControllerSupport.Steam;
                        break;
                    case XRHeadset.OculusQuestPro:
                        controllerType = XRControllerSupport.OculusQuestPro;
                        break;
                    case XRHeadset.None:
                        controllerType = XRControllerSupport.None;
                        break;
                    default:
                        controllerType = XRControllerSupport.Standard;
                        break;
                }

                SetControllersVisible(controllerType);
            }));
        }

        private void SetControllersVisible(XRControllerSupport id)
        {
            m_OculusQuestAndRiftController.SetActive(id == XRControllerSupport.OculusQuestAndRift);
            m_OculusQuest2Controller.SetActive(id == XRControllerSupport.OculusQuest2);
            m_OculusRiftController.SetActive(id == XRControllerSupport.OculusRiftCV1);
            m_OculusQuestProController.SetActive(id == XRControllerSupport.OculusQuestPro);
            m_SteamController.SetActive(id == XRControllerSupport.Steam);
            m_StandardController.SetActive(id == XRControllerSupport.Standard);
            m_WindowsMRController.SetActive(id == XRControllerSupport.WindowsMR);
        }
    }
}
