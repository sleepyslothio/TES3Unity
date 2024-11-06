﻿using UnityEngine;
using UnityEngine.UI;

namespace TES3Unity.UI
{
    public class UIInteractiveText : MonoBehaviour
    {
        private const int WidgetWidth = 200;
        private const int WidgetHeight = 100;

        private bool _opened;

        [SerializeField]
        private GameObject _container = null;
        [SerializeField]
        private Image _icon = null;
        [SerializeField]
        private Text _title = null;

        [SerializeField]
        private GameObject _inventoryInfos = null;
        [SerializeField]
        private Text _weight = null;
        [SerializeField]
        private Text _value = null;

        private void Start()
        {
            _opened = false;
            _container.SetActive(false);
        }

        public void Show(Sprite icon, string prefixTitle, string title, string weight, string value)
        {
            _icon.enabled = icon != null;
            if (_icon.enabled)
            {
                _icon.sprite = icon;
            }

            _title.text = string.IsNullOrEmpty(prefixTitle) ? title : prefixTitle + title;

            var showInventoryInfos = !string.IsNullOrEmpty(weight) && !string.IsNullOrEmpty(value);

            _inventoryInfos.SetActive(showInventoryInfos);
            _container.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, showInventoryInfos ? WidgetHeight : WidgetHeight / 2.0f);

            if (showInventoryInfos)
            {
                _weight.text = "Weight: " + weight;
                _value.text = "Value: " + value;
            }

            _container.SetActive(true);
            _opened = true;
        }

        public void Close()
        {
            if (_opened)
            {
                _container.SetActive(false);
                _opened = false;
            }
        }
    }
}