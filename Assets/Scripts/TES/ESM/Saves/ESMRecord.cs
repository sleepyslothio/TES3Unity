namespace TESUnity.ESM
{
    public sealed class ESSRecord : Record
    {
        public struct GMDTData
        {
            public float[] Unknowns;
            public string CellName;
            public float Unknown;
            public string CharacterName;
        }

        public struct SCPTData
        {
            public byte[] SLCS;
            public byte[] SLCD;
        }

        public struct QuestData
        {
            public string Name;
            public string Data;
        }

        public struct KillStatData
        {
            public string KNAM;
            public string CNAM;
        }

        public struct FMAPData
        {
            public int Size;
            public int Value;
            public byte[] MapData;
        }

        public struct PCDTData
        {
            public string DNAME;
            public string NMAM;
            public string PNAM;
            public string SNAM;
            public string NAM9;
        }

        public GMDTData GMDT;
        public byte[] SCRD;
        public byte[] SCRS;
        public SCPTData SCPT;
        public QuestData QUES;
        public string Journal;
        public KillStatData KLST;
        public FMAPData FMAP;
        public PCDTData PCDT;

        public ref GMDTData GMDTRef => ref GMDT;

        //public override bool NewFetchMethod => true;

        public override SubRecord CreateUninitializedSubRecord(string subRecordName, uint dataSize) => null;

        public override void DeserializeSubRecord(UnityBinaryReader reader, string subRecordName, uint dataSize)
        {
            if (subRecordName == "GMDT")
            {
                GMDT.Unknowns = new float[6];

                for (var i = 0; i < 6; i++)
                {
                    GMDT.Unknowns[i] = reader.ReadLESingle();
                }

                GMDT.CellName = reader.ReadASCIIString(64);
                GMDT.Unknown = reader.ReadLESingle();
                GMDT.CharacterName = reader.ReadASCIIString(32);
            }
            else if (subRecordName == "SCRD")
            {
                SCRD = reader.ReadBytes(20);
            }
            else if (subRecordName == "SCRS")
            {
                SCRS = reader.ReadBytes(65536);
            }
            else if (subRecordName == "QUES")
            {

            }
            else if (subRecordName == "JOUR")
            {

            }
            else if (subRecordName == "KLST")
            {

            }
            else if (subRecordName == "FMAP")
            {

            }
            else if (subRecordName == "PCDT")
            {

            }
            else
            {
                ReadMissingSubRecord(reader, subRecordName, dataSize);
            }
        }
    }
}
