namespace TESUnity.ESM
{
    public class SKILRecord : Record
    {
        public enum SkillSpecification
        {
            Combat = 0, Magic, Stealth
        }

        public class SKDTSubRecord : SubRecord
        {
            public long Attribute;
            public long Specification;
            public float[] UseValue;

            public SkillSpecification SkillSpecification => (SkillSpecification)Specification;

            public override void DeserializeData(UnityBinaryReader reader, uint dataSize)
            {
                Attribute = reader.ReadLEInt32();
                Specification = reader.ReadLEInt32();

                UseValue = new float[4];
                for (var i = 0; i < 4; i++)
                {
                    UseValue[i] = reader.ReadLESingle();
                }
            }
        }

        public Int32SubRecord INDX;
        public SKDTSubRecord SKDT;
        public NAMESubRecord DESC;

        public override SubRecord CreateUninitializedSubRecord(string subRecordName, uint dataSize)
        {
            if (subRecordName == "INDX")
            {
                INDX = new Int32SubRecord();
                return INDX;
            }
            else if (subRecordName == "SKDT")
            {

                SKDT = new SKDTSubRecord();
                return SKDT;
            }
            else if (subRecordName == "DESC")
            {
                DESC = new NAMESubRecord();
                return DESC;
            }

            return null;
        }
    }
}
