using System.Collections.Generic;
using UnityEngine;

namespace TES3Unity.Graphics
{
    public class MeshMerger : MonoBehaviour
    {
        private void Start() => AdvancedMerge(gameObject);

        public static void AdvancedMerge(GameObject target)
        {
            var meshFilters = target.GetComponentsInChildren<MeshFilter>(false);
            var materials = new List<Material>();
            var meshRenderers = target.GetComponentsInChildren<MeshRenderer>(false);
            var subMeshes = new List<Mesh>();
            var finalCombiners = new List<CombineInstance>();
            var combiners = new List<CombineInstance>();

            // Caches
            CombineInstance combineInstance;
            Mesh mesh;

            foreach (var renderer in meshRenderers)
            {
                if (renderer.transform == target.transform)
                {
                    continue;
                }

                var localMaterials = renderer.sharedMaterials;
                foreach (var localMaterial in localMaterials)
                {
                    if (!materials.Contains(localMaterial))
                    {
                        materials.Add(localMaterial);
                    }
                }
            }

            foreach (var material in materials)
            {
                combiners.Clear();

                foreach (var filter in meshFilters)
                {
                    if (filter.transform == target.transform)
                    {
                        continue;
                    }

                    if (!filter.TryGetComponent<MeshRenderer>(out MeshRenderer renderer))
                    {
                        continue;
                    }

                    var localMaterials = renderer.sharedMaterials;
                    for (var i = 0; i < localMaterials.Length; i++)
                    {
                        if (localMaterials[i] != material)
                        {
                            continue;
                        }
                        
                        combineInstance = new CombineInstance();
                        combineInstance.mesh = filter.sharedMesh;
                        combineInstance.subMeshIndex = i;
                        combineInstance.transform = Matrix4x4.identity;
                        combiners.Add(combineInstance);
                    }
                }

                mesh = new Mesh();
                mesh.CombineMeshes(combiners.ToArray(), true);
                subMeshes.Add(mesh);
            }

            // Final Mesh Generation.
            foreach (var subMesh in subMeshes)
            {
                combineInstance = new CombineInstance();
                combineInstance.mesh = subMesh;
                combineInstance.subMeshIndex = 0;
                combineInstance.transform = Matrix4x4.identity;
                finalCombiners.Add(combineInstance);
            }

            var finalMesh = new Mesh();
            finalMesh.CombineMeshes(finalCombiners.ToArray(), false);

            var finalFilter = target.AddComponent<MeshFilter>();
            finalFilter.sharedMesh = finalMesh;

            var finalRenderer = target.AddComponent<MeshRenderer>();
            finalRenderer.sharedMaterials = materials.ToArray();

            foreach (var renderer in meshRenderers)
            {
                renderer.enabled = false;
            }

            target.isStatic = true;
        }
    }
}
