using UnityEngine;

namespace Demonixis.ToolboxV2
{
    public static class GameObjectUtils
    {
        public static GameObject Create(string name, Transform parent)
        {
            var go = new GameObject(name);
            var tr = go.transform;
            tr.parent = parent;
            tr.localPosition = Vector3.zero;
            tr.localRotation = Quaternion.identity;
            tr.localScale = Vector3.one;
            return go;
        }

        public static T CreateComponent<T>(Transform parent = null) where T : MonoBehaviour
        {
            var go = Create(typeof(T).Name, parent);
            return go.AddComponent<T>();
        }
    }
}
