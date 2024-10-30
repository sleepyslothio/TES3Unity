using System.Collections;
using Demonixis.ToolboxV2.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace Demonixis.ToolboxV2.UI
{
    [RequireComponent(typeof(Graphic))]
    public sealed class FadeGraphicOnStart : MonoBehaviour
    {
        [SerializeField]
        private float m_Delay = 2.5f;
        [SerializeField]
        private float m_FadeDuration = 0.75f;

        private IEnumerator Start()
        {
            yield return CoroutineFactory.WaitForSeconds(m_Delay);

            var graphic = GetComponent<Graphic>();
            graphic.CrossFadeAlpha(0, m_FadeDuration, false);

            yield return CoroutineFactory.WaitForSeconds(m_FadeDuration);

            graphic.enabled = false;
        }
    }
}
