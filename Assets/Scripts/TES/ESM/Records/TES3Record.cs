namespace TESUnity.ESM
{
    public class TES3Record : Record
    {
        public class HEDRSubRecord : SubRecord
        {
            public float version;
            public uint fileType;
            public string companyName; // 32 bytes
            public string fileDescription; // 256 bytes
            public uint numRecords;

            public override void DeserializeData(UnityBinaryReader reader, uint dataSize)
            {
                version = reader.ReadLESingle();
                fileType = reader.ReadLEUInt32();
                companyName = reader.ReadASCIIString(32);
                fileDescription = reader.ReadASCIIString(256);
                numRecords = reader.ReadLEUInt32();
            }
        }

        /*public class MASTSubRecord : SubRecord
        {
            public override void DeserializeData(UnityBinaryReader reader) { }
        }

        public class DATASubRecord : SubRecord
        {
            public override void DeserializeData(UnityBinaryReader reader) { }
        }*/

        public HEDRSubRecord HEDR;
        //public MASTSubRecord[] MASTSs;
        //public DATASubRecord[] DATAs;

        public override SubRecord CreateUninitializedSubRecord(string subRecordName, uint dataSize)
        {
            switch (subRecordName)
            {
                case "HEDR":
                    HEDR = new HEDRSubRecord();
                    return HEDR;
                default:
                    return null;
            }
        }
    }
}
