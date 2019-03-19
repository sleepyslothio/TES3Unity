#if LWRP_ENABLED || HDRP_ENABLED
#define SRP_ENABLED
#endif
#if UNITY_ANDROID || UNITY_IOS
#define MOBILE_BUILD
#endif
using System;
using System.IO;
using TESUnity.UI;
using UnityEngine;
#if LWRP_ENABLED
using UnityEngine.Rendering.LWRP;
#endif
#if HDRP_ENABLED
using UnityEngine.Experimental.Rendering.HDPipeline;
#endif
using UnityEngine.SceneManagement;
using UnityEngine.Rendering;
using Demonixis.Toolbox.XR;
using TESUnity.Inputs;

namespace TESUnity
{
    public class TESManager : MonoBehaviour
    {
        public const string Version = "0.9.1";
        public const float NormalMapGeneratorIntensity = 0.75f;
        public static TESManager instance;

        #region Inspector-set Members

        [Header("Global")]
        public string[] dataPaths;
        public bool useKinematicRigidbodies = true;
        public bool enableLog = false;

        [Header("Lighting")]
        public float ambientIntensity = 1.5f;

        [Header("Effects")]
        public bool waterBackSideTransparent = false;

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

        #endregion

        public static MorrowindDataReader MWDataReader { get; set; }
        public MorrowindEngine MWEngine = null;
        private MusicPlayer musicPlayer = null;

        public MorrowindEngine Engine => MWEngine;
        public TextureManager TextureManager => MWEngine.textureManager;

        private void Awake()
        {
            instance = this;
        }

        private void Start()
        {
            var config = GameSettings.Get();
            var dataPath = GameSettings.GetDataPath();
            var renderPath = config.RenderPath;

#if UNITY_STANDALONE || UNITY_EDITOR

            foreach (var alt in dataPaths)
            {
                if (GameSettings.IsValidPath(alt))
                    dataPath = alt;
            }
#endif

            if (!GameSettings.IsValidPath(dataPath))
                SceneManager.LoadScene("Menu");

#if MOBILE_BUILD
            if (renderPath == RendererType.HDRP)
                config.RenderPath = RendererType.LightweightRP;

            if (renderPath == RendererType.Deferred)
                config.RenderPath = RendererType.Forward;
#endif

#if MOBILE_BUILD
            var xr = XRManager.Enabled;
            waterBackSideTransparent = false;

            if (xr)
                QualitySettings.SetQualityLevel(1, false);
#endif

            var srpEnabled = config.IsSRP();

            if (!srpEnabled)
                GraphicsSettings.renderPipelineAsset = null;

#if SRP_ENABLED
			if (srpEnabled)
			{
				var target = config.SRPQuality.ToString();
				
				if (renderPath == RendererType.LightweightRP)
				{
#if UNITY_ANDROID
					target = "Mobile";
#endif
					var lwrpAsset = Resources.Load<LightweightRenderPipelineAsset>($"Rendering/LWRP/LightweightAsset-{target}");
					lwrpAsset.renderScale = config.RenderScale;
					GraphicsSettings.renderPipelineAsset = lwrpAsset;
				}
				else
				{
					GraphicsSettings.renderPipelineAsset = Resources.Load<HDRPRenderPipeline>($"Rendering/HDRP/HDRPAsset-{target}");

					var volumeSettings = Resources.Load<GameObject>("Rendering/HDRP/HDRP-VolumeSettings");
					Instantiate(volumeSettings);
				}
				// Only this mode is compatible with SRP.
				config.WaterQuality = Water.WaterMode.Simple;
			}
#endif

            if (UIManager == null)
            {
                UIManager = FindObjectOfType<UIManager>();

                if (UIManager == null)
                    throw new UnityException("UI Manager is missing");
            }

            CellManager.cellRadius = config.CellRadius;
            CellManager.detailRadius = config.CellDetailRadius;
            MorrowindEngine.cellRadiusOnLoad = config.CellRadiusOnLoad;

            if (MWDataReader == null)
                MWDataReader = new MorrowindDataReader(dataPath);

            MWEngine = new MorrowindEngine(MWDataReader, UIManager);

            musicPlayer = new MusicPlayer();

            if (config.MusicEnabled)
            {
                // Start the music.
                var songs = Directory.GetFiles(dataPath + "/Music/Explore");

                if (songs.Length > 0)
                {
                    foreach (var songFilePath in songs)
                    {
                        if (!songFilePath.Contains("Morrowind Title"))
                            musicPlayer.AddSong(songFilePath);
                    }

                    musicPlayer.Play();
                }
            }

            MWEngine.SpawnPlayerOutside(playerPrefab, new Vector2i(-2, -9), new Vector3(-137.94f, 2.30f, -1037.6f));
        }

        private void OnApplicationQuit()
        {
            MWDataReader?.Close();
        }

        private void FixedUpdate()
        {
            MWEngine.Update();

            musicPlayer.Update();

#if UNITY_ANDROID
            if (InputManager.GetButtonDown(MWButton.Menu) || Input.GetKeyDown(KeyCode.Escape))
                SceneManager.LoadScene("Menu");
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