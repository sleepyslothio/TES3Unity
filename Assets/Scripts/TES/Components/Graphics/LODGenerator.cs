using UnityEngine;

namespace TES3Unity.Graphics
{
    public class LODGenerator : MonoBehaviour
    {
        private void Start()
        {
            var lodGroup = GetComponent<LODGroup>();

            if (lodGroup == null)
            {
                lodGroup = gameObject.AddComponent<LODGroup>();
            }

            var filters = GetComponentsInChildren<MeshFilter>();
            var renderer = GetComponentsInChildren<Renderer>();

            var lods = new LOD[]
            {
                new LOD(0.6f, renderer),
                new LOD(0.2f, GenerateLODs(filters, 0.9f)),
                new LOD(0.01f, GenerateLODs(filters, 0.8f)),
            };

            lodGroup.SetLODs(lods);
            lodGroup.RecalculateBounds();
        }

        private static Renderer[] GenerateLODs(MeshFilter[] filters, float quality)
        {
            var lods = new Renderer[filters.Length];

            for (var i = 0; i < filters.Length; i++)
            {
                lods[i] = GenerateLOD(filters[i], quality);
            }

            return lods;
        }

        public static Renderer GenerateLOD(MeshFilter filter, float quality)
        {
            var renderer = filter.GetComponent<MeshRenderer>();

            var mesh = filter.sharedMesh;
            var simplified = MeshSimplifier.SimplfyMesh(mesh, quality);

            var go = new GameObject($"{filter.name}_LOD_{quality}");
            go.isStatic = filter.gameObject.isStatic;

            var trGo = go.transform;
            var trFilter = filter.transform;
            trGo.parent = trFilter.parent;
            trGo.localPosition = trFilter.localPosition;
            trGo.localRotation = trFilter.localRotation;
            trGo.localScale = trFilter.localScale;

            var newFilter = go.AddComponent<MeshFilter>();
            newFilter.sharedMesh = simplified;

            var newRenderer = go.AddComponent<MeshRenderer>();
            newRenderer.sharedMaterials = renderer.sharedMaterials;
            newRenderer.enabled = renderer.enabled;

            return newRenderer;
        }
    }
}
