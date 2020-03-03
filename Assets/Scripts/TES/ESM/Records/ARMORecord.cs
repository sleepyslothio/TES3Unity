using System.Collections.Generic;

namespace TESUnity.ESM
{
    public enum ArmorType
    {
        Helmet = 0,
        Cuirass,
        LPauldron,
        RPauldron,
        Greaves,
        Boots,
        LGauntlet,
        RGauntlet,
        Shield,
        LBracer,
        RBracer,
    }

    public enum BodyPartIndex
    {
        Head = 0,
        Hair,
        Neck,
        Cuirass,
        Groin,
        Skirt,
        RightHand,
        LeftHand,
        RightWrist,
        LeftWrist,
        Shield,
        RightForearm,
        LeftForearm,
        RightUpperArm,
        LeftUpperArm,
        RightFoot,
        LeftFoot,
        RightAnkle,
        LeftAnkle,
        RightKnee,
        LeftKnee,
        RightUpperLeg,
        LeftUpperLeg,
        RightPauldron,
        LeftPauldron,
        Weapon,
        Tail
    }

    public class ARMORecord : Record
    {
        public class AODTSubRecord : SubRecord
        {
            public int type;
            public float weight;
            public int value;
            public int health;
            public int enchantPts;
            public int armour;

            public override void DeserializeData(UnityBinaryReader reader, uint dataSize)
            {
                type = reader.ReadLEInt32();
                weight = reader.ReadLESingle();
                value = reader.ReadLEInt32();
                health = reader.ReadLEInt32();
                enchantPts = reader.ReadLEInt32();
                armour = reader.ReadLEInt32();
            }
        }

        public NAMESubRecord NAME;
        public MODLSubRecord MODL;
        public FNAMSubRecord FNAM;
        public AODTSubRecord AODT;
        public ITEXSubRecord ITEX;

        public List<INDXBNAMCNAMGroup> INDXBNAMCNAMGroups = new List<INDXBNAMCNAMGroup>();

        public SCRISubRecord SCRI;
        public ENAMSubRecord ENAM;

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
                case "AODT":
                    AODT = new AODTSubRecord();
                    return AODT;
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
                case "SCRI":
                    SCRI = new SCRISubRecord();
                    return SCRI;
                case "ENAM":
                    ENAM = new ENAMSubRecord();
                    return ENAM;
                default:
                    return null;
            }
        }
    }
}
