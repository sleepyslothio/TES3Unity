namespace TESUnity.ESM
{
    public class PGRDRecord : Record
    {
        public class PathGridSubRecord : SubRecord
        {
            public byte[] PathGrid { get; private set; }

            public override void DeserializeData(UnityBinaryReader reader, uint dataSize)
            {
                PathGrid = reader.ReadBytes((int)dataSize);
            }
        }

        public PathGridSubRecord DATA { get; private set; }

        public override SubRecord CreateUninitializedSubRecord(string subRecordName, uint dataSize)
        {
            if (subRecordName == "DATA")
            {
                DATA = new PathGridSubRecord();
                return DATA;
            }

            return null;
        }
    }
}
