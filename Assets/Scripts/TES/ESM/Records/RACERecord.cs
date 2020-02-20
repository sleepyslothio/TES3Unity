namespace TESUnity.ESM
{
    public class RACERecord : Record
    {
        public class RADTSubRecord : SubRecord
        {
            // TODO implement RACERecord::RADT SubRecord
            public override void DeserializeData(UnityBinaryReader reader, uint dataSize)
            {
                reader.BaseStream.Position += dataSize;
            }
        }

        public NAMESubRecord NAME;
        public NAMESubRecord FNAME;
        public RADTSubRecord RADT;
        public NAMESubRecord NPCS;
        public NAMESubRecord DESC;

        public override SubRecord CreateUninitializedSubRecord(string subRecordName, uint dataSize)
        {
            if (subRecordName == "NAME")
            {
                NAME = new NAMESubRecord();
                return NAME;
            }
            else if (subRecordName == "FNAME")
            {
                FNAME = new NAMESubRecord();
                return FNAME;
            }
            else if (subRecordName == "RADT")
            {
                RADT = new RADTSubRecord();
                return RADT;
            }
            else if (subRecordName == "NPCS")
            {
                NPCS = new NAMESubRecord();
                return NPCS;
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
