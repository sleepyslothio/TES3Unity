using Demonixis.ToolboxV2.XR;
using System.Collections;
using Demonixis.ToolboxV2;
using UnityEngine;
using UnityEngine.UI;

namespace TES3Unity.UI
{
    public sealed class UIMiniMap : MonoBehaviour
    {
        private const string MiniMapCameraName = "MiniMapCamera";

        [SerializeField] private RawImage _rawImage;

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
            
            var cameras = player.GetComponentsInChildren<Camera>(true);

            foreach (var target in cameras)
            {
                if (target.name == MiniMapCameraName)
                {
                    SetupMiniCamera(target);
                    yield break;
                }
            }

            gameObject.SetActive(false);
        }

        private void SetupMiniCamera(Camera miniCamera)
        {
            var textureSize = PlatformUtility.IsMobilePlatform() ? 128 : 256;
            var rt = new RenderTexture(textureSize, textureSize, 16);
            miniCamera.targetTexture = rt;
            miniCamera.enabled = true;

            _rawImage.texture = rt;
        }
    }
}