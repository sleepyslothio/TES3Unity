using System;
using System.IO;
using TESUnity.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TESUnity
{
    public class TESManager : MonoBehaviour
    {
        public const string Version = "0.10.0";
        public const float NormalMapGeneratorIntensity = 0.75f;
        public static TESManager instance;
        public static MorrowindDataReader MWDataReader { get; set; }

        private MorrowindEngine m_MorrowindEngine = null;
        private MusicPlayer m_MusicPlayer = null;
        private UIManager m_UIManager;

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

        [Header("Prefabs")]
        public GameObject playerPrefab;
        public GameObject waterPrefab;

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

            m_UIManager = FindObjectOfType<UIManager>();
            if (m_UIManager == null)
                throw new UnityException("UI Manager is missing");

            CellManager.cellRadius = config.CellRadius;
            CellManager.detailRadius = config.CellDetailRadius;
            MorrowindEngine.cellRadiusOnLoad = config.CellRadiusOnLoad;
            MorrowindEngine.desiredWorkTimePerFrame = desiredWorkTimePerFrame;

            if (MWDataReader == null)
                MWDataReader = new MorrowindDataReader(dataPath);

            m_MorrowindEngine = new MorrowindEngine(MWDataReader, m_UIManager);
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

            m_MorrowindEngine.SpawnPlayerOutside(playerPrefab, new Vector2i(-2, -9), new Vector3(-137.94f, 2.30f, -1037.6f));
        }

        private void OnApplicationQuit()
        {
            MWDataReader?.Close();
        }

        private void Update()
        {
            MorrowindEngine.desiredWorkTimePerFrame = desiredWorkTimePerFrame;
            m_MorrowindEngine.Update();
            m_MusicPlayer.Update();
        }

#if UNITY_EDITOR
        private void TestAllCells(string resultsFilePath)
        {
            using (StreamWriter writer = new StreamWriter(resultsFilePath))
            {
                foreach (var record in MWDataReader.MorrowindESMFile.GetRecordsOfType<ESM.CELLRecord>())
                {
                    var CELL = (ESM.CELLRecord)record;

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