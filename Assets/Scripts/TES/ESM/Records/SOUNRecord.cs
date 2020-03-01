namespace TESUnity.ESM
{
    public sealed class SOUNRecord : Record, IIdRecord
    {
        public string Id;
        public string Name;
        public byte Volume;
        public byte MinRange;
        public byte MaxRange;

        string IIdRecord.Id => Id;

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

        #region Deprecated / Will be Removed Soon
        public override bool NewFetchMethod => true;
        public override SubRecord CreateUninitializedSubRecord(string subRecordName, uint dataSize) => null;
        #endregion
    }
}
