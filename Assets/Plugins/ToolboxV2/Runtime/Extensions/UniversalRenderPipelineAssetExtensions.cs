using System.Collections.Generic;
using System.Reflection;
using UnityEngine.Rendering.Universal;

namespace Demonixis.ToolboxV2
{
    public static class UniversalRenderPipelineAssetExtensions
    {
        public static bool TryGetRenderFeature<T>(this UniversalRenderPipelineAsset asset, out T renderFeature) where T : ScriptableRendererFeature
        {
            renderFeature = null;

            var type = asset.GetType();
            var propertyInfo = type.GetField("m_RendererDataList", BindingFlags.Instance | BindingFlags.NonPublic);

            if (propertyInfo == null)
            {
                return false;
            }

            var scriptableRenderData = (ScriptableRendererData[])propertyInfo.GetValue(asset);

            if (scriptableRenderData != null && scriptableRenderData.Length > 0)
            {
                foreach (var renderData in scriptableRenderData)
                {
                    foreach (var rendererFeature in renderData.rendererFeatures)
                    {
                        if (rendererFeature is T)
                        {
                            renderFeature = (T)rendererFeature;
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        public static ScriptableRendererFeature[] GetRenderFeatures<T>(this UniversalRenderPipelineAsset asset) where T : ScriptableRendererFeature
        {
            var list = new List<ScriptableRendererFeature>();
            var type = asset.GetType();
            var propertyInfo = type.GetField("m_RendererDataList", BindingFlags.Instance | BindingFlags.NonPublic);

            if (propertyInfo != null)
            {
                var scriptableRenderData = (ScriptableRendererData[])propertyInfo.GetValue(asset);

                if (scriptableRenderData != null && scriptableRenderData.Length > 0)
                {
                    foreach (var renderData in scriptableRenderData)
                    {
                        foreach (var rendererFeature in renderData.rendererFeatures)
                        {
                            if (rendererFeature is T)
                            {
                                list.Add(rendererFeature);
                            }
                        }
                    }
                }
            }

            return list.ToArray();
        }

        public static ScriptableRendererFeature GetRenderFeature(this UniversalRenderPipelineAsset asset, string name)
        {
            var type = asset.GetType();
            var propertyInfo = type.GetField("m_RendererDataList", BindingFlags.Instance | BindingFlags.NonPublic);

            if (propertyInfo != null)
            {
                var scriptableRenderData = (ScriptableRendererData[])propertyInfo.GetValue(asset);

                if (scriptableRenderData != null && scriptableRenderData.Length > 0)
                {
                    foreach (var renderData in scriptableRenderData)
                    {
                        foreach (var rendererFeature in renderData.rendererFeatures)
                        {
                            if (rendererFeature.name == name)
                            {
                                return rendererFeature;
                            }
                        }
                    }
                }
            }

            return null;
        }

        public static ScriptableRendererFeature[] DisableRenderFeature<T>(this UniversalRenderPipelineAsset asset) where T : ScriptableRendererFeature
        {
            var list = new List<ScriptableRendererFeature>();
            var type = asset.GetType();
            var propertyInfo = type.GetField("m_RendererDataList", BindingFlags.Instance | BindingFlags.NonPublic);

            if (propertyInfo == null)
            {
                return null;
            }

            var scriptableRenderData = (ScriptableRendererData[])propertyInfo.GetValue(asset);

            if (scriptableRenderData != null && scriptableRenderData.Length > 0)
            {
                foreach (var renderData in scriptableRenderData)
                {
                    foreach (var rendererFeature in renderData.rendererFeatures)
                    {
                        if (rendererFeature is T && rendererFeature.isActive)
                        {
                            rendererFeature.SetActive(false);

                            list.Add(rendererFeature);
                        }
                    }
                }
            }

            return list.ToArray();
        }

        public static ScriptableRendererFeature[] DisableRenderFeature(this UniversalRenderPipelineAsset asset, string typeName)
        {
            var list = new List<ScriptableRendererFeature>();
            var type = asset.GetType();
            var propertyInfo = type.GetField("m_RendererDataList", BindingFlags.Instance | BindingFlags.NonPublic);

            if (propertyInfo == null)
            {
                return null;
            }

            var scriptableRenderData = (ScriptableRendererData[])propertyInfo.GetValue(asset);

            if (scriptableRenderData != null && scriptableRenderData.Length > 0)
            {
                foreach (var renderData in scriptableRenderData)
                {
                    foreach (var rendererFeature in renderData.rendererFeatures)
                    {
                        if (rendererFeature.GetType().Name == typeName && rendererFeature.isActive)
                        {
                            rendererFeature.SetActive(false);

                            list.Add(rendererFeature);
                        }
                    }
                }
            }

            return list.ToArray();
        }
    }
}
