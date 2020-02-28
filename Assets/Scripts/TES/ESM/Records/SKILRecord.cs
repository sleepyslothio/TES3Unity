namespace TESUnity.ESM
{
    public enum SkillSpecification
    {
        Combat = 0, Magic, Stealth
    }

    public struct SkillData
    {
        public long Attribute;
        public long Specification;
        public float[] UseValue;
    }

    public class SKILRecord : Record
    {
        public int SkillId { get; private set; }
        public SkillData SKDT { get; private set; }
        public string Description { get; private set; }

        public override void DeserializeSubRecord(UnityBinaryReader reader, string subRecordName, uint dataSize)
        {
            if (subRecordName == "INDX")
            {
                SkillId = (int)ReadIntRecord(reader, dataSize);
            }
            else if (subRecordName == "SKDT")
            {
                SKDT = new SkillData
                {
                    Attribute = reader.ReadLEInt32(),
                    Specification = reader.ReadLEInt32(),
                    UseValue = ReadSingles(reader, 4)
                };
            }
            else if (subRecordName == "DESC")
            {
                Description = reader.ReadPossiblyNullTerminatedASCIIString((int)dataSize);
            }
        }

        #region Deprecated
        public override bool NewFetchMethod => true;
        public override SubRecord CreateUninitializedSubRecord(string subRecordName, uint dataSize) => null;
        #endregion
    }
}
