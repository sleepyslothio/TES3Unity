#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEngine;

namespace Demonixis.GunSpinningVR
{
    public sealed class FeatureEditor : EditorWindow
    {
        public enum RenderingPath
        {
            Legacy, LWRP, HDRP
        }

        public enum VRSDK
        {
            None = 0, OculusMobile, WaveVR
        }

        private static readonly string[] RenderingSymboles = new string[]
        {
            "LEGACY_ENABLED", "LWRP_ENABLED", "HDRP_ENABLED"
        };

        private static readonly string[] MobileVRSDKSymboles = new string[]
        {
             "OCULUS_SDK", "WAVEVR_SDK"
        };

        private static string WaveVRSDKPath => $"{Application.dataPath}/Vendors/WaveVR/Plugins";
        private const string LWRPPackageName = "com.unity.render-pipelines.lightweight";
        private const string HDRPPackageName = "com.unity.render-pipelines.high-definition";
        private const string OculusMobilePackage = "com.unity.xr.oculus.android";
        private const string OculusDesktopPackage = "com.unity.xr.oculus.standalone";
        private const string OpenVRPackage = "com.unity.xr.openvr.standalone";

        private static bool[] EnabledFeatures;
        private static string[] FeatureNames;

        public int callbackOrder => 0;

        [MenuItem("TESUnityXR/Features Manager")]
        public static void ShowWindow() => GetWindow(typeof(FeatureEditor));

        private static void Initialize()
        {
            var definesString = PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup);

            EnabledFeatures = new bool[RenderingSymboles.Length];
            FeatureNames = new string[RenderingSymboles.Length];

            for (var i = 0; i < FeatureNames.Length; i++)
            {
                FeatureNames[i] = RenderingSymboles[i].Replace("_ENABLED", "").ToLower();
                FeatureNames[i] = Char.ToUpper(FeatureNames[i][0]) + FeatureNames[i].Substring(1);
            }

            UpdateEnabled();
        }

        private static void UpdateEnabled()
        {
            var definesString = PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup);

