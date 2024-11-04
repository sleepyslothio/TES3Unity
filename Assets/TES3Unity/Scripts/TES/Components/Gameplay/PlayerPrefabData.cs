using Demonixis.ToolboxV2.XR;
using UnityEngine;

namespace TES3Unity
{
    [CreateAssetMenu(fileName = "PlayerPrefabData", menuName = "TES3 Unity/PlayerPrefabData")]
    public class PlayerPrefabData : ScriptableObject
    {
        [Header("Menu")]
        public GameObject playerMenu;
        public GameObject playerMenuVr;
        public GameObject playerMenuMeta;

        [Header("Character")]
        public GameObject playerCharacter;
        public GameObject playerCharacterVr;
        public GameObject playerCharacterMeta;

        public GameObject GetPlayerMenuPrefab()
        {
            if (!XRManager.Enabled)
                return playerMenu;

            return XRManager.Vendor == XRVendor.Meta ? playerMenuMeta : playerMenuVr;
        }
        
        public GameObject GetPlayerCharacterPrefab()
        {
            if (!XRManager.Enabled)
                return playerCharacter;

            return XRManager.Vendor == XRVendor.Meta ? playerCharacterMeta : playerCharacterVr;
        }
    }
}