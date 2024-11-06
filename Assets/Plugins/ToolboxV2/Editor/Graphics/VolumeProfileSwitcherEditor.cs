#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace Demonixis.ToolboxV2.Graphics
{
    [CustomEditor(typeof(VolumeProfileSwitcher))]
    public class VolumeProfileSwitcherEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            var script = (VolumeProfileSwitcher)target;

            if (GUILayout.Button("Setup PC"))
            {
                script.SetDesktopProfile();
            }

            if (GUILayout.Button("Setup Mobile"))
            {
                script.SetMobileProfile();
            }
        }
    }
}
#endif