namespace TESUnity.ESM
{
    /// <summary>
    /// A record that defines a Class.
    /// </summary>
    public class CLASRecord : Record
    {
        public enum AutoCalcFlagsType
        {
            Weapon = 0x00001,
            Armor = 0x00002,
            Clothing = 0x00004,
            Books = 0x00008,
            Ingrediant = 0x00010,
            Picks = 0x00020,
            Probes = 0x00040,
            Lights = 0x00080,
            Apparatus = 0x00100,
            Repair = 0x00200,
            Misc = 0x00400,
            Spells = 0x00800,
            MagicItems = 0x01000,
            Potions = 0x02000,
            Training = 0x04000,
            Spellmaking = 0x08000,
            Enchanting = 0x10000,
            RepairItem = 0x20000
        }

        public enum ClassSpecialization
        {
            Combat = 0,
            Magic = 1,
            Stealth = 2
        }

        public class CLDTSubRecord : SubRecord
        {
            public int AttributeID1;
            public int AttributeID2;
            public int Specialization;
            public int MinorID1;
            public int MajorID1;
            public int MinorID2;
            public int MajorID2;
            public int MinorID3;
            public int MajorID3;
            public int MinorID4;
            public int MajorID4;
            public int MinorID5;
            public int MajorID5;
            public int Flags;
            public int AutoCalcFlags;

            public override void DeserializeData(UnityBinaryReader reader, uint dataSize)
            {
                AttributeID1 = reader.ReadLEInt32();
                AttributeID2 = reader.ReadLEInt32();
                Specialization = reader.ReadLEInt32();
                MinorID1 = reader.ReadLEInt32();
                MajorID1 = reader.ReadLEInt32();
                MinorID2 = reader.ReadLEInt32();
                MajorID2 = reader.ReadLEInt32();
                MinorID3 = reader.ReadLEInt32();
                MajorID3 = reader.ReadLEInt32();
                MinorID4 = reader.ReadLEInt32();
                MajorID4 = reader.ReadLEInt32();
                MinorID5 = reader.ReadLEInt32();
                MajorID5 = reader.ReadLEInt32();
                Flags = reader.ReadLEInt32();
                AutoCalcFlags = reader.ReadLEInt32();
            }
        }

        public NAMESubRecord NAME;
        public FNAMSubRecord FNAM;
        public CLDTSubRecord CLDT;
        public NAMESubRecord DESC;

        public bool Playable => CLDT.Flags == 0x0001;
        public AutoCalcFlagsType AutoCalcFlags => (AutoCalcFlagsType)CLDT.Flags;
        public ClassSpecialization Specialization => (ClassSpecialization)CLDT.AutoCalcFlags;

        public override SubRecord CreateUninitializedSubRecord(string subRecordName, uint dataSize)
        {
            if (subRecordName == "NAME")
            {
                NAME = new NAMESubRecord();
                return NAME;
            }
            else if (subRecordName == "FNAM")
            {
                FNAM = new FNAMSubRecord();
                return FNAM;
            }
            else if (subRecordName == "CLDT")
            {
                CLDT = new CLDTSubRecord();
                return CLDT;
            }
            else if (subRecordName == "DESC")
            {
                DESC = new NAMESubRecord();
                return DESC;
            }

            return null;
        }
    }
}
