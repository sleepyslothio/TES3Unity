using TESUnity.ESM.Records;
using UnityEngine;

namespace TESUnity.Diagnostic
{
    public class NPC_Loader : MonoBehaviour
    {
        private NIFManager m_NifManager = null;

        [SerializeField]
        private string m_AnimFile;
        [SerializeField]
        private string m_HeadModel;
        [SerializeField]
        private string m_HairModel;
        [SerializeField]
        private string m_Race;
        [SerializeField]
        private bool m_IsFemale;

        private void Awake()
        {
            var tes = GetComponent<TESManagerLite>();
            tes.Initialized += Tes_Initialized;
        }

        private void Tes_Initialized(TESManagerLite tes)
        {
            if (m_HeadModel == null)
            {
                Debug.Log("Can't load NPC_ because some models are null.");
                return;
            }

            m_NifManager = tes.NifManager;

            var npc = new GameObject($"NPC_");
            var npcTransform = npc.transform;

            // Load animation file.
            var anim = m_NifManager.InstantiateNIF($"meshes\\{m_AnimFile}.NIF");

            var head = new GameObject("Head");
            head.transform.parent = npcTransform;
            head.transform.localPosition = new Vector3(0, 1.2f, 0); // FIXME

            // Load head model
            var headModel = m_NifManager.InstantiateNIF($"meshes\\b\\{m_HeadModel}.NIF");
            headModel.transform.parent = head.transform;
            headModel.transform.localPosition = Vector3.zero;

            // Load hair model
            var hairModel = m_NifManager.InstantiateNIF($"meshes\\b\\{m_HairModel}.NIF");
            hairModel.transform.parent = head.transform;
            hairModel.transform.localPosition = Vector3.zero;

            // Load body parts
            var race = m_Race;
            var gender = m_IsFemale ? "f" : "m";
            var ankle = $"b_n_{race}_{gender}_ankle";
            var foot = $"b_n_{race}_{gender}_foot";
            var forarm = $"b_n_{race}_{gender}_forearm";
            var groin = $"b_n_{race}_{gender}_groin";
            var hands1st = $"b_n_{race}_{gender}_hands.1st";
            var knee = $"b_n_{race}_{gender}_knee";
            var neck = $"b_n_{race}_{gender}_neck";
            var skins = $"b_n_{race}_{gender}_skins";
            var upperArm = $"b_n_{race}_{gender}_upper arm";
            var upperLeg = $"b_n_{race}_{gender}_upper leg";
            var wrist = $"b_n_{race}_{gender}_wrist";

            // Add a fake body: FIXME
            var body = new GameObject("Body");
            var bodyTransform = body.transform;
            bodyTransform.parent = npcTransform;
            bodyTransform.localPosition = Vector3.zero;

            /*CreateBodyPart(ankle, bodyTransform);
            CreateBodyPart(foot, bodyTransform);
            CreateBodyPart(forarm, bodyTransform);
            CreateBodyPart(groin, bodyTransform);
            CreateBodyPart(hands1st, bodyTransform);
            CreateBodyPart(knee, bodyTransform);
            CreateBodyPart(neck, bodyTransform);
            CreateBodyPart(skins, bodyTransform);
            CreateBodyPart(upperArm, bodyTransform);
            CreateBodyPart(upperLeg, bodyTransform);
            CreateBodyPart(wrist, bodyTransform);*/
        }

        private GameObject CreateBodyPart(string path, Transform parent)
        {
            var part = m_NifManager.InstantiateNIF($"meshes\\b\\{path}.NIF");
            part.transform.parent = parent;
            return part;
        }
    }
}