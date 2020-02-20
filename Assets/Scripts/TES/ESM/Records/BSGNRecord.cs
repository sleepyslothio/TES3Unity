namespace TESUnity.ESM
{
    public class BSGNRecord : Record
    {
        public NAMESubRecord NAME;
        public FNAMSubRecord FNAM;
        public NAMESubRecord TNAM;
        public NAMESubRecord DESC;
        public NAMESubRecord NPCS;

        public override SubRecord CreateUninitializedSubRecord(string subRecordName, uint dataSize)
        {
            if (subRecordName == "NAME")
            {
                NAME = new NAMESubRecord();
                return NAME;
            }
            else if (subRecordName == "FNAM")
            {
                FNAM = new FNAMSubRecord();
                return FNAM;
            }
            else if (subRecordName == "TNAM")
            {
                TNAM = new NAMESubRecord();
                return TNAM;
            }
            else if (subRecordName == "DESC")
            {
                DESC = new NAMESubRecord();
                return DESC;
            }
            else if (subRecordName == "NPCS")
            {
                NPCS = new NAMESubRecord();
                return NPCS;
            }

            return null;
        }
    }
}
