using System.Collections.Generic;

namespace TESUnity.ESM
{
    public sealed class REGNRecord : Record
    {
        public class WEATSubRecord : SubRecord
        {
            public byte clear;
            public byte cloudy;
            public byte foggy;
            public byte overcast;
            public byte rain;
            public byte thunder;
            public byte ash;
            public byte blight;

            public override void DeserializeData(UnityBinaryReader reader, uint dataSize)
            {
                clear = reader.ReadByte();
                cloudy = reader.ReadByte();
                foggy = reader.ReadByte();
                overcast = reader.ReadByte();
                rain = reader.ReadByte();
                thunder = reader.ReadByte();
                ash = reader.ReadByte();
                blight = reader.ReadByte();

                // v1.3 ESM files add 2 bytes to WEAT subrecords.
                if (dataSize == 10)
                {
                    reader.ReadByte();
                    reader.ReadByte();
                }
            }
        }

        public class CNAMSubRecord : SubRecord
        {
            byte red;
            byte green;
            byte blue;
            byte nullByte;

            public override void DeserializeData(UnityBinaryReader reader, uint dataSize)
            {
                red = reader.ReadByte();
                green = reader.ReadByte();
                blue = reader.ReadByte();
                nullByte = reader.ReadByte();
            }
        }

        public class SNAMSubRecord : SubRecord
        {
            byte[] soundName;
            byte chance;

            public override void DeserializeData(UnityBinaryReader reader, uint dataSize)
            {
                soundName = reader.ReadBytes(32);
                chance = reader.ReadByte();
            }
        }

        public NAMESubRecord NAME;
        public FNAMSubRecord FNAM;
        public WEATSubRecord WEAT;
        public BNAMSubRecord BNAM;
        public CNAMSubRecord CNAM;
        public List<SNAMSubRecord> SNAMs = new List<SNAMSubRecord>();

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
                case "WEAT":
                    WEAT = new WEATSubRecord();
                    return WEAT;
                case "BNAM":
                    BNAM = new BNAMSubRecord();
                    return BNAM;
                case "CNAM":
                    CNAM = new CNAMSubRecord();
                    return CNAM;
                case "SNAM":
                    var SNAM = new SNAMSubRecord();

                    SNAMs.Add(SNAM);

                    return SNAM;
                default:
                    return null;
            }
        }
    }
}
