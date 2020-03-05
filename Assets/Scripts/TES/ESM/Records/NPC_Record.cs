using System.Collections.Generic;
using UnityEngine;

namespace TESUnity.ESM.Records
{
    public enum NPCFlags
    {
        Female = 0x0001,
        Essential = 0x0002,
        Respawn = 0x0004,
        None = 0x0008,
        Autocalc = 0x0010,
        BloodSkel = 0x0400,
        BloodMetal = 0x0800
    }

    public enum NPCAIDataFlags
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

    public sealed class NPC_Record : Record
    {
        public string Id { get; private set; }
        public string Name { get; private set; }
        public string Model { get; private set; }
        public string Race { get; private set; }
        public string Faction { get; private set; }
        public string HeadModel { get; private set; }
        public string Class { get; private set; }
        public string HairModel { get; private set; }
        public NPC_Data Data { get; private set; }
        public NPCFlags Flags { get; private set; }
        public List<NPCOData> Items { get; private set; }
        public List<string> Spells { get; private set; }
        public NPC_AIData AIData { get; private set; }
        public NPC_AIW AIW { get; private set; }
        public NPC_AITravel AITravel { get; private set; }
        public NPC_AIFollow AIFollow { get; private set; }
        public NPC_AIFollow AIEscort { get; private set; }
        public string CellEscort { get; private set; }
        public NPC_AIActivate AIActivate { get; private set; }
        public NPC_CellTravelDestination CellTravelDestination { get; private set; }
        public string PreviousCellDestination { get; private set; }
        public float Scale { get; private set; }

        public NPC_Record()
        {
            Items = new List<NPCOData>();
            Spells = new List<string>();
        }


        #region Deprecated

        public class RNAMSubRecord : STRVSubRecord { }
        public class KNAMSubRecord : STRVSubRecord { }
        public class NPDTSubRecord : SubRecord
        {
            public short level;
            public byte strength;
            public byte intelligence;
            public byte willpower;
            public byte agility;
            public byte speed;
            public byte endurance;
            public byte personality;
            public byte luck;
            public byte[] skills;//[27];
            public byte reputation;
            public short health;
            public short spellPts;
            public short fatigue;
            public byte disposition;
            public byte factionID;
            public byte rank;
            public byte unknown1;
            public int gold;
            public byte version;

            // 12 byte version
            //public short level;
            //public byte disposition;
            //public byte factionID;
            //public byte rank;
            //public byte unknown1;
            public byte unknown2;
            public byte unknown3;
            //public long gold;

            public override void DeserializeData(UnityBinaryReader reader, uint dataSize)
            {
                if (dataSize == 52)
                {
                    level = reader.ReadLEInt16();
                    strength = reader.ReadByte();
                    intelligence = reader.ReadByte();
                    willpower = reader.ReadByte();
                    agility = reader.ReadByte();
                    speed = reader.ReadByte();
                    endurance = reader.ReadByte();
                    personality = reader.ReadByte();
                    luck = reader.ReadByte();
                    skills = reader.ReadBytes(26);
                    reputation = reader.ReadByte();
                    health = reader.ReadLEInt16();
                    spellPts = reader.ReadLEInt16();
                    fatigue = reader.ReadLEInt16();
                    disposition = reader.ReadByte();
                    factionID = reader.ReadByte();
                    rank = reader.ReadByte();
                    unknown1 = reader.ReadByte();
                    gold = reader.ReadLEInt32();
                    version = reader.ReadByte();
                }
                else
                {
                    level = reader.ReadLEInt16();
                    disposition = reader.ReadByte();
                    factionID = reader.ReadByte();
                    rank = reader.ReadByte();
                    unknown1 = reader.ReadByte();
                    unknown2 = reader.ReadByte();
                    unknown3 = reader.ReadByte();
                    gold = reader.ReadLEInt32();
                }
            }
        }
        public class FLAGSubRecord : INTVSubRecord
        {
            public NPCFlags Flags => (NPCFlags)value;
        }
        public class NPCOSubRecord : SubRecord
        {
            public int count;
            public char[] name;

            public override void DeserializeData(UnityBinaryReader reader, uint dataSize)
            {
                count = reader.ReadLEInt32();
                var bytes = reader.ReadBytes(32);
                name = new char[32];

                for (int i = 0; i < 32; i++)
                    name[i] = System.Convert.ToChar(bytes[i]);
            }
        }

        public class NPCSSubRecord : SubRecord
        {
            public char[] name;

