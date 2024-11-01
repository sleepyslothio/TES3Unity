using Demonixis.ToolboxV2.Inputs;
using UnityEngine;

namespace Demonixis.ToolboxV2.XR
{
    public sealed class Teleporter : MonoBehaviour
    {
        private Transform _transform;
        private GameObject m_GroundMarker;
        private Transform m_GroundMarkerTransform;
        private GameObject m_TeleporterLine;
        private LineRenderer m_TeleporterLineRenderer;
        private Vector3? m_TargetPosition;
        private Transform m_RootTransform;
        private Transform m_RayPoint;
        private bool m_Pressed;

        [SerializeField] private GameObject m_GroundMarkerPrefab;
        [SerializeField] private GameObject m_TeleporterLinePrefab;
        [SerializeField] private float m_MaxDistance = 15.0f;

        private void Start()
        {
            _transform = transform;

            m_GroundMarker = Instantiate(m_GroundMarkerPrefab);
            m_GroundMarker.SetActive(false);
            m_GroundMarkerTransform = m_GroundMarker.transform;

            m_TeleporterLine = Instantiate(m_TeleporterLinePrefab);
            m_TeleporterLine.SetActive(false);
            m_TeleporterLineRenderer = m_TeleporterLine.GetComponent<LineRenderer>();

            m_RayPoint = new GameObject("TeleporterRayPoint").transform;
            m_RayPoint.parent = transform;
            m_RayPoint.localPosition = new Vector3(0.0f, 0.0f, 1.0f);
            m_RayPoint.localRotation = Quaternion.Euler(-35.0f, 0.0f, 0.0f);

            m_RootTransform = _transform.root;
        }
        
        public void SetHand(bool left)
        {
            var m_TeleportAction = InputSystemManager.GetAction("XR", $"Teleport-{(left ? "Left" : "Right")}");
            m_TeleportAction.started += (c) => m_Pressed = true;
            m_TeleportAction.canceled += (c) =>
            {
                m_Pressed = false;
                InputWasJustReleased();
            };
        }

        public void InputIsPressed()
        {
            m_Pressed = true;

            Debug.DrawRay(m_RayPoint.transform.position, -m_RayPoint.transform.up * 10);

            RaycastHit hit;
            if (Physics.Raycast(m_RayPoint.transform.position, -m_RayPoint.transform.up, out hit))
            {
                var target = hit.point;
                target.y = 0;

                m_TeleporterLineRenderer.SetPosition(0, _transform.position);

                // Limit the distance.
                if (Vector3.Distance(m_RootTransform.position, target) <= m_MaxDistance)
                {
                    m_TargetPosition = target;
                    m_GroundMarkerTransform.position = target;
                    m_TeleporterLineRenderer.SetPosition(1, target);
                }
                else
                {
                    m_TargetPosition = null;
                }
            }
            else
            {
                m_TargetPosition = null;
            }

            SetMarkerActive(m_TargetPosition != null);
        }

        private void SetMarkerActive(bool active)
        {
            if (m_GroundMarker.activeSelf != active)
            {
                m_GroundMarker.SetActive(active);
            }

            if (m_TeleporterLine.activeSelf != active)
            {
                m_TeleporterLine.SetActive(active);
            }
        }

        public void InputWasJustReleased()
        {
            m_Pressed = false;

            if (m_TargetPosition.HasValue)
            {
                transform.root.position = m_TargetPosition.Value;
                m_TargetPosition = null;
            }

            SetMarkerActive(false);
        }

        private void Update()
        {
            if (m_Pressed)
            {
                InputIsPressed();
            }
        }
    }
}