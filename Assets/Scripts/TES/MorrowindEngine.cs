using Demonixis.Toolbox.XR;
using TESUnity.Components;
using TESUnity.Components.Records;
using TESUnity.Effects;
using TESUnity.ESM;
using TESUnity.Inputs;
using TESUnity.Rendering;
using TESUnity.UI;
using UnityEngine;
using UnityEngine.Rendering;
using UnityStandardAssets.Water;

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
        private PlayerComponent m_Player;
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
                m_SunObj.AddComponent<DayNightCycle>();

            m_WaterObj = GameObject.Instantiate(tes.waterPrefab);
            m_WaterObj.SetActive(false);

            var water = m_WaterObj.GetComponent<Water>();
            var renderer = water.GetComponent<MeshRenderer>();
            renderer.sharedMaterial.SetColor("_RefrColor", new Color(0.58f, 0.7f, 1.0f));

            if (!GameSettings.Get().WaterTransparency)
            {
                var side = m_WaterObj.transform.GetChild(0);
                var sideMaterial = side.GetComponent<Renderer>().sharedMaterial;
                sideMaterial.SetInt("_SrcBlend", (int)BlendMode.One);
                sideMaterial.SetInt("_DstBlend", (int)BlendMode.Zero);
                sideMaterial.SetInt("_ZWrite", 1);
                sideMaterial.DisableKeyword("_ALPHATEST_ON");
                sideMaterial.DisableKeyword("_ALPHABLEND_ON");
                sideMaterial.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                sideMaterial.renderQueue = -1;
            }

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
        public void SpawnPlayerInside(GameObject playerPrefab, string interiorCellName, Vector3 position)
        {
            m_CurrentCell = dataReader.FindInteriorCellRecord(interiorCellName);

            Debug.Assert(m_CurrentCell != null);

            CreatePlayer(playerPrefab, position, out m_PlayerCameraObj);

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
        public void SpawnPlayerInside(GameObject playerPrefab, Vector2i gridCoords, Vector3 position)
        {
            m_CurrentCell = dataReader.FindInteriorCellRecord(gridCoords);

            Debug.Assert(m_CurrentCell != null);

            CreatePlayer(playerPrefab, position, out m_PlayerCameraObj);

            var cellInfo = cellManager.StartCreatingInteriorCell(gridCoords);
            temporalLoadBalancer.WaitForTask(cellInfo.objectsCreationCoroutine);

            OnInteriorCell(m_CurrentCell);
        }

        /// <summary>
        /// Spawns the player outside using the cell's grid coordinates.
        /// </summary>
        /// <param name="playerPrefab">The player prefab.</param>
        /// <param name="gridCoords">The grid coordinates.</param>
        /// <param name="position">The target position of the player.</param>
        public void SpawnPlayerOutside(GameObject playerPrefab, Vector2i gridCoords, Vector3 position)
        {
            m_CurrentCell = dataReader.FindExteriorCellRecord(gridCoords);

            Debug.Assert(m_CurrentCell != null);

            CreatePlayer(playerPrefab, position, out m_PlayerCameraObj);

            var cellInfo = cellManager.StartCreatingExteriorCell(gridCoords);
            temporalLoadBalancer.WaitForTask(cellInfo.objectsCreationCoroutine);

            OnExteriorCell(m_CurrentCell);
        }

        /// <summary>
        /// Spawns the player outside using the position of the player.
        /// </summary>
        /// <param name="playerPrefab">The player prefab.</param>
        /// <param name="position">The target position of the player.</param>
        public void SpawnPlayerOutside(GameObject playerPrefab, Vector3 position)
        {
            var cellIndices = cellManager.GetExteriorCellIndices(position);
            m_CurrentCell = dataReader.FindExteriorCellRecord(cellIndices);

            CreatePlayer(playerPrefab, position, out m_PlayerCameraObj);
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
            var ray = new Ray(m_Player.RayCastTarget.position, m_Player.RayCastTarget.forward);
            var raycastHitCount = Physics.RaycastNonAlloc(ray, m_InteractRaycastHitBuffer, maxInteractDistance);

            if (raycastHitCount > 0 && !m_Player.Paused)
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
                        CloseInteractiveText(); //deactivate text if no interactable [ DOORS ONLY - REQUIRES EXPANSION ] is found
                }
            }
            else
                CloseInteractiveText(); //deactivate text if nothing is raycasted against
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

            m_UnderwaterEffect.enabled = CELL.WHGT != null;

            if (CELL.WHGT != null)
            {
                var offset = 1.6f; // Interiors cells needs this offset to render at the correct location.
                m_WaterObj.transform.position = new Vector3(0, (CELL.WHGT.value / Convert.meterInMWUnits) - offset, 0);
                m_WaterObj.SetActive(true);
                m_UnderwaterEffect.Level = m_WaterObj.transform.position.y;
            }
            // FIXME: The water is disabled in interior cells for now.
            //else
            {
                m_WaterObj.SetActive(false);
            }
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

        private GameObject CreatePlayer(GameObject playerPrefab, Vector3 position, out GameObject playerCamera)
        {
            var player = GameObject.FindWithTag("Player");
            if (player == null)
            {
                player = GameObject.Instantiate(playerPrefab);
                player.name = "Player";
            }

            player.transform.position = position;

            m_PlayerTransform = player.GetComponent<Transform>();
            playerCamera = player.GetComponentInChildren<Camera>().gameObject;
            m_Player = player.GetComponent<PlayerComponent>();
            m_PlayerInventory = player.GetComponent<PlayerInventory>();
            m_UnderwaterEffect = playerCamera.GetComponent<UnderwaterEffect>();

            player.GetComponent<Rigidbody>().isKinematic = false;

            return player;
        }

        #endregion
    }
}