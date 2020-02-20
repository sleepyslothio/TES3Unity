namespace TESUnity.ESM
{
    public class LEVIRecord : Record
    {
        public NAMESubRecord NAME;
        public Int32SubRecord DATA;
        public ByteSubRecord NNAM;
        public Int32SubRecord INDX;
        public NAMESubRecord INAM;
        public INTVSubRecord INTV;

        public override SubRecord CreateUninitializedSubRecord(string subRecordName, uint dataSize)
        {
            if (subRecordName == "NAME")
            {
                NAME = new NAMESubRecord();
                return NAME;
            }
            else if (subRecordName == "DATA")
            {
                DATA = new Int32SubRecord();
                return DATA;
            }
            else if (subRecordName == "NNAM")
            {
                NNAM = new ByteSubRecord();
                return NNAM;
            }
            else if (subRecordName == "INDX")
            {
                INDX = new Int32SubRecord();
                return INDX;
            }
            else if (subRecordName == "INAM")
            {
                INAM = new NAMESubRecord();
                return INAM;
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
