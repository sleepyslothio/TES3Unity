using System.IO;
using TES3Unity.ESM.Records;
using UnityEditor;
using UnityEngine;

namespace TES3Unity.Diagnostic
{
    public class ScriptExporter : MonoBehaviour
    {
        [MenuItem("Morrowind Unity/Export Scripts")]
        private static void ExportScripts()
        {
            if (TES3Engine.MWDataReader == null)
            {
                Debug.LogWarning("Morrowind Data are not yet loaded. It'll take some time to load. The editor will be freezed a bit...");

                var dataPath = GameSettings.GetDataPath();



                // Load the game from the alternative dataPath when in editor.
                if (!GameSettings.IsValidPath(dataPath))
                {
                    dataPath = string.Empty;

#if UNITY_EDITOR
                    var manager = FindObjectOfType<TES3Engine>();
                    var alternativeDataPaths = manager?.AlternativeDataPaths ?? null;

                    if (alternativeDataPaths == null)
                    {
                        Debug.LogError("No valid path was found. You can try to add a TESManager component on the scene with an alternative path.");
                        return;
                    }

                    foreach (var alt in alternativeDataPaths)
                    {
                        if (GameSettings.IsValidPath(alt))
                        {
                            dataPath = alt;
                        }
                    }
#endif
                }

                TES3Engine.MWDataReader = new TES3DataReader(dataPath);

                Debug.Log("Morrowind Data are now loaded!");
            }

            var exportPath = $"Exports/Scripts";
            var scripts = TES3Engine.MWDataReader.FindRecords<SCPTRecord>();

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
}
