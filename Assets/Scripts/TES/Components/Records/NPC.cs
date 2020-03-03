using TESUnity.ESM.Records;

namespace TESUnity.Components.Records
{
    public class NPC : RecordComponent
    {
        protected NPC_Record _npc;

        private void Start()
        {
            _npc = (NPC_Record)record;
        }
    }
}
