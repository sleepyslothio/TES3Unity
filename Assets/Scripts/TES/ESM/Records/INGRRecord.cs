namespace TESUnity.ESM
{
    public class INGRRecord : Record
    {
        public class IRDTSubRecord : SubRecord
        {
            public float weight;
            public int value;
            public int[] effectID;
            public int[] skillID;
            public int[] attributeID;

            public override void DeserializeData(UnityBinaryReader reader, uint dataSize)
            {
                weight = reader.ReadLESingle();
                value = reader.ReadLEInt32();

                effectID = new int[4];
                for (int i = 0; i < effectID.Length; i++)
                {
                    effectID[i] = reader.ReadLEInt32();
                }

                skillID = new int[4];
                for (int i = 0; i < skillID.Length; i++)
                {
                    skillID[i] = reader.ReadLEInt32();
                }

                attributeID = new int[4];
                for (int i = 0; i < attributeID.Length; i++)
                {
                    attributeID[i] = reader.ReadLEInt32();
                }
            }
        }

        public NAMESubRecord NAME;
        public MODLSubRecord MODL;
        public FNAMSubRecord FNAM;
        public IRDTSubRecord IRDT;
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
                case "IRDT":
                    IRDT = new IRDTSubRecord();
                    return IRDT;
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
