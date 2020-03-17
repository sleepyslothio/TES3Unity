using System;
using System.IO;
using TES3Unity.ESM.Records;
using UnityEngine;

namespace TES3Unity
{
    public enum PostProcessingQuality
    {
        None = 0, Low, Medium, High
    }

    public enum SRPQuality
    {
        Low = 0, Medium, High, Ultra
    }

    public enum AntiAliasingMode
    {
        None = 0, MSAA, FXAA, SMAA
    }

    [Serializable]
    public struct PlayerData
    {
        public string Name;
        public bool Woman;
        public RaceType Race;
        public string ClassName;
        public string Faction;
    }

    [Serializable]
    public sealed class GameSettings
    {
        private const string MorrowindPathKey = "tesunity.path";
        private const string StorageKey = "tesunity.settings";
        private const string AndroidFolderName = "TESUnityXR";
        private static GameSettings Instance = null;

        public bool MusicEnabled = true;
        public PostProcessingQuality PostProcessingQuality = PostProcessingQuality.High;
        public SRPQuality SRPQuality = SRPQuality.High;
        public AntiAliasingMode AntiAliasingMode = AntiAliasingMode.SMAA;
        public bool GenerateNormalMaps = true;
        public bool AnimateLights = true;
        public bool SunShadows = true;
        public bool PonctualLightShadows = true;
        public bool ExteriorLights = true;
        public float CameraFarClip = 500.0f;
        public int CellRadius = 2;
        public int CellDetailRadius = 2;
        public int CellRadiusOnLoad = 2;
        public bool KinematicRigidbody = true;
        public bool DayNightCycle = false;
        public bool FollowHead = true;
        public bool Teleportation = false;
        public bool RoomScale = false;
        public float RenderScale = 1.0f;
        public bool LogEnabled = false;
        public bool LoadExtensions = false;
        public PlayerData Player;

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

                Instance.Player = new PlayerData
                {
                    Name = "Player",
                    Woman = false,
                    Race = RaceType.Imperial
                };

#if UNITY_ANDROID || UNITY_IOS
                Instance.GenerateNormalMaps = false;
                Instance.SunShadows = false;
                Instance.RenderScale = 0.9f;
                Instance.PostProcessingQuality = PostProcessingQuality.None;
                Instance.LightShadows = false;
                Instance.ExteriorLights = false;
                Instance.CameraFarClip = 200;
                Instance.DayNightCycle = false;
                Instance.AntiAliasingMode = AntiAliasingMode.MSAA;
                Instance.RoomScale = false;
                Instance.CellDetailRadius = 2;
                Instance.CellRadius = 1;

                if (XRManager.IsXREnabled())
                {
                    Instance.CellDetailRadius = 2;
                    Instance.CameraFarClip = 150;
                    Instance.RoomScale = true;
                }
#endif
                if (PlayerPrefs.HasKey(StorageKey))
                {
                    var json = PlayerPrefs.GetString(StorageKey);
                    if (!string.IsNullOrEmpty(json) && json != "{}")
                    {
                        Instance = JsonUtility.FromJson<GameSettings>(json);
                    }
                }
            }

            return Instance;
        }

        #region Static Functions

        public static bool IsMobile()
        {
#if UNITY_ANDROID || UNITY_IOS
            return true;
#else
            return false;
#endif
        }

        public static LightShadows GetRecommandedShadows()
        {
            var config = Get();

            if (!config.PonctualLightShadows || !config.SunShadows)
            {
                return LightShadows.None;
            }
            else if (IsMobile() || config.SRPQuality == SRPQuality.Medium || config.SRPQuality == SRPQuality.Low)
            {
                return LightShadows.Hard;
            }

            return LightShadows.Soft;
        }

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
            {
                return path;
            }

#if UNITY_ANDROID
            return $"/sdcard/{AndroidFolderName}";
#else
            return string.Empty;
#endif
        }

        #endregion
    }
}
