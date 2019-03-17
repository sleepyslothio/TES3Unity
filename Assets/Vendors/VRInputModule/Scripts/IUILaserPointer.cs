using UnityEngine;

namespace Wacki
{
    abstract public class IUILaserPointer : MonoBehaviour
    {
        public float laserThickness = 0.002f;
        public float laserHitScale = 0.02f;
        public bool laserAlwaysOn = false;
        public Color color;

        protected GameObject m_hitPoint = null;
        protected GameObject m_pointer = null;
        protected bool m_enabled = true;
        protected bool m_initialized = false;

        public bool IsActive
        {
            get { return m_enabled; }
            set
            {
                if (!m_initialized)
                    Start();

                m_enabled = value;
                m_hitPoint.SetActive(value);
                m_pointer.SetActive(value);
            }
        }

        public bool LineVisible
        {
            get { return m_pointer.activeSelf; }
            set
            {
                if (!m_initialized)
                    Start();

                m_pointer.GetComponent<MeshRenderer>().enabled = value;
            }
        }

        public void SetActive(bool isActive)
        {
            gameObject.SetActive(isActive);
        }

        private float _distanceLimit;

        // Use this for initialization
        private void Start()
        {
            if (m_initialized)
                return;

            m_pointer = GameObject.CreatePrimitive(PrimitiveType.Cube);
            m_pointer.transform.SetParent(transform, false);
            m_pointer.transform.localScale = new Vector3(laserThickness, laserThickness, 100.0f);
            m_pointer.transform.localPosition = new Vector3(0.0f, 0.0f, 50.0f);

            m_hitPoint = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            m_hitPoint.transform.SetParent(transform, false);
            m_hitPoint.transform.localScale = new Vector3(laserHitScale, laserHitScale, laserHitScale);
            m_hitPoint.transform.localPosition = new Vector3(0.0f, 0.0f, 100.0f);

            m_hitPoint.SetActive(false);

            // remove the colliders on our primitives
            Object.DestroyImmediate(m_hitPoint.GetComponent<SphereCollider>());
            Object.DestroyImmediate(m_pointer.GetComponent<BoxCollider>());

            Material newMaterial = new Material(Shader.Find("Wacki/LaserPointer"));

            newMaterial.SetColor("_Color", color);
            m_pointer.GetComponent<MeshRenderer>().material = newMaterial;
            m_hitPoint.GetComponent<MeshRenderer>().material = newMaterial;
            // initialize concrete class
            Initialize();

            // register with the LaserPointerInputModule
            if (LaserPointerInputModule.instance == null)
                new GameObject().AddComponent<LaserPointerInputModule>();

            LaserPointerInputModule.instance.AddController(this);

            m_initialized = true;
        }

        protected virtual void Update()
        {
            if (!m_enabled || m_pointer == null)
                return;

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

            m_pointer.transform.localScale = new Vector3(laserThickness, laserThickness, distance);
            m_pointer.transform.localPosition = new Vector3(0.0f, 0.0f, distance * 0.5f);

            if (bHit)
            {
                m_hitPoint.SetActive(true);
                m_hitPoint.transform.localPosition = new Vector3(0.0f, 0.0f, distance);
            }
            else
                m_hitPoint.SetActive(false);

            // reset the previous distance limit
            _distanceLimit = -1.0f;
        }

        // limits the laser distance for the current frame
        public virtual void LimitLaserDistance(float distance)
        {
            if (distance < 0.0f)
                return;

            if (_distanceLimit < 0.0f)
                _distanceLimit = distance;
            else
                _distanceLimit = Mathf.Min(_distanceLimit, distance);
        }

        protected virtual void Initialize() { }
        public virtual void OnEnterControl(GameObject control) { }
        public virtual void OnExitControl(GameObject control) { }
        abstract public bool ButtonDown();
        abstract public bool ButtonUp();
    }
}