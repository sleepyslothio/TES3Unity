namespace TESUnity.ESM.Records
{
    public sealed class LEVCRecord : Record
    {
        public string Id { get; private set; }
        public int Data { get; private set; }
        public byte Chance { get; private set; }
        public int NumberOfItems { get; private set; }
        public string Item { get; private set; }
        public int PCLevel { get; private set; }

        public override void DeserializeSubRecord(UnityBinaryReader reader, string subRecordName, uint dataSize)
        {
            if (subRecordName == "NAME")
            {
                Id = reader.ReadPossiblyNullTerminatedASCIIString((int)dataSize);
            }
            else if (subRecordName == "DATA")
            {
                Data = (int)ReadIntRecord(reader, dataSize);
            }
            else if (subRecordName == "NNAM")
            {
                Chance = reader.ReadByte();
            }
            else if (subRecordName == "INDX")
            {
                NumberOfItems = (int)ReadIntRecord(reader, dataSize);
            }
            else if (subRecordName == "CNAM")
            {
                Item = reader.ReadPossiblyNullTerminatedASCIIString((int)dataSize);
            }
            else if (subRecordName == "INTV")
            {
                PCLevel = (int)ReadIntRecord(reader, dataSize);
            }
            else
            {
                ReadMissingSubRecord(reader, subRecordName, dataSize);
            }
        }
    }
}
