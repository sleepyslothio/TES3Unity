namespace TESUnity.ESM
{
    public class APPARecord : Record
    {
        public class AADTSubRecord : SubRecord
        {
            public int type;
            public float quality;
            public float weight;
            public int value;

            public override void DeserializeData(UnityBinaryReader reader, uint dataSize)
            {
                type = reader.ReadLEInt32();
                quality = reader.ReadLESingle();
                weight = reader.ReadLESingle();
                value = reader.ReadLEInt32();
            }
        }

        public NAMESubRecord NAME;
        public MODLSubRecord MODL;
        public FNAMSubRecord FNAM;
        public AADTSubRecord AADT;
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
                case "AADT":
                    AADT = new AADTSubRecord();
                    return AADT;
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
