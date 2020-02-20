namespace TESUnity.ESM
{
    public class SPELRecord : Record
    {
        public enum SpellType
        {
            Spell = 0,
            Ability = 1,
            Blight = 2,
            Disease = 3,
            Curse = 4,
            Power = 5
        }

        public enum SpellFlags
        {
            AutoCalc = 0x0001,
            PCStart = 0x0002,
            AlwaysSucceeds = 0x0004
        }

        public class SPDTSubRecord : SubRecord
        {
            public int Type;
            public int SpellCost;
            public int Flags;

            public override void DeserializeData(UnityBinaryReader reader, uint dataSize)
            {
                Type = reader.ReadLEInt32();
                SpellCost = reader.ReadLEInt32();
                Flags = reader.ReadLEInt32();
            }
        }

        public NAMESubRecord NAME;
        public FNAMSubRecord FNAM;
        public SPDTSubRecord SPDT;
        public ENAMSubRecord ENAM;

        public SpellType Type => (SpellType)SPDT.Type;
        public SpellFlags Flags => (SpellFlags)SPDT.Flags;

        public override SubRecord CreateUninitializedSubRecord(string subRecordName, uint dataSize)
        {
            if (subRecordName == "NAME")
            {
                NAME = new NAMESubRecord();
                return NAME;
            }
            else if (subRecordName == "FNAME")
            {
                FNAM = new FNAMSubRecord();
                return FNAM;
            }
            else if (subRecordName == "SPDT")
            {
                SPDT = new SPDTSubRecord();
                return SPDT;
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
