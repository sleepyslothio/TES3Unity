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
            LegacyForward, LegacyDeferred, LWRP, HDRP
        }

        private static readonly string[] AllSymbols = new string[]
        {
            "LWRP_ENABLED", "HDRP_ENABLED"
        };

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

            for (var i = 0; i < EnabledFeatures.Length; i++)
                EnabledFeatures[i] = definesString.Contains(AllSymbols[i]);
        }

        private void OnGUI()
        {
            Initialize();

            GUILayout.Label($"Features:", EditorStyles.boldLabel);

            for (var i = 0; i < FeatureNames.Length; i++)
            {
                var previous = EnabledFeatures[i];

                EnabledFeatures[i] = GUILayout.Toggle(EnabledFeatures[i], FeatureNames[i]);

                if (previous != EnabledFeatures[i])
                    ChangeDefine(AllSymbols[i], EnabledFeatures[i]);
            }

            GUILayout.Label("Platforms:", EditorStyles.boldLabel);

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
            Client.Remove("com.unity.xr.openvr.standalone");
        }

        public void SetupLWRP()
        {
            Client.Add("com.unity.xr.oculus.standalone");
            ChangeDefine("LWRP_ENABLED", true);
        }

        public void SetupHDRP()
        {
            ChangeDefine("HDRP_ENABLED", true);
        }

        public void SetupAll()
        {
            ChangeDefine("LWRP_ENABLED", true);
            ChangeDefine("HDRP_ENABLED", true);
            ChangeDefine("LEGACY_ENABLED", true);
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

        public static void ChangeDefine(string symbol, bool add)
        {
            var definesString = PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup);
            var symbols = definesString.Split(';').ToList();

            if (add)
            {
                if (symbol == null || symbols.Contains(symbol))
                    return;

                symbols.Add(symbol);
            }
            else
            {
                if (symbol == null || !symbols.Contains(symbol))
                    return;

                symbols.Remove(symbol);
            }

            PlayerSettings.SetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup, string.Join(";", symbols.ToArray()));
        }
    }
}
#endif