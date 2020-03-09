using System.Collections;
using TES3Unity.Components.Records;
using UnityEngine;

namespace TES3Unity.UI
{
    [RequireComponent(typeof(Canvas))]
    public class UIManager : MonoBehaviour
    {
        [Header("HUD Elements")]
        [SerializeField]
        private UICrosshair _crosshair = null;
        [SerializeField]
        private UIInteractiveText _interactiveText = null;

        [Header("UI Elements")]
        [SerializeField]
        private UIBook _book = null;
        [SerializeField]
        private UIScroll _scroll = null;

        public UIBook Book => _book;
        public UIInteractiveText InteractiveText => _interactiveText;
        public UIScroll Scroll => _scroll;
        public UICrosshair Crosshair => _crosshair;

        public IEnumerator Start()
        {
            var player = GameObject.FindWithTag("Player");

            while (player == null)
            {
                player = GameObject.FindWithTag("Player");
                yield return null;
            }

            var playerCharacter = player.GetComponent<PlayerCharacter>();
            playerCharacter.InteractiveTextChanged += OnInteractiveTextChanged;
        }

        private void OnInteractiveTextChanged(RecordComponent component, bool visible)
        {
            if (visible)
            {
                var data = component.objData;
                _interactiveText.Show(GUIUtils.CreateSprite(data.icon), data.interactionPrefix, data.name, data.value, data.weight);
            }
            else
            {
                _interactiveText.Close();
            }
        }
    }
}
