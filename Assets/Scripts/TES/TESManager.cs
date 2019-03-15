using System;
using System.IO;
using TESUnity.UI;
using UnityEngine;
#if LWRP_ENABLED
using UnityEngine.Rendering.LWRP;
using UnityEngine.Rendering;
#endif
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.SceneManagement;
using UnityStandardAssets.Water;
using TESUnity.Inputs;

namespace TESUnity
{
    public class TESManager : MonoBehaviour
    {
        public static TESManager instance;

        public enum MWMaterialType
        {
            PBR, Standard
        }

        public enum PostProcessingQuality
        {
            None = 0, Low, Medium, High
        }

        public enum SRPQuality
        {
            Low, Medium, High
        }

        public enum RendererType
        {
            Forward, Deferred, LightweightRP
        }

        public const string Version = "0.9.0";

#region Inspector-set Members

#if UNITY_EDITOR
        [Header("Editor Only")]
        [SerializeField]
        public bool _bypassINIConfig = false;
        [SerializeField]
        private bool _allowChangeCellInRealtime = false;
#endif

        [Header("Global")]
        public string dataPath;
        public string[] alternativeDataPath;
        public bool useKinematicRigidbodies = true;
        public bool playMusic = false;
        public bool enableLog = false;
        public Water.WaterMode waterQuality = Water.WaterMode.Simple;
        public bool useStaticBatching = false;

        [Header("Optimizations")]
        public int cellRadius = 4;
        public int cellDetailRadius = 3;
        public int cellRadiusOnLoad = 2;

        [Header("Rendering")]
        public MWMaterialType materialType = MWMaterialType.Standard;
        public RendererType renderPath = RendererType.Forward;
        public float cameraFarClip = 500.0f;
        public SRPQuality srpQuality = SRPQuality.Medium;
        public float renderScale = 1.0f;

        [Header("Lighting")]
        public float ambientIntensity = 1.5f;
        public bool renderSunShadows = false;
        public bool renderLightShadows = false;
        public bool renderExteriorCellLights = false;
        public bool animateLights = false;
        public bool dayNightCycle = false;
        public bool generateNormalMap = true;
        public float normalGeneratorIntensity = 0.75f;

        [Header("Effects")]
        public PostProcessingQuality postProcessingQuality = PostProcessingQuality.High;
        public PostProcessLayer.Antialiasing antiAliasing = PostProcessLayer.Antialiasing.TemporalAntialiasing;
        public bool waterBackSideTransparent = false;

        [Header("VR")]
        public bool followHeadDirection = false;
        public bool roomScale = true;
        public bool forceControllers = false;

        [Header("UI")]
        public UIManager UIManager;
        public Sprite UIBackgroundImg;
        public Sprite UICheckmarkImg;
        public Sprite UIDropdownArrowImg;
        public Sprite UIInputFieldBackgroundImg;
        public Sprite UIKnobImg;
        public Sprite UIMaskImg;
        public Sprite UISpriteImg;

        [Header("Prefabs")]
        public GameObject playerPrefab;
        public GameObject waterPrefab;

        [Header("Debug")]
        public bool creaturesEnabled = false;
        public bool npcsEnabled = false;

#endregion

        private MorrowindDataReader MWDataReader = null;
        private MorrowindEngine MWEngine = null;
        private MusicPlayer musicPlayer = null;

        public MorrowindEngine Engine => MWEngine;
        public TextureManager TextureManager => MWEngine.textureManager;

        private void Awake()
        {
            Debug.unityLogger.logEnabled = enableLog;

            instance = this;

#if UNITY_STANDALONE || UNITY_EDITOR
            var path = dataPath;

#if UNITY_EDITOR
            if (!_bypassINIConfig)
                path = GameSettings.CheckSettings(this);
#else
            path = GameSettings.CheckSettings(this);
#endif

            if (!GameSettings.IsValidPath(path))
            {
#if UNITY_EDITOR
                foreach (var alt in alternativeDataPath)
                {
                    if (GameSettings.IsValidPath(alt))
                    {
                        dataPath = alt;
                        return;
                    }
                }
#endif

                GameSettings.SetDataPath(string.Empty);
                SceneManager.LoadScene("AskPathScene");
            }
            else
                dataPath = path;
#endif
        }

