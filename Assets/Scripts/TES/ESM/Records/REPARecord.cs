namespace TESUnity.ESM
{
    public class REPARecord : Record
    {
        public class RIDTSubRecord : SubRecord
        {
            public float weight;
            public int value;
            public int uses;
            public float quality;

            public override void DeserializeData(UnityBinaryReader reader, uint dataSize)
            {
                weight = reader.ReadLESingle();
                value = reader.ReadLEInt32();
                uses = reader.ReadLEInt32();
                quality = reader.ReadLESingle();
            }
        }

        public NAMESubRecord NAME;
        public MODLSubRecord MODL;
        public FNAMSubRecord FNAM;
        public RIDTSubRecord RIDT;
        public ITEXSubRecord ITEX;
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
                case "RIDT":
                    RIDT = new RIDTSubRecord();
                    return RIDT;
                case "ITEX":
                    ITEX = new ITEXSubRecord();
                    return ITEX;
                case "SCRI":
                    SCRI = new SCRISubRecord();
                    return SCRI;
                default:
                    return null;
            }
        }
    }
}
