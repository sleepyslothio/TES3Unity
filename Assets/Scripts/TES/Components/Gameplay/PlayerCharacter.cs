using Demonixis.Toolbox.XR;
using UnityEngine;

namespace TESUnity
{
    public class PlayerCharacter : MonoBehaviour
    {
        [SerializeField]
        private Transform m_LeftHand = null;
        [SerializeField]
        private Transform m_RightHand = null;

        public Transform LeftHand => m_LeftHand;
        public Transform RightHand => m_RightHand;
        public Transform RayCastTarget { get; private set; }

        private void Start()
        {
            var camera = GetComponentInChildren<Camera>();

            RayCastTarget = camera.transform;

            if (XRManager.IsXREnabled())
                RayCastTarget = m_RightHand;
        }
    }
}
