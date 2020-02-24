using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace TESUnity.Rendering
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

    public struct TESMaterialTextures
    {
        public string mainFilePath;
        public string darkFilePath;
        public string detailFilePath;
        public string glossFilePath;
        public string glowFilePath;
        public string bumpFilePath;
    }

    public struct TESMaterialProps
    {
        public TESMaterialTextures textures;
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
    public sealed class TESMaterial
    {
        // Material Settings
        public const string URPLitPath = "TESUnity/URP-Lit";
        public const string URPLitCutoffPath = "TESUnity/URP-Lit-Cutoff";
        public const string URPTerrainPath = "Universal Render Pipeline/Terrain/Lit";
        public const string HDRPLitPath = "TESUnity/HDRP-Lit";
        public const string HDRPLitCutoffPath = "TESUnity/HDRP-Lit-Cutoff";
        public const string HDRPTerrainPath = "HDRP/TerrainLit";
        public const string DiffuseParameterName = "_Albedo";
        public const string m_SrcBlendParameter = "_SrcBlend";
        public const string m_DstBlendParameter = "_DstBlend";
        public const string m_CutoutParameter = "_Cutout";
        // Static variables
        private static Material TerrainMaterial = null;
        private static Dictionary<Texture2D, Texture2D> GeneratedNormalMapsStore = new Dictionary<Texture2D, Texture2D>();
        private static Dictionary<TESMaterialProps, Material> MaterialStore = new Dictionary<TESMaterialProps, Material>();
        // Private variables
        private TextureManager m_textureManager;
        private Shader m_Shader = null;
        private Shader m_CutoutShader = null;
        
        private bool m_HDRP;

        public static Material GetTerrainMaterial(bool hdrp = false)
        {
            if (TerrainMaterial == null)
                TerrainMaterial = new Material(Shader.Find(hdrp ? HDRPTerrainPath : URPTerrainPath));

            return TerrainMaterial;
        }

        public TESMaterial(TextureManager textureManager)
        {
            m_textureManager = textureManager;
            m_HDRP = GameSettings.Get().RendererMode == RendererMode.HDRP;
            m_Shader = Shader.Find(m_HDRP ? HDRPLitPath : URPLitPath);
            m_CutoutShader = Shader.Find(m_HDRP ?  HDRPLitCutoffPath : URPLitCutoffPath);
        }

        public Material BuildMaterialFromProperties(TESMaterialProps mp)
        {
            if (MaterialStore.ContainsKey(mp))
                return MaterialStore[mp];

            var material = new Material(mp.alphaBlended ? m_CutoutShader : m_Shader);

            if (mp.alphaBlended)
            {
                material.SetInt(m_SrcBlendParameter, (int)mp.srcBlendMode);
                material.SetInt(m_DstBlendParameter, (int)mp.dstBlendMode);
                material.SetFloat(m_CutoutParameter, 0.75f);
            }

            if (mp.textures.mainFilePath != null)
            {
                var mainTexture = m_textureManager.LoadTexture(mp.textures.mainFilePath);
                material.SetTexture(DiffuseParameterName, mainTexture);
            }

            MaterialStore.Add(mp, material);

            return material;
        }

        public static Texture2D GenerateNormalMap(Texture2D source)
        {
            return GenerateNormalMap(source, TESManager.NormalMapGeneratorIntensity);
        }

        // https://gamedev.stackexchange.com/questions/106703/create-a-normal-map-using-a-script-unity
        public static Texture2D GenerateNormalMap(Texture2D source, float strength)
        {
            if (GeneratedNormalMapsStore.ContainsKey(source))
                return GeneratedNormalMapsStore[source];

            strength = Mathf.Clamp(strength, 0.0F, 100.0f);

            Texture2D normalTexture;
            float xLeft;
            float xRight;
            float yUp;
            float yDown;
            float yDelta;
            float xDelta;

            normalTexture = new Texture2D(source.width, source.height, TextureFormat.RGB24, true);

            for (int y = 0; y < normalTexture.height; y++)
            {
                for (int x = 0; x < normalTexture.width; x++)
                {
                    xLeft = source.GetPixel(x - 1, y).grayscale * strength;
                    xRight = source.GetPixel(x + 1, y).grayscale * strength;
                    yUp = source.GetPixel(x, y - 1).grayscale * strength;
                    yDown = source.GetPixel(x, y + 1).grayscale * strength;
                    xDelta = ((xLeft - xRight) + 1) * 0.5f;
                    yDelta = ((yUp - yDown) + 1) * 0.5f;
                    normalTexture.SetPixel(x, y, new Color(xDelta, yDelta, 1.0f, yDelta));
                }
            }

            normalTexture.Apply();

            return normalTexture;
        }
    }
}
