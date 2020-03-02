using Demonixis.Toolbox.XR;
using TESUnity.Components;
using TESUnity.Components.Records;
using TESUnity.Effects;
using TESUnity.ESM;
using TESUnity.Inputs;
using TESUnity.Rendering;
using TESUnity.UI;
using UnityEngine;
using TESUnity.Components.XR;

namespace TESUnity
{
    public class MorrowindEngine
    {
        public static MorrowindEngine instance;

        public const float maxInteractDistance = 3;

        #region Private Fields

        private const float playerHeight = 2;
        private const float playerRadius = 0.4f;
        public static float desiredWorkTimePerFrame = 1.0f / 200;
        public static int cellRadiusOnLoad = 2;

        private CELLRecord m_CurrentCell;
        private GameObject m_SunObj;
        private GameObject m_WaterObj;
        private Transform m_PlayerTransform;
        private PlayerController m_PlayerController;
        private PlayerCharacter m_PlayerCharacter;
        private PlayerInventory m_PlayerInventory;
        private GameObject m_PlayerCameraObj;
        private UnderwaterEffect m_UnderwaterEffect;
        private Color32 m_DefaultAmbientColor = new Color32(137, 140, 160, 255);
        private RaycastHit[] m_InteractRaycastHitBuffer = new RaycastHit[32];

        #endregion

        #region Public Fields

        public MorrowindDataReader dataReader;
        public TextureManager textureManager;
        public TESMaterial materialManager;
        public NIFManager nifManager;
        public CellManager cellManager;
        public TemporalLoadBalancer temporalLoadBalancer;

        public CELLRecord currentCell => m_CurrentCell;

        public static int markerLayer => LayerMask.NameToLayer("Marker");

        public UIManager UIManager { get; set; }

        #endregion

        public MorrowindEngine(MorrowindDataReader mwDataReader)
        {
            Debug.Assert(instance == null);

            instance = this;
            dataReader = mwDataReader;
            textureManager = new TextureManager(dataReader);
            materialManager = new TESMaterial(textureManager);
            nifManager = new NIFManager(dataReader, materialManager);
            temporalLoadBalancer = new TemporalLoadBalancer();
            cellManager = new CellManager(dataReader, textureManager, nifManager, temporalLoadBalancer);

            var tes = TESManager.instance;
            var config = GameSettings.Get();

            RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Flat;
            RenderSettings.ambientIntensity = tes.ambientIntensity;

            m_SunObj = GameObjectUtils.CreateSunLight(Vector3.zero, Quaternion.Euler(new Vector3(50, 330, 0)));
            m_SunObj.GetComponent<Light>().shadows = config.SunShadows ? LightShadows.Soft : LightShadows.None;
            m_SunObj.SetActive(false);

            if (config.DayNightCycle)
            {
                m_SunObj.AddComponent<DayNightCycle>();
            }

            var waterPrefab = Resources.Load<GameObject>("Prefabs/WaterRP");
            m_WaterObj = GameObject.Instantiate(waterPrefab);
            m_WaterObj.SetActive(false);

            var waterRenderer = m_WaterObj.GetComponent<Renderer>();
            var waterMaterial = Resources.Load<Material>(TESMaterial.GetWaterMaterialPath(config.RendererMode == RendererMode.HDRP));
            waterRenderer.sharedMaterial = waterMaterial;

#if UNITY_STANDALONE
            if (!XRManager.IsXREnabled())
            {
                var texture = textureManager.LoadTexture("tx_cursor", true);
                Cursor.SetCursor(texture, Vector2.zero, CursorMode.Auto);
            }
#endif

            var uiCanvasPrefab = Resources.Load<GameObject>("Prefabs/GameUI");
            var uiCanvas = GameObject.Instantiate(uiCanvasPrefab);

            UIManager = uiCanvas.GetComponent<UIManager>();

#if UNITY_ANDROID
            RenderSettings.ambientIntensity = 4;
#endif
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
            m_CurrentCell = dataReader.FindInteriorCellRecord(interiorCellName);

            Debug.Assert(m_CurrentCell != null);

            CreatePlayer(position, rotation);

            var cellInfo = cellManager.StartCreatingInteriorCell(interiorCellName);
            temporalLoadBalancer.WaitForTask(cellInfo.objectsCreationCoroutine);

            OnInteriorCell(m_CurrentCell);
        }

        /// <summary>
        /// Spawns the player inside using the cell's grid coordinates.
        /// </summary>
        /// <param name="playerPrefab">The player prefab.</param>
        /// <param name="gridCoords">The grid coordinates.</param>
        /// <param name="position">The target position of the player.</param>
        public void SpawnPlayerInside(Vector2i gridCoords, Vector3 position, Quaternion rotation)
        {
            m_CurrentCell = dataReader.FindInteriorCellRecord(gridCoords);

            Debug.Assert(m_CurrentCell != null);

            CreatePlayer(position, rotation);

            var cellInfo = cellManager.StartCreatingInteriorCell(gridCoords);
            temporalLoadBalancer.WaitForTask(cellInfo.objectsCreationCoroutine);

            OnInteriorCell(m_CurrentCell);
        }

        /// <summary>
        /// Spawns the player outside using the cell's grid coordinates.
        /// </summary>
        /// <param name="gridCoords">The grid coordinates.</param>
        /// <param name="position">The target position of the player.</param>
        /// <param name="rotation">The target rotation of the player.</param>
        public void SpawnPlayerOutside(Vector2i gridCoords, Vector3 position, Quaternion rotation)
        {
            m_CurrentCell = dataReader.FindExteriorCellRecord(gridCoords);

            Debug.Assert(m_CurrentCell != null);

            CreatePlayer(position, rotation);

            var cellInfo = cellManager.StartCreatingExteriorCell(gridCoords);
            temporalLoadBalancer.WaitForTask(cellInfo.objectsCreationCoroutine);

            OnExteriorCell(m_CurrentCell);
        }

