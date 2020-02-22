using System.Collections.Generic;
using System.Threading.Tasks;
using TESUnity.NIF;
using TESUnity.Rendering;
using UnityEngine;

namespace TESUnity
{
    /// <summary>
    /// Manages loading and instantiation of NIF models.
    /// </summary>
    public sealed class NIFManager
    {
        private MorrowindDataReader _dataReader;
        private MaterialManager _materialManager;
        private GameObject _prefabContainerObj;
        private Dictionary<string, GameObject> nifPrefabs = new Dictionary<string, GameObject>();

        public NIFManager(MorrowindDataReader dataReader, MaterialManager materialManager)
        {
            _dataReader = dataReader;
            _materialManager = materialManager;
        }

        private void EnsurePrefabContainerObjectExists()
        {
            if (_prefabContainerObj == null)
            {
                _prefabContainerObj = new GameObject("NIF Prefabs");
                _prefabContainerObj.SetActive(false);
            }
        }

        /// <summary>
        /// Instantiates a NIF file.
        /// </summary>
        public GameObject InstantiateNIF(string filePath)
        {
            EnsurePrefabContainerObjectExists();

            // Get the prefab.
            GameObject prefab;
            if (!nifPrefabs.TryGetValue(filePath, out prefab))
            {
                // Load & cache the NIF prefab.
                prefab = LoadNifPrefabDontAddToPrefabCache(filePath);
                nifPrefabs[filePath] = prefab;
            }

            // Instantiate the prefab.
            return GameObject.Instantiate(prefab);
        }

        public void PreloadNifFileAsync(string filePath)
        {
        }

        private GameObject LoadNifPrefabDontAddToPrefabCache(string filePath)
        {
            Debug.Assert(!nifPrefabs.ContainsKey(filePath));

            PreloadNifFileAsync(filePath);

            var file = _dataReader.LoadNif(filePath);
            var objBuilder = new NIFObjectBuilder(file, _materialManager);
            var prefab = objBuilder.BuildObject();
            prefab.transform.parent = _prefabContainerObj.transform;

            // Add LOD support to the prefab.
            var lodGroup = prefab.AddComponent<LODGroup>();
            lodGroup.SetLODs(new LOD[1]
            {
                new LOD(0.015f, prefab.GetComponentsInChildren<Renderer>())
            });

            return prefab;
        }
    }
}