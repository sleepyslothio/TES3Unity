using UnityEngine;

namespace TES3Unity.Components
{
    /// <summary>
    /// This class is used in editor mode only. It allows to override the GameSettings class.
    /// This is usefull to test custom parameters without the need to change them from the option menu.
    /// </summary>
    public sealed class GameSettingsOverride : MonoBehaviour
    {
        public bool Override;
        public GameSettings m_GameSettings = new();
        
        public void OnEnable()
        {
#if UNITY_EDITOR
            if (Override)
            {
                GameSettings.Override(m_GameSettings);
            }
#endif
        }
    }
}
