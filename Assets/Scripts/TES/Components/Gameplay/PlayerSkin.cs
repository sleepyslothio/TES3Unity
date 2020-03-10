using System.Collections.Generic;
using TES3Unity.ESM;
using TES3Unity.ESM.Records;
using TES3Unity.World;
using UnityEngine;

namespace TES3Unity
{
    public sealed class PlayerSkin : MonoBehaviour
    {
        private void Start()
        {
            var playerData = GameSettings.Get().Player;
            var nifManager = NIFManager.Instance;
            var items = new List<NPCOData>();
            var race = playerData.Race.ToString().ToLower().Replace("_", " ");
            var gender = playerData.Woman ? "f" : "m";
            var player = NPC_Record.CreateRaw(playerData.Name, playerData.Race.ToString(), playerData.Faction, playerData.ClassName, $"b_n_{race}_{gender}_head_01", $"b_n_{race}_{gender}_hair_00", playerData.Woman ? 1 : 0, items);

            var obj = NPCFactory.InstanciateNPC(nifManager, player, false, false);
            obj.transform.parent = transform;
            obj.transform.localPosition = Vector3.zero;
            obj.transform.localRotation = Quaternion.identity;
            obj.SetActive(false); // For now
        }
    }
}
