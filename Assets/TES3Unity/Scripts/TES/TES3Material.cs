using System.Collections.Generic;
using UnityEditor.Rendering;
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
    public sealed class Tes3Material
    {
        // Material Settings
        private const string Tes3LitPath = "TESUnity/URP-Lit";
        private const string Tes3LitCutoffPath = "TESUnity/URP-Lit-Cutoff";
        public const string UrpLitPath = "Universal Render Pipeline/Lit";
        private const string UrpSimpleLitPath = "Universal Render Pipeline/Simple Lit";
        private const string UrpTerrainPath = "Universal Render Pipeline/Terrain/Lit";
        private const string DiffuseParameterName = "_BaseMap";
        private const string NormalParameterName = "_BumpMap";
        public const string SrcBlendParameter = "_SrcBlend";
        public const string DstBlendParameter = "_DstBlend";
        private const string CutoutParameter = "_Cutoff";
        // Static variables
        private static Material _terrainMaterial;
        private static Material _simpleLitCutoffMaterial;
        private static readonly Dictionary<TES3MaterialProps, Material> MaterialStore = new Dictionary<TES3MaterialProps, Material>();
        // Private variables
        private readonly TextureManager _textureManager;
        private readonly Shader _shader;
        private readonly Shader _cutoutShader;
        private readonly bool _lowQuality;
        private readonly bool _generateNormals;
        private static readonly int BumpMap = Shader.PropertyToID(NormalParameterName);
        private static readonly int BaseMap = Shader.PropertyToID(DiffuseParameterName);
        private static readonly int Cutoff = Shader.PropertyToID(CutoutParameter);

        public Tes3Material(TextureManager textureManager, bool lowQuality, bool generateNormal)
        {
            _textureManager = textureManager;
            _lowQuality = lowQuality;
            _shader = Shader.Find(!_lowQuality ? Tes3LitPath : UrpSimpleLitPath);
            _cutoutShader = Shader.Find(!_lowQuality ? Tes3LitCutoffPath : UrpSimpleLitPath);
            _generateNormals = generateNormal;
        }

        public Material BuildMaterialFromProperties(TES3MaterialProps mp)
        {
            if (MaterialStore.TryGetValue(mp, out var properties))
            {
                return properties;
            }

            var material = new Material(mp.alphaBlended ? _cutoutShader : _shader);

            if (mp.alphaBlended)
            {
                if (_lowQuality)
                {
                    if (_simpleLitCutoffMaterial == null)
                    {
                        _simpleLitCutoffMaterial = Resources.Load<Material>($"{GetMaterialAssetPath()}/URP-SimpleLit-Cutout");
                    }

                    material.CopyPropertiesFromMaterial(_simpleLitCutoffMaterial);
                }

                material.SetFloat(Cutoff, 0.5f);
            }

            if (mp.textures.mainFilePath != null)
            {
                var mainTexture = _textureManager.LoadTexture(mp.textures.mainFilePath);
                material.SetTexture(BaseMap, mainTexture);

                if (_lowQuality && _generateNormals)
                {
                    var normalMap = TextureManager.GenerateNormalMap(mainTexture, 2);
                    material.SetTexture(BumpMap, normalMap);
                }
            }

            MaterialStore.Add(mp, material);

            return material;
        }

        public static Material GetTerrainMaterial()
        {
            if (_terrainMaterial == null)
            {
                _terrainMaterial = new Material(Shader.Find(UrpTerrainPath));
            }

            return _terrainMaterial;
        }

        public static string GetMaterialAssetPath()
        {
            return "Rendering/UniversalRP/Materials";
        }

        public static string GetWaterMaterialPath()
        {
            return $"{GetMaterialAssetPath()}/TES3-Water";
        }
    }
}
