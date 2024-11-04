using System.Collections.Generic;
using Demonixis.ToolboxV2;
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
        private const string Tes3LitMobilePath = "TESUnity/URP-Lit_Mobile";
        private const string Tes3LitCutoffPath = "TESUnity/URP-Lit-Cutoff";
        private const string Tes3LitCutoffMobilePath = "TESUnity/URP-Lit-Cutoff_Mobile";
        private const string UrpTerrainPath = "Universal Render Pipeline/Terrain/Lit";
        private const string DiffuseParameterName = "_BaseMap";

        private const string CutoutParameter = "_Cutoff";

        // Static variables
        private static Material _terrainMaterial;

        private static readonly Dictionary<TES3MaterialProps, Material> MaterialStore = new();

        // Private variables
        private readonly TextureManager _textureManager;
        private readonly Shader _shader;
        private readonly Shader _cutoutShader;
        private static readonly int BaseMap = Shader.PropertyToID(DiffuseParameterName);
        private static readonly int Cutoff = Shader.PropertyToID(CutoutParameter);

        private static (string, string) GetShadersPaths(bool lq)
        {
            return lq
                ? (Tes3LitMobilePath, Tes3LitCutoffMobilePath)
                : (Tes3LitPath, Tes3LitCutoffPath);
        }

        public Tes3Material(TextureManager textureManager, bool lq)
        {
            _textureManager = textureManager;

            var paths = GetShadersPaths(lq);
            _shader = Shader.Find(paths.Item1);
            _cutoutShader = Shader.Find(paths.Item2);
        }

        public Material BuildMaterialFromProperties(TES3MaterialProps mp)
        {
            if (MaterialStore.TryGetValue(mp, out var properties))
                return properties;

            var material = new Material(mp.alphaBlended ? _cutoutShader : _shader);

            if (mp.alphaBlended)
                material.SetFloat(Cutoff, 0.5f);

            if (mp.textures.mainFilePath != null)
            {
                var mainTexture = _textureManager.LoadTexture(mp.textures.mainFilePath);
                material.SetTexture(BaseMap, mainTexture);
            }

            MaterialStore.Add(mp, material);

            return material;
        }

        public static Material GetTerrainMaterial()
        {
            if (_terrainMaterial == null)
                _terrainMaterial = new Material(Shader.Find(UrpTerrainPath));

            return _terrainMaterial;
        }
    }
}