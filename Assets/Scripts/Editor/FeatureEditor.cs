#if UNITY_EDITOR
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
            UniversalRP, HDRP
        }

        private static readonly string[] RenderingSymboles = new string[]
        {
            "HDRP_ENABLED"
        };

        private const string HDRPPackageName = "com.unity.render-pipelines.high-definition";

        public int callbackOrder => 0;

        [MenuItem("TESUnityXR/Features Manager")]
        public static void ShowWindow() => GetWindow(typeof(FeatureEditor));

        private void OnGUI()
        {
            GUILayout.Label("Rendering", EditorStyles.boldLabel);

            if (GUILayout.Button("Enable HDRP"))
            {
                Client.Add(HDRPPackageName);
                RemoveAllSymbolsAdd("HDRP_ENABLED", false);
            }

            if (GUILayout.Button("Disable HDRP"))
            {
                Client.Remove(HDRPPackageName);
                RemoveAllSymbolsAdd(null, false);
            }
        }
 
        private static void RemoveAllSymbolsAdd(string symbolToAdd, bool mobile)
        {
            var definesString = PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup);
            var symbols = definesString.Split(';').ToList();

            foreach (var symbol in RenderingSymboles)
                symbols.Remove(symbol);

            if (!string.IsNullOrEmpty(symbolToAdd))
                symbols.Add(symbolToAdd);

            PlayerSettings.SetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup, string.Join(";", symbols.ToArray()));
        }
    }
}
#endif