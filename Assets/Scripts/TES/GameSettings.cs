using Demonixis.Toolbox.XR;
using System;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

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

    [Serializable]
    public sealed class GameSettings
    {
        private const string MorrowindPathKey = "tesunity.path";
        private const string StorageKey = "tesunity.settings";
        private const string ConfigFile = "config.ini"; // Deprecated
        private const string MWDataPathName = "MorrowindDataPath";
        private static GameSettings Instance = null;

        public bool Audio = true;
        public PostProcessingQuality PostProcessing = PostProcessingQuality.High;
        public MWMaterialType Material = MWMaterialType.PBR;
        public bool GenerateNormalMaps = true;
        public bool AnimateLights = true;
        public bool SunShadows = true;
        public bool LightShadows = false;
        public bool ExteriorLight = false;
        public float CameraFarClip = 1000;
        public int CellRadius = 2;
        public int CellDistance = 2;
        public bool VRFollowHead = true;
        public bool VRRoomScale = false;
        public float RenderScale = 1.0f;

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
                Instance.Material = MWMaterialType.Standard;
                Instance.PostProcessing = PostProcessingQuality.None;
                Instance.LightShadows = false;
                Instance.ExteriorLight = false;
                Instance.CameraFarClip = 200;
                Instance.VRRoomScale = false;
                Instance.CellDistance = 2;
                Instance.CellRadius = 1;

                if (XRManager.Enabled)
                {
                    Instance.CameraFarClip = 50;
                    Instance.Material = MWMaterialType.Unlit;
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
            SaveValue(MWDataPathName, dataPath);
        }

        public static string GetDataPath()
        {
            var path = PlayerPrefs.GetString(MorrowindPathKey);

            if (!string.IsNullOrEmpty(path))
                return path;

#if UNITY_STANDALONE || UNITY_EDITOR
            // Deprecated
            {
                if (!File.Exists("config.ini"))
                    return string.Empty;

                var lines = File.ReadAllLines(ConfigFile);
                foreach (var line in lines)
                {
                    if (line.Contains(MWDataPathName))
                    {
                        var tmp = line.Split('=');
                        if (tmp.Length == 2)
                            return tmp[1].Trim();
                    }
                }
            }
            return string.Empty;
#elif UNITY_ANDROID
            return "/sdcard/TESUnityXR";
#else
            return Application.persistentDataPath;
#endif
        }

        #endregion

        #region Deprected soon


        public static void SaveValue(string parameter, string value)
        {
#if UNITY_STANDALONE || UNITY_EDITOR
            if (!File.Exists(ConfigFile))
                return;

            var lines = File.ReadAllLines(ConfigFile);

            for (var i = 0; i < lines.Length; i++)
            {
                if (lines[i].Contains(parameter))
                {
                    lines[i] = string.Format("{0} = {1}", parameter, value);
                    break;
                }
            }

            File.WriteAllLines(ConfigFile, lines);
#endif
        }

        /// <summary>
        /// Checks if a file named Config.ini is located left to the main executable.
        /// Open/Parse it and configure default values.
        /// </summary>
        public static string CheckSettings(TESManager tes)
        {
#if UNITY_STANDALONE || UNITY_EDITOR
            var path = Application.persistentDataPath;
            var lines = File.ReadAllLines(ConfigFile);
            var temp = new string[2];
            var value = string.Empty;

            foreach (var line in lines)
            {
                temp = line.Split('=');

                if (temp.Length == 2)
                {
                    value = temp[1].Trim();

                    switch (temp[0].Trim())
                    {
                        case "AntiAliasing":
                            {
                                int result;
                                if (int.TryParse(value, out result))
                                {
                                    if (result >= 0 && result < 4)
                                        tes.antiAliasing = (PostProcessLayer.Antialiasing)result;
                                }
                            }
                            break;
                        case "PostProcessQuality":
                            {
                                int result;
                                if (int.TryParse(value, out result))
                                {
                                    if (result >= 0 && result < 4)
                                        tes.postProcessingQuality = (PostProcessingQuality)result;
                                }
                            }
                            break;
                        case "AnimateLights": ParseBool(value, ref tes.animateLights); break;
                        case "MorrowindDataPath": path = value; break;
                        case "FollowHeadDirection": ParseBool(value, ref tes.followHeadDirection); break;
                        case "SunShadows": ParseBool(value, ref tes.renderSunShadows); break;
                        case "LightShadows": ParseBool(value, ref tes.renderLightShadows); break;
                        case "PlayMusic": ParseBool(value, ref tes.playMusic); break;
                        case "RenderExteriorCellLights": ParseBool(value, ref tes.renderExteriorCellLights); break;
                        case "WaterBackSideTransparent": ParseBool(value, ref tes.waterBackSideTransparent); break;
                        case "RenderPath":
                            if (value == "Forward")
                                tes.renderPath = RendererType.Forward;
                            else if (value == "Deferred")
                                tes.renderPath = RendererType.Deferred;
                            else if (value == "Lightweight")
                                tes.renderPath = RendererType.LightweightRP;
                            break;
                        case "Shader":
                            switch (value)
                            {
                                case "PBR": tes.materialType = MWMaterialType.PBR; break;
                                default: tes.materialType = MWMaterialType.Standard; break;
                            }
                            break;
                        case "RoomScale": ParseBool(value, ref tes.roomScale); break;
                        case "ForceControllers": ParseBool(value, ref tes.forceControllers); break;
                        case "CreaturesEnabled": ParseBool(value, ref tes.creaturesEnabled); break;
                        case "CameraFarClip": ParseFloat(value, ref tes.cameraFarClip, 5); break;
                        case "WaterQuality":
                            {
                                int result;
                                if (int.TryParse(value, out result))
                                {
                                    if (result > -1 && result < 3)
                                        tes.waterQuality = (UnityStandardAssets.Water.Water.WaterMode)result;
                                }
                            }
                            break;

                        case "DayNightCycle": ParseBool(value, ref tes.dayNightCycle); break;
                        case "GenerateNormalMap": ParseBool(value, ref tes.generateNormalMap); break;
                        case "NormalGeneratorIntensity": ParseFloat(value, ref tes.normalGeneratorIntensity); break;
                        case "RenderScale": ParseFloat(value, ref tes.renderScale, 0.1f, 4.0f); break;
                        case "SRPQuality":
                            {
                                int result;
                                if (int.TryParse(value, out result))
                                {
                                    if (result > -1 && result < 3)
                                        tes.srpQuality = (SRPQuality)result;
                                }
                            }
                            break;

                        case "CellRadius": ParseInt(value, ref tes.cellRadius, 1); break;
                        case "CellDetailRadius": ParseInt(value, ref tes.cellDetailRadius, 1); break;
                        case "CellRadiusOnLoad": ParseInt(value, ref tes.cellRadiusOnLoad, 1); break;
                        default: break;
                    }
                }
            }

            return path;
#else
            return Application.persistentDataPath;
#endif
        }

        public static void CreateConfigFile()
        {
#if UNITY_STANDALONE || UNITY_EDITOR
            var sb = new StringBuilder();
            sb.Append("# TESUnity Configuration File\r\n");
            sb.Append(string.Format("{0} = \r\n", MWDataPathName));
            sb.Append("\r\n");
            File.WriteAllText(ConfigFile, sb.ToString());
#endif
        }

        private static bool ParseBool(string value, bool defaultValue)
        {
            var val = value.ToLower();
            if (val == "true" || val == "1")
                return true;
            else if (val == "false" || value == "0")
                return false;

            return defaultValue;
        }

        private static void ParseBool(string key, ref bool value)
        {
            var val = key.ToLower();
            if (val == "true" || val == "1")
                value = true;
            else if (val == "false" || key == "0")
                value = false;
        }

        private static int ParseInt(string value, int defaultValue)
        {
            int result;

            if (int.TryParse(value, out result))
                return result;

            return defaultValue;
        }

        private static void ParseInt(string key, ref int value, int? min = null, int? max = null)
        {
            int result;

            if (int.TryParse(key, out result))
            {
                var valid = true;

                if (min.HasValue)
                    valid &= result >= min;

                if (max.HasValue)
                    valid &= result <= max;

                if (valid)
                    value = result;
            }
        }

        private static float ParseFloat(string value, float defaultValue)
        {
            float result;

            if (float.TryParse(value, out result))
                return result;

            return defaultValue;
        }

        private static void ParseFloat(string key, ref float value, float? min = null, float? max = null)
        {
            float result;

            if (float.TryParse(key, out result))
            {
                var valid = true;

                if (min.HasValue)
                    valid &= result >= min;

                if (max.HasValue)
                    valid &= result <= max;

                if (valid)
                    value = result;
            }
        }

        #endregion
    }
}
