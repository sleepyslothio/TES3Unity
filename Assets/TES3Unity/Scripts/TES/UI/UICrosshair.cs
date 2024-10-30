﻿using Demonixis.Toolbox.XR;
using Demonixis.ToolboxV2.XR;
using UnityEngine;
using UnityEngine.UI;

namespace TES3Unity.UI
{
    [RequireComponent(typeof(Image))]
    public class UICrosshair : MonoBehaviour
    {
        private Image _crosshair = null;

        public bool Enabled
        {
            get { return _crosshair.enabled; }
            set { _crosshair.enabled = value; }
        }

        private void Awake()
        {
            _crosshair = GetComponent<Image>();
        }

        private void Start()
        {
            var textureManager = TES3Engine.Instance?.textureManager;
            if (textureManager != null)
            {
                var crosshairTexture = textureManager.LoadTexture("target", true);
                _crosshair.sprite = GUIUtils.CreateSprite(crosshairTexture);
            }

            _crosshair.enabled = !XRManager.IsXREnabled();
        }

        public void SetActive(bool active)
        {
            gameObject.SetActive(active);
        }
    }
}