            public override void DeserializeData(UnityBinaryReader reader, uint dataSize)
            {
                var bytes = reader.ReadBytes(32);
                name = new char[32];

                for (int i = 0; i < 32; i++)
                    name[i] = System.Convert.ToChar(bytes[i]);
            }
        }

        public class AIDTSubRecord : SubRecord
        {
            public enum FlagsType
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

            public FlagsType Flags => (FlagsType)flags;

            public byte hello;
            public byte unknown1;
            public byte fight;
            public byte flee;
            public byte alarm;
            public byte unknown2;
            public byte unknown3;
            public byte unknown4;
            public int flags;

            public override void DeserializeData(UnityBinaryReader reader, uint dataSize)
            {
                hello = reader.ReadByte();
                unknown1 = reader.ReadByte();
                fight = reader.ReadByte();
                flee = reader.ReadByte();
                alarm = reader.ReadByte();
                unknown2 = reader.ReadByte();
                unknown3 = reader.ReadByte();
                unknown4 = reader.ReadByte();
                flags = reader.ReadLEInt32();
            }
        }

        public class AI_WSubRecord : SubRecord
        {
            public short distance;
            public short duration;
            public byte timeOfDay;
            public byte[] idle;
            public byte unknow;

            public override void DeserializeData(UnityBinaryReader reader, uint dataSize)
            {
                distance = reader.ReadLEInt16();
                duration = reader.ReadLEInt16();
                timeOfDay = reader.ReadByte();
                idle = reader.ReadBytes(8);
                unknow = reader.ReadByte();
            }
        }

        public class AI_TSubRecord : SubRecord
        {
            public float x;
            public float y;
            public float z;
            public float unknown;

            public Vector3 ToVector3() => new Vector3(x, y, z);

            public override void DeserializeData(UnityBinaryReader reader, uint dataSize)
            {
                x = reader.ReadLESingle();
                y = reader.ReadLESingle();
                z = reader.ReadLESingle();
                unknown = reader.ReadLESingle();
            }
        }

        public class AI_FSubRecord : SubRecord
        {
            public float x;
            public float y;
            public float z;
            public short duration;
            public char[] id;
            public float unknown;

            public Vector3 ToVector3() => new Vector3(x, y, z);

            public override void DeserializeData(UnityBinaryReader reader, uint dataSize)
            {
                x = reader.ReadLESingle();
                y = reader.ReadLESingle();
                z = reader.ReadLESingle();
                duration = reader.ReadLEInt16();

                var bytes = reader.ReadBytes(32);
                id = new char[32];

                for (int i = 0; i < 32; i++)
                    id[i] = System.Convert.ToChar(bytes[i]);

                unknown = reader.ReadLESingle();
            }
        }

        public class AI_ESubRecord : SubRecord
        {
            public float x;
            public float y;
            public float z;
            public short duration;
            public char[] id;
            public float unknown;

            public Vector3 ToVector3() => new Vector3(x, y, z);

            public override void DeserializeData(UnityBinaryReader reader, uint dataSize)
            {
                x = reader.ReadLESingle();
                y = reader.ReadLESingle();
                z = reader.ReadLESingle();
                duration = reader.ReadLEInt16();

                var bytes = reader.ReadBytes(32);
                id = new char[32];

                for (int i = 0; i < 32; i++)
                    id[i] = System.Convert.ToChar(bytes[i]);

                unknown = reader.ReadLESingle();
            }
        }

        public class CNDTSubRecord : STRVSubRecord { }

        public class AI_ASubRecord : SubRecord
        {
            public char[] name;
            public byte unknown;

            public override void DeserializeData(UnityBinaryReader reader, uint dataSize)
            {
            }
        }

        public class DODTSubRecord : SubRecord
        {
            public float xPos;
            public float yPos;
            public float zPos;
            public float xRot;
            public float yRot;
            public float zRot;

            public Vector3 ToVector3() => new Vector3(xPos, yPos, zPos);
            public Quaternion ToQuaternion() => Quaternion.Euler(xRot, yRot, zRot);

            public override void DeserializeData(UnityBinaryReader reader, uint dataSize)
            {
                xPos = reader.ReadLESingle();
                yPos = reader.ReadLESingle();
                zPos = reader.ReadLESingle();
                xRot = reader.ReadLESingle();
                yRot = reader.ReadLESingle();
                zRot = reader.ReadLESingle();
            }
        }

        public class DNAMSubRecord : STRVSubRecord { }

        public class XSCLSubRecord : FLTVSubRecord { }

        /// <summary>
        /// NPC ID
        /// </summary>
        public NAMESubRecord NAME;

