namespace TESUnity.ESM
{
    public class LTEXRecord : Record
    {
        public class DATASubRecord : STRVSubRecord { }

        public NAMESubRecord NAME;
        public INTVSubRecord INTV;
        public DATASubRecord DATA;

        public string Id;
        public long IntV;
        public string Data;

        //public override bool NewFetchMethod => true;

        public override void DeserializeSubRecord(UnityBinaryReader reader, string subRecordName, uint dataSize)
        {
            if (subRecordName == "NAME")
            {
                Id = reader.ReadPossiblyNullTerminatedASCIIString((int)dataSize);
            }
            else if (subRecordName == "INTV")
            {
                IntV = ReadIntRecord(reader, dataSize);
            }
            else if (subRecordName == "DATA")
            {
                Data = reader.ReadPossiblyNullTerminatedASCIIString((int)dataSize);
            }
            else
            {
                ReadMissingSubRecord(reader, subRecordName, dataSize);
            }
        }

        public override SubRecord CreateUninitializedSubRecord(string subRecordName, uint dataSize)
        {
            switch (subRecordName)
            {
                case "NAME":
                    NAME = new NAMESubRecord();
                    return NAME;
                case "INTV":
                    INTV = new INTVSubRecord();
                    return INTV;
                case "DATA":
                    DATA = new DATASubRecord();
                    return DATA;
                default:
                    return null;
            }
        }
    }
}
