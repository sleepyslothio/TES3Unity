namespace TESUnity.ESM
{
    public class DIALRecord : Record
    {
        public enum DialogueTopic
        {
            RegularTopic = 0,
            Voice = 1,
            Greeting = 2,
            Persuasion = 3,
            Journal = 4
        }

        public NAMESubRecord NAME;
        public ByteSubRecord DATA;

        public DialogueTopic Topic => (DialogueTopic)DATA.value;

        public override SubRecord CreateUninitializedSubRecord(string subRecordName, uint dataSize)
        {
            if (subRecordName == "NAME")
            {
                NAME = new NAMESubRecord();
                return NAME;
            }
            else if (subRecordName == "DATA")
            {
                DATA = new ByteSubRecord();
                return DATA;
            }

            return null;
        }
    }
}
