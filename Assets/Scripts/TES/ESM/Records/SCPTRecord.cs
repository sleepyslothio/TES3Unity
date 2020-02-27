using System.Collections.Generic;

namespace TESUnity.ESM
{
    public class SCPTRecord : Record
    {
        public class SCHDSubRecord : SubRecord
        {
            public char[] CharName { get; private set; }
            public uint NumShorts { get; private set; }
            public uint NumLongs { get; private set; }
            public uint NumFloats { get; private set; }
            public uint ScriptDataSize { get; private set; }
            public uint LocalVarSize { get; private set; }

            public override void DeserializeData(UnityBinaryReader reader, uint dataSize)
            {
                var bytes = reader.ReadBytes(32);
                CharName = new char[32];

                for (int i = 0; i < 32; i++)
                    CharName[i] = System.Convert.ToChar(bytes[i]);

                NumShorts = reader.ReadLEUInt32();
                NumLongs = reader.ReadLEUInt32();
                NumFloats = reader.ReadLEUInt32();
                ScriptDataSize = reader.ReadLEUInt32();
                LocalVarSize = reader.ReadLEUInt32();
            }
        }

        public SCHDSubRecord SCHD;
        public NAMESubRecord SCVR;
        public ByteArraySubRecord SCDT;
        public NAMESubRecord SCTX;

        // TODO: Cache the result.
        public string GetName()
        {
            var charName = SCHD.CharName;
            var list = new List<char>();

            for (var i = 0; i < charName.Length; i++)
            {
                if (charName[i] != '\0')
                {
                    list.Add(charName[i]);
                }
            }

            return new string(list.ToArray());
        }

        public override SubRecord CreateUninitializedSubRecord(string subRecordName, uint dataSize)
        {
            if (subRecordName == "SCHD")
            {
                SCHD = new SCHDSubRecord();
                return SCHD;
            }
            else if (subRecordName == "SCVR")
            {
                SCVR = new NAMESubRecord();
                return SCVR;
            }
            else if (subRecordName == "SCDT")
            {
                SCDT = new ByteArraySubRecord();
                return SCDT;
            }
            else if (subRecordName == "SCTX")
            {
                SCTX = new NAMESubRecord();
                return SCTX;
            }

            return null;
        }
    }
}
