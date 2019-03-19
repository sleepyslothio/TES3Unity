using Demonixis.Toolbox.XR;
using System;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityStandardAssets.Water;

namespace TESUnity
{
    public enum MWMaterialType
    {
        PBR, Standard, Unlit
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
        Forward, Deferred, LightweightRP, HDRP
    }

    public enum AntiAliasingMode
    {
        None = 0, MSAA2X, MSAA4X, MSAA8X, FXAA, SMAA, TAA
    }

    [Serializable]
    public sealed class GameSettings
    {
        private const string MorrowindPathKey = "tesunity.path";
        private const string StorageKey = "tesunity.settings";
        private const string ConfigFile = "config.ini"; // Deprecated
        private const string MWDataPathName = "MorrowindDataPath";
        private static GameSettings Instance = null;

        public bool MusicEnabled = true;
        public PostProcessingQuality PostProcessing = PostProcessingQuality.High;
        public MWMaterialType MaterialType = MWMaterialType.PBR;
        public RendererType RenderPath = RendererType.Forward;
        public SRPQuality SRPQuality = SRPQuality.High;
        public AntiAliasingMode AntiAliasing = AntiAliasingMode.TAA;
        public bool GenerateNormalMaps = true;
        public bool AnimateLights = true;
        public bool SunShadows = true;
        public bool LightShadows = false;
        public bool ExteriorLights = false;
        public float CameraFarClip = 1000;
        public int CellRadius = 2;
        public int CellDetailRadius = 2;
        public int CellRadiusOnLoad = 2;
        public Water.WaterMode WaterQuality = UnityStandardAssets.Water.Water.WaterMode.Simple;
        public bool DayNightCycle = false;
        public bool FollowHead = true;
        public bool RoomScale = false;
        public float RenderScale = 1.0f;

        public bool IsSRP()
        {
            return RenderPath == RendererType.LightweightRP || RenderPath == RendererType.HDRP;
        }

        public static void Save()
        {
            var instance = Get();
            var json = JsonUtility.ToJson(instance);
            PlayerPrefs.SetString(StorageKey, json);
            PlayerPrefs.Save();
        }

        public static GameSettings Get()
        {
            if (Instance == null)
            {
                Instance = new GameSettings();

#if UNITY_ANDROID || UNITY_IOS
                Instance.GenerateNormalMaps = false;
                Instance.SunShadows = false;
                Instance.RenderScale = 0.9f;
                Instance.MaterialType = MWMaterialType.Standard;
                Instance.PostProcessing = PostProcessingQuality.None;
                Instance.LightShadows = false;
                Instance.ExteriorLights = false;
                Instance.CameraFarClip = 200;
                Instance.DayNightCycle = false;
                Instance.AntiAliasing = AntiAliasingMode.MSAA2X;
                Instance.RoomScale = false;
                Instance.CellDetailRadius = 1;
                Instance.CellRadius = 1;

                if (XRManager.Enabled)
                {
                    Instance.CameraFarClip = 50;
                    Instance.MaterialType = MWMaterialType.Unlit;
                }
#endif

                if (PlayerPrefs.HasKey(StorageKey))
                {
                    var json = PlayerPrefs.GetString(StorageKey);
                    if (!string.IsNullOrEmpty(json) && json != "{}")
                        Instance = JsonUtility.FromJson<GameSettings>(json);
                }
            }

            return Instance;
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

            if (!string.IsNullOrEmpty(path))
                return path;

#if UNITY_ANDROID
            return "/sdcard/TESUnityXR";
#else
            return string.Empty;
#endif
        }

        #endregion
    }
}
