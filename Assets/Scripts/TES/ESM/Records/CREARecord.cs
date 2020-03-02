namespace TESUnity.ESM
{
    public enum Flags
    {
        Biped = 0x0001,
        Respawn = 0x0002,
        WeaponAndShield = 0x0004,
        None = 0x0008,
        Swims = 0x0010,
        Flies = 0x0020,
        Walks = 0x0040,
        DefaultFlags = 0x0048,
        Essential = 0x0080,
        SkeletonBlood = 0x0400,
        MetalBlood = 0x0800
    }

    public struct CreatureData
    {
        public int type;
        public int level;
        public int strength;
        public int intelligence;
        public int willpower;
        public int agility;
        public int speed;
        public int endurance;
        public int personality;
        public int luck;
        public int health;
        public int spellPts;
        public int fatigue;
        public int soul;
        public int combat;
        public int magic;
        public int stealth;
        public int attackMin1;
        public int attackMax1;
        public int attackMin2;
        public int attackMax2;
        public int attackMin3;
        public int attackMax3;
        public int gold;
    }

    public sealed class CREARecord : Record, IIdRecord, IModelRecord
    {
        public string Id { get; private set; }
        public string Model { get; private set; }
        public string Name { get; private set; }
        public CreatureData Data { get; private set; }
        public int Flags { get; private set; }
        public string Script { get; private set; }
        public float Scale { get; set; }

        public override void DeserializeSubRecord(UnityBinaryReader reader, string subRecordName, uint dataSize)
        {
            if (subRecordName == "NAME")
            {
                Id = reader.ReadPossiblyNullTerminatedASCIIString((int)dataSize);
            }
            else if (subRecordName == "MODL")
            {
                Model = reader.ReadPossiblyNullTerminatedASCIIString((int)dataSize);
            }
            else if (subRecordName == "FNAM")
            {
                Name = reader.ReadPossiblyNullTerminatedASCIIString((int)dataSize);
            }
            else if (subRecordName == "NPDT")
            {
                Data = new CreatureData
                {
                    type = reader.ReadLEInt32(),
                    level = reader.ReadLEInt32(),
                    strength = reader.ReadLEInt32(),
                    intelligence = reader.ReadLEInt32(),
                    willpower = reader.ReadLEInt32(),
                    agility = reader.ReadLEInt32(),
                    speed = reader.ReadLEInt32(),
                    endurance = reader.ReadLEInt32(),
                    personality = reader.ReadLEInt32(),
                    luck = reader.ReadLEInt32(),
                    health = reader.ReadLEInt32(),
                    spellPts = reader.ReadLEInt32(),
                    fatigue = reader.ReadLEInt32(),
                    soul = reader.ReadLEInt32(),
                    combat = reader.ReadLEInt32(),
                    magic = reader.ReadLEInt32(),
                    stealth = reader.ReadLEInt32(),
                    attackMin1 = reader.ReadLEInt32(),
                    attackMax1 = reader.ReadLEInt32(),
                    attackMin2 = reader.ReadLEInt32(),
                    attackMax2 = reader.ReadLEInt32(),
                    attackMin3 = reader.ReadLEInt32(),
                    attackMax3 = reader.ReadLEInt32(),
                    gold = reader.ReadLEInt32()
                };
            }
            else if (subRecordName == "FLAG")
            {
                Flags = (int)ReadIntRecord(reader, dataSize);
            }
            else if (subRecordName == "SCRI")
            {
                Script = reader.ReadPossiblyNullTerminatedASCIIString((int)dataSize);
            }
            else if (subRecordName == "XSCL")
            {
                Scale = reader.ReadLESingle();
            }
            else
            {
                ReadMissingSubRecord(reader, subRecordName, dataSize);
            }
        }

        #region Deprecated
        public override bool NewFetchMethod => true;
        public override SubRecord CreateUninitializedSubRecord(string subRecordName, uint dataSize) => null;
        #endregion
    }
}
