namespace TESUnity.ESM.ESS
{
    public sealed class GMDTRecord : Record
    {
        public float[] Unknowns { get; private set; }
        public string CellName { get; private set; }
        public float Unknown { get; private set; }
        public string CharacterName { get; private set; }

        public override void DeserializeSubRecord(UnityBinaryReader reader, string subRecordName, uint dataSize)
        {
            var s = "";
        }

        #region Deprecated
        public override bool NewFetchMethod => true;
        public override SubRecord CreateUninitializedSubRecord(string subRecordName, uint dataSize) => null;
        #endregion
    }
}
