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

            /*var obj = NPCFactory.InstanciateNPC(nifManager, player, false, false);
            obj.transform.parent = transform;
            obj.transform.localPosition = Vector3.zero;
            obj.transform.localRotation = Quaternion.identity;
            obj.SetActive(false); // For now*/
        }

        public static (Transform, Transform) AddHands(Transform leftHand, Transform rightHand, bool xrEnabled)
        {
            // Loading hands.
            var nifManager = NIFManager.Instance;

            var race = "nord";
            var gender = "m";
            var hands1st = $"b_n_{race}_{gender}_hands.1st";

            var hands = nifManager.InstantiateNIF($"meshes\\b\\{hands1st}.NIF");

            var meshColliders = hands.GetComponentsInChildren<MeshCollider>(true);
            foreach (var collider in meshColliders)
            {
                Destroy(collider);
            }

            var leftHandObject = CreateHand(hands, leftHand, rightHand, true);
            var rightHandObject = CreateHand(hands, leftHand, rightHand, false);

            if (!xrEnabled)
            {
                leftHand.localPosition = new Vector3(-0.2f, -0.2f, 0.4f);
                leftHand.localRotation = Quaternion.Euler(0, 0, -75);
                rightHand.localPosition = new Vector3(0.2f, -0.2f, 0.4f);
                rightHand.localRotation = Quaternion.Euler(0, 0, 75);
            }

            Destroy(hands.gameObject);

            return (leftHandObject, rightHandObject);
        }

        private static Transform CreateHand(GameObject hands, Transform leftHand, Transform rightHand, bool left)
        {
            var hand = hands.transform.FindChildRecursiveExact($"{(left ? "Left" : "Right")} Hand");
            hand.gameObject.isStatic = false;
            hand.parent = left ? leftHand : rightHand;
            hand.localPosition = Vector3.zero;
            hand.localRotation = Quaternion.Euler(left ? -90.0f : 90.0f, 90.0f, 0.0f);

            var anchor = new GameObject($"{(left ? "Left" : "Right")} Hand Socket");
            var anchorTransform = anchor.transform;
            anchorTransform.parent = hand;
            anchorTransform.localPosition = new Vector3(left ? 0.03f : -0.03f, 0, 0);
            anchorTransform.localRotation = Quaternion.Euler(0, left ? 180 : -180, 0);

            return anchorTransform;
        }
    }
}
