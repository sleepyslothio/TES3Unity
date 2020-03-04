using System.Collections.Generic;

namespace TESUnity.ESM.Records
{
    public class CONTRecord : Record
    {
        public string Id { get; private set; }
        public string Model { get; private set; }
        public string Name { get; private set; }
        public float Data { get; private set; }
        public int Flags { get; private set; }
        public List<ContNPCOData> Items;

        public CONTRecord()
        {
            Items = new List<ContNPCOData>();
        }

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
            else if (subRecordName == "CNDT")
            {
                Data = reader.ReadLESingle();
            }
            else if (subRecordName == "FLAG")
            {
                Flags = (int)reader.ReadIntRecord(dataSize);
            }
            else if (subRecordName == "NPCO")
            {
                Items.Add(new ContNPCOData
                {
                    Count = reader.ReadLEUInt32(),
                    Name = reader.ReadPossiblyNullTerminatedASCIIString(32)
                });
            }
            else
            {
                ReadMissingSubRecord(reader, subRecordName, dataSize);
            }
        }
    }
}
