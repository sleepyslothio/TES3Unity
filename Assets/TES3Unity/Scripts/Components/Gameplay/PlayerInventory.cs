using System;
using System.Collections.Generic;
using TES3Unity.Components.Records;
using TES3Unity.ESM.Records;
using UnityEngine;

namespace TES3Unity.Components
{
    public class PlayerInventory : MonoBehaviour
    {
        private List<RecordComponent> _itemStore = new();
        private Transform _disabledObjects;
        private PlayerCharacter _playerCharacter;

        public event Action<RecordComponent, bool> ItemAddedChanged;

        private void Start()
        {
            var disabledObjectGo = new GameObject("DisabledObjects");
            disabledObjectGo.SetActive(false);

            _disabledObjects = disabledObjectGo.GetComponent<Transform>();
            _playerCharacter = GetComponent<PlayerCharacter>();
        }

        public void Equip(RecordComponent item, BodyPartIndex part)
        {

        }

        public void Unequip(RecordComponent item)
        {

        }

        public void AddItem(RecordComponent item)
        {
            // For now.
            var weapon = item as Weapon;
            if (weapon != null)
            {
                var rightHand = _playerCharacter.RightHandSocket;
                if (rightHand.childCount > 0)
                {
                    rightHand.GetChild(0).parent = _disabledObjects;
                }
                weapon.Equip(rightHand, _playerCharacter.RightHandContainer);
                return;
            }

            item.transform.parent = _disabledObjects.transform;
            ItemAddedChanged?.Invoke(item, true);
        }

        public void DropItem(RecordComponent item, Transform parent, Vector3 position, Quaternion rotation)
        {
            item.transform.parent = parent;
            item.transform.position = position;
            item.transform.rotation = rotation;

            ItemAddedChanged?.Invoke(item, false);
        }
    }
}