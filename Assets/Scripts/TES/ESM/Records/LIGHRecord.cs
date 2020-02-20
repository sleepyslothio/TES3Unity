namespace TESUnity.ESM
{
    public class LIGHRecord : Record
    {
        public class LHDTSubRecord : SubRecord
        {
            public float weight;
            public int value;
            public int time;
            public int radius;
            public byte red;
            public byte green;
            public byte blue;
            public byte nullByte;
            public int flags;

            public override void DeserializeData(UnityBinaryReader reader, uint dataSize)
            {
                weight = reader.ReadLESingle();
                value = reader.ReadLEInt32();
                time = reader.ReadLEInt32();
                radius = reader.ReadLEInt32();
                red = reader.ReadByte();
                green = reader.ReadByte();
                blue = reader.ReadByte();
                nullByte = reader.ReadByte();
                flags = reader.ReadLEInt32();
            }
        }

        public NAMESubRecord NAME;
        public FNAMSubRecord FNAM;
        public LHDTSubRecord LHDT;
        public SCPTSubRecord SCPT;
        public ITEXSubRecord ITEX;
        public MODLSubRecord MODL;
        public SNAMSubRecord SNAM;

        public override SubRecord CreateUninitializedSubRecord(string subRecordName, uint dataSize)
        {
            switch (subRecordName)
            {
                case "NAME":
                    NAME = new NAMESubRecord();
                    return NAME;
                case "FNAM":
                    FNAM = new FNAMSubRecord();
                    return FNAM;
                case "LHDT":
                    LHDT = new LHDTSubRecord();
                    return LHDT;
                case "SCPT":
                    SCPT = new SCPTSubRecord();
                    return SCPT;
                case "ITEX":
                    ITEX = new ITEXSubRecord();
                    return ITEX;
                case "MODL":
                    MODL = new MODLSubRecord();
                    return MODL;
                case "SNAM":
                    SNAM = new SNAMSubRecord();
                    return SNAM;
                default:
                    return null;
            }
        }
    }
}
