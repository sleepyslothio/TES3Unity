using System;
using System.Collections.Generic;
using System.IO;
using Demonixis.ToolboxV2;
using Demonixis.ToolboxV2.Graphics;
using Demonixis.ToolboxV2.XR;
using TES3Unity.ESM;
using TES3Unity.ESM.Records;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

namespace TES3Unity
{
    public interface ISettingsNotify
    {
        void UpdateSettings();
    }

    [Serializable]
    public sealed class GameSettings : GameSave
    {
        private const string MorrowindPathKey = "tes3unity.path";
        private const string StorageKey = "tes3unity.settings";
        private const string AndroidFolderName = "TES3Unity";
        private static short IsLessThanA12 = -1;

        public const int FileVersion = 3;
        public const int FileMinVersion = 3;
        public const int CameraFarMin = 2500;
        public const int CameraFarMax = 100000;
        private const string Filename = "Settings.json";

        public static readonly ushort[] CellDistanceValues =
        {
            0, 1, 2, 3, 4
        };

#if UNITY_EDITOR
        [HideInInspector] public bool FromMenu;
#endif

        private static GameSettings _instance;

        [Header("Audio")] public bool MusicEnabled = true;

        [Header("Preferences")] public int version = FileVersion;
        public ushort CellRadius = 2;
        public ushort CellDetailRadius = 2;
        public ushort CellRadiusOnLoad = 2;
        public bool KinematicRigidbody = true;
        public bool DayNightCycle;
        public bool LoadExtensions;
        public PlayerData playerData;

        [Header("Graphics")] public GraphicsQuality2 srpQuality = GraphicsQuality2.High;
        public GraphicsQuality terrainQuality = GraphicsQuality.High;
        public AntiAliasingProxy antiAliasing = AntiAliasingProxy.TAA;
        public float renderScale = 1.0f;
        public DeviceRefreshRate refreshRate = DeviceRefreshRate._60;
        public bool lowQualityShader;
        public float terrainError = 5;
        public float treeDistance = 80;
        public bool animateLights = true;
        public bool exteriorLights = true;
        public float nearClip = 0.1f;
        public float farClip = 1500.0f;

        [Header("Shadows")] public GameShadowCascades shadowCascadeCount = GameShadowCascades._4;
        public float shadowDistance = 150;
        public bool directionalShadows = true;
        public bool additionalShadows;
        public bool softShadows = true;

        [Header("Post Processing")] public PostProcessingQuality postProcessingQuality = PostProcessingQuality.High;
        public bool ssao = true;
        public bool bloom = true;
        public bool ssr = true;
        public bool ssgi;

        [Header("VR")] public bool vrSeated = true;
        public VRFFR vrFoveationLevel = VRFFR.High;
        public bool vrAsyncSpaceWarp;
        public VRRefreshRate vrDisplayFrequency = VRRefreshRate._72;
        public bool startInVr;
        public float vrHeadHeight = 1.65f;
        public bool vrHandTracking = true;
        public bool vrDynamicResolution;
        public bool startInMr = true;
        public bool vrFollowHead = true;
        public bool vrTeleportation = true;

        public bool IsValid => version >= FileMinVersion;

        public static NPC_Record GetPlayerRecord()
        {
            var playerData = Get().playerData;
            var items = new List<NPCOData>();
            var race = playerData.Race.ToString().ToLower().Replace("_", " ");
            var gender = playerData.Woman ? "f" : "m";

            return NPC_Record.CreateRaw(
                playerData.Name,
                playerData.Race.ToString(),
                playerData.Faction,
                playerData.ClassName,
                $"b_n_{race}_{gender}_head_01",
                $"b_n_{race}_{gender}_hair_00",
                playerData.Woman ? 1 : 0,
                items);
        }

        public bool StartInMixedReality()
        {
#if UNITY_VISIONOS
            return startInMr;
#elif UNITY_ANDROID
            return startInMr && XRManager.Enabled && SceneManager.GetActiveScene().name.ToLower().Contains("menu");
#else
            return false;
#endif
        }

