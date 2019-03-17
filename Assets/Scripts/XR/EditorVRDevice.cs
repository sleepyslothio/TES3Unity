using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Demonixis.Toolbox.XR
{
    public class EditorVRDevice : XRDeviceBase
    {
#if UNITY_EDITOR
        [SerializeField]
        private bool m_Enable = false;
#endif

        public override bool IsAvailable
        {
            get
            {
#if UNITY_EDITOR
                if (m_Enable)
                    return true;
#endif

                return false;
            }
        }

        public override XRDeviceType VRDeviceType => XRDeviceType.UnityXR;

        public override float RenderScale { get; set; } = 1.0f;

        public override int EyeTextureWidth => Screen.width;

        public override int EyeTextureHeight => Screen.height;

        public override void Recenter()
        {
        }

        public override void SetActive(bool isEnabled)
        {
        }

        public override void SetTrackingSpaceType(UnityEngine.XR.TrackingSpaceType type, Transform headTransform, float height)
        {
            var position = headTransform.localPosition;
            headTransform.localPosition = new Vector3(position.x, height, position.z);
        }
    }
}
