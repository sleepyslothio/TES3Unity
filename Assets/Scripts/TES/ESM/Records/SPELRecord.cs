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

        public string Id;
        public string Name;
        public SpellType Type;
        public int SpellCost;
        public SpellFlags Flags;
        public string Data;

        public override bool NewFetchMethod => true;

        //public SpellType Type => (SpellType)SPDT.Type;
        //public SpellFlags Flags => (SpellFlags)SPDT.Flags;

        public override void DeserializeSubRecord(UnityBinaryReader reader, string subRecordName, uint dataSize)
        {
            if (subRecordName == "NAME")
            {
                Id = reader.ReadPossiblyNullTerminatedASCIIString((int)dataSize);
            }
            else if (subRecordName == "FNAM")
            {
                Name = reader.ReadPossiblyNullTerminatedASCIIString((int)dataSize);
            }
            else if (subRecordName == "SPDT")
            {
                Type = (SpellType)reader.ReadBEInt32();
                SpellCost = reader.ReadLEInt32();
                Flags = (SpellFlags)reader.ReadLEInt32();
            }
            else if (subRecordName == "ENAM")
            {
                Data = reader.ReadPossiblyNullTerminatedASCIIString((int)dataSize);
            }
            else
            {
                ReadMissingSubRecord(reader, subRecordName, dataSize);
            }
        }

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
