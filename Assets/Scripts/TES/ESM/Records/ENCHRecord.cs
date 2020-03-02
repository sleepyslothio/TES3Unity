namespace TESUnity.ESM
{
    public enum EnchantType
    {
        CastOne = 0,
        CastStrikes = 1,
        CastWhenUsed = 2,
        ConstantEffect = 3
    }

    public enum EnchantRangeType
    {
        Self = 0,
        Touch = 1,
        Target = 2
    }

    public struct EnchantData
    {
        public EnchantType Type;
        public int EnchantCost;
        public int Charge;
        public int AutoCalc;
    }

    public struct SingleEnchantData
    {
        public short EffectID;
        public byte SkillID;
        public byte AttributeID;
        public EnchantRangeType RangeType;
        public int Area;
        public int Duration;
        public int MagMin;
        public int MagMax;
    }

    public class ENCHRecord : Record
    {
        public string Id { get; private set; }
        public EnchantData Data { get; private set; }
        public SingleEnchantData SingleData { get; private set; }

        public override void DeserializeSubRecord(UnityBinaryReader reader, string subRecordName, uint dataSize)
        {
            if (subRecordName == "NAME")
            {
                Id = reader.ReadPossiblyNullTerminatedASCIIString((int)dataSize);
            }
            else if (subRecordName == "ENDT")
            {
                Data = new EnchantData
                {
                    Type = (EnchantType)reader.ReadBEInt32(),
                    EnchantCost = reader.ReadBEInt32(),
                    Charge = reader.ReadBEInt32(),
                    AutoCalc = reader.ReadBEInt32(),
                };
            }
            else if (subRecordName == "ENAM")
            {
                SingleData = new SingleEnchantData
                {
                    EffectID = reader.ReadLEInt16(),
                    SkillID = reader.ReadByte(),
                    AttributeID = reader.ReadByte(),
                    RangeType = (EnchantRangeType)reader.ReadLEInt32(),
                    Area = reader.ReadLEInt32(),
                    Duration = reader.ReadLEInt32(),
                    MagMin = reader.ReadLEInt32(),
                    MagMax = reader.ReadLEInt32()
                };
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
