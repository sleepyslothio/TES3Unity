namespace TESUnity.ESM
{
    public class STATRecord : Record
    {
        public NAMESubRecord NAME;
        public MODLSubRecord MODL;

        public override SubRecord CreateUninitializedSubRecord(string subRecordName, uint dataSize)
        {
            switch (subRecordName)
            {
                case "NAME":
                    NAME = new NAMESubRecord();
                    return NAME;
                case "MODL":
                    MODL = new MODLSubRecord();
                    return MODL;
                default:
                    return null;
            }
        }
    }
}
