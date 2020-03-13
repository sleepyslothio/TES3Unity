using Demonixis.Toolbox.XR;
using TES3Unity.Components.XR;
using UnityEngine;

namespace TES3Unity
{
    public class SpectatorSpawner : MonoBehaviour
    {
        private void Start()
        {
            var xrEnabled = XRManager.IsXREnabled();
            var playerPrefabPath = "Prefabs/Spectator";

            // First, create the interaction system if XR is enabled.
            if (xrEnabled)
            {
                PlayerXR.CreateInteractionSystem();
                playerPrefabPath += "XR";
            }

            // Then create the player.
            var player = GameObject.FindWithTag("Player");
            if (player == null)
            {
                var playerPrefab = Resources.Load<GameObject>(playerPrefabPath);
                player = GameObject.Instantiate(playerPrefab);
                player.name = "Player";
            }

            player.transform.position = transform.position;
            player.transform.rotation = transform.rotation;
        }
    }
}
