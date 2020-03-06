using System;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using TESUnity.Rendering;
using TESUnity.ESS;
using System.Diagnostics;

namespace TESUnity
{
    using Logger = UnityEngine.Debug;

    public class TESManagerLite : MonoBehaviour
    {
        public MorrowindDataReader DataReader { get; private set; }
        public TextureManager TextureManager { get; private set; }
        public TESMaterial MaterialManager { get; private set; }
        public NIFManager NifManager { get; private set; }

        [SerializeField]
        public string[] m_AlternativeDataPaths = null;
        [SerializeField]
        public bool m_LoadExtensions = false;
        [SerializeField]
        private string[] m_LoadMods = null;
        [SerializeField]
        public string m_LoadSaveGameFilename = string.Empty;

        public event Action<ESSFile> SaveFileLoaded = null;
        public event Action<TESManagerLite> Initialized = null;

        private void Start()
        {
            var watch = new Stopwatch();
            var dataPath = GameSettings.GetDataPath();

            // Load the game from the alternative dataPath when in editor.
            if (!GameSettings.IsValidPath(dataPath))
            {
                dataPath = string.Empty;

                foreach (var alt in m_AlternativeDataPaths)
                {
                    if (GameSettings.IsValidPath(alt))
                        dataPath = alt;
                }
            }

            if (string.IsNullOrEmpty(dataPath))
            {
                SceneManager.LoadScene("Menu");
                enabled = false;
                return;
            }

            watch.Start();
            DataReader = new MorrowindDataReader(dataPath);
            watch.Stop();
            Logger.Log($"DataReader took {watch.Elapsed.Seconds} seconds to load.");
            
            TextureManager = new TextureManager(DataReader);
            MaterialManager = new TESMaterial(TextureManager);
            NifManager = new NIFManager(DataReader, MaterialManager);

            // Mod loading
            if (m_LoadMods != null && m_LoadMods.Length > 0)
            {
                watch.Reset();
                watch.Start();

                foreach (var mod in m_LoadMods)
                {
                    // TODO: load mod.
                }

                watch.Stop();
                Logger.Log($"Mods took {watch.Elapsed.Seconds} seconds to load.");
            }

            // Check for a previously saved game.
            if (m_LoadSaveGameFilename != null)
            {
                var path = $"{DataReader.FolderPath}\\Saves\\{m_LoadSaveGameFilename}.ess";

                if (File.Exists(path))
                {
                    var ess = new ESS.ESSFile(path);
                    SaveFileLoaded?.Invoke(ess);
                }
            }

            Initialized?.Invoke(this);
        }

        private void OnApplicationQuit()
        {
            DataReader?.Close();
        }
    }
}