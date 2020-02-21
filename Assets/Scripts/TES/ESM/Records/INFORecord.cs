namespace TESUnity.ESM
{
    public class INFORecord : Record
    {
        public override SubRecord CreateUninitializedSubRecord(string subRecordName, uint dataSize)
        {
            return null;
        }
    }
}
