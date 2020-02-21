using UnityEngine;
using UnityEngine.Rendering;

namespace TESUnity.Rendering
{
    public enum MatTestMode { Always, Less, LEqual, Equal, GEqual, Greater, NotEqual, Never }

    public struct MWMaterialTextures
    {
        public string mainFilePath;
        public string darkFilePath;
        public string detailFilePath;
        public string glossFilePath;
        public string glowFilePath;
        public string bumpFilePath;
    }

    public struct MWMaterialProps
    {
        public MWMaterialTextures textures;
        public bool alphaBlended;
        public BlendMode srcBlendMode;
        public BlendMode dstBlendMode;
        public bool alphaTest;
        public float alphaCutoff;
        public bool zWrite;
        public Color diffuseColor;
        public Color specularColor;
        public Color emissiveColor;
        public float glossiness;
        public float alpha;
    }

    /// <summary>
    /// Manages loading and instantiation of Morrowind materials.
    /// </summary>
    public class MaterialManager
    {
        private BaseMaterial m_Material;

        public TextureManager TextureManager { get; private set; }

        public MaterialManager(TextureManager textureManager)
        {
            TextureManager = textureManager;

            var config = GameSettings.Get();
            var materialType = config.MaterialType;
            var renderPath = config.RendererMode;

            // Order is important
            if (renderPath == RendererMode.UniversalRP)
            {
                if (config.MaterialType == MWMaterialType.PBR)
                    m_Material = new URPMaterial(textureManager);
                else
                    m_Material = new URPMaterial(textureManager);
            }
#if HDRP_ENABLED
			    if (renderPath == RendererMode.HDRP)
                    m_Material = new HDRPMaterial(textureManager);
#endif

        }

        public Material BuildMaterialFromProperties(MWMaterialProps mp)
        {
            return m_Material.BuildMaterialFromProperties(mp);
        }
    }
}