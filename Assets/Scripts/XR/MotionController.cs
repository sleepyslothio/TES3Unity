#if UNITY_STANDALONE || UNITY_WSA || UNITY_PS4 || UNITY_IOS || (UNITY_ANDROID && !PICOVR_SDK && !WAVEVR_SDK)
#define UNITY_XR
#endif

using UnityEngine;
using UnityEngine.XR;

namespace Demonixis.Toolbox.XR
{
    public sealed class MotionController : MonoBehaviour
    {
#if UNITY_XR
        private Transform m_Transform = null;
#endif

        [SerializeField]
        private bool m_LeftHand = true;
        [SerializeField]
        private float m_SmoothPosition = 7.5f;
        [SerializeField]
        private float m_SmoothRotation = 7.5f;

        public bool LeftHand
        {
            get => m_LeftHand;
            set => m_LeftHand = value;
        }

        private void Start()
        {
#if UNITY_XR
            m_Transform = transform;

            if (!XRSettings.enabled)
                enabled = false;
#endif
        }

        public void Setup(float smoothPosition, float smoothRotation)
        {
            m_SmoothPosition = smoothPosition;
            m_SmoothRotation = smoothRotation;
        }

#if UNITY_XR
        private void Update() => UpdateController();
        private void LateUpdate() => UpdateController();

        private void UpdateController()
        {
            m_Transform.localPosition = Vector3.Lerp(m_Transform.localPosition, InputTracking.GetLocalPosition(m_LeftHand ? XRNode.LeftHand : XRNode.RightHand), Time.unscaledDeltaTime * m_SmoothPosition);
            m_Transform.localRotation = Quaternion.Slerp(m_Transform.localRotation, InputTracking.GetLocalRotation(m_LeftHand ? XRNode.LeftHand : XRNode.RightHand), Time.unscaledDeltaTime * m_SmoothPosition);
        }
#endif
    }
}
