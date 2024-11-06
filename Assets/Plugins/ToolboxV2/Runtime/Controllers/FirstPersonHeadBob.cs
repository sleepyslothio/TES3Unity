using System;
using UnityEngine;
using UnityEngine.XR;

namespace Demonixis.ToolboxV2
{
    public class FirstPersonHeadBob : MonoBehaviour
    {
        private float m_NextStepTime = 0.5f;
        private float m_HeadBobCycle = 0;
        private float m_HeadBobFade = 0;
        private float m_SpringPos = 0;
        private float m_SpringVelocity = 0;
        private float m_SpringElastic = 1.1f;
        private float m_SpringDampen = 0.8f;
        private float m_SpringVelocityThreshold = 0.05f;
        private float m_SpringPositionThreshold = 0.05f;
        private Vector3 m_PrevPosition;
        private Vector3 m_PrevVelocity = Vector3.zero;
        private bool m_PrevGrounded = true;

        [SerializeField]
        private AudioSource m_AudioSource = null;
        [SerializeField]
        private Transform m_TargetTransform = null;
        [SerializeField]
        private Transform m_HeadTransform = null;
        [Tooltip("the base speed of the head bobbing (in cycles per metre)")]
        [SerializeField]
        private float m_HeadBobFrequency = 1.5f;
        [Tooltip("the height range of the head bob")]
        [SerializeField]
        private float m_HeadBobHeight = 0.1f;
        [Tooltip("the angle which the head tilts to left & right during the bob cycle")]
        [SerializeField]
        private float m_HeadBobSwayAngle = 0.5f;
        [Tooltip("the distance the head moves to left & right during the bob cycle")]
        [SerializeField]
        private float m_HeadBobSideMovement = 0.05f;
        [Tooltip("the amount the bob height increases as the character's speed increases (for a good 'run' effect compared with walking)")]
        [SerializeField]
        private float m_BobHeightSpeedMultiplier = 0.3f;
        [Tooltip("the amount the stride lengthens based on speed (so that running isn't like a silly speedwalk!)")]
        [SerializeField]
        private float m_BobStrideSpeedLengthen = 0.3f;
        [Tooltip("these control the amount of movement applied to the head when the character jumps or lands")]
        [SerializeField]
        private float m_JumpLandMove = 3;
        [SerializeField]
        private float m_JumpLandTilt = 60;

        [Header("Audio")]
        [Tooltip("an array of footstep sounds that will be randomly selected from.")]
        [SerializeField]
        private AudioClip[] m_FootstepSounds = null;
        [Tooltip("the sound played when character leaves the ground.")]
        [SerializeField]
        private AudioClip m_JumpSound = null;
        [Tooltip("the sound played when character touches back on ground.")]
        [SerializeField]
        private AudioClip m_LandSound = null;

        public Vector3 OriginalLocalPos { get; set; }
        public bool XREnabled { get; set; }

        public Func<bool> CheckForGrounded = null;

        private void Start()
        {
            if (m_TargetTransform == null)
            {
                m_TargetTransform = transform.root;
            }

            if (m_HeadTransform == null)
            {
                m_HeadTransform = transform;
            }

            if (m_AudioSource == null)
            {
                m_AudioSource = gameObject.AddComponent<AudioSource>();
            }

            if (!XREnabled)
            {
                XREnabled = XRSettings.enabled;
            }

            OriginalLocalPos = m_HeadTransform.localPosition;
            m_AudioSource.volume = 0.25f;
            m_PrevPosition = m_TargetTransform.position;
        }

        private void FixedUpdate()
        {
            var velocity = (m_TargetTransform.position - m_PrevPosition) / Time.deltaTime;
            var velocityChange = velocity - m_PrevVelocity;
            m_PrevPosition = m_TargetTransform.position;
            m_PrevVelocity = velocity;
            m_SpringVelocity -= velocityChange.y;
            m_SpringVelocity -= m_SpringPos * m_SpringElastic;
            m_SpringVelocity *= m_SpringDampen;
            m_SpringPos += m_SpringVelocity * Time.deltaTime;
            m_SpringPos = Mathf.Clamp(m_SpringPos, -.3f, .3f);

            if (Mathf.Abs(m_SpringVelocity) < m_SpringVelocityThreshold && Mathf.Abs(m_SpringPos) < m_SpringPositionThreshold)
            {
                m_SpringVelocity = 0;
                m_SpringPos = 0;
            }

            var flatVelocity = new Vector3(velocity.x, 0, velocity.z).magnitude;
            var strideLengthen = 1 + (flatVelocity * m_BobStrideSpeedLengthen);

            m_HeadBobCycle += (flatVelocity / strideLengthen) * (Time.deltaTime / m_HeadBobFrequency);

            var bobFactor = Mathf.Sin(m_HeadBobCycle * Mathf.PI * 2);
            var bobSwayFactor = Mathf.Sin(m_HeadBobCycle * Mathf.PI * 2 + Mathf.PI * .5f);
            bobFactor = 1 - (bobFactor * .5f + 1);
            bobFactor *= bobFactor;

            if (new Vector3(velocity.x, 0, velocity.z).magnitude < 0.1f)
            {
                m_HeadBobFade = Mathf.Lerp(m_HeadBobFade, 0, Time.deltaTime);
            }
            else
            {
                m_HeadBobFade = Mathf.Lerp(m_HeadBobFade, 1, Time.deltaTime);
            }

            var speedHeightFactor = 1 + (flatVelocity * m_BobHeightSpeedMultiplier);

            var xPos = -m_HeadBobSideMovement * bobSwayFactor;
            var yPos = m_SpringPos * m_JumpLandMove + bobFactor * m_HeadBobHeight * m_HeadBobFade * speedHeightFactor;
            var xTilt = -m_SpringPos * m_JumpLandTilt;
            var zTilt = bobSwayFactor * m_HeadBobSwayAngle * m_HeadBobFade;

            if (!XREnabled)
            {
                m_HeadTransform.localPosition = OriginalLocalPos + new Vector3(xPos, yPos, 0);
                m_HeadTransform.localRotation = Quaternion.Euler(xTilt, 0, zTilt);
            }

            if (m_FootstepSounds == null || m_FootstepSounds.Length == 0)
            {
                return;
            }

            var grounded = CheckForGrounded != null ? CheckForGrounded() : true;
            if (grounded)
            {
                if (!m_PrevGrounded)
                {
                    m_AudioSource.clip = m_LandSound;
                    m_AudioSource.Play();
                    m_NextStepTime = m_HeadBobCycle + .5f;
                }
                else
                {

                    if (m_HeadBobCycle > m_NextStepTime)
                    {
                        m_NextStepTime = m_HeadBobCycle + .5f;

                        var n = UnityEngine.Random.Range(1, m_FootstepSounds.Length);
                        m_AudioSource.clip = m_FootstepSounds[n];
                        m_AudioSource.Play();
                        m_FootstepSounds[n] = m_FootstepSounds[0];
                        m_FootstepSounds[0] = m_AudioSource.clip;
                    }
                }
                m_PrevGrounded = true;
            }
            else
            {

                if (m_PrevGrounded)
                {
                    m_AudioSource.clip = m_JumpSound;
                    m_AudioSource.Play();
                }
                m_PrevGrounded = false;
            }
        }
    }
}