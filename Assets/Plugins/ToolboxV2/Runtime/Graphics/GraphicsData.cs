using UnityEngine.Rendering.Universal;
#if HDRP_ENABLED
using UnityEngine.Rendering.HighDefinition;
#endif
using System;

namespace Demonixis.ToolboxV2.Graphics
{
    public enum GameShadowCascades
    {
        _1 = 0,
        _2 = 1,
        _4 = 2
    }

    public enum AntiAliasingProxy
    {
        Disabled = 0,
        MSAA_2x,
        MSAA_4x,
        FXAA,
        TAA
    }

    public enum GraphicsQuality2
    {
        Low = 0,
        High
    }

    public enum GraphicsQuality
    {
        Low = 0,
        Medium,
        High
    }

    public enum GraphicsQuality4
    {
        Low = 0,
        Medium,
        High,
        Ultra
    }

    public enum PostProcessingQuality
    {
        None = 0,
        Low,
        Medium,
        High
    }

    public enum VRFFR
    {
        None = 0,
        Low,
        Medium,
        High,
        HighTop
    }

    public enum VRRefreshRate
    {
        _72 = 0,
        _80,
        _90,
        _120
    }

    public enum DeviceRefreshRate
    {
        _30 = 0,
        _60,
        _120,
        _240,
        Unlimited
    }

    [Serializable]
    public struct GraphicsSettingsData
    {
        // Common
        public GraphicsQuality2 SRPQuality;
        public GraphicsQuality TerrainQuality;
        public AntiAliasingProxy AntiAliasing;
        public float RenderScale;
        public int RefreshRate;
        public bool LowQualityShader;
        public float nearClip;
        public float farClip;

        // Shadows
        public GameShadowCascades ShadowCascadeCount;
        public int ShadowmapResolution;
        public float ShadowDistance;
        public bool DirectionalShadows;
        public bool AdditionalShadows;
        public bool SoftShadows;

        // Post Processing
        public PostProcessingQuality PostProcessingQuality;
        public bool SSAO;
        public bool Bloom;
        public bool SSR;
        public bool LenseEffect;
        public bool FilmGrain;
        public bool SSGI;
        public bool Distortion;

        public int GetShadowCascadeCount()
        {
            return ShadowCascadeCount switch
            {
                GameShadowCascades._1 => 1,
                GameShadowCascades._2 => 2,
                GameShadowCascades._4 => 4,
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        public MsaaQuality GetMsaaQuality()
        {
            return AntiAliasing switch
            {
                AntiAliasingProxy.MSAA_2x => MsaaQuality._2x,
                AntiAliasingProxy.MSAA_4x => MsaaQuality._4x,
                _ => MsaaQuality.Disabled
            };
        }

        public AntialiasingMode GetUrpAntiAliasing()
        {
            return AntiAliasing switch
            {
                AntiAliasingProxy.FXAA => AntialiasingMode.FastApproximateAntialiasing,
                AntiAliasingProxy.TAA => AntialiasingMode.TemporalAntiAliasing,
                _ => AntialiasingMode.None
            };
        }

#if HDRP_ENABLED
        public HDAdditionalCameraData.AntialiasingMode GetHdrpAntiAliasing()
        {
            return AntiAliasing switch
            {
                AntiAliasingProxy.FXAA => HDAdditionalCameraData.AntialiasingMode.FastApproximateAntialiasing,
                AntiAliasingProxy.TAA => HDAdditionalCameraData.AntialiasingMode.TemporalAntialiasing,
                _ => HDAdditionalCameraData.AntialiasingMode.None
            };
        }
#endif
    }

    [Serializable]
    public struct XRSettingsData
    {
        public bool VRSeatedMode;
        public float VRHeadHeight;
        public VRFFR VRFFRLevel;
        public bool VRAppSpaceWarp;
        public VRRefreshRate VRRefreshRate;
        public bool VRAutoStart;
        public bool VRHandTracking;
        public bool VRDynamicResolution;

        public int GetRefreshRate()
        {
            return VRRefreshRate switch
            {
                VRRefreshRate._72 => 72,
                VRRefreshRate._80 => 80,
                VRRefreshRate._90 => 90,
                VRRefreshRate._120 => 120,
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        public float GetFoveationLevel()
        {
            return VRFFRLevel switch
            {
                VRFFR.None => 0.0f,
                VRFFR.Low => 0.2f,
                VRFFR.Medium => 0.4f,
                VRFFR.High => 0.65f,
                VRFFR.HighTop => 1.0f,
                _ => throw new ArgumentOutOfRangeException()
            };
        }
    }
}