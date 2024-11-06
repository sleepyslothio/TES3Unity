using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Demonixis.ToolboxV2.Utils
{
    public struct TransformRecordData
    {
        public Vector3[] Positions;
        public Quaternion[] Rotations;
    }

    public class TransformRecorder : MonoBehaviour
    {
        public enum UpdateRecordMode
        {
            Update = 0, Custom
        }

        private List<Vector3> m_Positions;
        private List<Quaternion> m_Rotations;
        private Transform m_Target = null;
        private UpdateRecordMode m_RecordMode;
        private bool m_Recording;

        private void Awake()
        {
            m_Positions = new List<Vector3>();
            m_Rotations = new List<Quaternion>();
        }

        private void Update()
        {
            if (m_Recording && m_RecordMode == UpdateRecordMode.Update)
            {
                RecordData();
            }
        }

        private IEnumerator RecordCoroutine(float interval)
        {
            var wait = new WaitForSeconds(interval);

            while (m_Recording)
            {
                RecordData();
                yield return wait;
            }
        }

        private void RecordData()
        {
            m_Positions.Add(m_Target.position);
            m_Rotations.Add(m_Target.rotation);
        }

        public void StartRecording(float interval = 0)
        {
            StopRecording();

            m_Positions.Clear();
            m_Rotations.Clear();

            m_RecordMode = interval > 0 ? UpdateRecordMode.Custom : UpdateRecordMode.Update;
            m_Recording = true;

            if (m_RecordMode == UpdateRecordMode.Custom)
            {
                StartCoroutine(RecordCoroutine(interval));
            }
        }

        public void StopRecording()
        {
            m_Recording = false;
            StopAllCoroutines();
        }

        public TransformRecordData GetRecordedData()
        {
            return new TransformRecordData
            {
                Positions = m_Positions.ToArray(),
                Rotations = m_Rotations.ToArray()
            };
        }
    }
}
