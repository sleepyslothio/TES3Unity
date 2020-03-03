using TESUnity.ESM.Records;

namespace TESUnity.ESM
{
    public struct AlchemyData
    {
        public float Weight;
        public int Value;
        public int AutoCalc;
    }

    public struct AppaData
    {
        public int Type;
        public float Quality;
        public float Weight;
        public int Value;
    }

    public struct ArmorData
    {
        public ArmorType Type;
        public float Weight;
        public int Value;
        public int Health;
        public int EnchantPts;
        public int Armour;
    }

    public struct BookData
    {
        public float Weight;
        public int Value;
        public int Scroll;
        public int SkillID;
        public int EnchantPts;
    }

    public struct CreatureData
    {
        public int Type;
        public int Level;
        public int Strength;
        public int Intelligence;
        public int Willpower;
        public int Agility;
        public int Speed;
        public int Endurance;
        public int Personality;
        public int Luck;
        public int Health;
        public int SpellPts;
        public int Fatigue;
        public int Soul;
        public int Combat;
        public int Magic;
        public int Stealth;
        public int AttackMin1;
        public int AttackMax1;
        public int AttackMin2;
        public int AttackMax2;
        public int AttackMin3;
        public int AttackMax3;
        public int Gold;
    }

    public struct EnchantData
    {
        public EnchantType Type;
        public int EnchantCost;
        public int Charge;
        public int AutoCalc;
    }

    public struct FactionData
    {
        public int AttributeID1;
        public int AttributeID2;
        public FactionRankData[] Data;
        public int[] SkillID;
        public int Unknown1;
        public int Flags;
    }

    public struct FactionRankData
    {
        public int Attribute1;
        public int Attribute2;
        public int FirstSkill;
        public int SecondSkill;
        public int Faction;
    }

    public struct SingleEnchantData
    {
        public short EffectID;
        public byte SkillID;
        public byte AttributeID;
        public EnchantRangeType RangeType;
        public int Area;
        public int Duration;
        public int MagMin;
        public int MagMax;
    }

    public struct IngrediantData
    {
        public float Weight;
        public int Value;
        public int[] EffectID;
        public int[] SkillID;
        public int[] AttributeID;
    }

    public struct LightData
    {
        public float Weight;
        public int Value;
        public int Time;
        public int Radius;
        public byte Red;
        public byte Green;
        public byte Blue;
        public byte NullByte;
        public int Flags;
    }

    public struct LockData
    {
        public float Weight;
        public int Value;
        public float Quality;
        public int Uses;
    }

    public struct MiscData
    {
        public float Weight;
        public uint Value;
        public uint Unknown;
    }

    public struct ProbData
    {
        public float Weight;
        public int Value;
        public float Quality;
        public int Uses;
    }

    public struct WeatherData
    {
        public byte Clear;
        public byte Cloudy;
        public byte Foggy;
        public byte Overcast;
        public byte Rain;
        public byte Thunder;
        public byte Ash;
        public byte Blight;
    }

    public struct MapColorData
    {
        public byte Red;
        public byte Green;
        public byte Blue;
        public byte NullByte;
    }

    public struct SoundRecordData
    {
        public string Sound;
        public byte Chance;
    }

    public struct RepaData
    {
        public float Weight;
        public int Value;
        public int Uses;
        public float Quality;
    }

    public struct ScriptHeader
    {
        public string Name;
        public uint NumShorts;
        public uint NumLongs;
        public uint NumFloats;
        public uint ScriptDataSize;
        public uint LocalVarSize;
    }

    public struct SkillData
    {
        public long Attribute;
        public long Specification;
        public float[] UseValue;
    }

    public struct WeaponData
    {
        public float Weight;
        public int Value;
        public short Type;
        public short Health;
        public float Speed;
        public float Reach;
        public short EnchantPts;
        public byte ChopMin;
        public byte ChopMax;
        public byte SlashMin;
        public byte SlashMax;
        public byte ThrustMin;
        public byte ThrustMax;
        public int Flags;
    }
}
