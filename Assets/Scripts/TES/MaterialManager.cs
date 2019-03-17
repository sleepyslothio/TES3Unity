using System.Collections.Generic;
using UnityEngine;
using ur = UnityEngine.Rendering;

namespace TESUnity
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
        public ur.BlendMode srcBlendMode;
        public ur.BlendMode dstBlendMode;
        public bool alphaTest;
        public float alphaCutoff;
        public bool zWrite;
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

            var tes = TESManager.instance;

            // Order is important
            if (tes.materialType != MWMaterialType.Unlit)
            {
#if LWRP_ENABLED
            if (tes.renderPath == RendererType.LightweightRP)
                m_Material = new LightweightMaterial(textureManager);
#endif
#if HDRP_ENABLED
			if (tes.renderPath == RendererType.HDRP)
                m_Material = new HDRPMaterial(textureManager);
#endif
                if (tes.materialType == MWMaterialType.PBR)
                    m_Material = new StandardMaterial(textureManager);
                else
                    m_Material = new DiffuseMaterial(textureManager);
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