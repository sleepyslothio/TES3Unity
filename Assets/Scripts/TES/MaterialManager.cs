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
            if (materialType != MWMaterialType.Unlit)
            {
#if LWRP_ENABLED
                if (renderPath == RendererMode.LightweightRP)
                {
                    if (config.MaterialType == MWMaterialType.PBR)
                        m_Material = new LWRPMaterial(textureManager);
                    else
                        m_Material = new LWRPSimpleMaterial(textureManager);
                }
#endif
#if HDRP_ENABLED
			    if (renderPath == RendererMode.HDRP)
                    m_Material = new HDRPMaterial(textureManager);
#endif
                if (!config.IsSRP())
                {
                    if (materialType == MWMaterialType.PBR)
                        m_Material = new StandardMaterial(textureManager);
                    else
                        m_Material = new DiffuseMaterial(textureManager);
                }
            }
            else
                m_Material = new UnliteMaterial(textureManager);
        }

        public Material BuildMaterialFromProperties(MWMaterialProps mp)
        {
            return m_Material.BuildMaterialFromProperties(mp);
        }
    }
}