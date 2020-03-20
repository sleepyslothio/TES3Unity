using UnityEngine;
using UnityEngine.InputSystem;

namespace Wacki
{
    public sealed class IUILaserPointer : MonoBehaviour
    {
        private GameObject m_HitPoint = null;
        private GameObject m_Pointer = null;
        private bool m_Enabled = true;
        private bool m_Initialized = false;
        private float _distanceLimit;

        [SerializeField]
        private float m_LaserThickness = 0.002f;
        [SerializeField]
        private float m_LaserHitScale = 0.02f;
        [SerializeField]
        private bool m_LaserAlwaysOn = false;
        [SerializeField]
        private Color m_Color;

        public InputAction PressAction = null;

        public bool IsActive
        {
            get => m_Enabled;
            set
            {
                if (!m_Initialized)
                {
                    Start();
                }

                m_Enabled = value;
                m_HitPoint.SetActive(value);
                m_Pointer.SetActive(value);
            }
        }

        public bool LineVisible
        {
            get => m_Pointer.activeSelf;
            set
            {
                if (!m_Initialized)
                {
                    Start();
                }

                m_Pointer.GetComponent<MeshRenderer>().enabled = value;
            }
        }

        private void Start()
        {
            if (m_Initialized)
            {
                return;
            }

            m_Pointer = GameObject.CreatePrimitive(PrimitiveType.Cube);
            m_Pointer.transform.SetParent(transform, false);
            m_Pointer.transform.localScale = new Vector3(m_LaserThickness, m_LaserThickness, 100.0f);
            m_Pointer.transform.localPosition = new Vector3(0.0f, 0.0f, 50.0f);

            m_HitPoint = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            m_HitPoint.transform.SetParent(transform, false);
            m_HitPoint.transform.localScale = new Vector3(m_LaserHitScale, m_LaserHitScale, m_LaserHitScale);
            m_HitPoint.transform.localPosition = new Vector3(0.0f, 0.0f, 100.0f);

            m_HitPoint.SetActive(false);

            // remove the colliders on our primitives
            Object.DestroyImmediate(m_HitPoint.GetComponent<SphereCollider>());
            Object.DestroyImmediate(m_Pointer.GetComponent<BoxCollider>());

            Material newMaterial = new Material(Shader.Find("Wacki/LaserPointer"));

            newMaterial.SetColor("_Color", m_Color);
            m_Pointer.GetComponent<MeshRenderer>().material = newMaterial;
            m_HitPoint.GetComponent<MeshRenderer>().material = newMaterial;

            // register with the LaserPointerInputModule
            if (LaserPointerInputModule.instance == null)
            {
                new GameObject().AddComponent<LaserPointerInputModule>();
            }

            LaserPointerInputModule.instance.AddController(this);

            m_Initialized = true;
        }

        public void SetActive(bool isActive)
        {
            gameObject.SetActive(isActive);
        }

        private void Update()
        {
            if (!m_Enabled || m_Pointer == null)
            {
                return;
            }

            Ray ray = new Ray(transform.position, transform.forward);
            RaycastHit hitInfo;
            bool bHit = Physics.Raycast(ray, out hitInfo);

            float distance = 100.0f;

            if (bHit)
            {
                distance = hitInfo.distance;
            }

            // ugly, but has to do for now
            if (_distanceLimit > 0.0f)
            {
                distance = Mathf.Min(distance, _distanceLimit);
                bHit = true;
            }

            m_Pointer.transform.localScale = new Vector3(m_LaserThickness, m_LaserThickness, distance);
            m_Pointer.transform.localPosition = new Vector3(0.0f, 0.0f, distance * 0.5f);

            if (bHit)
            {
                m_HitPoint.SetActive(true);
                m_HitPoint.transform.localPosition = new Vector3(0.0f, 0.0f, distance);
            }
            else
            {
                m_HitPoint.SetActive(false);
            }

            // reset the previous distance limit
            _distanceLimit = -1.0f;
        }

        public void LimitLaserDistance(float distance)
        {
            if (distance < 0.0f)
            {
                return;
            }

            if (_distanceLimit < 0.0f)
            {
                _distanceLimit = distance;
            }
            else
            {
                _distanceLimit = Mathf.Min(_distanceLimit, distance);
            }
        }

        public void OnEnterControl(GameObject control)
        {
            // TODO: Add vibration.
        }

        public void OnExitControl(GameObject control)
        {
        }

        public bool ButtonDown()
        {
            if (PressAction == null)
            {
                return false;
            }

            return PressAction.phase == InputActionPhase.Started;
        }

        public bool ButtonUp()
        {
            if (PressAction == null)
            {
                return false;
            }

            return PressAction.phase == InputActionPhase.Canceled;
        }
    }
}