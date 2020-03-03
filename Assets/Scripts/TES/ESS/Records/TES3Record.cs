using TESUnity.ESM;

namespace TESUnity.ESS.Records
{
    public sealed class TES3Record : Record
    {
        public string CellName { get; private set; }

        public override void DeserializeSubRecord(UnityBinaryReader reader, string subRecordName, uint dataSize)
        {
            if (subRecordName == "GMDT")
            {
                var health = reader.ReadLESingle();
                var maxHealth = reader.ReadLESingle();
                var time = reader.ReadLESingle();
                var month = reader.ReadLESingle();
                var day = reader.ReadLESingle();
                var year = reader.ReadLESingle();
                var cell = reader.ReadPossiblyNullTerminatedASCIIString(64);
                var dayPassed = reader.ReadLESingle();
                var characterName = reader.ReadPossiblyNullTerminatedASCIIString(32);

                CellName = Convert.RemoveNullChar(cell);
            }
            else
            {
                ReadMissingSubRecord(reader, subRecordName, dataSize);
            }
        }
    }
}