        public void Initialize()
        {
            if (PlatformUtility.IsMobilePlatform())
            {
                // Rendering
                srpQuality = GraphicsQuality2.High;
                terrainQuality = GraphicsQuality.High;
                antiAliasing = AntiAliasingProxy.MSAA_2x;
                postProcessingQuality = PostProcessingQuality.High;
                lowQualityShader = false;
                directionalShadows = true;
                additionalShadows = false;
                ssr = false;
                refreshRate = DeviceRefreshRate._30;
#if UNITY_IOS
                if (IsSoCInferiorToA12())
                {
                    srpQuality = GraphicsQuality2.Low;
                    terrainQuality = GraphicsQuality.Low;
                    antiAliasing = AntiAliasingProxy.Disabled;
                    postProcessingQuality = PostProcessingQuality.None;
                    directionalShadows = false;
                    additionalShadows = false;
                    ssao = false;
                    bloom = false;
                }
#endif

                playerData = new PlayerData
                {
                    Name = "Player",
                    Woman = false,
                    Race = RaceType.Imperial
                };

                if (PlatformUtility.IsMobilePlatform())
                {
                    directionalShadows = false;
                    renderScale = 0.9f;
                    postProcessingQuality = PostProcessingQuality.None;
                    exteriorLights = false;
                    lowQualityShader = true;
                    farClip = 200;
                    DayNightCycle = false;
                    antiAliasing = AntiAliasingProxy.Disabled;
                    CellDetailRadius = 2;
                    CellRadius = 1;
                }

                if (PlatformUtility.IsMobileVR())
                {
                    postProcessingQuality = PostProcessingQuality.None;
                    vrDisplayFrequency = VRRefreshRate._72;
                    vrFoveationLevel = VRFFR.Medium;

                    var quest1 = XRManager.Headset == XRHeadset.OculusQuest;
                    var quest2 = XRManager.Headset == XRHeadset.OculusQuest2;

                    if (quest1 || quest2)
                    {
                        srpQuality = GraphicsQuality2.Low;
                        antiAliasing = AntiAliasingProxy.MSAA_2x;
                        postProcessingQuality = PostProcessingQuality.None;
                        lowQualityShader = true;
                        directionalShadows = false;
                        additionalShadows = false;
                        renderScale = 0.90f;
                        vrFoveationLevel = VRFFR.High;
                        softShadows = false;

                        if (quest1)
                        {
                            antiAliasing = AntiAliasingProxy.Disabled;
                            terrainQuality = GraphicsQuality.Low;
                            renderScale = 0.80f;
                            vrDisplayFrequency = VRRefreshRate._72;
                            vrFoveationLevel = VRFFR.HighTop;
                        }
                    }
                }
            }

            if (PlatformUtility.IsConsolePlatform() || PlatformUtility.IsWebPlatform())
            {
                antiAliasing = AntiAliasingProxy.FXAA;
                srpQuality = GraphicsQuality2.Low;
                postProcessingQuality = PostProcessingQuality.Low;
                shadowCascadeCount = GameShadowCascades._2;
                bloom = true;
            }

#if UNITY_VISIONOS
            srpQuality = GraphicsQuality2.High;
            terrainQuality = GraphicsQuality.High;
            antiAliasing = AntiAliasingProxy.Disabled;
            postProcessingQuality = PostProcessingQuality.High;
            directionalShadows = true;
            shadowDistance = 150;
            bloom = true;
            softShadows = true;
            shadowCascadeCount = GameShadowCascades._4;
            softShadows = true;
            directionalShadows = true;
            
#if HDRP_ENABLED
            srpQuality = GraphicsQuality2.Low;
            postProcessingQuality = PostProcessingQuality.None;
            bloom = false;
            ssao = false;
            ssgi = false;
            ssr = false;
            shadowCascadeCount = GameShadowCascades._2;
#endif
#endif
        }

        public void Save()
        {
            version = FileVersion;
            SaveData(GetPreferredStorageModeForSettings(), this, Filename);
        }

        public static GameSettings Get()
        {
            if (_instance != null)
                return _instance;

            _instance = LoadData<GameSettings>(GetPreferredStorageModeForSettings(), Filename);

            if (_instance != null)
            {
                if (_instance.IsValid)
                {
                    _instance.Sanitize(false);
                }
                else
                {
                    _instance = null;
                }
            }

            if (_instance == null)
            {
                CreateInstance();
            }

            return _instance;
        }

