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
            URP, HDRP
        }

        private static readonly string[] RenderingSymboles = new string[]
        {
            "HDRP_ENABLED"
        };

        private const string LWRPPackageName = "com.unity.render-pipelines.lightweight";
        private const string HDRPPackageName = "com.unity.render-pipelines.high-definition";

        public int callbackOrder => 0;

        [MenuItem("TESUnityXR/Features Manager")]
        public static void ShowWindow() => GetWindow(typeof(FeatureEditor));

        private void OnGUI()
        {
            GUILayout.Label("Rendering", EditorStyles.boldLabel);

            if (GUILayout.Button("LWRP"))
                SetupLWRP();

            if (GUILayout.Button("HDRP"))
                SetupHDRP();

            if (GUILayout.Button("All"))
                SetupAll();
        }

        public void SetupLWRP()
        {
            Client.Add(LWRPPackageName);
            Client.Remove(HDRPPackageName);
        }

        public void SetupHDRP()
        {
            Client.Remove(LWRPPackageName);
            Client.Add(HDRPPackageName);
            RemoveAllSymbolsAdd("HDRP_ENABLED", false);
        }

        public void SetupAll()
        {
            Client.Add(LWRPPackageName);
            Client.Add(HDRPPackageName);
            AddDefines("HDRP_ENABLED");
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