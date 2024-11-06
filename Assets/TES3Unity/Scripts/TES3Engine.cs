using System;
using System.IO;
using Demonixis.ToolboxV2;
using Demonixis.ToolboxV2.XR;
using TES3Unity.Components.Records;
using TES3Unity.ESM.Records;
using TES3Unity.ESS;
using TES3Unity.Rendering;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TES3Unity
{
    public sealed class Tes3Engine : MonoBehaviour
    {
        // Static.
        public const float NormalMapGeneratorIntensity = 0.75f;
        public static int MarkerLayer => LayerMask.NameToLayer("Marker");
        private static int CellRadiusOnLoad = 2;
        public static bool AutoLoadSavedGame;
        public static bool LogEnabled;
        private static Tes3Engine _instance;
        public static TES3DataReader DataReader { get; set; }

        private TemporalLoadBalancer _temporalLoadBalancer;
        private Tes3Material _materialManager;
        private NIFManager _nifManager;

        [Header("Global")] public float ambientIntensity = 1.5f;
        public float desiredWorkTimePerFrame = 0.0005f;

        [Header("Spawn")] [SerializeField] private PlayerPrefabData _playerPrefabData;
        [SerializeField] private GameObject _touchInterface;

#if UNITY_EDITOR
        [Header("Editor Only")] public string loadSaveGameFilename = string.Empty;
        public int CellRadius = 1;
        public int CellDetailRadius = 1;
#endif

        // Private.
        private CELLRecord m_CurrentCell;
        private Transform m_PlayerTransform;
        private Transform m_CameraTransform;
        private bool m_Initialized;

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

        public static Tes3Engine Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<Tes3Engine>();
                }

                return _instance;
            }
        }

        public event Action<CELLRecord> CurrentCellChanged;

        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(this);
            }
            else
            {
                _instance = this;
            }

            // When loaded from the Menu, this variable is already preloaded.
            if (DataReader == null)
            {
                var dataPath = GameSettings.GetDataPath();
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

            CellManager.CellRadius = config.CellRadius;
            CellManager.DetailRadius = config.CellDetailRadius;
            CellRadiusOnLoad = config.CellRadiusOnLoad;

            textureManager = new TextureManager(DataReader);
            _materialManager = new Tes3Material(textureManager, config.lowQualityShader);
            _nifManager = new NIFManager(DataReader, _materialManager);
            _temporalLoadBalancer = new TemporalLoadBalancer();
            cellManager = new CellManager(DataReader, textureManager, _nifManager, _temporalLoadBalancer);

#if UNITY_STANDALONE
            if (!XRManager.IsXREnabled())
            {
                var texture = textureManager.LoadTexture("tx_cursor", true);
                Cursor.SetCursor(texture, Vector2.zero, CursorMode.Auto);
            }
#endif
            m_Initialized = true;

            var soundManager = FindFirstObjectByType<SoundManager>();
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
                    GameSettings.Get().playerData = save.Data;
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
                    var ess = new ESSFile(path);

                    ess.FindStartLocation(out string cellName, out float[] pos, out float[] rot);
                    // TODO: Find the correct grid/cell from these data.

                    var grid = Instance.cellManager.GetExteriorCellIndices(new Vector3(pos[0], pos[1],
                        pos[2]));
                    var exterior = DataReader.FindExteriorCellRecord(grid);
                    var interior = DataReader.FindInteriorCellRecord(cellName);
                }
            }
#endif

            if (PlatformUtility.IsMobilePlatform() && !XRManager.IsXREnabled())
                Instantiate(_touchInterface, Vector3.zero, Quaternion.identity);

            SpawnPlayer(cellGridCoords, cellIsInterior, spawnPosition, spawnRotation);
        }

        #region Player Spawn

        private void SpawnPlayer(Vector2i gridCoords, bool outside, Vector3 position, Quaternion rotation)
        {
            InRangeCellInfo cellInfo;

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

            _temporalLoadBalancer.WaitForTask(cellInfo.ObjectsCreationCoroutine);
        }

        #endregion

        public void LateUpdate()
        {
            if (!m_Initialized) return;

            // The current cell can be null if the player is outside of the defined game world.
            if (m_CurrentCell == null || !m_CurrentCell.isInterior)
                cellManager.UpdateExteriorCells(m_CameraTransform.position);

            _temporalLoadBalancer.RunTasks(desiredWorkTimePerFrame);
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
                m_PlayerTransform.localEulerAngles =
                    new Vector3(0, component.doorData.doorExitOrientation.eulerAngles.y, 0);

                // Load the new cell.
                CELLRecord newCell;

                if (component.doorData.leadsToInteriorCell)
                {
                    var cellInfo = cellManager.StartCreatingInteriorCell(component.doorData.doorExitName);
                    _temporalLoadBalancer.WaitForTask(cellInfo.ObjectsCreationCoroutine);

                    newCell = cellInfo.CellRecord;
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

        private void CreatePlayer(Vector3 position, Quaternion rotation)
        {
            var player = GameObject.FindWithTag("Player");
            if (player == null)
            {
                player = Instantiate(_playerPrefabData.GetPlayerCharacterPrefab());
                player.name = "Player";
            }

            m_PlayerTransform = player.transform;
            m_PlayerTransform.position = position;
            m_PlayerTransform.rotation = rotation;
            m_CameraTransform = player.GetComponentInChildren<Camera>().transform;
        }

        #endregion

#if UNITY_EDITOR
        private void OnValidate()
        {
            CellManager.CellRadius = CellRadius;
            CellManager.DetailRadius = CellDetailRadius;
        }
#endif
    }
}