using System;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using TES3Unity.ESM.Records;
using Demonixis.Toolbox.XR;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace TES3Unity
{
    [RequireComponent(typeof(TES3Engine))]
    public class TES3Manager : MonoBehaviour
    {
        public const string Version = "2020.1";
        public const float NormalMapGeneratorIntensity = 0.75f;
        public static TES3Manager instance;
        public static TES3DataReader MWDataReader { get; set; }

        private TES3Engine m_MorrowindEngine = null;

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

        public TES3Engine Engine => m_MorrowindEngine;
        public TextureManager TextureManager => m_MorrowindEngine.textureManager;

        public static TES3Manager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindObjectOfType<TES3Manager>();
                }

                return instance;
            }
        }

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
            TES3Engine.cellRadiusOnLoad = config.CellRadiusOnLoad;
            TES3Engine.desiredWorkTimePerFrame = desiredWorkTimePerFrame;

            // When loaded from the Menu, this variable is already preloaded.
            if (MWDataReader == null)
            {
                MWDataReader = new TES3DataReader(dataPath);
            }

            m_MorrowindEngine = GetComponent<TES3Engine>();
            m_MorrowindEngine.Initialize(MWDataReader);

            var soundManager = FindObjectOfType<SoundManager>();
            soundManager?.Initialize(dataPath);

            // Start Position
            var cellGridCoords = new Vector2i(-2, -9);
            var cellIsInterior = false;
            var spawnPosition = new Vector3(-137.94f, 2.30f, -1037.6f);
            var spawnRotation = Quaternion.identity;

#if UNITY_EDITOR
            // Check for a previously saved game.
            if (loadSaveGameFilename != null)
            {
                var path = $"{MWDataReader.FolderPath}\\Saves\\{loadSaveGameFilename}.ess";

                if (File.Exists(path))
                {
                    var ess = new ESS.ESSFile(path);

                    ess.FindStartLocation(out string cellName, out float[] pos, out float[] rot);
                    // TODO: Find the correct grid/cell from these data.
                    //TES3Manager.MWDataReader.FindExteriorCellRecord(TES3Engine.Instance.cellManager.GetExteriorCellIndices(doorData.doorExitPos));
                }
            }
#endif

            if (GameSettings.IsMobile() && !XRManager.IsXREnabled())
            {
                var touchPrefab = Resources.Load<GameObject>("Input/TouchJoysticks");
                Instantiate(touchPrefab, Vector3.zero, Quaternion.identity);
            }

            m_MorrowindEngine.SpawnPlayer(cellGridCoords, cellIsInterior, spawnPosition, spawnRotation);
        }

        private void OnApplicationQuit()
        {
            MWDataReader?.Close();
        }

#if UNITY_EDITOR
        private void Update()
        {
            TES3Engine.desiredWorkTimePerFrame = desiredWorkTimePerFrame;
        }

        private void OnValidate()
        {
            CellManager.cellRadius = CellRadius;
            CellManager.detailRadius = CellDetailRadius;
            TES3Engine.cellRadiusOnLoad = CellRadiusOnLoad;
        }

        [MenuItem("Morrowind Unity/Export Scripts")]
        private static void ExportScripts()
        {
            if (MWDataReader == null)
            {
                Debug.LogWarning("Morrowind Data are not yet loaded. It'll take some time to load. The editor will be freezed a bit...");

                var dataPath = GameSettings.GetDataPath();

                var manager = FindObjectOfType<TES3Manager>();
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

                MWDataReader = new TES3DataReader(dataPath);

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
                        m_MorrowindEngine.m_TemporalLoadBalancer.WaitForTask(cellInfo.objectsCreationCoroutine);

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