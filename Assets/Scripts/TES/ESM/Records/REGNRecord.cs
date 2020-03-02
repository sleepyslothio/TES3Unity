using System.Collections.Generic;

namespace TESUnity.ESM
{
    public struct WeatherData
    {
        public byte clear;
        public byte cloudy;
        public byte foggy;
        public byte overcast;
        public byte rain;
        public byte thunder;
        public byte ash;
        public byte blight;
    }

    public struct MapColorData
    {
        public byte red;
        public byte green;
        public byte blue;
        public byte nullByte;
    }

    public struct SoundRecordData
    {
        public string Sound;
        public byte Chance;
    }

    public sealed class REGNRecord : Record, IIdRecord
    {
        public string Id { get; private set; }
        public string Name { get; private set; }
        public WeatherData Data { get; private set; }
        public string SleepCreature { get; private set; }
        public MapColorData MapColor { get; private set; }
        public List<SoundRecordData> Sounds { get; private set; }

        public REGNRecord()
        {
            Sounds = new List<SoundRecordData>();
        }

        public override void DeserializeSubRecord(UnityBinaryReader reader, string subRecordName, uint dataSize)
        {
            if (subRecordName == "NAME")
            {
                Id = reader.ReadPossiblyNullTerminatedASCIIString((int)dataSize);
            }
            else if (subRecordName == "FNAM")
            {
                Name = reader.ReadPossiblyNullTerminatedASCIIString((int)dataSize);
            }
            else if (subRecordName == "WEAT")
            {
                Data = new WeatherData
                {
                    clear = reader.ReadByte(),
                    cloudy = reader.ReadByte(),
                    foggy = reader.ReadByte(),
                    overcast = reader.ReadByte(),
                    rain = reader.ReadByte(),
                    thunder = reader.ReadByte(),
                    ash = reader.ReadByte(),
                    blight = reader.ReadByte()
                };

                // v1.3 ESM files add 2 bytes to WEAT subrecords.
                if (dataSize == 10)
                {
                    reader.ReadByte();
                    reader.ReadByte();
                }
            }
            else if (subRecordName == "BNAM")
            {
                SleepCreature = reader.ReadPossiblyNullTerminatedASCIIString((int)dataSize);
            }
            else if (subRecordName == "CNAM")
            {
                MapColor = new MapColorData
                {
                    red = reader.ReadByte(),
                    green = reader.ReadByte(),
                    blue = reader.ReadByte(),
                    nullByte = reader.ReadByte()
                };
            }
            else if (subRecordName == "SNAM")
            {
                var data = new SoundRecordData
                {
                    Sound = ReadStringFromByte(reader, 32),
                    Chance = reader.ReadByte()
                };

                Sounds.Add(data);
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
