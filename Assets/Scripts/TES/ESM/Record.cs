using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace TESUnity.ESM
{
    public struct RecordHeader
    {
        public string Name;
        public uint DataSize;
        public uint Unknown0;
        public uint Flags;
    }

    public class RecordHeaderD
    {
        public string name; // 4 bytes
        public uint dataSize;
        public uint unknown0;
        public uint flags;

        public virtual void Deserialize(UnityBinaryReader reader)
        {
            name = reader.ReadASCIIString(4);
            dataSize = reader.ReadLEUInt32();
            unknown0 = reader.ReadLEUInt32();
            flags = reader.ReadLEUInt32();
        }
    }

    public class SubRecordHeader
    {
        public string name; // 4 bytes
        public uint dataSize;

        public virtual void Deserialize(UnityBinaryReader reader)
        {
            name = reader.ReadASCIIString(4);
            dataSize = reader.ReadLEUInt32();
        }
    }

    public abstract class SubRecord
    {
        public SubRecordHeader header;
        public abstract void DeserializeData(UnityBinaryReader reader, uint dataSize);
    }

    public abstract class Record
    {
        private static List<string> MissingRecordLogs = new List<string>();

        #region Deprecated
        public virtual bool NewFetchMethod { get; }
        public RecordHeaderD header;
        public abstract SubRecord CreateUninitializedSubRecord(string subRecordName, uint dataSize);

        public void DeserializeData(UnityBinaryReader reader)
        {
            var dataEndPos = reader.BaseStream.Position + header.dataSize;

            while (reader.BaseStream.Position < dataEndPos)
            {
                var subRecordStartStreamPosition = reader.BaseStream.Position;

                var subRecordHeader = new SubRecordHeader();
                subRecordHeader.Deserialize(reader);

                var subRecord = CreateUninitializedSubRecord(subRecordHeader.name, subRecordHeader.dataSize);

                // Read or skip the record.
                if (subRecord != null)
                {
                    subRecord.header = subRecordHeader;

                    var subRecordDataStreamPosition = reader.BaseStream.Position;
                    subRecord.DeserializeData(reader, subRecordHeader.dataSize);

                    if (reader.BaseStream.Position != (subRecordDataStreamPosition + subRecord.header.dataSize))
                    {
                        throw new FormatException("Failed reading " + subRecord.header.name + " subrecord at offset " + subRecordStartStreamPosition);
                    }
                }
                else
                {
                    reader.BaseStream.Position += subRecordHeader.dataSize;
                }
            }
        }

        #endregion

        #region New API to deserialize SubRecords

        public virtual void DeserializeSubRecord(UnityBinaryReader reader, string subRecordName, uint dataSize)
        {
        }

        public void ReadMissingSubRecord(UnityBinaryReader reader, string subRecordName, uint dataSize)
        {
            reader.BaseStream.Position += dataSize;

            var log = $"{GetType()} have missing subRecord: {subRecordName}";

            if (!MissingRecordLogs.Contains(log))
            {
                MissingRecordLogs.Add(log);

                if (TESManager.instance?.logEnabled ?? true)
                {
                    Debug.Log(log);
                }
            }
        }

        public void DeserializeDataNew(UnityBinaryReader reader)
        {
            var dataEndPos = reader.BaseStream.Position + header.dataSize;

            while (reader.BaseStream.Position < dataEndPos)
            {
                var subRecordName = reader.ReadASCIIString(4);
                var dataSize = reader.ReadLEUInt32();

                DeserializeSubRecord(reader, subRecordName, dataSize);
            }
        }

        public static long ReadIntRecord(UnityBinaryReader reader, uint dataSize)
        {
            if (dataSize == 1)
            {
                return reader.ReadByte();
            }
            else if (dataSize == 2)
            {
                return reader.ReadLEInt16();
            }
            else if (dataSize == 4)
            {
                return reader.ReadLEInt32();
            }
            else if (dataSize == 8)
            {
                return reader.ReadLEInt64();
            }

            reader.BaseStream.Position += dataSize;

            return 0;
        }

        public static float[] ReadDoubleArray(UnityBinaryReader reader, int size)
        {
            var array = new float[size];
            for (var i = 0; i < 4; i++)
            {
                array[i] = reader.ReadLESingle();
            }
            return array;
        }

        public static int[] ReadInt32Array(UnityBinaryReader reader, int size)
        {
            var array = new int[size];
            for (var i = 0; i < 4; i++)
            {
                array[i] = reader.ReadLEInt32();
            }
            return array;
        }

        public static string ReadStringFromChar(UnityBinaryReader reader, int size)
        {
            var bytes = reader.ReadBytes(size);
            var array = new char[size];

            for (var i = 0; i < size; i++)
                array[i] = System.Convert.ToChar(bytes[i]);

            return Convert.CharToString(array);
        }

        public static string ReadStringFromByte(UnityBinaryReader reader, int size)
        {
            var bytes = reader.ReadBytes(size);
            var str = Encoding.ASCII.GetString(bytes, 0, bytes.Length);
            return Convert.RemoveNullChar(str);
        }

        #endregion
    }
}