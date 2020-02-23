using TESUnity.ESM;

namespace TESUnity.Components.Records
{
    public class NPCComponent : RecordComponent
    {
        protected NPC_Record _npc;

        private void Start()
        {
            _npc = (NPC_Record)record;
        }
    }
}
