namespace TESUnity.ESM
{
    public class WEAPRecord : Record
    {
        public class WPDTSubRecord : SubRecord
        {
            public float weight;
            public int value;
            public short type;
            public short health;
            public float speed;
            public float reach;
            public short enchantPts;
            public byte chopMin;
            public byte chopMax;
            public byte slashMin;
            public byte slashMax;
            public byte thrustMin;
            public byte thrustMax;
            public int flags;

            public override void DeserializeData(UnityBinaryReader reader, uint dataSize)
            {
                weight = reader.ReadLESingle();
                value = reader.ReadLEInt32();
                type = reader.ReadLEInt16();
                health = reader.ReadLEInt16();
                speed = reader.ReadLESingle();
                reach = reader.ReadLESingle();
                enchantPts = reader.ReadLEInt16();
                chopMin = reader.ReadByte();
                chopMax = reader.ReadByte();
                slashMin = reader.ReadByte();
                slashMax = reader.ReadByte();
                thrustMin = reader.ReadByte();
                thrustMax = reader.ReadByte();
                flags = reader.ReadLEInt32();
            }
        }

        public NAMESubRecord NAME;
        public MODLSubRecord MODL;
        public FNAMSubRecord FNAM;
        public WPDTSubRecord WPDT;
        public ITEXSubRecord ITEX;
        public ENAMSubRecord ENAM;
        public SCRISubRecord SCRI;

        public override SubRecord CreateUninitializedSubRecord(string subRecordName, uint dataSize)
        {
            switch (subRecordName)
            {
                case "NAME":
                    NAME = new NAMESubRecord();
                    return NAME;
                case "MODL":
                    MODL = new MODLSubRecord();
                    return MODL;
                case "FNAM":
                    FNAM = new FNAMSubRecord();
                    return FNAM;
                case "WPDT":
                    WPDT = new WPDTSubRecord();
                    return WPDT;
                case "ITEX":
                    ITEX = new ITEXSubRecord();
                    return ITEX;
                case "ENAM":
                    ENAM = new ENAMSubRecord();
                    return ENAM;
                case "SCRI":
                    SCRI = new SCRISubRecord();
                    return SCRI;
                default:
                    return null;
            }
        }
    }
}
