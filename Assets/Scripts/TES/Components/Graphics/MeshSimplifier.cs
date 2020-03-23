using UnityEngine;

namespace TES3Unity.Graphics
{
    public class MeshSimplifier : MonoBehaviour
    {
        [SerializeField]
        private float m_Quality = 0.5f;

        private void Start()
        {
            Simplify();
        }

#if UNITY_EDITOR
        [ContextMenu("Simplify Mesh")]
#endif
        public void Simplify()
        {
            var filters = GetComponentsInChildren<MeshFilter>();

            foreach (var filter in filters)
            {
                filter.sharedMesh = SimplfyMesh(filter.sharedMesh, m_Quality);
            }
        }

        public static Mesh SimplfyMesh(Mesh sourceMesh, float quality = 0.5f)
        {
            var meshSimplifier = new UnityMeshSimplifier.MeshSimplifier();
            meshSimplifier.Initialize(sourceMesh);
            meshSimplifier.SimplifyMesh(quality);
            return meshSimplifier.ToMesh();
        }
    }
}
