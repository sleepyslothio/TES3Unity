using UnityEditor;
using UnityEngine;

namespace Demonixis.DVRSimulator.Gameplay
{
    public sealed class PlayerStart : MonoBehaviour
    {
        public byte PlayerIndex;

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            Handles.zTest = UnityEngine.Rendering.CompareFunction.Less;
            Handles.color = new Color(1, 0, 0, 0.4f);
            Handles.DrawSolidDisc(transform.position, transform.TransformDirection(Vector3.up), 0.5f);
            Handles.ArrowHandleCap(0, transform.position, transform.rotation, 1.5f, EventType.Repaint);
        }
#endif
    }
}