        /// <summary>
        /// NPC Name
        /// </summary>
        public FNAMSubRecord FNAM;

        /// <summary>
        /// NPC Animation
        /// </summary>
        public MODLSubRecord MODL;

        /// <summary>
        /// Race Name
        /// </summary>
        public RNAMSubRecord RNAM;

        /// <summary>
        /// Faction Name
        /// </summary>
        public ANAMSubRecord ANAM;

        /// <summary>
        /// Head Model
        /// </summary>
        public BNAMSubRecord BNAM;

        /// <summary>
        /// Class Name
        /// </summary>
        public CNAMSubRecord CNAM;

        /// <summary>
        /// Hear Model
        /// </summary>
        public KNAMSubRecord KNAM;

        /// <summary>
        /// NPC Data
        /// </summary>
        public NPDTSubRecord NPDT;

        /// <summary>
        /// NPC Flags
        /// </summary>
        public FLAGSubRecord FLAG;

        /// <summary>
        /// NPC Item
        /// </summary>
        public NPCOSubRecord NPCO;

        /// <summary>
        /// NPC AI DATA
        /// </summary>
        public AIDTSubRecord AIDT;

        /// <summary>
        /// NPC AI Bytes
        /// </summary>
        public AI_WSubRecord AI_W;

        /// <summary>
        /// NPC AI Travel
        /// </summary>
        public AI_TSubRecord AI_T;

        /// <summary>
        /// NPC Follow
        /// </summary>
        public AI_FSubRecord AI_F;

        /// <summary>
        /// NPC Escort
        /// </summary>
        public AI_ESubRecord AI_E;

        /// <summary>
        /// Cell escort/follow
        /// </summary>
        public CNDTSubRecord CNDT;

        /// <summary>
        /// AI Activate
        /// </summary>
        public AI_ASubRecord AI_A;

        /// <summary>
        /// Cell Travel Destination
        /// </summary>
        public DODTSubRecord DODT;

        /// <summary>
        /// Cell Name for previous DODT, if interior
        /// </summary>
        public DNAMSubRecord DNAM;

        /// <summary>
        /// Scale
        /// </summary>
        public XSCLSubRecord XSCL;

        public override bool NewFetchMethod => false;

        public override void DeserializeSubRecord(UnityBinaryReader reader, string subRecordName, uint dataSize)
        {
            throw new System.NotImplementedException();
        }

        public override SubRecord CreateUninitializedSubRecord(string subRecordName, uint dataSize)
        {
            switch (subRecordName)
            {
                case "NAME":
                    NAME = new NAMESubRecord();
                    return NAME;
                case "FNAM":
                    FNAM = new FNAMSubRecord();
                    return FNAM;
                case "MODL":
                    MODL = new MODLSubRecord();
                    return MODL;
                case "RNAM":
                    RNAM = new RNAMSubRecord();
                    return RNAM;
                case "ANAM":
                    ANAM = new ANAMSubRecord();
                    return ANAM;
                case "BNAM":
                    BNAM = new BNAMSubRecord();
                    return BNAM;
                case "CNAM":
                    CNAM = new CNAMSubRecord();
                    return CNAM;
                case "KNAM":
                    KNAM = new KNAMSubRecord();
                    return KNAM;
                case "NPDT":
                    NPDT = new NPDTSubRecord();
                    return NPDT;
                case "FLAG":
                    FLAG = new FLAGSubRecord();
                    return FLAG;
                case "NPCO":
                    NPCO = new NPCOSubRecord();
                    return NPCO;
                case "AIDT":
                    AIDT = new AIDTSubRecord();
                    return AIDT;
                case "AI_W":
                    AI_W = new AI_WSubRecord();
                    return AI_W;
                //case "AI_T":
                //AI_T = new AI_TSubRecord();
                //return AI_T;
                //case "AI_F":
                //AI_F = new AI_FSubRecord();
                //return AI_F;
                case "AI_E":
                    AI_E = new AI_ESubRecord();
                    return AI_E;
                case "CNDT":
                    CNDT = new CNDTSubRecord();
                    return CNDT;
                case "AI_A":
                    AI_A = new AI_ASubRecord();
                    return AI_A;
                case "DODT":
                    DODT = new DODTSubRecord();
                    return DODT;
                case "DNAM":
                    DNAM = new DNAMSubRecord();
                    return DNAM;
                case "XSCL":
                    XSCL = new XSCLSubRecord();
                    return XSCL;
            }

            return null;
        }

        #endregion
    }
}