        public void Sanitize(bool resetLanguage)
        {
            renderScale = Mathf.Clamp(renderScale, 0.5f, 2.0f);
            nearClip = Mathf.Clamp(nearClip, 0.1f, 0.3f);
            farClip = Mathf.Clamp(farClip, 100, 100000);
        }

        private static void CreateInstance()
        {
            _instance = new GameSettings();
            _instance.Initialize();
        }

        public static void Override(GameSettings other)
        {
            _instance = other;
        }

        public static void Clear()
        {
            ClearData(GetPreferredStorageModeForSettings(), Filename);
            _instance = null;
        }

        public GraphicsSettingsData GetGraphicsSettingsData()
        {
            var targetRefreshRate = refreshRate switch
            {
                DeviceRefreshRate._30 => 30,
                DeviceRefreshRate._60 => 60,
                DeviceRefreshRate._120 => 120,
                DeviceRefreshRate._240 => 240,
                DeviceRefreshRate.Unlimited => -1,
                _ => 60
            };

            return new GraphicsSettingsData
            {
                SRPQuality = srpQuality,
                TerrainQuality = terrainQuality,
                AntiAliasing = antiAliasing,
                nearClip = nearClip,
                farClip = farClip,
                RenderScale = Mathf.Clamp(renderScale, 0.5f, 2.0f),
                RefreshRate = targetRefreshRate,
                LowQualityShader = lowQualityShader,
                ShadowCascadeCount = shadowCascadeCount,
                ShadowDistance = Mathf.Clamp(shadowDistance, 10, 1000),
                DirectionalShadows = directionalShadows,
                AdditionalShadows = additionalShadows,
                PostProcessingQuality = postProcessingQuality,
                SoftShadows = softShadows,
                SSAO = ssao,
                Bloom = bloom,
                SSR = ssr,
                LenseEffect = false,
                FilmGrain = false,
                SSGI = ssgi,
                Distortion = false
            };
        }

        public XRSettingsData GetXRSettingsData()
        {
            return new XRSettingsData
            {
                VRAppSpaceWarp = vrAsyncSpaceWarp,
                VRAutoStart = startInVr,
                VRFFRLevel = vrFoveationLevel,
                VRHandTracking = true,
                VRRefreshRate = vrDisplayFrequency,
                VRSeatedMode = vrSeated,
                VRHeadHeight = vrHeadHeight,
                VRDynamicResolution = vrDynamicResolution
            };
        }

        public static LightShadows GetRecommandedShadows(bool main)
        {
            var settings = Get();

            if (main && settings.directionalShadows)
                return settings.softShadows ? LightShadows.Soft : LightShadows.Hard;

            if (!main && settings.additionalShadows)
                return settings.softShadows ? LightShadows.Soft : LightShadows.Hard;

            return LightShadows.None;
        }

        #region Static Functions

        public static bool IsValidPath(string path)
        {
            return File.Exists(Path.Combine(path, "Morrowind.esm"));
        }

        public static void SetDataPath(string dataPath)
        {
            PlayerPrefs.SetString(MorrowindPathKey, dataPath);
        }

        public static string GetDataPath()
        {
            var path = PlayerPrefs.GetString(MorrowindPathKey);

            if (!Directory.Exists(path))
                path = Application.streamingAssetsPath + "/Data Files";

            if (!string.IsNullOrEmpty(path))
            {
                return path;
            }

#if UNITY_ANDROID
            return $"/sdcard/{AndroidFolderName}/Data Files";
#else
            return string.Empty;
#endif
        }

        public static bool IsSoCInferiorToA12()
        {
            if (IsLessThanA12 > -1)
                return IsLessThanA12 == 1;

            var deviceModel = SystemInfo.deviceModel;

            var modelsWithInferiorSoC = new[]
            {
                "iPhone10,1", "iPhone10,2", "iPhone10,3", "iPhone10,4", "iPhone10,5",
                "iPhone10,6", // iPhone 8, 8 Plus, X
                "iPad7,5", "iPad7,6", "iPad7,11", "iPad7,12", // iPad 6th and 7th generation
            };

            foreach (var model in modelsWithInferiorSoC)
            {
                if (!deviceModel.Contains(model)) continue;
                IsLessThanA12 = 1;
                return true;
            }

            IsLessThanA12 = 0;
            return false;
        }

        #endregion
    }
}