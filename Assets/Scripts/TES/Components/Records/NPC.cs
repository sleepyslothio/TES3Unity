using TES3Unity.ESM.Records;
using UnityEngine;

namespace TES3Unity.Components.Records
{
    public class NPC : RecordComponent
    {
        private void Start()
        {
            var NPC_ = (NPC_Record)record;

            // Add items
            var db = TES3Manager.MWDataReader.MorrowindESMFile.ObjectsByIDString;
            var nifManager = TES3Manager.Instance.Engine.nifManager;

            foreach (var npcItem in NPC_.Items)
            {
                if (db.ContainsKey(npcItem.Name))
                {
                    var item = db[npcItem.Name];
                    Debug.Log($"Item {npcItem.Name} of type {item.header.name} found.");

                    var weapon = item as WEAPRecord;
                    if (weapon != null)
                    {
                        var weaponModel = nifManager.InstantiateNIF($"meshes\\{weapon.Model}", false);
                        weaponModel.transform.SetParent(transform.FindChildRecursiveExact("Right Hand"));
                        weaponModel.transform.localPosition = Vector3.zero;
                        weaponModel.transform.localRotation = Quaternion.Euler(180, 0, 180);
                    }

                    var cloth = item as CLOTRecord;
                    if (cloth != null)
                    {
                        //var clothModel = nifManager.InstantiateNIF($"meshes\\{cloth.Model}", false);
                    }

                    var armor = item as ARMORecord;
                    if (armor != null)
                    {
                        //var armorModel = nifManager.InstantiateNIF($"meshes\\{armor.Model}", false);
                    }
                }
            }
        }
    }
}
