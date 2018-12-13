#if UNITY_STANDALONE || UNITY_EDITOR
#define RUDDER
#endif

#if RUDDER
using ns3DRudder;
using Unity3DRudder;
#endif
using UnityEngine;

namespace TESUnity.Components.VR
{
    public class RudderManager : MonoBehaviour
    {
#if RUDDER
        private Transform m_Tranform = null;
        private Axis m_Axis = null;
        private Rudder m_Rudder = null;
        private ModeAxis m_ModeAxis;
#endif

        [SerializeField]
        private float m_MoveSpeed = 1.0f;
        [SerializeField]
        private float m_StrafeSpeed = 0.5f;
        [SerializeField]
        private float m_RotationSpeed = 1.0f;

#if RUDDER

        private void Start()
        {
            m_Tranform = transform;
    
            try
            {
                m_Axis = new Axis();
                m_Rudder = s3DRudderManager.Instance.GetRudder(0);
                m_ModeAxis = ModeAxis.NormalizedValue;
            }
            catch (System.Exception ex) 
            {
                Debug.Log(ex.Message);
                enabled = false;
            }
        }

        public void Update()
        {
            if (!s3DRudderManager.Instance.IsDeviceConnected(0))
                return;

            if (m_Rudder == null)
                m_Rudder = s3DRudderManager.Instance.GetRudder(0);

            if (m_Rudder == null)
                return;



            m_Axis = m_Rudder.GetAxis(m_ModeAxis);
            m_Tranform.Translate(m_Axis.GetXAxis() * m_StrafeSpeed * Time.deltaTime, 0, m_Axis.GetYAxis() * m_MoveSpeed * Time.deltaTime);
            m_Tranform.Rotate(0.0f, m_Axis.GetZRotation() * m_RotationSpeed * Time.deltaTime, 0.0f);
        }
#endif
    }
}