        private void Start()
        {
#if !SRP_ENABLED
            if (renderPath == RendererType.LightweightRP)
                renderPath = RendererType.Forward;
#endif

#if UNITY_ANDROID
            renderPath = RendererType.Forward;
            postProcessingQuality = PostProcessingQuality.None;
            materialType = MWMaterialType.PBR;
            renderSunShadows = false;
            cameraFarClip = 100;
            generateNormalMap = false;
            renderSunShadows = false;
            renderLightShadows = false;
            ambientIntensity = 3;
            renderLightShadows = false;
            renderExteriorCellLights = true;
            animateLights = true;
            dayNightCycle = false;
            cellRadius = 1;
            cellDetailRadius = 2;
            cellRadiusOnLoad = 1;
            playMusic = true;
            waterBackSideTransparent = false;
            waterQuality = Water.WaterMode.Simple;

            InputManager.TryInitializeMobileTouch();
#if !UNITY_EDITOR
            dataPath = "/sdcard/TESUnityXR";
#endif
#endif

#if LWRP_ENABLED || HDRP_ENABLED
            if (renderPath == RendererType.LightweightRP)
            {
                var target = srpQuality.ToString();

#if UNITY_ANDROID
                target = "Mobile";
#endif

                var asset = Resources.Load<LightweightRenderPipelineAsset>($"Rendering/LWRP/Assets/LightweightAsset-{target}");
                asset.renderScale = renderScale;
                GraphicsSettings.renderPipelineAsset = asset;

                // Only this mode is compatible with SRP.
                waterQuality = Water.WaterMode.Simple;
            }
            else
                GraphicsSettings.renderPipelineAsset = null;
#endif

            if (UIManager == null)
            {
                UIManager = FindObjectOfType<UIManager>();

                if (UIManager == null)
                    throw new UnityException("UI Manager is missing");
            }

            CellManager.cellRadius = cellRadius;
            CellManager.detailRadius = cellDetailRadius;
            MorrowindEngine.cellRadiusOnLoad = cellRadiusOnLoad;

            MWDataReader = new MorrowindDataReader(dataPath);
            MWEngine = new MorrowindEngine(MWDataReader, UIManager);

            if (playMusic)
            {
                // Start the music.
                musicPlayer = new MusicPlayer();

                var songs = Directory.GetFiles(dataPath + "/Music/Explore");

                if (songs.Length > 0)
                {
                    foreach (var songFilePath in songs)
                    {
                        if (!songFilePath.Contains("Morrowind Title"))
                        {
                            musicPlayer.AddSong(songFilePath);
                        }
                    }

                    musicPlayer.Play();
                }
            }

            MWEngine.SpawnPlayerOutside(playerPrefab, new Vector2i(-2, -9), new Vector3(-137.94f, 2.30f, -1037.6f));
        }

        private void OnDestroy()
        {
            if (MWDataReader != null)
            {
                MWDataReader.Close();
                MWDataReader = null;
            }

#if UNITY_EDITOR && (LWRP_ENABLED || HDRP_ENABLED)
            if (renderPath == RendererType.LightweightRP)
            {
                var asset = lightweightAssets[(int)srpQuality];
                asset.renderScale = 1.0f;
            }
#endif
        }

        private void FixedUpdate()
        {
            MWEngine.Update();

            if (playMusic)
            {
                musicPlayer.Update();
            }

#if UNITY_EDITOR
            if (_allowChangeCellInRealtime)
            {
                CellManager.cellRadius = cellRadius;
                CellManager.detailRadius = cellDetailRadius;
                MorrowindEngine.cellRadiusOnLoad = cellRadiusOnLoad;
            }
#endif
        }

        private void TestAllCells(string resultsFilePath)
        {
            using (StreamWriter writer = new StreamWriter(resultsFilePath))
            {
                foreach (var record in MWDataReader.MorrowindESMFile.GetRecordsOfType<ESM.CELLRecord>())
                {
                    var CELL = (ESM.CELLRecord)record;

                    try
                    {
                        var cellInfo = MWEngine.cellManager.StartInstantiatingCell(CELL);
                        MWEngine.temporalLoadBalancer.WaitForTask(cellInfo.objectsCreationCoroutine);

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
    }
}