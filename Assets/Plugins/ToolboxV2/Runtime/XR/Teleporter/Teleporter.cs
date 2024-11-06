using Demonixis.ToolboxV2.Inputs;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Demonixis.ToolboxV2.XR
{
    public sealed class Teleporter : MonoBehaviour
    {
        private Transform _transform;
        private GameObject _marker;
        private Transform _markerTranform;
        private GameObject _teleporterLine;
        private LineRenderer _lineRenderer;
        private Vector3? _targetPosition;
        private Transform _rootTransform;
        private Transform _rayPoint;

        [SerializeField] private GameObject m_GroundMarkerPrefab;
        [SerializeField] private GameObject m_TeleporterLinePrefab;
        [SerializeField] private float m_MaxDistance = 15.0f;

        private void Start()
        {
            _transform = transform;

            _marker = Instantiate(m_GroundMarkerPrefab);
            _marker.SetActive(false);
            _markerTranform = _marker.transform;

            _teleporterLine = Instantiate(m_TeleporterLinePrefab);
            _teleporterLine.SetActive(false);
            _lineRenderer = _teleporterLine.GetComponent<LineRenderer>();

            _rayPoint = new GameObject("TeleporterRayPoint").transform;
            _rayPoint.parent = transform;
            _rayPoint.localPosition = new Vector3(0.0f, 0.0f, 1.0f);
            _rayPoint.localRotation = Quaternion.Euler(-35.0f, 0.0f, 0.0f);

            _rootTransform = _transform.root;
        }
        
        public void InputIsPressed()
        {
            Debug.DrawRay(_rayPoint.transform.position, -_rayPoint.transform.up * 10);

            RaycastHit hit;
            if (Physics.Raycast(_rayPoint.transform.position, -_rayPoint.transform.up, out hit))
            {
                var target = hit.point;
                target.y = 0;

                _lineRenderer.SetPosition(0, _transform.position);

                // Limit the distance.
                if (Vector3.Distance(_rootTransform.position, target) <= m_MaxDistance)
                {
                    _targetPosition = target;
                    _markerTranform.position = target;
                    _lineRenderer.SetPosition(1, target);
                }
                else
                {
                    _targetPosition = null;
                }
            }
            else
            {
                _targetPosition = null;
            }

            SetMarkerActive(_targetPosition != null);
        }

        private void SetMarkerActive(bool active)
        {
            if (_marker.activeSelf != active)
            {
                _marker.SetActive(active);
            }

            if (_teleporterLine.activeSelf != active)
            {
                _teleporterLine.SetActive(active);
            }
        }

        public void InputWasJustReleased()
        {
            if (_targetPosition.HasValue)
            {
                transform.root.position = _targetPosition.Value;
                _targetPosition = null;
            }

            SetMarkerActive(false);
        }
    }
}