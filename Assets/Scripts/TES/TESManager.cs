using System;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using TESUnity.ESM;
using TESUnity.ESM.Records;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace TESUnity
{
    public class TESManager : MonoBehaviour
    {
        public const string Version = "2020.1";
        public const float NormalMapGeneratorIntensity = 0.75f;
        public static TESManager instance;
        public static MorrowindDataReader MWDataReader { get; set; }

        private MorrowindEngine m_MorrowindEngine = null;
        private MusicPlayer m_MusicPlayer = null;

        #region Inspector Members

#if UNITY_EDITOR
        [Header("Editor Only")]
        [SerializeField]
        private string[] m_AlternativeDataPaths = null;
#endif

        [Header("Global")]
        public bool logEnabled = false;
        public float ambientIntensity = 1.5f;
        public float desiredWorkTimePerFrame = 0.0005f;

        [Header("Debug")]
        public bool loadExtensions = false;
        public string loadSaveGameFilename = string.Empty;
#if UNITY_EDITOR
        public int CellRadius = 0;
        public int CellDetailRadius = 0;
        public int CellRadiusOnLoad = 0;
#endif
        #endregion

        public TextureManager TextureManager => m_MorrowindEngine.textureManager;

        private void Awake()
        {
            instance = this;
        }

        private void Start()
        {
            var config = GameSettings.Get();
            var dataPath = GameSettings.GetDataPath();

#if UNITY_EDITOR
            CellRadius = config.CellRadius;
            CellDetailRadius = config.CellDetailRadius;
            CellRadiusOnLoad = config.CellRadiusOnLoad;

            // Load the game from the alternative dataPath when in editor.
            if (!GameSettings.IsValidPath(dataPath))
            {
                dataPath = string.Empty;

                foreach (var alt in m_AlternativeDataPaths)
                {
                    if (GameSettings.IsValidPath(alt))
                        dataPath = alt;
                }
            }
#endif

            if (string.IsNullOrEmpty(dataPath))
            {
                SceneManager.LoadScene("Menu");
                enabled = false;
                return;
            }

            CellManager.cellRadius = config.CellRadius;
            CellManager.detailRadius = config.CellDetailRadius;
            MorrowindEngine.cellRadiusOnLoad = config.CellRadiusOnLoad;
            MorrowindEngine.desiredWorkTimePerFrame = desiredWorkTimePerFrame;

            // When loaded from the Menu, this variable is already preloaded.
            if (MWDataReader == null)
            {
                MWDataReader = new MorrowindDataReader(dataPath);
            }

            m_MorrowindEngine = new MorrowindEngine(MWDataReader);
            m_MusicPlayer = new MusicPlayer();

            if (config.MusicEnabled)
            {
                var songs = Directory.GetFiles(dataPath + "/Music/Explore");
                if (songs.Length > 0)
                {
                    foreach (var songFilePath in songs)
                    {
                        if (!songFilePath.Contains("Morrowind Title"))
                            m_MusicPlayer.AddSong(songFilePath);
                    }

                    m_MusicPlayer.Play();
                }
            }

            // Start Position
            var cellGridCoords = new Vector2i(-2, -9);
            var cellIsInterior = false;
            var playerSpawnPosition = new Vector3(-137.94f, 2.30f, -1037.6f);
            var playerSpawnRotation = Quaternion.identity;

            if (loadSaveGameFilename != null)
            {
                var path = $"{MWDataReader.FolderPath}\\Saves\\{loadSaveGameFilename}.ess";

                if (File.Exists(path))
                {
                    var ess = new ESS.ESSFile(path);

                    ess.FindStartLocation(out string location, out float[] pos, out float[] rot);
                    /*playerSpawnPosition = new Vector3(pos[0], pos[1], pos[2]);
                    cellGridCoords = m_MorrowindEngine.cellManager.GetExteriorCellIndices(playerSpawnPosition);

                    cellIsInterior = MWDataReader.FindInteriorCellRecord(cellGridCoords) != null;*/
                }
            }

            if (!cellIsInterior)
            {
                m_MorrowindEngine.SpawnPlayerOutside(cellGridCoords, playerSpawnPosition, playerSpawnRotation);
            }
            else
            {
                m_MorrowindEngine.SpawnPlayerInside(cellGridCoords, playerSpawnPosition, playerSpawnRotation);
            }
        }

        private void OnApplicationQuit()
        {
            MWDataReader?.Close();
        }

        private void Update()
        {
#if UNITY_EDITOR
            MorrowindEngine.desiredWorkTimePerFrame = desiredWorkTimePerFrame;
#endif
            m_MorrowindEngine.Update();
            m_MusicPlayer.Update();
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            CellManager.cellRadius = CellRadius;
            CellManager.detailRadius = CellDetailRadius;
            MorrowindEngine.cellRadiusOnLoad = CellRadiusOnLoad;
        }

        [MenuItem("Morrowind Unity/Export Scripts")]
        private static void ExportScripts()
        {
            if (MWDataReader == null)
            {
                Debug.LogWarning("Morrowind Data are not yet loaded. It'll take some time to load. The editor will be freezed a bit...");

                var dataPath = GameSettings.GetDataPath();

                var manager = FindObjectOfType<TESManager>();
                var alternativeDataPaths = manager?.m_AlternativeDataPaths ?? null;

                // Load the game from the alternative dataPath when in editor.
                if (!GameSettings.IsValidPath(dataPath))
                {
                    dataPath = string.Empty;

                    if (alternativeDataPaths == null)
                    {
                        Debug.LogError("No valid path was found. You can try to add a TESManager component on the scene with an alternative path.");
                        return;
                    }

                    foreach (var alt in alternativeDataPaths)
                    {
                        if (GameSettings.IsValidPath(alt))
                            dataPath = alt;
                    }
                }

                MWDataReader = new MorrowindDataReader(dataPath);

                Debug.Log("Morrowind Data are now loaded!");
            }

            var exportPath = $"Exports/Scripts";
            var scripts = MWDataReader.FindRecords<SCPTRecord>();

            if (scripts == null)
            {
                Debug.LogError("Can't retrieve scripts from the ESM file. There is probably a big problem...");
                return;
            }

            Debug.Log($"Exporting {scripts.Length} scripts into {exportPath}");

            if (!Directory.Exists(exportPath))
            {
                Directory.CreateDirectory(exportPath);
            }

            foreach (var script in scripts)
            {
                // We use the .vb extention because it's easier to read scripts with a text editor in basic language mode.
                File.WriteAllText($"{exportPath}/{script.Header.Name}.vb", script.Text);
            }

            Debug.Log("Script export done.");
        }

        private void TestAllCells(string resultsFilePath)
        {
            using (StreamWriter writer = new StreamWriter(resultsFilePath))
            {
                foreach (var record in MWDataReader.MorrowindESMFile.GetRecordsOfType<CELLRecord>())
                {
                    var CELL = (CELLRecord)record;

                    try
                    {
                        var cellInfo = m_MorrowindEngine.cellManager.StartInstantiatingCell(CELL);
                        m_MorrowindEngine.temporalLoadBalancer.WaitForTask(cellInfo.objectsCreationCoroutine);

                        DestroyImmediate(cellInfo.gameObject);

                        writer.Write("Pass: ");
                    }
                    catch (Exception)
                    {
                        writer.Write("Fail: ");
                    }

                    if (!CELL.isInterior)
                    {
                        writer.WriteLine(CELL.gridCoords.ToString());
                    }
                    else
                    {
                        writer.WriteLine(CELL.NAME.value);
                    }

                    writer.Flush();
                }
            }
        }
#endif
    }
}