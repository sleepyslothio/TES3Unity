using System;
using System.Collections;
using TESUnity.ESM;
using TESUnity.ESM.Records;
using TESUnity.Inputs;
using UnityEngine;
using UnityEngine.UI;

namespace TESUnity.UI
{
    public class UIScroll : MonoBehaviour
    {
        private BOOKRecord _bookRecord;

        [SerializeField]
        private GameObject _container = null;
        [SerializeField]
        private Image _background = null;
        [SerializeField]
        private Text _content = null;

        public event Action<BOOKRecord> OnTake = null;
        public event Action<BOOKRecord> OnClosed = null;

        void Start()
        {
            var texture = TESManager.instance.TextureManager.LoadTexture("scroll", true);
            _background.sprite = GUIUtils.CreateSprite(texture);

            // If the book is already opened, don't change its transform.
            if (_bookRecord == null)
                Close();

            var gameplayActionMap = InputManager.GetActionMap("Gameplay");
            gameplayActionMap["Use"].started += (c) =>
            {
                if (_container.activeSelf)
                {
                    Take();
                }
            };

            gameplayActionMap["Cancel"].started += (c) =>
            {
                if (_container.activeSelf)
                {
                    Close();
                }
            };
        }

        public void Show(BOOKRecord book)
        {
            _bookRecord = book;

            var words = _bookRecord.Text;
            words = words.Replace("\r\n", "");
            words = words.Replace("<BR><BR>", "");
            words = words.Replace("<BR>", "\n");
            words = System.Text.RegularExpressions.Regex.Replace(words, @"<[^>]*>", string.Empty);

            _content.text = words;

            StartCoroutine(SetScrollActive(true));
        }

        public void Take()
        {
            OnTake?.Invoke(_bookRecord);
            Close();
        }

        public void Close()
        {
            OnClosed?.Invoke(_bookRecord);

            _container.SetActive(false);
            _bookRecord = null;
        }

        private IEnumerator SetScrollActive(bool active)
        {
            yield return new WaitForEndOfFrame();

            _container.SetActive(active);
        }
    }
}
