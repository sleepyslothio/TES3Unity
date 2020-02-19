using System;
using System.Collections.Generic;
using System.IO;

namespace TESUnity
{
    using ESM;
    using System.Threading.Tasks;
    using UnityEngine;

    public class MorrowindDataReader : IDisposable
    {
        private string _dataFilePath;
        public ESMFile MorrowindESMFile;
        public BSAFile MorrowindBSAFile;
        public ESMFile BloodmoonESMFile;
        public BSAFile BloodmoonBSAFile;
        public ESMFile TribunalESMFile;
        public BSAFile TribunalBSAFile;

        public MorrowindDataReader(string dataFilePath)
        {
            _dataFilePath = dataFilePath;

            MorrowindESMFile = new ESMFile(dataFilePath + "/Morrowind.esm");
            MorrowindBSAFile = new BSAFile(dataFilePath + "/Morrowind.bsa");

            if (TESManager.instance.loadExtensions)
            {
                BloodmoonESMFile = new ESMFile(dataFilePath + "/Bloodmoon.esm");
                BloodmoonBSAFile = new BSAFile(dataFilePath + "/Bloodmoon.bsa");

                TribunalESMFile = new ESMFile(dataFilePath + "/Tribunal.esm");
                TribunalBSAFile = new BSAFile(dataFilePath + "/Tribunal.bsa");
            }
        }

        void IDisposable.Dispose()
        {
            Close();
        }

        ~MorrowindDataReader()
        {
            Close();
        }

        public void Close()
        {
            if (TESManager.instance.loadExtensions)
            {
                TribunalBSAFile.Close();
                TribunalESMFile.Close();

                BloodmoonBSAFile.Close();
                BloodmoonESMFile.Close();
            }

            MorrowindBSAFile.Close();
            MorrowindESMFile.Close();
        }

        public Task<Texture2DInfo> LoadTextureAsync(string texturePath)
        {
#if LOAD_LOCAL_DATA
            var localFilePath = FindLocalTexture(texturePath);
            if (localFilePath != null)
            {
                return LoadLocalTexture(localFilePath);
            }
#endif

            var bsaFilePath = FindTexture(MorrowindBSAFile, texturePath);
            if (bsaFilePath != null)
            {
                return LoadTextureAsync(MorrowindBSAFile, bsaFilePath);
            }
            else
            {
                Debug.LogWarning("Could not find file \"" + texturePath + "\" in a BSA file.");
                return Task.FromResult<Texture2DInfo>(null);
            }
        }

        private Task<Texture2DInfo> LoadLocalTexture(string filePath)
        {
            return Task.Run(() =>
            {
                var fileExtension = Path.GetExtension(filePath);

                if (fileExtension?.ToLower() == ".dds")
                {
                    var textureData = File.ReadAllBytes(filePath);
                    return DDS.DDSReader.LoadDDSTexture(new MemoryStream(textureData));
                }
                else
                {
                    throw new NotSupportedException($"Unsupported texture type: {fileExtension}");
                }
            });
        }

        private Task<Texture2DInfo> LoadTextureAsync(BSAFile bsaFile, string bsaFilePath)
        {
            var fileData = bsaFile.LoadFileData(bsaFilePath);

            return Task.Run(() =>
            {
                var fileExtension = Path.GetExtension(bsaFilePath);

                if (fileExtension?.ToLower() == ".dds")
                {
                    return DDS.DDSReader.LoadDDSTexture(new MemoryStream(fileData));
                }
                else
                {
                    throw new NotSupportedException($"Unsupported texture type: {fileExtension}");
                }
            });
        }

        public Task<NIF.NiFile> LoadNifAsync(string filePath)
        {
#if LOAD_LOCAL_DATA
            var localPath = Path.Combine(_dataFilePath, filePath);
            if (File.Exists(localPath))
            {
                return LoadLocalNifAsync(filePath, localPath);
            }
#endif
            return LoadNifAsync(MorrowindBSAFile, filePath);
        }

