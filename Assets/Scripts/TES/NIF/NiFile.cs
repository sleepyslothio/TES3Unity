namespace TESUnity.NIF
{
    public class NiFile
    {
        public string name;
        public NiHeader header;
        public NiObject[] blocks;
        public NiFooter footer;

        public NiFile(string filename)
        {
            name = filename;
        }

        public void Deserialize(UnityBinaryReader reader)
        {
            header = new NiHeader();
            header.Deserialize(reader);

            blocks = new NiObject[header.numBlocks];
            for (int i = 0; i < header.numBlocks; i++)
            {
                blocks[i] = NiReaderUtils.ReadNiObject(reader);
            }

            footer = new NiFooter();
            footer.Deserialize(reader);
        }
    }
}
