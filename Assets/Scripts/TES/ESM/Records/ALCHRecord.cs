namespace TESUnity.ESM
{
    public struct AlchemyData
    {
        public float Weight;
        public int Value;
        public int AutoCalc;
    }

    public struct EnchantmentData
    {
        public short EffectID;
        public byte SkillID;
        public byte AttributeID;
        public int Unknown1;
        public int Unknown2;
        public int Duration;
        public int Magnitude;
        public int Unknown4;
    }

    public class ALCHRecord : Record, IIdRecord, IModelRecord
    {
        public string Id { get; private set; }
        public string Model { get; private set; }
        public string Name { get; private set; }
        public AlchemyData Data { get; private set; }
        public EnchantmentData Enchantment { get; private set; }
        public string Icon { get; private set; }
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
            else if (subRecordName == "ALDT")
            {
                Data = new AlchemyData
                {
                    Weight = reader.ReadLESingle(),
                    Value = reader.ReadLEInt32(),
                    AutoCalc = reader.ReadLEInt32()
                };
            }
            else if (subRecordName == "ENAM")
            {
                Enchantment = new EnchantmentData
                {
                    EffectID = reader.ReadLEInt16(),
                    SkillID = reader.ReadByte(),
                    AttributeID = reader.ReadByte(),
                    Unknown1 = reader.ReadLEInt32(),
                    Unknown2 = reader.ReadLEInt32(),
                    Duration = reader.ReadLEInt32(),
                    Magnitude = reader.ReadLEInt32(),
                    Unknown4 = reader.ReadLEInt32()
                };
            }
            else if (subRecordName == "TEXT")
            {
                Icon = reader.ReadPossiblyNullTerminatedASCIIString((int)dataSize);
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
