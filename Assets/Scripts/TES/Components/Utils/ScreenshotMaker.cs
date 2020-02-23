#define XRINPUT_ENABLED_
#if XRINPUT_ENABLED
using Demonixis.Toolbox.XR;
#endif
using System;
using System.IO;
using UnityEngine;

namespace TESUnity
{
    public sealed class ScreenshotMaker : MonoBehaviour
    {
        private RenderTexture m_LeftRenderTexture = null;
        private RenderTexture m_RightRenderTexture = null;
        private RenderTexture m_StereoEquirect = null;
        private RenderTexture m_MonoscopicEquirect = null;

        [Header("Input")]
        [SerializeField]
        private KeyCode m_ShotKey = KeyCode.Tab;
        [SerializeField]
        private KeyCode m_Show360ModKey = KeyCode.LeftShift;
        [SerializeField]
        private KeyCode m_Show360StereoModKey = KeyCode.LeftAlt;

#if XRINPUT_ENABLED
        [SerializeField]
        private bool m_LeftHand = true;
        [SerializeField]
        private XRButton m_ShotButton = XRButton.Grip;
        [SerializeField]
        private XRButton m_Shot360Modifer = XRButton.Trigger;
        [SerializeField]
        private XRButton m_Shot360StereoModifer = XRButton.Menu;
#endif

        [Header("Standard Capture")]
        [SerializeField]
        private int m_ScreenshotSuperSampling = 1;

        [Header("360 Capture")]
        [SerializeField]
        private float m_StereoSeparation = 0.064f;
        [SerializeField]
        private int m_TextureSize = 1024;

        [Header("Misc")]
        [SerializeField]
        private bool m_EditorOnly = false;

        private void Start()
        {
            if (m_EditorOnly && !Application.isEditor)
            {
                Destroy(this);
                return;
            }

            m_LeftRenderTexture = new RenderTexture(m_TextureSize, m_TextureSize, 24);
            m_LeftRenderTexture.dimension = UnityEngine.Rendering.TextureDimension.Cube;
            m_RightRenderTexture = new RenderTexture(m_TextureSize, m_TextureSize, 24);
            m_RightRenderTexture.dimension = UnityEngine.Rendering.TextureDimension.Cube;
            m_StereoEquirect = new RenderTexture(m_TextureSize, m_TextureSize, 24);
            m_MonoscopicEquirect = new RenderTexture(m_TextureSize, m_TextureSize / 2, 24);
        }

        private void Update()
        {
            var screenshot = Input.GetKeyDown(m_ShotKey);
            var mod360 = Input.GetKey(m_Show360ModKey);
            var stereo360 = Input.GetKey(m_Show360StereoModKey);

#if XRINPUT_ENABLED
            var xr = XRInput.Instance;
            screenshot |= xr.GetButtonDown(m_ShotButton, m_LeftHand);
            mod360 |= xr.GetButton(m_Shot360Modifer, m_LeftHand);
            stereo360 |= xr.GetButton(m_Shot360StereoModifer, m_LeftHand);
#endif

            if (!screenshot)
                return;

            // Regular Screenshot
            if (!mod360 && !stereo360)
                CaptureScreenshot();

            // 360 Monoscopic Screenshot.
            if (mod360 && !stereo360)
                Capture360Screenshot(false);

            if (!mod360 && stereo360)
                Capture360Screenshot(true);
        }

        private string GetSavePath(string folder)
        {
            var path = string.Format("{0}/..", Application.dataPath);

            if (folder != string.Empty)
                path = string.Format("{0}/{1}", path, folder);

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            return path;
        }

        public void CaptureScreenshot()
        {
            var name = string.Format("{0}_{1}.png", Application.productName, DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss"));
            var folder = GetSavePath("Screenshots");

            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);

            ScreenCapture.CaptureScreenshot(Path.Combine(folder, name), m_ScreenshotSuperSampling);
        }

        public void Capture360Screenshot(bool stereo)
        {
            var camera = Camera.main;
            var oldStereoSeparation = camera.stereoSeparation;
            var oldRT = RenderTexture.active;

            camera.stereoSeparation = m_StereoSeparation;

            if (stereo)
            {
                camera.RenderToCubemap(m_LeftRenderTexture, 63, Camera.MonoOrStereoscopicEye.Left);
                camera.RenderToCubemap(m_RightRenderTexture, 63, Camera.MonoOrStereoscopicEye.Right);

                m_LeftRenderTexture.ConvertToEquirect(m_StereoEquirect, Camera.MonoOrStereoscopicEye.Left);
                m_RightRenderTexture.ConvertToEquirect(m_StereoEquirect, Camera.MonoOrStereoscopicEye.Right);
            }
            else
            {
                camera.RenderToCubemap(m_LeftRenderTexture, 63, Camera.MonoOrStereoscopicEye.Mono);
                m_LeftRenderTexture.ConvertToEquirect(m_MonoscopicEquirect, Camera.MonoOrStereoscopicEye.Mono);
            }

            camera.stereoSeparation = oldStereoSeparation;

            var equirect = stereo ? m_StereoEquirect : m_MonoscopicEquirect;

            // Save the shot to a png file.
            RenderTexture.active = equirect;

            var texture = new Texture2D(equirect.width, equirect.height);
            texture.ReadPixels(new Rect(0, 0, equirect.width, equirect.height), 0, 0);
            texture.Apply();

            var name = string.Format("{0}-360{1}_{2}.png", Application.productName, (stereo ? "-3D" : ""), DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss"));
            var folder = GetSavePath("Screenshots");

            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);

            File.WriteAllBytes(Path.Combine(folder, name), texture.EncodeToPNG());

            RenderTexture.active = oldRT;
        }

#if UNITY_EDITOR

        private void OnValidate()
        {
            m_LeftRenderTexture = new RenderTexture(m_TextureSize, m_TextureSize, 24);
            m_LeftRenderTexture.dimension = UnityEngine.Rendering.TextureDimension.Cube;
            m_RightRenderTexture = new RenderTexture(m_TextureSize, m_TextureSize, 24);
            m_RightRenderTexture.dimension = UnityEngine.Rendering.TextureDimension.Cube;
            m_StereoEquirect = new RenderTexture(m_TextureSize, m_TextureSize, 24);
            m_MonoscopicEquirect = new RenderTexture(m_TextureSize, m_TextureSize / 2, 24);
        }

#endif
    }
}