namespace TESUnity.ESM
{
    public enum TESHeaderType
    {
        ESP = 0, ESM = 1, ESS = 2
    }

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

        public HEDRSubRecord HEDR;


        public float Version;
        public TESHeaderType Type;
        public string Company;
        public string Description;
        public uint RecordCount;
        public string Master;
        public long PreviousMasterSize;

        //public override bool NewFetchMethod => true;

        public override void DeserializeSubRecord(UnityBinaryReader reader, string subRecordName, uint dataSize)
        {
            if (subRecordName == "HEDR")
            {
                Version = reader.ReadLESingle();
                Type = (TESHeaderType)reader.ReadLEUInt32();
                Company = reader.ReadASCIIString(32);
                Description = reader.ReadASCIIString(256);
                RecordCount = reader.ReadLEUInt32();
            }
            else if (subRecordName == "MAST")
            {
                Master = reader.ReadPossiblyNullTerminatedASCIIString((int)dataSize);
            }
            else if (subRecordName == "DATA")
            {
                PreviousMasterSize = reader.ReadLEInt64();
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
                case "HEDR":
                    HEDR = new HEDRSubRecord();
                    return HEDR;
                default:
                    return null;
            }
        }
    }
}
