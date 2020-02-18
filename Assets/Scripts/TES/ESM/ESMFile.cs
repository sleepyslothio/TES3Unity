using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace TESUnity.ESM
{
    public class ESMFile : IDisposable
    {
        private const int recordHeaderSizeInBytes = 16;

        /* Public */
        public Record[] records;
        public Dictionary<Type, List<Record>> recordsByType;
        public Dictionary<string, Record> objectsByIDString;
        public Dictionary<Vector2i, CELLRecord> exteriorCELLRecordsByIndices;
        public Dictionary<Vector2i, LANDRecord> LANDRecordsByIndices;

        public ESMFile(string filePath)
        {
            recordsByType = new Dictionary<Type, List<Record>>();
            objectsByIDString = new Dictionary<string, Record>();
            exteriorCELLRecordsByIndices = new Dictionary<Vector2i, CELLRecord>();
            LANDRecordsByIndices = new Dictionary<Vector2i, LANDRecord>();

            ReadRecords(filePath);
            PostProcessRecords();
        }

        public void Dispose()
        {
            Close();
        }

        public void Close() 
        {
        
        }

        public List<Record> GetRecordsOfType<T>() where T : Record
        {
            if (recordsByType.TryGetValue(typeof(T), out List<Record> records))
            {
                return records;
            }

            return null;
        }

        private Record CreateUninitializedRecord(string recordName)
        {
            switch (recordName)
            {
                case "TES3":
                    return new TES3Record();
                case "GMST":
                    return new GMSTRecord();
                case "GLOB":
                    return new GLOBRecord();
                case "SOUN":
                    return new SOUNRecord();
                case "REGN":
                    return new REGNRecord();
                case "LTEX":
                    return new LTEXRecord();
                case "STAT":
                    return new STATRecord();
                case "DOOR":
                    return new DOORRecord();
                case "MISC":
                    return new MISCRecord();
                case "WEAP":
                    return new WEAPRecord();
                case "CONT":
                    return new CONTRecord();
                case "LIGH":
                    return new LIGHRecord();
                case "ARMO":
                    return new ARMORecord();
                case "CLOT":
                    return new CLOTRecord();
                case "REPA":
                    return new REPARecord();
                case "ACTI":
                    return new ACTIRecord();
                case "APPA":
                    return new APPARecord();
                case "LOCK":
                    return new LOCKRecord();
                case "PROB":
                    return new PROBRecord();
                case "INGR":
                    return new INGRRecord();
                case "BOOK":
                    return new BOOKRecord();
                case "ALCH":
                    return new ALCHRecord();
                case "CELL":
                    return new CELLRecord();
                case "LAND":
                    return new LANDRecord();
                case "CREA":
                    return TESManager.instance.loadCreatures ? new CREARecord() : null;
                case "NPC_":
                    return TESManager.instance.loadNPCs ? new NPC_Record() : null;
                default:
                    Debug.LogWarning("Unsupported ESM record type: " + recordName);
                    return null;
            }
        }

        private void ReadRecords(string filePath)
        {
            var reader = new UnityBinaryReader(File.Open(filePath, FileMode.Open, FileAccess.Read));
            var recordList = new List<Record>();

            while (reader.BaseStream.Position < reader.BaseStream.Length)
            {
                var recordStartStreamPosition = reader.BaseStream.Position;

                var recordHeader = new RecordHeader();
                recordHeader.Deserialize(reader);

                var recordName = recordHeader.name;
                var record = CreateUninitializedRecord(recordName);

                // Read or skip the record.
                if (record != null)
                {
                    record.header = recordHeader;

                    var recordDataStreamPosition = reader.BaseStream.Position;
                    record.DeserializeData(reader);

                    if (reader.BaseStream.Position != (recordDataStreamPosition + record.header.dataSize))
                    {
                        throw new FormatException("Failed reading " + recordName + " record at offset " + recordStartStreamPosition + " in " + filePath);
                    }

                    recordList.Add(record);
                }
                else
                {
                    // Skip the record.
                    reader.BaseStream.Position += recordHeader.dataSize;

                    recordList.Add(null);
                }
            }

            records = recordList.ToArray();
        }

        private void PostProcessRecords()
        {
            foreach (var record in records)
            {
                if (record == null)
                {
                    continue;
                }

                // Add the record to the list for it's type.
                var recordType = record.GetType();
                List<Record> recordsOfSameType;

                if (recordsByType.TryGetValue(recordType, out recordsOfSameType))
                {
                    recordsOfSameType.Add(record);
                }
                else
                {
                    recordsOfSameType = new List<Record>();
                    recordsOfSameType.Add(record);

                    recordsByType.Add(recordType, recordsOfSameType);
                }

                // Add the record to the object dictionary if applicable.
                if (record is GMSTRecord)
                {
                    objectsByIDString.Add(((GMSTRecord)record).NAME.value, record);
                }
                else if (record is GLOBRecord)
                {
                    objectsByIDString.Add(((GLOBRecord)record).NAME.value, record);
                }
                else if (record is SOUNRecord)
                {
                    objectsByIDString.Add(((SOUNRecord)record).NAME.value, record);
                }
                else if (record is REGNRecord)
                {
                    objectsByIDString.Add(((REGNRecord)record).NAME.value, record);
                }
                else if (record is LTEXRecord)
                {
                    objectsByIDString.Add(((LTEXRecord)record).NAME.value, record);
                }
                else if (record is STATRecord)
                {
                    objectsByIDString.Add(((STATRecord)record).NAME.value, record);
                }
                else if (record is DOORRecord)
                {
                    objectsByIDString.Add(((DOORRecord)record).NAME.value, record);
                }
                else if (record is MISCRecord)
                {
                    objectsByIDString.Add(((MISCRecord)record).NAME.value, record);
                }
                else if (record is WEAPRecord)
                {
                    objectsByIDString.Add(((WEAPRecord)record).NAME.value, record);
                }
                else if (record is CONTRecord)
                {
                    objectsByIDString.Add(((CONTRecord)record).NAME.value, record);
                }
                else if (record is LIGHRecord)
                {
                    objectsByIDString.Add(((LIGHRecord)record).NAME.value, record);
                }
                else if (record is ARMORecord)
                {
                    objectsByIDString.Add(((ARMORecord)record).NAME.value, record);
                }
                else if (record is CLOTRecord)
                {
                    objectsByIDString.Add(((CLOTRecord)record).NAME.value, record);
                }
                else if (record is REPARecord)
                {
                    objectsByIDString.Add(((REPARecord)record).NAME.value, record);
                }
                else if (record is ACTIRecord)
                {
                    objectsByIDString.Add(((ACTIRecord)record).NAME.value, record);
                }
                else if (record is APPARecord)
                {
                    objectsByIDString.Add(((APPARecord)record).NAME.value, record);
                }
                else if (record is LOCKRecord)
                {
                    objectsByIDString.Add(((LOCKRecord)record).NAME.value, record);
                }
                else if (record is PROBRecord)
                {
                    objectsByIDString.Add(((PROBRecord)record).NAME.value, record);
                }
                else if (record is INGRRecord)
                {
                    objectsByIDString.Add(((INGRRecord)record).NAME.value, record);
                }
                else if (record is BOOKRecord)
                {
                    objectsByIDString.Add(((BOOKRecord)record).NAME.value, record);
                }
                else if (record is ALCHRecord)
                {
                    objectsByIDString.Add(((ALCHRecord)record).NAME.value, record);
                }
                else if (record is CREARecord)
                {
                    objectsByIDString.Add(((CREARecord)record).NAME.value, record);
                }
                else if (record is NPC_Record)
                {
                    objectsByIDString.Add(((NPC_Record)record).NAME.value, record);
                }

                // Add the record to exteriorCELLRecordsByIndices if applicable.
                if (record is CELLRecord)
                {
                    var CELL = (CELLRecord)record;

                    if (!CELL.isInterior)
                    {
                        exteriorCELLRecordsByIndices[CELL.gridCoords] = CELL;
                    }
                }

                // Add the record to LANDRecordsByIndices if applicable.
                if (record is LANDRecord)
                {
                    var LAND = (LANDRecord)record;

                    LANDRecordsByIndices[LAND.gridCoords] = LAND;
                }
            }
        }
    }
}