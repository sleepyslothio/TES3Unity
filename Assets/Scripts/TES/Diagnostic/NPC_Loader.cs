using System.Collections.Generic;
using TES3Unity.Components.Records;
using TES3Unity.ESM.Records;
using TES3Unity.World;
using UnityEngine;

namespace TES3Unity.Diagnostic
{
    public class NPC_Loader : MonoBehaviour
    {
        private NIFManager m_NifManager = null;

        [SerializeField]
        private int m_Index = 0;

        private void Awake()
        {
            var tes = GetComponent<TES3ManagerLite>();
            tes.Initialized += Tes_Initialized;
        }

        private void Tes_Initialized(TES3ManagerLite tes)
        {
            m_NifManager = tes.NifManager;

            var npcs = tes.DataReader.MorrowindESMFile.GetRecords<NPC_Record>();

            if (m_Index < 0 || m_Index > npcs.Count - 1)
            {
                m_Index = 0;
            }

            var npcRecord = npcs[m_Index];

            // Instanciate NPC
            var npcObj = NPCFactory.InstanciateNPC(m_NifManager, npcRecord);
            var component = npcObj.AddComponent<NPC>();
            component.record = npcRecord;   
        }
    }
}