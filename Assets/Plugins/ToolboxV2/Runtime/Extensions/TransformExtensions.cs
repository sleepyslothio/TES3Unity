using UnityEngine;

namespace Demonixis.ToolboxV2
{
    public static class TransformExtensions
    {
        public static Transform FindChildRecursiveExact(this Transform parent, string name)
        {
            if (parent.childCount > 0)
            {
                foreach (Transform child in parent)
                {
                    if (child.name == name)
                    {
                        return child;
                    }

                    var result = child.FindChildRecursiveExact(name);
                    if (result != null)
                    {
                        return result;
                    }
                }
            }

            return null;
        }

        public static void ClearChildren(this Transform transform)
        {
            foreach (Transform child in transform)
            {
                Object.Destroy(child.gameObject);
            }
        }
    }
}