namespace TESUnity.ESM
{
    public sealed class ACTIRecord : Record, IIdRecord, IModelRecord
    {
        public string Id { get; private set; }
        public string Model { get; private set; }
        public string Name { get; private set; }
        public string Script { get; private set; }

        public override void DeserializeSubRecord(UnityBinaryReader reader, string subRecordName, uint dataSize)
        {
            if (subRecordName == "NAME")
            {
                Id = reader.ReadPossiblyNullTerminatedASCIIString((int)dataSize);
            }
            else if (subRecordName == "MODL")
            {
                Model = reader.ReadPossiblyNullTerminatedASCIIString((int)dataSize);
            }
            else if (subRecordName == "FNAM")
            {
                Name = reader.ReadPossiblyNullTerminatedASCIIString((int)dataSize);
            }
            else if (subRecordName == "SCRI")
            {
                Script = reader.ReadPossiblyNullTerminatedASCIIString((int)dataSize);
            }
            else
            {
                ReadMissingSubRecord(reader, subRecordName, dataSize);
            }
        }

        public override bool NewFetchMethod => true;
        public override SubRecord CreateUninitializedSubRecord(string subRecordName, uint dataSize) => null;
    }
}
