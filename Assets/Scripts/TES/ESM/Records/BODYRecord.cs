namespace TESUnity.ESM
{
    public class BODYRecord : Record
    {
        public BYDTSubRecord BYDT;

        public override SubRecord CreateUninitializedSubRecord(string subRecordName, uint dataSize)
        {
            switch (subRecordName)
            {
                case "BYDT":
                    BYDT = new BYDTSubRecord();
                    return BYDT;
            }

            return null;
        }
    }
}
