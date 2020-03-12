using System;
using UnityEngine;
using UnityEngine.UI;

namespace TES3Unity.UI
{
    public sealed class HUDHealth : MonoBehaviour
    {
        [SerializeField]
        private Slider m_Health = null;
        [SerializeField]
        private Slider m_Magic = null;
        [SerializeField]
        private Slider m_Stamina = null;
        [SerializeField]
        private Image m_WeaponBox = null;
        [SerializeField]
        private Image m_MagicBox = null;
    }
}
