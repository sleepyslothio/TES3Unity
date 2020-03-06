using TES3Unity.ESM.Records;

namespace TES3Unity.Components.Records
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
