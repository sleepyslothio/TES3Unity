//using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Demonixis.ToolboxV2
{
    public class AnimateScaleOnHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        private Button m_Button = null;
        //private Tween m_Tween = null;
        private float m_ScaleDuration = 0.5f;
        private float m_ScaleMax = 1.05f;

        private void Awake()
        {
            m_Button = GetComponent<Button>();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            DoScale(m_ScaleMax);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            DoScale(1.0f, 0.75f);
        }

        private void DoScale(float value, float durationMultiplier = 1.0f)
        {
            if (m_Button != null && !m_Button.interactable)
            {
                return;
            }

            /*if (m_Tween != null && m_Tween.active)
            {
                m_Tween.Kill();
            }

            m_Tween = transform.DOScale(value, m_ScaleDuration * durationMultiplier);*/
        }
    }
}