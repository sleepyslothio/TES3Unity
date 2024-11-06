using UnityEngine;

namespace Demonixis.ToolboxV2.Utils
{
    public sealed class AutoRotation : MonoBehaviour
    {
        private Transform _transform;
        public Vector3 axis = Vector3.up;
        public float speed = 25.0f;

        private void Start()
        {
            _transform = GetComponent(typeof(Transform)) as Transform;
        }

        private void Update()
        {
            _transform.Rotate(axis, Time.deltaTime * speed);
        }
    }
}