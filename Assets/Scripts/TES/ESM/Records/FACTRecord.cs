namespace TESUnity.ESM
{
    public class FACTRecord : Record
    {
        public class FADTSubRecord : SubRecord
        {
            // TODO implement FACTRecord::FADT SubRecord
            public override void DeserializeData(UnityBinaryReader reader, uint dataSize)
            {
                reader.BaseStream.Position += dataSize;
            }
        }

        public NAMESubRecord NAME;
        public FNAMSubRecord FNAME;
        public NAMESubRecord RNAME;
        public FADTSubRecord FADT;
        public NAMESubRecord ANAM;
        public INTVSubRecord INTV;

        public override SubRecord CreateUninitializedSubRecord(string subRecordName, uint dataSize)
        {
            if (subRecordName == "NAME")
            {
                NAME = new NAMESubRecord();
                return NAME;
            }
            else if (subRecordName == "FNAME")
            {
                FNAME = new FNAMSubRecord();
                return FNAME;
            }
            else if (subRecordName == "RNAME")
            {
                RNAME = new NAMESubRecord();
                return RNAME;
            }
            else if (subRecordName == "FADT")
            {
                FADT = new FADTSubRecord();
                return FADT;
            }
            else if (subRecordName == "ANAM")
            {
                ANAM = new NAMESubRecord();
                return ANAM;
            }
            else if (subRecordName == "INTV")
            {
                INTV = new INTVSubRecord();
                return INTV;
            }

            return null;
        }
    }
}
