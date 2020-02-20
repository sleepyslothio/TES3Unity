namespace TESUnity.ESM
{
    public class ENCHRecord : Record
    {
        public enum EnchantDataType
        {
            CastOne = 0,
            CastStrikes = 1,
            CastWhenUsed = 2,
            ConstantEffect = 3
        }

        public enum RangeType
        {
            Self = 0,
            Touch = 1,
            Target = 2
        }

        public class ENDTSubRecord : SubRecord
        {
            public int Type;
            public int EnchantCost;
            public int Charge;
            public int AutoCalc;

            public override void DeserializeData(UnityBinaryReader reader, uint dataSize)
            {
                Type = reader.ReadBEInt32();
                EnchantCost = reader.ReadBEInt32();
                Charge = reader.ReadBEInt32();
                AutoCalc = reader.ReadBEInt32();
            }
        }

        public class ENAMSubRecord : SubRecord
        {
            public short EffectID;
            public byte SkillID;
            public byte AttributeID;
            public int RangeType;
            public int Area;
            public int Duration;
            public int MagMin;
            public int MagMax;

            public override void DeserializeData(UnityBinaryReader reader, uint dataSize)
            {
                EffectID = reader.ReadLEInt16();
                SkillID = reader.ReadByte();
                AttributeID = reader.ReadByte();
                RangeType = reader.ReadLEInt32();
                Area = reader.ReadLEInt32();
                Duration = reader.ReadLEInt32();
                MagMin = reader.ReadLEInt32();
                MagMax = reader.ReadLEInt32();
            }
        }

        public NAMESubRecord NAME;
        public ENDTSubRecord ENDT;
        public ENAMSubRecord ENAM;

        public EnchantDataType EnchantType => (EnchantDataType)ENDT.Type;
        public RangeType EnchantRangeType => (RangeType)ENAM.RangeType;

        public override SubRecord CreateUninitializedSubRecord(string subRecordName, uint dataSize)
        {
            if (subRecordName == "NAME")
            {
                NAME = new NAMESubRecord();
                return NAME;
            }
            else if (subRecordName == "ENDT")
            {
                ENDT = new ENDTSubRecord();
                return ENDT;
            }
            else if (subRecordName == "ENAM")
            {
                ENAM = new ENAMSubRecord();
                return ENAM;
            }

            return null;
        }
    }
}
