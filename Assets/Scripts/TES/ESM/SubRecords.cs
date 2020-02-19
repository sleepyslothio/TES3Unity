using System;
using UnityEngine;

namespace TESUnity.ESM
{
    public class BYDTSubRecord : SubRecord
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

        public enum Flag
        {
            Female = 1, Playabe = 2
        }

        public enum BodyPartType
        {
            Skin = 0, Clothing = 1, Armor = 2
        }

        public byte part;
        public byte vampire;
        public byte flags;
        public byte partType;

        public override void DeserializeData(UnityBinaryReader reader, uint dataSize)
        {
            part = reader.ReadByte();
            vampire = reader.ReadByte();
            flags = reader.ReadByte();
            partType = reader.ReadByte();
        }
    }

    // TODO: implement DATA subrecord
    public class LANDRecord : Record
    {
        /*public class DATASubRecord : SubRecord
        {
            public override void DeserializeData(UnityBinaryReader reader) {}
        }*/

        public class VNMLSubRecord : SubRecord
        {
            // XYZ 8 bit floats

            public override void DeserializeData(UnityBinaryReader reader, uint dataSize)
            {
                var vertexCount = header.dataSize / 3;

                for (int i = 0; i < vertexCount; i++)
                {
                    var xByte = reader.ReadByte();
                    var yByte = reader.ReadByte();
                    var zByte = reader.ReadByte();
                }
            }
        }
        public class VHGTSubRecord : SubRecord
        {
            public float referenceHeight;
            public sbyte[] heightOffsets;

            public override void DeserializeData(UnityBinaryReader reader, uint dataSize)
            {
                referenceHeight = reader.ReadLESingle();

                var heightOffsetCount = header.dataSize - 4 - 2 - 1;
                heightOffsets = new sbyte[heightOffsetCount];

                for (int i = 0; i < heightOffsetCount; i++)
                {
                    heightOffsets[i] = reader.ReadSByte();
                }

                // unknown
                reader.ReadLEInt16();

                // unknown
                reader.ReadSByte();
            }
        }
        public class WNAMSubRecord : SubRecord
        {
            // Low-LOD heightmap (signed chars)

            public override void DeserializeData(UnityBinaryReader reader, uint dataSize)
            {
                var heightCount = header.dataSize;

                for (int i = 0; i < heightCount; i++)
                {
                    var height = reader.ReadByte();
                }
            }
        }
        public class VCLRSubRecord : SubRecord
        {
            // 24 bit RGB

            public override void DeserializeData(UnityBinaryReader reader, uint dataSize)
            {
                var vertexCount = header.dataSize / 3;

                for (int i = 0; i < vertexCount; i++)
                {
                    var rByte = reader.ReadByte();
                    var gByte = reader.ReadByte();
                    var bByte = reader.ReadByte();
                }
            }
        }
        public class VTEXSubRecord : SubRecord
        {
            public ushort[] textureIndices;

            public override void DeserializeData(UnityBinaryReader reader, uint dataSize)
            {
                var textureIndexCount = header.dataSize / 2;
                textureIndices = new ushort[textureIndexCount];

                for (int i = 0; i < textureIndexCount; i++)
                {
                    textureIndices[i] = reader.ReadLEUInt16();
                }
            }
        }

        public Vector2i gridCoords
        {
            get
            {
                return new Vector2i(INTV.value0, INTV.value1);
            }
        }

        public INTVTwoI32SubRecord INTV;
        //public DATASubRecord DATA;
        public VNMLSubRecord VNML;
        public VHGTSubRecord VHGT;
        public WNAMSubRecord WNAM;
        public VCLRSubRecord VCLR;
        public VTEXSubRecord VTEX;

        public override SubRecord CreateUninitializedSubRecord(string subRecordName, uint dataSize)
        {
            switch (subRecordName)
            {
                case "INTV":
                    INTV = new INTVTwoI32SubRecord();
                    return INTV;
                /*case "DATA":
                    DATA = new DATASubRecord();
                    return DATA;*/
                case "VNML":
                    VNML = new VNMLSubRecord();
                    return VNML;
                case "VHGT":
                    VHGT = new VHGTSubRecord();
                    return VHGT;
                case "WNAM":
                    WNAM = new WNAMSubRecord();
                    return WNAM;
                case "VCLR":
                    VCLR = new VCLRSubRecord();
                    return VCLR;
                case "VTEX":
                    VTEX = new VTEXSubRecord();
                    return VTEX;
                default:
                    return null;
            }
        }
    }

    // Common sub-records.
    public class STRVSubRecord : SubRecord
    {
        public string value;

        public override void DeserializeData(UnityBinaryReader reader, uint dataSize)
        {
            value = reader.ReadPossiblyNullTerminatedASCIIString((int)header.dataSize);
        }
    }

    // variable size
    public class INTVSubRecord : SubRecord
    {
        public long value;

        public override void DeserializeData(UnityBinaryReader reader, uint dataSize)
        {
            switch (header.dataSize)
            {
                case 1:
                    value = reader.ReadByte();
                    break;
                case 2:
                    value = reader.ReadLEInt16();
                    break;
                case 4:
                    value = reader.ReadLEInt32();
                    break;
                case 8:
                    value = reader.ReadLEInt64();
                    break;
                default:
                    throw new NotImplementedException("Tried to read an INTV subrecord with an unsupported size (" + header.dataSize.ToString() + ").");
            }
        }
    }
    public class INTVTwoI32SubRecord : SubRecord
    {
        public int value0, value1;

        public override void DeserializeData(UnityBinaryReader reader, uint dataSize)
        {
            Debug.Assert(header.dataSize == 8);

            value0 = reader.ReadLEInt32();
            value1 = reader.ReadLEInt32();
        }
    }
    public class INDXSubRecord : INTVSubRecord { }

    public class FLTVSubRecord : SubRecord
    {
        public float value;

        public override void DeserializeData(UnityBinaryReader reader, uint dataSize)
        {
            value = reader.ReadLESingle();
        }
    }

    public class ByteSubRecord : SubRecord
    {
        public byte value;

        public override void DeserializeData(UnityBinaryReader reader, uint dataSize)
        {
            value = reader.ReadByte();
        }
    }
    public class Int32SubRecord : SubRecord
    {
        public int value;

        public override void DeserializeData(UnityBinaryReader reader, uint dataSize)
        {
            value = reader.ReadLEInt32();
        }
    }
    public class UInt32SubRecord : SubRecord
    {
        public uint value;

        public override void DeserializeData(UnityBinaryReader reader, uint dataSize)
        {
            value = reader.ReadLEUInt32();
        }
    }

    public class NAMESubRecord : STRVSubRecord { }
    public class FNAMSubRecord : STRVSubRecord { }
    public class SNAMSubRecord : STRVSubRecord { }
    public class ANAMSubRecord : STRVSubRecord { }
    public class ITEXSubRecord : STRVSubRecord { }
    public class ENAMSubRecord : STRVSubRecord { }
    public class BNAMSubRecord : STRVSubRecord { }
    public class CNAMSubRecord : STRVSubRecord { }
    public class SCRISubRecord : STRVSubRecord { }
    public class SCPTSubRecord : STRVSubRecord { }
    public class MODLSubRecord : STRVSubRecord { }
    public class TEXTSubRecord : STRVSubRecord { }

    public class INDXBNAMCNAMGroup
    {
        public INDXSubRecord INDX;
        public BNAMSubRecord BNAM;
        public CNAMSubRecord CNAM;
    }
}
