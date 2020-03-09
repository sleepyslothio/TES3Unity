using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace TES3Unity.Rendering
{
    public enum MatTestMode
    {
        Always = 0,
        Less,
        LEqual,
        Equal,
        GEqual,
        Greater,
        NotEqual,
        Never
    }

    public struct TES3MaterialTextures
    {
        public string mainFilePath;
        public string darkFilePath;
        public string detailFilePath;
        public string glossFilePath;
        public string glowFilePath;
        public string bumpFilePath;
    }

    public struct TES3MaterialProps
    {
        public TES3MaterialTextures textures;
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
    /// A material container compatible with Universal RP and HDRP.
    /// </summary>
    public sealed class TES3Material
    {
        // Material Settings
        public const string URPLitPath = "TESUnity/URP-Lit";
        public const string URPLitCutoffPath = "TESUnity/URP-Lit-Cutoff";
        public const string URPTerrainPath = "Universal Render Pipeline/Terrain/Lit";
        public const string DiffuseParameterName = "_Albedo";
        public const string m_SrcBlendParameter = "_SrcBlend";
        public const string m_DstBlendParameter = "_DstBlend";
        public const string m_CutoutParameter = "_Cutout";
        // Static variables
        private static Material TerrainMaterial = null;
        private static Dictionary<TES3MaterialProps, Material> MaterialStore = new Dictionary<TES3MaterialProps, Material>();
        // Private variables
        private TextureManager m_textureManager;
        private Shader m_Shader = null;
        private Shader m_CutoutShader = null;

        public TES3Material(TextureManager textureManager)
        {
            m_textureManager = textureManager;
            m_Shader = Shader.Find(URPLitPath);
            m_CutoutShader = Shader.Find(URPLitCutoffPath);
        }

        public Material BuildMaterialFromProperties(TES3MaterialProps mp)
        {
            if (MaterialStore.ContainsKey(mp))
            {
                return MaterialStore[mp];
            }

            var material = new Material(mp.alphaBlended ? m_CutoutShader : m_Shader);

            if (mp.alphaBlended)
            {                
                material.SetFloat(m_CutoutParameter, 0.5f);
            }

            if (mp.textures.mainFilePath != null)
            {
                var mainTexture = m_textureManager.LoadTexture(mp.textures.mainFilePath);
                material.SetTexture(DiffuseParameterName, mainTexture);
            }

            MaterialStore.Add(mp, material);

            return material;
        }

        public static Material GetTerrainMaterial()
        {
            if (TerrainMaterial == null)
            {
                TerrainMaterial = new Material(Shader.Find(URPTerrainPath));
            }

            return TerrainMaterial;
        }

        public static string GetMaterialAssetPath()
        {
            return $"Rendering/UniversalRP/Materials";
        }

        public static string GetWaterMaterialPath()
        {
            return $"{GetMaterialAssetPath()}/URP-Water";
        }
    }
}
