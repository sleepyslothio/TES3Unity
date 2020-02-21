﻿using Demonixis.Toolbox.XR;
using System;
using System.IO;
using UnityEngine;
using UnityStandardAssets.Water;

namespace TESUnity
{
    public enum MWMaterialType
    {
        PBR = 0, Standard, Unlit
    }

    public enum PostProcessingQuality
    {
        None = 0, Low, Medium, High
    }

    public enum SRPQuality
    {
        Low = 0, Medium, High
    }

    public enum RendererMode
    {
        Forward = 0, Deferred, UniversalRP, HDRP
    }

    public enum AntiAliasingMode
    {
        None = 0, MSAA, FXAA, SMAA, TAA
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
        public MWMaterialType MaterialType = MWMaterialType.PBR;
        public RendererMode RendererMode = RendererMode.Deferred;
        public SRPQuality SRPQuality = SRPQuality.High;
        public AntiAliasingMode AntiAliasingMode = AntiAliasingMode.TAA;
        public bool GenerateNormalMaps = true;
        public bool AnimateLights = true;
        public bool SunShadows = true;
        public bool LightShadows = true;
        public bool ExteriorLights = true;
        public float CameraFarClip = 500.0f;
        public int CellRadius = 2;
        public int CellDetailRadius = 2;
        public int CellRadiusOnLoad = 2;
        public Water.WaterMode WaterQuality = Water.WaterMode.Simple;
        public bool WaterTransparency = false;
        public bool KinematicRigidbody = true;
        public bool DayNightCycle = false;
        public bool FollowHead = true;
        public bool RoomScale = false;
        public float RenderScale = 1.0f;
        public bool HandTracking = false;

        public bool IsSRP()
        {
            var hdrp = RendererMode == RendererMode.HDRP;

#if !HDRP_ENABLED
            hdrp = false;
#endif

            return true;
        }

        public static void Save()
        {
            var instance = Get();
            instance.CheckSettings();

            var json = JsonUtility.ToJson(instance);
            PlayerPrefs.SetString(StorageKey, json);
            PlayerPrefs.Save();
        }

        public void CheckSettings()
        {
#if !HDRP_ENABLED
            if (RendererMode == RendererMode.HDRP)
                RendererMode = RendererMode.UniversalRP;
#endif

#if UNITY_ANDROID || UNITY_IOS
            // Avoid HDRP or Deferred Rendering on mobile
            if (RendererMode == RendererMode.HDRP)
                RendererMode = RendererMode.UniversalRP;
#endif

            // SMAA is not supported in VR.
            var xrEnabled = XRManager.IsXREnabled();
            if (xrEnabled && AntiAliasingMode == AntiAliasingMode.SMAA)
                AntiAliasingMode = AntiAliasingMode.TAA;

            // MSAA 4X max on mobile.
            if (AntiAliasingMode == AntiAliasingMode.MSAA && IsMobile())
                AntiAliasingMode = AntiAliasingMode.MSAA;

            // FXAA post process on mobile.
            if ((AntiAliasingMode == AntiAliasingMode.SMAA || AntiAliasingMode == AntiAliasingMode.TAA) && IsMobile())
                AntiAliasingMode = AntiAliasingMode.FXAA;
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
                Instance.PostProcessingQuality = PostProcessingQuality.None;
                Instance.LightShadows = false;
                Instance.ExteriorLights = false;
                Instance.CameraFarClip = 200;
                Instance.DayNightCycle = false;
                Instance.AntiAliasingMode = AntiAliasingMode.MSAA2X;
                Instance.RoomScale = false;
                Instance.CellDetailRadius = 2;
                Instance.CellRadius = 1;
                Instance.WaterTransparency = false;

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
                        Instance = JsonUtility.FromJson<GameSettings>(json);
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
            return $"/sdcard/{AndroidFolderName}";
#else
            return string.Empty;
#endif
        }

        #endregion
    }
}
