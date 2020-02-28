namespace TESUnity.ESM
{
    public class SOUNRecord : Record
    {
        public class DATASubRecord : SubRecord
        {
            public byte volume;
            public byte minRange;
            public byte maxRange;

            public override void DeserializeData(UnityBinaryReader reader, uint dataSize)
            {
                volume = reader.ReadByte();
                minRange = reader.ReadByte();
                maxRange = reader.ReadByte();
            }
        }

        public NAMESubRecord NAME;
        public FNAMSubRecord FNAM;
        public DATASubRecord DATA;

        public string Id;
        public string Name;
        public byte Volume;
        public byte MinRange;
        public byte MaxRange;

        public override bool NewFetchMethod => true;

        public override void DeserializeSubRecord(UnityBinaryReader reader, string subRecordName, uint dataSize)
        {
            if (subRecordName == "NAME")
            {
                Id = reader.ReadPossiblyNullTerminatedASCIIString((int)dataSize);
            }
            else if (subRecordName == "FNAM")
            {
                Name = reader.ReadPossiblyNullTerminatedASCIIString((int)dataSize);
            }
            else if (subRecordName == "DATA")
            {
                Volume = reader.ReadByte();
                MinRange = reader.ReadByte();
                MaxRange = reader.ReadByte();
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
                case "FNAM":
                    FNAM = new FNAMSubRecord();
                    return FNAM;
                case "DATA":
                    DATA = new DATASubRecord();
                    return DATA;
                default:
                    return null;
            }
        }
    }
}
