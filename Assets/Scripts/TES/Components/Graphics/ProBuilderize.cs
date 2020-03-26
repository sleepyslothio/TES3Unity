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

            foreach (var filter in filters)
            {
                var mesh = filter.gameObject.AddComponent<ProBuilderMesh>();

                var importer = new MeshImporter(mesh);
                importer.Import(filter.sharedMesh);

                filter.sharedMesh = new Mesh();

                mesh.ToMesh();
                mesh.Refresh();
            }

            if (m_Merge)
            {
                Merge();
            }
        }

#if UNITY_EDITOR
        [ContextMenu("Merge")]
#endif
        private void Merge()
        {
            var probuilderMeshes = GetComponentsInChildren<ProBuilderMesh>();
            var current = gameObject.AddComponent<ProBuilderMesh>();
            CombineMeshes.Combine(probuilderMeshes, current);
        }
    }
}
