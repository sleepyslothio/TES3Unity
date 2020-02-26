using TESUnity.ESM;

namespace TESUnity.Components.Records
{
    public class TESScript : RecordComponent
    {
        private SCPTRecord _script;

        private void Start()
        {
            _script = (SCPTRecord)record;
        }
    }
}
