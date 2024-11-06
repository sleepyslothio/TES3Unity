using Demonixis.ToolboxV2.XR;
using UnityEngine;
using UnityEngine.UI;

namespace TES3Unity.UI
{
    public sealed class HUDHealth : MonoBehaviour
    {
        [SerializeField] private Slider m_Health;
        [SerializeField] private Slider m_Magic;
        [SerializeField] private Slider m_Stamina;
        [SerializeField] private Image m_WeaponBox;
        [SerializeField] private Image m_MagicBox;

        public int Health
        {
            set => m_Health.value = value;
        }

        public int Magic
        {
            set => m_Magic.value = value;
        }

        public int Stamina
        {
            set => m_Stamina.value = value;
        }

        public void EquipWeapon(Sprite icon)
        {
            m_WeaponBox.sprite = icon;
            m_WeaponBox.transform.parent.gameObject.SetActive(icon != null);
        }

        public void EquipMagic(Sprite icon)
        {
            m_MagicBox.sprite = icon;
            m_MagicBox.transform.parent.gameObject.SetActive(icon != null);
        }

        private void Start()
        {
            if (XRManager.Enabled)
                gameObject.SetActive(false);
        }
    }
}