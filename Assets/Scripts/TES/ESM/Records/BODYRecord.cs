namespace TESUnity.ESM
{
    public enum BodyPart
    {
        Head = 0,
        Hair = 1,
        Neck = 2,
        Chest = 3,
        Groin = 4,
        Hand = 5,
        Wrist = 6,
        Forearm = 7,
        Upperarm = 8,
        Foot = 9,
        Ankle = 10,
        Knee = 11,
        Upperleg = 12,
        Clavicle = 13,
        Tail = 14
    }

    public enum BodyFlags
    {
        Female = 1, Playabe = 2
    }

    public enum BodyPartType
    {
        Skin = 0, Clothing = 1, Armor = 2
    }

    public class BODYRecord : Record
    {
        public BYDTSubRecord BYDT;

        public override bool NewFetchMethod => true;

        public string Id;
        public string Name;
        public string Model;
        public BodyPart Part;
        public byte Vampire;
        public BodyFlags Flags;
        public BodyPartType PartType;

        public override void DeserializeSubRecord(UnityBinaryReader reader, string subRecordName, uint dataSize)
        {
            if (subRecordName == "NAME")
            {
                Id = reader.ReadASCIIString((int)dataSize);
            }
            else if (subRecordName == "FNAM")
            {
                Name = reader.ReadPossiblyNullTerminatedASCIIString((int)dataSize);
            }
            else if (subRecordName == "BYDT")
            {
                Part = (BodyPart)reader.ReadByte();
                Vampire = reader.ReadByte();
                Flags = (BodyFlags)reader.ReadByte();
                PartType = (BodyPartType)reader.ReadByte();
            }
            else if (subRecordName == "MODL")
            {
                Model = reader.ReadASCIIString((int)dataSize);
            }
            else
            {
                ReadMissingSubRecord(reader, subRecordName, dataSize);
            }
        }

        public override SubRecord CreateUninitializedSubRecord(string subRecordName, uint dataSize)
        {
            switch (subRecordName)
            {
                case "BYDT":
                    BYDT = new BYDTSubRecord();
                    return BYDT;
            }

            return null;
        }
    }
}
