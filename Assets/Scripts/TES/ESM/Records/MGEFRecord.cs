namespace TESUnity.ESM.Records
{
    public class MGEFRecord : Record
    {
        public enum EffectDataFlags
        {
            SpellMaking = 0x0200,
            Enchanting = 0x0400,
            Negative = 0x0800
        }

        public class MEDTSubRecord : SubRecord
        {
            public int SpellSchool { get; private set; }
            public float BaseCost { get; private set; }
            public int Flags { get; private set; }
            public int Red { get; private set; }
            public int Blue { get; private set; }
            public int Green { get; private set; }
            public float SpeedX { get; private set; }
            public float SizeX { get; private set; }
            public float SizeCap { get; private set; }

            public override void DeserializeData(UnityBinaryReader reader, uint dataSize)
            {
                SpellSchool = reader.ReadLEInt32();
                BaseCost = reader.ReadLESingle();
                Flags = reader.ReadLEInt32();
                Red = reader.ReadLEInt32();
                Blue = reader.ReadLEInt32();
                Green = reader.ReadLEInt32();
                SpeedX = reader.ReadLESingle();
                SizeX = reader.ReadLESingle();
                SizeCap = reader.ReadLESingle();
            }
        }

        public Int32SubRecord INDX;
        public MEDTSubRecord MEDT;
        public NAMESubRecord ITEX;
        public NAMESubRecord PTEX;
        public NAMESubRecord CVFX;
        public NAMESubRecord BVFX;
        public NAMESubRecord HVFX;
        public NAMESubRecord AVFX;
        public NAMESubRecord DESC;
        public NAMESubRecord CSND;
        public NAMESubRecord BSND;
        public NAMESubRecord HSND;
        public NAMESubRecord ASND;

        public EffectDataFlags Flags => (EffectDataFlags)MEDT.Flags;

        public override bool NewFetchMethod => false;

        public override void DeserializeSubRecord(UnityBinaryReader reader, string subRecordName, uint dataSize)
        {
            throw new System.NotImplementedException();
        }

        public override SubRecord CreateUninitializedSubRecord(string subRecordName, uint dataSize)
        {
            if (subRecordName == "INDX")
            {
                INDX = new Int32SubRecord();
                return INDX;
            }
            else if (subRecordName == "MEDT")
            {
                MEDT = new MEDTSubRecord();
                return MEDT;
            }
            else if (subRecordName == "ITEX")
            {
                ITEX = new NAMESubRecord();
                return ITEX;
            }
            else if (subRecordName == "PTEX")
            {
                PTEX = new NAMESubRecord();
                return PTEX;
            }
            else if (subRecordName == "CVFX")
            {
                CVFX = new NAMESubRecord();
                return CVFX;
            }
            else if (subRecordName == "BVFX")
            {
                BVFX = new NAMESubRecord();
                return BVFX;
            }
            else if (subRecordName == "HVFX")
            {
                HVFX = new NAMESubRecord();
                return HVFX;
            }
            else if (subRecordName == "AVFX")
            {
                AVFX = new NAMESubRecord();
                return AVFX;
            }
            else if (subRecordName == "DESC")
            {
                DESC = new NAMESubRecord();
                return DESC;
            }
            else if (subRecordName == "CSND")
            {
                CSND = new NAMESubRecord();
                return CSND;
            }
            else if (subRecordName == "BSND")
            {
                BSND = new NAMESubRecord();
                return BSND;
            }
            else if (subRecordName == "HSND")
            {
                HSND = new NAMESubRecord();
                return HSND;
            }
            else if (subRecordName == "ASND")
            {
                ASND = new NAMESubRecord();
                return ASND;
            }

            return null;
        }
    }
}
