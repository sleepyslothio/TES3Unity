namespace TESUnity.ESM
{
    public class BOOKRecord : Record
    {
        public class BKDTSubRecord : SubRecord
        {
            public float weight;
            public int value;
            public int scroll;
            public int skillID;
            public int enchantPts;

            public override void DeserializeData(UnityBinaryReader reader, uint dataSize)
            {
                weight = reader.ReadLESingle();
                value = reader.ReadLEInt32();
                scroll = reader.ReadLEInt32();
                skillID = reader.ReadLEInt32();
                enchantPts = reader.ReadLEInt32();
            }
        }

        public NAMESubRecord NAME;
        public MODLSubRecord MODL;
        public FNAMSubRecord FNAM;
        public BKDTSubRecord BKDT;
        public ITEXSubRecord ITEX;
        public SCRISubRecord SCRI;
        public TEXTSubRecord TEXT;

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
                case "BKDT":
                    BKDT = new BKDTSubRecord();
                    return BKDT;
                case "ITEX":
                    ITEX = new ITEXSubRecord();
                    return ITEX;
                case "SCRI":
                    SCRI = new SCRISubRecord();
                    return SCRI;
                case "TEXT":
                    TEXT = new TEXTSubRecord();
                    return TEXT;
                default:
                    return null;
            }
        }
    }
}