        /// <summary>
        /// Spawns the player outside using the position of the player.
        /// </summary>
        /// <param name="position">The target position of the player.</param>
        /// <param name="rotation">The target rotation of the player.</param>
        public void SpawnPlayerOutside(Vector3 position, Quaternion rotation)
        {
            var cellIndices = cellManager.GetExteriorCellIndices(position);
            m_CurrentCell = dataReader.FindExteriorCellRecord(cellIndices);

            CreatePlayer(position, rotation);
            cellManager.UpdateExteriorCells(m_PlayerCameraObj.transform.position, true, cellRadiusOnLoad);
            OnExteriorCell(m_CurrentCell);
        }

        #endregion

        public void Update()
        {
            // The current cell can be null if the player is outside of the defined game world.
            if ((m_CurrentCell == null) || !m_CurrentCell.isInterior)
            {
                cellManager.UpdateExteriorCells(m_PlayerCameraObj.transform.position);
            }

            temporalLoadBalancer.RunTasks(desiredWorkTimePerFrame);
            CastInteractRay();
        }

        public void CastInteractRay()
        {
            // Cast a ray to see what the camera is looking at.
            var ray = new Ray(m_PlayerCharacter.RayCastTarget.position, m_PlayerCharacter.RayCastTarget.forward);
            var raycastHitCount = Physics.RaycastNonAlloc(ray, m_InteractRaycastHitBuffer, maxInteractDistance);

            if (raycastHitCount > 0 && !m_PlayerController.Paused)
            {
                for (int i = 0; i < raycastHitCount; i++)
                {
                    var hitInfo = m_InteractRaycastHitBuffer[i];
                    var component = hitInfo.collider.GetComponentInParent<RecordComponent>();

                    if (component != null)
                    {
                        if (string.IsNullOrEmpty(component.objData.name))
                            return;

                        ShowInteractiveText(component);

                        if (InputManager.GetButtonDown(MWButton.Use))
                        {
                            if (component is Door)
                                OpenDoor((Door)component);

                            else if (component.usable)
                                component.Interact();

                            else if (component.pickable)
                                m_PlayerInventory.Add(component);
                        }
                        break;
                    }
                    else
                    {
                        //deactivate text if no interactable [ DOORS ONLY - REQUIRES EXPANSION ] is found
                        CloseInteractiveText();
                    }
                }
            }
            else
            {
                //deactivate text if nothing is raycasted against
                CloseInteractiveText();
            }
        }

        public void ShowInteractiveText(RecordComponent component)
        {
            var data = component.objData;
            UIManager.InteractiveText.Show(GUIUtils.CreateSprite(data.icon), data.interactionPrefix, data.name, data.value, data.weight);
        }

        public void CloseInteractiveText()
        {
            UIManager.InteractiveText.Close();
        }

        #region Private

        private void SetAmbientLight(Color color)
        {
            RenderSettings.ambientLight = color;
        }

        private void OnExteriorCell(CELLRecord CELL)
        {
            SetAmbientLight(m_DefaultAmbientColor);

            m_SunObj.SetActive(true);

            m_WaterObj.transform.position = Vector3.zero;
            m_WaterObj.SetActive(true);
            m_UnderwaterEffect.enabled = true;
            m_UnderwaterEffect.Level = 0.0f;
        }

        private void OnInteriorCell(CELLRecord CELL)
        {
            if (CELL.AMBI != null)
            {
                SetAmbientLight(ColorUtils.B8G8R8ToColor32(CELL.AMBI.ambientColor));
            }

            m_SunObj.SetActive(false);

            if (CELL.WHGT != null)
            {
                var offset = 1.6f; // Interiors cells needs this offset to render at the correct location.
                m_WaterObj.transform.position = new Vector3(0, (CELL.WHGT.value / Convert.MeterInMWUnits) - offset, 0);
                m_WaterObj.SetActive(true);
                m_UnderwaterEffect.Level = m_WaterObj.transform.position.y;
            }
            // FIXME: The water is disabled in interior cells for now.
            //else
            {
                m_WaterObj.SetActive(false);
            }

            m_UnderwaterEffect.enabled = m_WaterObj.activeSelf;
        }

        private void OpenDoor(Door component)
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
                    temporalLoadBalancer.WaitForTask(cellInfo.objectsCreationCoroutine);

                    newCell = cellInfo.cellRecord;

                    OnInteriorCell(cellInfo.cellRecord);
                }
                else
                {
                    var cellIndices = cellManager.GetExteriorCellIndices(component.doorData.doorExitPos);
                    newCell = dataReader.FindExteriorCellRecord(cellIndices);

                    cellManager.UpdateExteriorCells(m_PlayerCameraObj.transform.position, true, cellRadiusOnLoad);

                    OnExteriorCell(newCell);
                }

                m_CurrentCell = newCell;
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

            m_PlayerCharacter = player.GetComponent<PlayerCharacter>();
            m_PlayerController = player.GetComponent<PlayerController>();
            m_PlayerInventory = player.GetComponent<PlayerInventory>();
            m_UnderwaterEffect = m_PlayerCameraObj.GetComponent<UnderwaterEffect>();

            return player;
        }

        #endregion
    }
}