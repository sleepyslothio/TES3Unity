using Demonixis.Toolbox.XR;
using System;
using System.IO;
using TES3Unity.Components;
using TES3Unity.Components.Records;
using TES3Unity.ESM.Records;
using TES3Unity.Rendering;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TES3Unity
{
    public sealed class TES3Engine : MonoBehaviour
    {
        // Static.
        public const string Version = "2020.1";
        public const float NormalMapGeneratorIntensity = 0.75f;
        public static int MarkerLayer => LayerMask.NameToLayer("Marker");
        public static int CellRadiusOnLoad = 2;
        public static bool AutoLoadSavedGame = false;
        private static TES3Engine instance = null;
        public static TES3DataReader DataReader { get; set; }

        private TemporalLoadBalancer m_TemporalLoadBalancer;
        private TES3Material m_MaterialManager;
        private NIFManager m_NIFManager;

        [Header("Global")]
        public float ambientIntensity = 1.5f;
        public float desiredWorkTimePerFrame = 0.0005f;

        [Header("Debug")]
        public string loadSaveGameFilename = string.Empty;

#if UNITY_EDITOR
        [Header("Editor Only")]
        public string[] AlternativeDataPaths = null;
        public int CellRadius = 1;
        public int CellDetailRadius = 1;
        public bool ForceAutoloadSavedGame = true;
#endif

        // Private.
        private CELLRecord m_CurrentCell;
        private Transform m_PlayerTransform;
        private Transform m_CameraTransform;
        private bool m_Initialized = false;

        // Public.
        public CellManager cellManager;
        public TextureManager textureManager;

        public CELLRecord CurrentCell
        {
            get => m_CurrentCell;
            private set
            {
                if (m_CurrentCell == value)
                {
                    return;
                }

                m_CurrentCell = value;
                CurrentCellChanged?.Invoke(m_CurrentCell);
            }
        }

        public static TES3Engine Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindObjectOfType<TES3Engine>();
                }

                return instance;
            }
        }

        public event Action<CELLRecord> CurrentCellChanged = null;

        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(this);
            }
            else
            {
                instance = this;
            }

            // When loaded from the Menu, this variable is already preloaded.
            if (DataReader == null)
            {
                var dataPath = GameSettings.GetDataPath();

#if UNITY_EDITOR
                // Load the game from the alternative dataPath when in editor.
                if (!GameSettings.IsValidPath(dataPath))
                {
                    dataPath = string.Empty;

                    foreach (var alt in AlternativeDataPaths)
                    {
                        if (GameSettings.IsValidPath(alt))
                        {
                            dataPath = alt;
                            GameSettings.SetDataPath(dataPath);
                            break;
                        }
                    }
                }

                if (ForceAutoloadSavedGame)
                {
                    AutoLoadSavedGame = true;
                }
#endif

                if (string.IsNullOrEmpty(dataPath))
                {
                    SceneManager.LoadScene("Menu");
                    enabled = false;
                    return;
                }

                DataReader = new TES3DataReader(dataPath);
            }
        }

        private void Start()
        {
            var config = GameSettings.Get();

#if UNITY_EDITOR
            CellRadius = config.CellRadius;
            CellDetailRadius = config.CellDetailRadius;
#endif

            CellManager.cellRadius = config.CellRadius;
            CellManager.detailRadius = config.CellDetailRadius;
            CellRadiusOnLoad = config.CellRadiusOnLoad;

            textureManager = new TextureManager(DataReader);
            m_MaterialManager = new TES3Material(textureManager, config.ShaderType, config.GenerateNormalMaps);
            m_NIFManager = new NIFManager(DataReader, m_MaterialManager);
            m_TemporalLoadBalancer = new TemporalLoadBalancer();
            cellManager = new CellManager(DataReader, textureManager, m_NIFManager, m_TemporalLoadBalancer);

            var sunLight = GameObjectUtils.CreateSunLight(Vector3.zero, Quaternion.Euler(new Vector3(50, 330, 0)));
            var weatherManager = FindObjectOfType<WeatherManager>();
            weatherManager.SetSun(sunLight);

            var waterPrefab = Resources.Load<GameObject>("Prefabs/WaterRP");
            GameObject.Instantiate(waterPrefab);

#if UNITY_STANDALONE
            if (!XRManager.IsXREnabled())
            {
                var texture = textureManager.LoadTexture("tx_cursor", true);
                Cursor.SetCursor(texture, Vector2.zero, CursorMode.Auto);
            }
#endif

            var uiCanvasPrefab = Resources.Load<GameObject>("Prefabs/GameUI");
            var uiCanvas = GameObject.Instantiate(uiCanvasPrefab);

            m_Initialized = true;

            var soundManager = FindObjectOfType<SoundManager>();
            soundManager?.Initialize(GameSettings.GetDataPath());

            // Start Position
            var cellGridCoords = new Vector2i(-2, -9);
            var cellIsInterior = false;
            var spawnPosition = new Vector3(-137.94f, 2.30f, -1037.6f);
            var spawnRotation = Quaternion.identity;

            if (AutoLoadSavedGame)
            {
                var save = TES3Save.Get();
                if (!save.IsEmpty())
                {
                    cellGridCoords = save.CellGrid;
                    cellIsInterior = save.IsInterior;
                    spawnPosition = save.Position;
                    spawnRotation = save.Rotation;
                    GameSettings.Get().Player = save.Data;
                }

                AutoLoadSavedGame = false;
            }

#if UNITY_EDITOR
            // Check for a previously saved game.
            if (loadSaveGameFilename != null)
            {
                var path = $"{DataReader.FolderPath}\\Saves\\{loadSaveGameFilename}.ess";

                if (File.Exists(path))
                {
                    var ess = new ESS.ESSFile(path);

                    ess.FindStartLocation(out string cellName, out float[] pos, out float[] rot);
                    // TODO: Find the correct grid/cell from these data.

                    var grid = TES3Engine.Instance.cellManager.GetExteriorCellIndices(new Vector3(pos[0], pos[1], pos[2]));
                    var exterior = DataReader.FindExteriorCellRecord(grid);
                    var interior = DataReader.FindInteriorCellRecord(cellName);
                }
            }
#endif

            if (GameSettings.IsMobile() && !XRManager.IsXREnabled())
            {
                var touchPrefab = Resources.Load<GameObject>("Input/TouchJoysticks");
                Instantiate(touchPrefab, Vector3.zero, Quaternion.identity);
            }

            SpawnPlayer(cellGridCoords, cellIsInterior, spawnPosition, spawnRotation);
        }

        #region Player Spawn

        /// <summary>
        /// Spawns the player inside. Be carefull, the name of the cell is not the same for each languages.
        /// Use it with the correct name.
        /// </summary>
        /// <param name="playerPrefab">The player prefab.</param>
        /// <param name="interiorCellName">The name of the desired cell.</param>
        /// <param name="position">The target position of the player.</param>
        public void SpawnPlayerInside(string interiorCellName, Vector3 position, Quaternion rotation)
        {
            CurrentCell = DataReader.FindInteriorCellRecord(interiorCellName);

            Debug.Assert(m_CurrentCell != null);

            CreatePlayer(position, rotation);

            var cellInfo = cellManager.StartCreatingInteriorCell(interiorCellName);
            m_TemporalLoadBalancer.WaitForTask(cellInfo.objectsCreationCoroutine);
        }

        public void SpawnPlayer(Vector2i gridCoords, bool outside, Vector3 position, Quaternion rotation)
        {
            InRangeCellInfo cellInfo = null;

            if (outside)
            {
                CurrentCell = DataReader.FindExteriorCellRecord(gridCoords);
                cellInfo = cellManager.StartCreatingExteriorCell(gridCoords);
            }
            else
            {
                CurrentCell = DataReader.FindInteriorCellRecord(gridCoords);
                cellInfo = cellManager.StartCreatingInteriorCell(gridCoords);
            }

            CreatePlayer(position, rotation);

            m_TemporalLoadBalancer.WaitForTask(cellInfo.objectsCreationCoroutine);
        }

        #endregion

        public void LateUpdate()
        {
            if (!m_Initialized)
            {
                return;
            }

            // The current cell can be null if the player is outside of the defined game world.
            if ((m_CurrentCell == null) || !m_CurrentCell.isInterior)
            {
                cellManager.UpdateExteriorCells(m_CameraTransform.position);
            }

            m_TemporalLoadBalancer.RunTasks(desiredWorkTimePerFrame);
        }

        private void OnApplicationQuit()
        {
            DataReader?.Close();
        }

        #region Private

        public void OpenDoor(Door component)
        {
            if (!component.doorData.leadsToAnotherCell)
            {
                component.Interact();
            }
            else
            {
                // The door leads to another cell, so destroy all currently loaded cells.
                cellManager.DestroyAllCells();

                // Move the player.
                m_PlayerTransform.position = component.doorData.doorExitPos;
                m_PlayerTransform.localEulerAngles = new Vector3(0, component.doorData.doorExitOrientation.eulerAngles.y, 0);

                // Load the new cell.
                CELLRecord newCell;

                if (component.doorData.leadsToInteriorCell)
                {
                    var cellInfo = cellManager.StartCreatingInteriorCell(component.doorData.doorExitName);
                    m_TemporalLoadBalancer.WaitForTask(cellInfo.objectsCreationCoroutine);

                    newCell = cellInfo.cellRecord;
                }
                else
                {
                    var cellIndices = cellManager.GetExteriorCellIndices(component.doorData.doorExitPos);
                    newCell = DataReader.FindExteriorCellRecord(cellIndices);

                    cellManager.UpdateExteriorCells(m_CameraTransform.position, true, CellRadiusOnLoad);
                }

                CurrentCell = newCell;
            }
        }

        private GameObject CreatePlayer(Vector3 position, Quaternion rotation)
        {
            var player = GameObject.FindWithTag("Player");
            if (player == null)
            {
                var playerPrefabPath = "Prefabs/Player";
                var playerPrefab = Resources.Load<GameObject>(playerPrefabPath);
                player = GameObject.Instantiate(playerPrefab);
                player.name = "Player";
            }

            m_PlayerTransform = player.transform;
            m_PlayerTransform.position = position;
            m_PlayerTransform.rotation = rotation;

            m_CameraTransform = player.GetComponentInChildren<Camera>().transform;

            return player;
        }

        #endregion

#if UNITY_EDITOR
        private void OnValidate()
        {
            CellManager.cellRadius = CellRadius;
            CellManager.detailRadius = CellDetailRadius;
        }
#endif
    }
}