        private Task<NIF.NiFile> LoadNifAsync(BSAFile bsaFile, string filePath)
        {
            var fileData = bsaFile.LoadFileData(filePath);

            return Task.Run(() =>
            {
                var file = new NIF.NiFile(Path.GetFileNameWithoutExtension(filePath));
                file.Deserialize(new UnityBinaryReader(new MemoryStream(fileData)));

                return file;
            });
        }

        private Task<NIF.NiFile> LoadLocalNifAsync(string filePath, string localFilePath)
        {
            var fileData = File.ReadAllBytes(localFilePath);

            return Task.Run(() =>
            {
                var file = new NIF.NiFile(Path.GetFileNameWithoutExtension(filePath));
                file.Deserialize(new UnityBinaryReader(new MemoryStream(fileData)));

                return file;
            });
        }

        public LTEXRecord FindLTEXRecord(int index)
        {
            List<Record> records = MorrowindESMFile.GetRecordsOfType<LTEXRecord>();
            LTEXRecord LTEX = null;

            for (int i = 0, l = records.Count; i < l; i++)
            {
                LTEX = (LTEXRecord)records[i];

                if (LTEX.INTV.value == index)
                    return LTEX;
            }

            return null;
        }

        public LANDRecord FindLANDRecord(Vector2i cellIndices)
        {
            MorrowindESMFile.LANDRecordsByIndices.TryGetValue(cellIndices, out LANDRecord LAND);
            return LAND;
        }

        public CELLRecord FindExteriorCellRecord(Vector2i cellIndices)
        {
            MorrowindESMFile.ExteriorCELLRecordsByIndices.TryGetValue(cellIndices, out CELLRecord CELL);
            return CELL;
        }

        public CELLRecord FindInteriorCellRecord(string cellName)
        {
            List<Record> records = MorrowindESMFile.GetRecordsOfType<CELLRecord>();
            CELLRecord CELL = null;

            for (int i = 0, l = records.Count; i < l; i++)
            {
                CELL = (CELLRecord)records[i];

                if (CELL.NAME.value == cellName)
                    return CELL;
            }

            return null;
        }

        public CELLRecord FindInteriorCellRecord(Vector2i gridCoords)
        {
            List<Record> records = MorrowindESMFile.GetRecordsOfType<CELLRecord>();
            CELLRecord CELL = null;

            for (int i = 0, l = records.Count; i < l; i++)
            {
                CELL = (CELLRecord)records[i];

                if (CELL.gridCoords.x == gridCoords.x && CELL.gridCoords.y == gridCoords.y)
                    return CELL;
            }

            return null;
        }

        /// <summary>
        /// Finds the actual path of a texture.
        /// </summary>
        private string FindTexture(BSAFile bsaFile, string texturePath)
        {
            var textureName = Path.GetFileNameWithoutExtension(texturePath);
            var textureNameInTexturesDir = "textures/" + textureName;
            var texturePathWithoutExtension = Path.GetDirectoryName(texturePath) + '/' + textureName;

            var filePath = textureNameInTexturesDir + ".dds";
            if (bsaFile.ContainsFile(filePath))
            {
                return filePath;
            }

            filePath = textureNameInTexturesDir + ".tga";
            if (bsaFile.ContainsFile(filePath))
            {
                return filePath;
            }

            filePath = texturePathWithoutExtension + ".dds";
            if (bsaFile.ContainsFile(filePath))
            {
                return filePath;
            }

            filePath = texturePathWithoutExtension + ".tga";
            if (bsaFile.ContainsFile(filePath))
            {
                return filePath;
            }

            // Could not find the file.
            return null;
        }

        private string FindLocalTexture(string texturePath)
        {
            var textureName = Path.GetFileNameWithoutExtension(texturePath);
            var texturePathWithoutExtension = Path.Combine(_dataFilePath, "textures", textureName);
            var filePath = texturePathWithoutExtension + ".dds";

            if (File.Exists(filePath))
            {
                return filePath;
            }

            return null;
        }
    }
}