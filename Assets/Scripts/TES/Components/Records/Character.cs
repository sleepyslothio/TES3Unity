using TESUnity.ESM;
using TESUnity.ESM.Records;

namespace TESUnity.Components.Records
{
    public class Character : RecordComponent
    {
        public CLASRecord Class { get; protected set; }
        public FACTRecord Faction { get; protected set; }
        public RACERecord Race { get; protected set; }
        public SKILRecord Skills { get; protected set; }
        public BSGNRecord BirthSign { get; protected set; }
    }
}
