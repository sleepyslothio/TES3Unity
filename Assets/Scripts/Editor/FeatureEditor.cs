#if UNITY_EDITOR
using System;
using System.Linq;
using UnityEditor;
using UnityEditor.Build;
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

        private static readonly string[] AllSymbols = new string[]
        {
            "LEGACY_ENABLED", "LWRP_ENABLED", "HDRP_ENABLED"
        };

        private const string LWRPPackageName = "com.unity.render-pipelines.lightweight";
        private const string HDRPPackageName = "com.unity.render-pipelines.high-definition";

        private static bool[] EnabledFeatures;
        private static string[] FeatureNames;

        public int callbackOrder => 0;

        [MenuItem("TESUnityXR/Features Manager")]
        public static void ShowWindow() => GetWindow(typeof(FeatureEditor));

        private static void Initialize()
        {
            var definesString = PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup);

            EnabledFeatures = new bool[AllSymbols.Length];
            FeatureNames = new string[AllSymbols.Length];

            for (var i = 0; i < FeatureNames.Length; i++)
            {
                FeatureNames[i] = AllSymbols[i].Replace("_ENABLED", "").ToLower();
                FeatureNames[i] = Char.ToUpper(FeatureNames[i][0]) + FeatureNames[i].Substring(1);
            }

            UpdateEnabled();
        }

        private static void UpdateEnabled()
        {
            var definesString = PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup);

            for (var i = 0; i < EnabledFeatures.Length; i++)
                EnabledFeatures[i] = definesString.Contains(AllSymbols[i]);
        }

        private void OnGUI()
        {
            Initialize();

            GUILayout.Label($"Features", EditorStyles.boldLabel);

            for (var i = 0; i < FeatureNames.Length; i++)
                EnabledFeatures[i] = GUILayout.Toggle(EnabledFeatures[i], FeatureNames[i]);

            GUILayout.Label("Actions", EditorStyles.boldLabel);

            if (GUILayout.Button("Legacy"))
                SetupLegacy();

            if (GUILayout.Button("LWRP"))
                SetupLWRP();

            if (GUILayout.Button("HDRP"))
                SetupHDRP();

            if (GUILayout.Button("All"))
                SetupAll();
        }

        public void SetupLegacy()
        {
            Client.Remove(LWRPPackageName);
            Client.Remove(HDRPPackageName);
            RemoveAllSymbolsAdd("LEGACY_ENABLED");
            UpdateEnabled();
        }

        public void SetupLWRP()
        {
            Client.Add(LWRPPackageName);
            Client.Remove(HDRPPackageName);
            RemoveAllSymbolsAdd("LWRP_ENABLED");
            UpdateEnabled();
        }

        public void SetupHDRP()
        {
            Client.Remove(LWRPPackageName);
            Client.Add(HDRPPackageName);
            RemoveAllSymbolsAdd("HDRP_ENABLED");
            UpdateEnabled();
        }

        public void SetupAll()
        {
            Client.Add(LWRPPackageName);
            Client.Add(HDRPPackageName);
            AddDefines("LWRP_ENABLED", "HDRP_ENABLED", "LEGACY_ENABLED");
            UpdateEnabled();
        }

        private static void RemoveAllSymbolsAdd(string symbolToAdd)
        {
            var definesString = PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup);
            var symbols = definesString.Split(';').ToList();

            foreach (var symbol in AllSymbols)
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