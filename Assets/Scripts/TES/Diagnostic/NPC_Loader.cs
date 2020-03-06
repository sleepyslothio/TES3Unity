using System.Collections.Generic;
using UnityEngine;

namespace TESUnity.Diagnostic
{
    public class NPC_Loader : MonoBehaviour
    {
        private NIFManager m_NifManager = null;

        [SerializeField]
        private string m_NPCName = null;
        [SerializeField]
        private string m_AnimFile = null;
        [SerializeField]
        private string m_HeadModel = null;
        [SerializeField]
        private string m_HairModel = null;
        [SerializeField]
        private string m_Race = null;
        [SerializeField]
        private bool m_IsFemale = false;
        [SerializeField]
        private Material m_DebugMaterial = null;
        [SerializeField]
        private bool m_DebugSkeleton = true;

        private void Awake()
        {
            var tes = GetComponent<TESManagerLite>();
            tes.Initialized += Tes_Initialized;
        }

        private void Tes_Initialized(TESManagerLite tes)
        {
            if (string.IsNullOrEmpty(m_HeadModel))
            {
                Debug.Log("Can't load NPC_ because some models are null.");
                return;
            }

            m_NifManager = tes.NifManager;

            var npc = new GameObject($"{m_NPCName}");
            var npcTransform = npc.transform;

            // Load animation file.
            if (string.IsNullOrEmpty(m_AnimFile))
            {
                m_AnimFile = "xbase_anim";

                if (m_IsFemale)
                {
                    m_AnimFile += "_female";
                }
            }

            var anim = m_NifManager.InstantiateNIF($"meshes\\{m_AnimFile}.NIF", false);

            var boneMapping = new Dictionary<string, Transform>();

            // Debug
            var renderers = anim.GetComponentsInChildren<MeshRenderer>(true);
            Transform bone = null;
            foreach (var renderer in renderers)
            {          
                if (renderer.name.Contains("Shadow") || renderer.name.Contains("QuadPatch"))
                {
                    continue;
                }

                bone = renderer.transform.parent;
                boneMapping.Add(bone.name, bone);

                if (m_DebugSkeleton)
                {
                    renderer.enabled = true;
                    renderer.sharedMaterial = m_DebugMaterial;
                }
            }

            // Load body parts
            var race = m_Race;
            var gender = m_IsFemale ? "f" : "m";
            var ankle = $"b_n_{race}_{gender}_ankle";
            var foot = $"b_n_{race}_{gender}_foot";
            var forearm = $"b_n_{race}_{gender}_forearm";
            var groin = $"b_n_{race}_{gender}_groin";
            var knee = $"b_n_{race}_{gender}_knee";
            var neck = $"b_n_{race}_{gender}_neck";
            var skins = $"b_n_{race}_{gender}_skins";
            var upperArm = $"b_n_{race}_{gender}_upper arm";
            var upperLeg = $"b_n_{race}_{gender}_upper leg";
            var wrist = $"b_n_{race}_{gender}_wrist";

            AddBodyPart(m_HeadModel, boneMapping["Head"]);
            AddBodyPart(m_HairModel, boneMapping["Head"]);
            AddBodyPart(ankle, boneMapping["Left Ankle"], true);
            AddBodyPart(ankle, boneMapping["Right Ankle"]);
            AddBodyPart(foot, boneMapping["Left Foot"], true);
            AddBodyPart(foot, boneMapping["Right Foot"]);
            AddBodyPart(forearm, boneMapping["Left Forearm"], true);
            AddBodyPart(forearm, boneMapping["Right Forearm"]);
            AddBodyPart(groin, boneMapping["Groin"]);
            AddBodyPart(knee, boneMapping["Left Knee"], true);
            AddBodyPart(knee, boneMapping["Right Knee"]);
            AddBodyPart(neck, boneMapping["Neck"]);
            AddBodyPart(upperArm, boneMapping["Left Upper Arm"], true);
            AddBodyPart(upperArm, boneMapping["Right Upper Arm"]);
            AddBodyPart(upperLeg, boneMapping["Left Upper Leg"], true);
            AddBodyPart(upperLeg, boneMapping["Right Upper Leg"]);
            AddBodyPart(wrist, boneMapping["Left Wrist"], true);
            AddBodyPart(wrist, boneMapping["Right Wrist"]);

            var chest = AddBodyPart(skins, boneMapping["Chest"]);
            chest.localPosition = new Vector3(0.019f, 0.091f, 0.02f); // Magic Vector3...

            var leftHand = chest.Find("Bip01 Pelvis/Bip01 Spine/Bip01 Spine1/Bip01 Spine2/Bip01 Neck/Bip01 L Clavicle/Bip01 L UpperArm/Left Hand");
            leftHand.SetParent(boneMapping["Left Hand"], false);

            var position = leftHand.localPosition;
            var rotation = leftHand.localRotation.eulerAngles;
            var scale = leftHand.localScale;
            leftHand.localPosition = new Vector3(-position.x, position.y, position.z);
            leftHand.localRotation = Quaternion.Euler(rotation.x, rotation.y, -rotation.z);
            leftHand.localScale = new Vector3(scale.x *= -1f, scale.y, scale.z);

            var rightHand = chest.Find("Bip01 Pelvis/Bip01 Spine/Bip01 Spine1/Bip01 Spine2/Bip01 Neck/Bip01 R Clavicle/Bip01 R UpperArm/Right Hand");
            rightHand.SetParent(boneMapping["Right Hand"], false);
            // Load Items...
            // TODO
        }

        private Transform AddBodyPart(string path, Transform parent, bool invertXScale = false)
        {
            var part = m_NifManager.InstantiateNIF($"meshes\\b\\{path}.NIF", false);
            var partTransform = part.transform;

            if (invertXScale)
            {
                var position = partTransform.localPosition;
                var rotation = partTransform.localRotation.eulerAngles;
                var scale = partTransform.localScale;
                partTransform.localPosition = new Vector3(-position.x, position.y, position.z);
                partTransform.localRotation = Quaternion.Euler(rotation.x, rotation.y, -rotation.z);
                partTransform.localScale = new Vector3(scale.x *= - 1f, scale.y, scale.z);
            }

            partTransform.SetParent(parent, false);

            return partTransform;
        }

        private Transform GetChildRecursive(Transform target, string name)
        {
            if (target.childCount == 0)
            {
                return null;
            }

            var child = target.GetChild(0);
            if (child.name == name)
            {
                return child;
            }

            return GetChildRecursive(child, name);
        }
    }
}