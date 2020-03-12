using System;
using System.Collections;
using TES3Unity.ESM;
using TES3Unity.ESM.Records;
using TES3Unity.Inputs;
using UnityEngine;
using UnityEngine.UI;

namespace TES3Unity.UI
{
    public class UIScroll : UIWindow
    {
        private BOOKRecord _bookRecord;

        [SerializeField]
        private Image _background = null;
        [SerializeField]
        private Text _content = null;

        public event Action<BOOKRecord> OnTake = null;
        public event Action<BOOKRecord> OnClosed = null;

        void Start()
        {
            var textureManager = TES3Engine.Instance.textureManager;
            var texture = textureManager.LoadTexture("scroll", true);
            _background.sprite = GUIUtils.CreateSprite(texture);

            // If the book is already opened, don't change its transform.
            if (_bookRecord == null)
            {
                Close();
            }

            var gameplayActionMap = InputManager.GetActionMap("Gameplay");
            gameplayActionMap["Use"].started += (c) =>
            {
                if (m_Container.activeSelf)
                {
                    Take();
                }
            };

            gameplayActionMap["Cancel"].started += (c) =>
            {
                if (m_Container.activeSelf)
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

            m_Container.SetActive(false);
            _bookRecord = null;
        }

        private IEnumerator SetScrollActive(bool active)
        {
            yield return new WaitForEndOfFrame();

            m_Container.SetActive(active);
        }
    }
}
