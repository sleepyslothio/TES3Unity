using System;

namespace TESUnity.ESM
{
    public class RecordHeader
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
        // A flag that indicates if we use the new method or not.
        // This flag will be removed when the new system is finished.
        public virtual bool NewFetchMethod { get; }

        // Get ride of this when the new system is finished.
        public RecordHeader header;

        /// <summary>
        /// Return an uninitialized subrecord to deserialize, or null to skip.
        /// </summary>
        /// <returns>Return an uninitialized subrecord to deserialize, or null to skip.</returns>
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

        #region New API to deserialize SubRecords

        public virtual bool DeserializeSubRecord(UnityBinaryReader reader, string subRecordName, uint dataSize)
        {
            return false;
        }

        public void DeserializeDataNew(UnityBinaryReader reader)
        {
            var dataEndPos = reader.BaseStream.Position + header.dataSize;

            while (reader.BaseStream.Position < dataEndPos)
            {
                var subRecordName = reader.ReadASCIIString(4);
                var dataSize = reader.ReadLEUInt32();

                if (!DeserializeSubRecord(reader, subRecordName, dataSize))
                {
                    reader.BaseStream.Position += dataSize;
                }
            }
        }

        #endregion
    }
}