            for (var i = 0; i < EnabledFeatures.Length; i++)
                EnabledFeatures[i] = definesString.Contains(RenderingSymboles[i]);
        }

        private void OnGUI()
        {
            Initialize();

            GUILayout.Label($"Features", EditorStyles.boldLabel);

            for (var i = 0; i < FeatureNames.Length; i++)
                EnabledFeatures[i] = GUILayout.Toggle(EnabledFeatures[i], FeatureNames[i]);

            GUILayout.Label("Rendering", EditorStyles.boldLabel);

            if (GUILayout.Button("Legacy"))
                SetupLegacy();

            if (GUILayout.Button("LWRP"))
                SetupLWRP();

            if (GUILayout.Button("HDRP"))
                SetupHDRP();

            if (GUILayout.Button("All"))
                SetupAll();

            GUILayout.Label("Mobile VR SDK", EditorStyles.boldLabel);

            if (GUILayout.Button("Enable Oculus"))
            {
                RemoveAllSymbolsAdd("OCULUS_SDK", true);
                SetVRSDK(VRSDK.OculusMobile);
            }

            if (GUILayout.Button("Enable WaveVR"))
            {
                RemoveAllSymbolsAdd("WAVEVR_SDK", true);
                SetVRSDK(VRSDK.WaveVR);
            }

            if (GUILayout.Button("Disable All VR SDK"))
            {
                RemoveAllSymbolsAdd(string.Empty, true);
                SetVRSDK(VRSDK.None);
            }
        }

        private void SetupMobile()
        {
            Client.Remove(OpenVRPackage);
            Client.Remove(OculusDesktopPackage);
        }

        public void SetupLegacy()
        {
            Client.Remove(LWRPPackageName);
            Client.Remove(HDRPPackageName);
            RemoveAllSymbolsAdd("LEGACY_ENABLED", false);
            UpdateEnabled();
        }

        public void SetupLWRP()
        {
            Client.Add(LWRPPackageName);
            Client.Remove(HDRPPackageName);
            RemoveAllSymbolsAdd("LWRP_ENABLED", false);
            UpdateEnabled();
        }

        public void SetupHDRP()
        {
            Client.Remove(LWRPPackageName);
            Client.Add(HDRPPackageName);
            RemoveAllSymbolsAdd("HDRP_ENABLED", false);
            UpdateEnabled();
        }

        public void SetupAll()
        {
            Client.Add(LWRPPackageName);
            Client.Add(HDRPPackageName);
            AddDefines("LWRP_ENABLED", "HDRP_ENABLED", "LEGACY_ENABLED");
            UpdateEnabled();
        }

        public void SetVRSDK(VRSDK vrSDK)
        {
            if (vrSDK == VRSDK.None)
            {
                EnablePluginFolder(VRSDK.None);
                SetAndroidManifest(null);
            }
            else if (vrSDK == VRSDK.WaveVR)
            {
                PlayerSettings.SetVirtualRealitySupported(BuildTargetGroup.Android, false);
                PlayerSettings.SetVirtualRealitySDKs(BuildTargetGroup.Android, null);
                Client.Remove(OculusMobilePackage);

                SetAndroidManifest("wavevr");
            }
            else if (vrSDK == VRSDK.OculusMobile)
            {
                PlayerSettings.SetVirtualRealitySupported(BuildTargetGroup.Android, true);

                var sdks = PlayerSettings.GetAvailableVirtualRealitySDKs(BuildTargetGroup.Android);

                foreach (var sdk in sdks)
                {
                    if (sdk.ToLower().Contains("oculus"))
                    {
                        PlayerSettings.SetVirtualRealitySDKs(BuildTargetGroup.Android, new string[] { sdk });
                        Client.Add(OculusMobilePackage);
                        EnablePluginFolder(VRSDK.OculusMobile);
                        SetAndroidManifest("oculus");
                    }
                }
            }

            EnablePluginFolder(vrSDK);
        }

        private static void EnablePluginFolder(VRSDK id)
        {
            try
            {
                // Disable non required plugins.
                if (id != VRSDK.WaveVR && Directory.Exists(WaveVRSDKPath))
                    Directory.Move(WaveVRSDKPath, $"{WaveVRSDKPath}~");

                // Enable required plugins.
                if (id == VRSDK.WaveVR && Directory.Exists($"{WaveVRSDKPath}~"))
                    Directory.Move($"{WaveVRSDKPath}~", $"{WaveVRSDKPath}");
            }
            catch (Exception ex)
            {
                Debug.Log(ex.Message);
            }
        }

        private static void SetAndroidManifest(string target)
        {
            var androidPluginsPath = $"{Application.dataPath}/Plugins/Android";
            var androidManifestPath = $"{androidPluginsPath}/AndroidManifest.xml";

            if (target == null)
            {
                File.Delete(androidManifestPath);
                return;
            }

            try
            {
                var manifestContent = File.ReadAllText($"{androidPluginsPath}/_AndroidManifest-{target}.xml");
                File.WriteAllText(androidManifestPath, manifestContent);
            }
            catch (Exception ex)
            {
                Debug.Log(ex.Message);
            }
        }

        private static void RemoveAllSymbolsAdd(string symbolToAdd, bool mobile)
        {
            var definesString = PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup);
            var symbols = definesString.Split(';').ToList();
            var collection = mobile ? MobileVRSDKSymboles : RenderingSymboles;

            foreach (var symbol in collection)
                symbols.Remove(symbol);

            if (!string.IsNullOrEmpty(symbolToAdd))
                symbols.Add(symbolToAdd);

            PlayerSettings.SetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup, string.Join(";", symbols.ToArray()));
        }

        public static void AddDefines(params string[] symbolsToAdd)
        {
            var definesString = PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup);
            var symbols = definesString.Split(';').ToList();


            if (symbolsToAdd == null)
                return;

            foreach (var symbol in symbolsToAdd)
            {
                if (!symbols.Contains(symbol))
                    symbols.AddRange(symbolsToAdd);
            }

            PlayerSettings.SetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup, string.Join(";", symbols.ToArray()));
        }
    }
}
#endif