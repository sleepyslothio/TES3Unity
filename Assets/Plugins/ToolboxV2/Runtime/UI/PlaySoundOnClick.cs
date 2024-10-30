using UnityEngine;
using UnityEngine.UI;

namespace Demonixis.ToolboxV2.UI
{
    [RequireComponent(typeof(Button))]
    public sealed class PlaySoundOnClick : MonoBehaviour
    {
        private Button m_Button = null;
        private AudioSource m_Audio = null;

        [SerializeField]
        private AudioClip m_Sound = null;

        private void Start()
        {
            if (Camera.main != null)
            {
                m_Audio = Camera.main.GetComponent(typeof(AudioSource)) as AudioSource;
                if (m_Audio == null)
                {
                    m_Audio = (AudioSource)Camera.main.gameObject.AddComponent(typeof(AudioSource));
                }
            }

            m_Button = GetComponent(typeof(Button)) as Button;
            m_Button.onClick.AddListener(PlaySound);
        }

        private void PlaySound()
        {
            if (m_Audio == null)
            {
                AudioSource.PlayClipAtPoint(m_Sound, Vector3.zero);
            }
            else
            {
                m_Audio.PlayOneShot(m_Sound);
            }
        }
    }
}