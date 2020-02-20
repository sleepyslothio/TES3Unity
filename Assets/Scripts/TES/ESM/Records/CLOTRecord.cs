using System.Collections.Generic;

namespace TESUnity.ESM
{
    public class CLOTRecord : Record
    {
        public class CTDTSubRecord : SubRecord
        {
            public int type;
            public float weight;
            public short value;
            public short enchantPts;

            public override void DeserializeData(UnityBinaryReader reader, uint dataSize)
            {
                type = reader.ReadLEInt32();
                weight = reader.ReadLESingle();
                value = reader.ReadLEInt16();
                enchantPts = reader.ReadLEInt16();
            }
        }

        public NAMESubRecord NAME;
        public MODLSubRecord MODL;
        public FNAMSubRecord FNAM;
        public CTDTSubRecord CTDT;
        public ITEXSubRecord ITEX;

        public List<INDXBNAMCNAMGroup> INDXBNAMCNAMGroups = new List<INDXBNAMCNAMGroup>();

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
                case "CTDT":
                    CTDT = new CTDTSubRecord();
                    return CTDT;
                case "ITEX":
                    ITEX = new ITEXSubRecord();
                    return ITEX;
                case "INDX":
                    var INDX = new INDXSubRecord();

                    var group = new INDXBNAMCNAMGroup();
                    group.INDX = INDX;

                    INDXBNAMCNAMGroups.Add(group);

                    return INDX;
                case "BNAM":
                    var BNAM = new BNAMSubRecord();

                    ArrayUtils.Last(INDXBNAMCNAMGroups).BNAM = BNAM;

                    return BNAM;
                case "CNAM":
                    var CNAM = new CNAMSubRecord();

                    ArrayUtils.Last(INDXBNAMCNAMGroups).CNAM = CNAM;

                    return CNAM;
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
