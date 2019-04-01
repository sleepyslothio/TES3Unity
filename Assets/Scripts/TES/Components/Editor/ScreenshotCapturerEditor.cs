using UnityEditor;
using UnityEngine;

namespace TESUnity.Components.Utilities
{
    [CustomEditor(typeof(ScreenshotMaker))]
    public class ScreenshotCapturerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            var script = (ScreenshotMaker)target;

            if (GUILayout.Button("Capture Screenshot"))
                script.CaptureScreenshot();

            if (GUILayout.Button("Capture Screenshot 360"))
                script.Capture360Screenshot(false);

            if (GUILayout.Button("Capture Screenshot 360 3D"))
                script.Capture360Screenshot(true);
        }
    }
}
