namespace TESUnity.ESM
{
    public struct WeaponData
    {
        public float Weight;
        public int Value;
        public short Type;
        public short Health;
        public float Speed;
        public float Reach;
        public short EnchantPts;
        public byte ChopMin;
        public byte ChopMax;
        public byte SlashMin;
        public byte SlashMax;
        public byte ThrustMin;
        public byte ThrustMax;
        public int Flags;
    }

    public sealed class WEAPRecord : Record, IIdRecord, IModelRecord, IScriptRecord
    {
        public string Id { get; private set; }
        public string Model { get; private set; }
        public string Name { get; private set; }
        public WeaponData Data { get; private set; }
        public string Icon { get; private set; }
        public string Enchantment { get; private set; }
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
            else if (subRecordName == "WPDT")
            {
                Data = new WeaponData
                {
                    Weight = reader.ReadLESingle(),
                    Value = reader.ReadLEInt32(),
                    Type = reader.ReadLEInt16(),
                    Health = reader.ReadLEInt16(),
                    Speed = reader.ReadLESingle(),
                    Reach = reader.ReadLESingle(),
                    EnchantPts = reader.ReadLEInt16(),
                    ChopMin = reader.ReadByte(),
                    ChopMax = reader.ReadByte(),
                    SlashMin = reader.ReadByte(),
                    SlashMax = reader.ReadByte(),
                    ThrustMin = reader.ReadByte(),
                    ThrustMax = reader.ReadByte(),
                    Flags = reader.ReadLEInt32(),
                };
            }
            else if (subRecordName == "ITEX")
            {
                Icon = reader.ReadPossiblyNullTerminatedASCIIString((int)dataSize);
            }
            else if (subRecordName == "ENAM")
            {
                Enchantment = reader.ReadPossiblyNullTerminatedASCIIString((int)dataSize);
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

        #region Deprecated
        public override bool NewFetchMethod => true;
        public override SubRecord CreateUninitializedSubRecord(string subRecordName, uint dataSize) => null;
        #endregion
    }
}
