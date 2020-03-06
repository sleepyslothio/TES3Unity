using UnityEngine;

namespace TESUnity.Diagnostic
{
    public class ModelLoader : MonoBehaviour
    {
        [SerializeField]
        private string m_ModelToLoad = null;

        private void Awake()
        {
            var tes = GetComponent<TESManagerLite>();
            tes.Initialized += Tes_Initialized;
        }

        private void Tes_Initialized(TESManagerLite tes)
        {
            if (string.IsNullOrEmpty(m_ModelToLoad))
            {
                Debug.LogError("No model to load.");
                return;
            }

            var path = $"meshes\\{m_ModelToLoad.Replace("/", "\\")}";

            Debug.Log($"Loading model {path}");

            var model = tes.NifManager.InstantiateNIF(path);

            var colliders = model.GetComponentsInChildren<Collider>();
            var maxY = 0.0f;

            foreach (var collider in colliders)
            {
                maxY = Mathf.Max(maxY, collider.bounds.size.y);
            }

            model.transform.Translate(0, maxY, 0);
        }
    }
}