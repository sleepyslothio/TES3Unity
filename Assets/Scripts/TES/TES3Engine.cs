using Demonixis.Toolbox.XR;
using TES3Unity.Components;
using TES3Unity.Components.Records;
using TES3Unity.Rendering;
using UnityEngine;
using TES3Unity.Components.XR;
using TES3Unity.ESM.Records;
using System;

namespace TES3Unity
{
    public sealed class TES3Engine : MonoBehaviour
    {
        // Static.
        public static int markerLayer => LayerMask.NameToLayer("Marker");
        public static float desiredWorkTimePerFrame = 1.0f / 200;
        public static int cellRadiusOnLoad = 2;
        private static TES3Engine instance = null;

        // Private.
        private CELLRecord m_CurrentCell;
        private Transform m_PlayerTransform;
        private GameObject m_PlayerCameraObj;
        private bool m_Initialized = false;
        
        // Public.
        public TES3DataReader dataReader;
        public TextureManager textureManager;
        public TES3Material materialManager;
        public NIFManager nifManager;
        public CellManager cellManager;
        public TemporalLoadBalancer m_TemporalLoadBalancer;

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
        }

        public void Initialize(TES3DataReader mwDataReader)
        {
            dataReader = mwDataReader;
            textureManager = new TextureManager(dataReader);
            materialManager = new TES3Material(textureManager);
            nifManager = new NIFManager(dataReader, materialManager);
            m_TemporalLoadBalancer = new TemporalLoadBalancer();
            cellManager = new CellManager(dataReader, textureManager, nifManager, m_TemporalLoadBalancer);

            var tes = TES3Manager.Instance;
            var config = GameSettings.Get();

            var sunLight = GameObjectUtils.CreateSunLight(Vector3.zero, Quaternion.Euler(new Vector3(50, 330, 0)));
            sunLight.AddComponent<LightingManager>();

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
            CurrentCell = dataReader.FindInteriorCellRecord(interiorCellName);

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
                CurrentCell = dataReader.FindExteriorCellRecord(gridCoords);
                cellInfo = cellManager.StartCreatingExteriorCell(gridCoords);
            }
            else
            {
                CurrentCell = dataReader.FindInteriorCellRecord(gridCoords);
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
                cellManager.UpdateExteriorCells(m_PlayerCameraObj.transform.position);
            }

            m_TemporalLoadBalancer.RunTasks(desiredWorkTimePerFrame);
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
                    newCell = dataReader.FindExteriorCellRecord(cellIndices);

                    cellManager.UpdateExteriorCells(m_PlayerCameraObj.transform.position, true, cellRadiusOnLoad);
                }

                CurrentCell = newCell;
            }
        }

        private GameObject CreatePlayer(Vector3 position, Quaternion rotation)
        {
            var xrEnabled = XRManager.IsXREnabled();
            var playerPrefabPath = "Prefabs/Player";

            // First, create the interaction system if XR is enabled.
            if (xrEnabled)
            {
                PlayerXR.CreateInteractionSystem();
                playerPrefabPath += "XR";
            }

            // Then create the player.
            var player = GameObject.FindWithTag("Player");
            if (player == null)
            {
                var playerPrefab = Resources.Load<GameObject>(playerPrefabPath);
                player = GameObject.Instantiate(playerPrefab);
                player.name = "Player";
            }

            m_PlayerTransform = player.transform;
            m_PlayerTransform.position = position;
            m_PlayerTransform.rotation = rotation;

            m_PlayerCameraObj = player.GetComponentInChildren<Camera>().gameObject;

            return player;
        }

        #endregion
    }
}