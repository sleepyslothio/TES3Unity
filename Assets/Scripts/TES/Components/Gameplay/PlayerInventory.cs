using System.Collections.Generic;
using TESUnity.Components.Records;
using TESUnity.ESM;
using UnityEngine;

namespace TESUnity.Components
{
    public class PlayerInventory : MonoBehaviour
    {
        private List<Record> _inventory = new List<Record>();
        private Transform _disabledObjects = null;
        private PlayerCharacter _player = null;

        void Start()
        {
            var disabledObjectGO = new GameObject("DisabledObjects");
            disabledObjectGO.SetActive(false);
            _disabledObjects = disabledObjectGO.GetComponent<Transform>();
            _player = GetComponent<PlayerCharacter>();
        }

        public void Add(RecordComponent item)
        {
            Add(item.record);

            // For now.
            var weapon = item as Weapon;
            if (weapon != null)
            {
                var rightHand = _player.RightHand;
                if (rightHand.childCount > 0)
                    rightHand.GetChild(0).parent = _disabledObjects;

                ((Weapon)item).Equip(rightHand);
                return;
            }

            item.transform.parent = _disabledObjects.transform;
        }

        public void Add(Record record)
        {
            _inventory.Add(record);
        }
    }
}