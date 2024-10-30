#if UNITY_STANDALONE || UNITY_EDITOR
using System;
using System.IO;
#endif
using UnityEngine;
using UnityEngine.InputSystem;

namespace Demonixis.ToolboxV2.Graphics
{
    public sealed class ScreenshotMaker : MonoBehaviour
    {
        [Header("Standard Capture")]
        [SerializeField]
        private int m_ScreenshotSuperSampling = 1;

        [SerializeField]
        private InputAction m_ScreenshotAction = null;

        private void Start()
        {
            SetScreenshotInputAction(m_ScreenshotAction);
        }

        public void SetScreenshotInputAction(InputAction inputAction)
        {
#if UNITY_STANDALONE || UNITY_EDITOR
            m_ScreenshotAction = inputAction;

            if (m_ScreenshotAction != null)
            {
                m_ScreenshotAction.Enable();
                m_ScreenshotAction.started += c => CaptureScreenshot();
            }
#endif
        }

        public void CaptureScreenshot()
        {
#if UNITY_STANDALONE || UNITY_EDITOR
            var name = string.Format("{0}_{1}.png", Application.productName, DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss"));
            var folder = GameSave.GetSavePath("Screenshots");
            ScreenCapture.CaptureScreenshot(Path.Combine(folder, name), m_ScreenshotSuperSampling);
#endif
        }
    }
}