using System.Collections.Generic;
using System.Text;

namespace TESUnity.ESM.ESS
{
    public sealed class GAMERecord : Record
    {
        public string CellName { get; private set; }

        public override void DeserializeSubRecord(UnityBinaryReader reader, string subRecordName, uint dataSize)
        {
            if (subRecordName == "GMDT")
            {
                var content = reader.ReadUTF8String((int)dataSize);
                var list = new List<char>();

                foreach (var c in content)
                {
                    if (c != '\0')
                    {
                        list.Add(c);
                    }
                    else
                    {
                        break;
                    }
                }

                CellName = Convert.CharToString(list.ToArray());
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
