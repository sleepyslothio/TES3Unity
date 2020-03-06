namespace TESUnity.ESM.Records
{
    public struct TESVector3
    {
        public float X;
        public float Y;
        public float Z;
    }

    public struct LandVertexNormalData
    {
        public TESVector3[] Data;
    }

    public struct LandVertexHeightFieldData
    {
        public float ReferenceHeight;
        public sbyte[] HeightOffsets;
    }

    public struct LandHeightLODData
    {
        public byte[] Data;
    }

    public struct LandVertexColorData
    {
        public TESVector3[] Data;
    }

    public struct LandVertexIndiceData
    {
        public ushort[] TextureIndices;
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

        public override bool NewFetchMethod => false;

        public Vector2i GridCoords { get; private set; }

        public override void DeserializeSubRecord(UnityBinaryReader reader, string subRecordName, uint dataSize)
        {
            if (subRecordName == "INTV")
            {

            }
            else if (subRecordName == "DATA")
            {

            }
            else if (subRecordName == "VNML")
            {

            }
            else if (subRecordName == "VHGT")
            {

            }
            else if (subRecordName == "WNAM")
            {

            }
            else if (subRecordName == "VCLR")
            {

            }
            else if (subRecordName == "VTEX")
            {

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
}
