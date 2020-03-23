using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ProBuilder;
using UnityEngine.ProBuilder.MeshOperations;

namespace TES3Unity.Graphics
{
    public class ProBuilderize : MonoBehaviour
    {
        [SerializeField]
        private bool m_Merge = false;

        private void Start()
        {
            var filters = GetComponentsInChildren<MeshFilter>();
            var count = filters.Length;
            var probuilderMeshes = new List<ProBuilderMesh>(count);

            MeshImporter importer = null;

            for(var i = 0; i < count; i++)
            {
                importer = new MeshImporter(filters[i].gameObject);
                importer.Import();

                probuilderMeshes.Add(filters[i].GetComponent<ProBuilderMesh>());
            }

            if (m_Merge)
            {
                var mesh = gameObject.AddComponent<ProBuilderMesh>();
                CombineMeshes.Combine(probuilderMeshes, mesh);
            }
        }
    }
}
