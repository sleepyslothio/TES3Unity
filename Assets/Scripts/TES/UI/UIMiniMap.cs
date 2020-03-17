using Demonixis.Toolbox.XR;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace TES3Unity.UI
{
    public sealed class UIMiniMap : MonoBehaviour
    {
        [SerializeField]
        private RawImage m_RawImage = null;

        private IEnumerator Start()
        {
            // For obvious reasons, the minimap is disabled in VR.
            if (XRManager.IsXREnabled())
            {
                gameObject.SetActive(false);
                yield break;
            }

            var playerTag = "Player";
            var player = GameObject.FindWithTag(playerTag);

            while (player == null)
            {
                player = GameObject.FindWithTag(playerTag);
                yield return null;
            }

            Camera miniCamera = null;
            var cameras = player.GetComponentsInChildren<Camera>(true);

            foreach (var camera in cameras)
            {
                if (!camera.enabled)
                {
                    miniCamera = camera;
                }
            }

            if (miniCamera != null)
            {
                var rt = new RenderTexture(256, 256, 16);
                miniCamera.targetTexture = rt;
                miniCamera.enabled = true;

                m_RawImage.texture = rt;
            }
            else
            {
                gameObject.SetActive(false);
            }
        }
    }
}
