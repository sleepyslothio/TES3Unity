namespace TESUnity.ESM
{
    public class PROBRecord : Record
    {
        public class PBDTSubRecord : SubRecord
        {
            public float weight;
            public int value;
            public float quality;
            public int uses;

            public override void DeserializeData(UnityBinaryReader reader, uint dataSize)
            {
                weight = reader.ReadLESingle();
                value = reader.ReadLEInt32();
                quality = reader.ReadLESingle();
                uses = reader.ReadLEInt32();
            }
        }

        public NAMESubRecord NAME;
        public MODLSubRecord MODL;
        public FNAMSubRecord FNAM;
        public PBDTSubRecord PBDT;
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
                case "PBDT":
                    PBDT = new PBDTSubRecord();
                    return PBDT;
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
