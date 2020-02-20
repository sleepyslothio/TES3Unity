namespace TESUnity.ESM
{
    public class SNDGRecord : Record
    {
        public enum SoundTypeData
        {
            LeftFoot = 0,
            RightFoot = 1,
            SwimLeft = 2,
            SwimRight = 3,
            Moan = 4,
            Roar = 5,
            Scream = 6,
            Land = 7
        }

        public NAMESubRecord NAME;
        public Int32SubRecord DATA;
        public NAMESubRecord SNAM;
        public NAMESubRecord CNAM;

        public SoundTypeData SoundType => (SoundTypeData)DATA.value;

        public override SubRecord CreateUninitializedSubRecord(string subRecordName, uint dataSize)
        {
            if (subRecordName == "NAME")
            {
                NAME = new NAMESubRecord();
                return NAME;
            }
            else if (subRecordName == "DATA")
            {
                DATA = new Int32SubRecord();
                return DATA;
            }
            else if (subRecordName == "SNAM")
            {
                SNAM = new NAMESubRecord();
                return SNAM;
            }
            else if (subRecordName == "CNAM")
            {
                CNAM = new NAMESubRecord();
                return CNAM;
            }

            return null;
        }
    }
}
