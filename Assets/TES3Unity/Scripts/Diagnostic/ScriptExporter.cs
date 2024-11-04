using System.IO;
using TES3Unity.ESM.Records;
using UnityEditor;
using UnityEngine;

namespace TES3Unity.Diagnostic
{
#if UNITY_EDITOR 
    public class ScriptExporter : MonoBehaviour
    {
        [MenuItem("Morrowind Unity/Export Scripts")]
        private static void ExportScripts()
        {
            if (Tes3Engine.DataReader == null)
            {
                Debug.LogWarning("Morrowind Data are not yet loaded. It'll take some time to load. The editor will be freezed a bit...");

                var dataPath = GameSettings.GetDataPath();
                Tes3Engine.DataReader = new TES3DataReader(dataPath);

                Debug.Log("Morrowind Data are now loaded!");
            }

            var exportPath = $"Exports/Scripts";
            var scripts = Tes3Engine.DataReader.FindRecords<SCPTRecord>();

            if (scripts == null)
            {
                Debug.LogError("Can't retrieve scripts from the ESM file. There is probably a big problem...");
                return;
            }

            Debug.Log($"Exporting {scripts.Length} scripts into {exportPath}");

            if (!Directory.Exists(exportPath))
            {
                Directory.CreateDirectory(exportPath);
            }

            foreach (var script in scripts)
            {
                // We use the .vb extention because it's easier to read scripts with a text editor in basic language mode.
                File.WriteAllText($"{exportPath}/{script.Header.Name}.vb", script.Text);
            }

            Debug.Log("Script export done.");
        }
    }
#endif
}
