using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Demonixis.ToolboxV2.UI
{
    public sealed class UIRectSelector : MonoBehaviour
    {
        private static Color RectBackgroundColor = new Color32(50, 110, 185, 175);
        private static Color ChildTextColor = Color.white;

        private Image m_RectImage = null;
        private Text m_ChildText = null;
        private TextMeshProUGUI m_ChildTextTMP;
        private Color m_OriginalTextColor;

        [SerializeField] private Sprite m_RectSprite = null;

        public static bool Enabled { get; set; } = true;

        private void OnDisable()
        {
            OnRectUnselected(null);
        }

        private void Start()
        {
            var selectable = GetComponentInChildren<Selectable>();
            if (selectable == null)
            {
                selectable = GetComponent<Selectable>();
            }

            if (selectable == null)
            {
                Debug.LogError(
                    $"The GameObject {name} doesn't have a Selectable component. UIRectSelector will be destroyed.");
                Destroy(this);
                return;
            }

            var trigger = selectable.GetComponent<EventTrigger>();
            if (trigger == null)
            {
                trigger = selectable.gameObject.AddComponent<EventTrigger>();
            }

            var triggers = trigger.triggers;
            triggers.Add(SetupEntry(EventTriggerType.Select, OnRectSelected));
            triggers.Add(SetupEntry(EventTriggerType.Deselect, OnRectUnselected));
            triggers.Add(SetupEntry(EventTriggerType.PointerEnter, OnRectSelected));
            triggers.Add(SetupEntry(EventTriggerType.PointerExit, OnRectUnselected));

            // Buttons
            var image = gameObject.GetComponent<Image>();

            if (image && transform.childCount == 1)
            {
                m_ChildText = GetComponentInChildren<Text>();

                if (m_ChildText != null)
                    m_OriginalTextColor = m_ChildText.color;

                m_ChildTextTMP = GetComponentInChildren<TextMeshProUGUI>();
                if (m_ChildTextTMP != null)
                    m_OriginalTextColor = m_ChildTextTMP.color;
            }

            var rectSelectorGo = new GameObject("RectSelector");

            m_RectImage = rectSelectorGo.AddComponent<Image>();
            m_RectImage.type = Image.Type.Sliced;
            m_RectImage.sprite = m_RectSprite;
            m_RectImage.color = RectBackgroundColor;

            var rectParent = (RectTransform)transform;
            var rectTransform = m_RectImage.GetComponent<RectTransform>();
            rectTransform.SetParent(transform, false);
            rectTransform.SetAsFirstSibling();
            rectTransform.localScale = Vector3.one;
            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.one;
            rectTransform.pivot = new Vector2(0.5f, 0.5f);
            rectTransform.sizeDelta = new Vector2(10, 10);
            rectTransform.anchoredPosition = Vector2.zero;
            m_RectImage.enabled = false;

            if (m_RectSprite == null && image != null)
            {
                m_RectImage.sprite = image.sprite;
            }
        }

        private void OnRectSelected(BaseEventData data)
        {
            if (Enabled)
            {
                m_RectImage.enabled = true;

                if (m_ChildText != null)
                    m_ChildText.color = ChildTextColor;

                if (m_ChildTextTMP != null)
                    m_ChildTextTMP.color = ChildTextColor;
            }
        }

        private void OnRectUnselected(BaseEventData data)
        {
            if (m_RectImage != null)
            {
                m_RectImage.enabled = false;
            }

            if (m_ChildText != null)
            {
                m_ChildText.color = m_OriginalTextColor;
            }

            if (m_ChildTextTMP != null)
                m_ChildTextTMP.color = m_OriginalTextColor;
        }

        public static EventTrigger.Entry SetupEntry(EventTriggerType type, UnityAction<BaseEventData> callback)
        {
            var entry = new EventTrigger.Entry();
            entry.eventID = type;
            entry.callback.AddListener(callback);
            return entry;
        }
    }
}