using UnityEngine;
using UnityEngine.UI;

namespace Demonixis.ToolboxV2.UI
{
    public sealed class UISoundPatcher : MonoBehaviour
    {
        private AudioSource m_AudioSource = null;

        [SerializeField]
        private AudioClip m_ClickSound = null;

        private void Start()
        {
            m_AudioSource = gameObject.AddComponent<AudioSource>();

            var buttons = GetComponentsInChildren<Button>(true);
            foreach (var button in buttons)
            {
                button.onClick.AddListener(PlayClickSound);
            }
        }

        private void PlayClickSound()
        {
            m_AudioSource.PlayOneShot(m_ClickSound);
        }
    }
}
