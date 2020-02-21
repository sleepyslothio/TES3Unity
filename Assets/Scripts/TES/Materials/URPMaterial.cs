using UnityEngine;

namespace TESUnity.Rendering
{
    /// <summary>
    /// A material that uses the legacy Bumped Diffuse Shader.
    /// </summary>
    public class URPMaterial : BaseMaterial
    {
        public const string LitPath = "TESUnity/MWStandard";
        public const string LitCutoffPath = "TESUnity/MWStandard-Cutoff";
        private const string DiffuseParameterName = "Albedo";
        private const string BumpMapParameterName = "";
        private const string BumpMapKeyword = "";
        private static Material TerrainMaterial = null;

        public static Material GetTerrainMaterial()
        {
            if (TerrainMaterial == null)
                TerrainMaterial = new Material(Shader.Find("Universal Render Pipeline/Terrain/Lit"));

            return TerrainMaterial;
        }

        public URPMaterial(TextureManager textureManager)
            : base(textureManager)
        {
            m_Shader = Shader.Find(LitPath);
            m_CutoutShader = Shader.Find(LitCutoffPath);
            m_CutoutParameter = "_Cutoff";
        }

        protected override void SetupMaterial(Material material, MWMaterialProps mp)
        {
            if (mp.textures.mainFilePath != null)
            {
                var mainTexture = m_textureManager.LoadTexture(mp.textures.mainFilePath);
                material.SetTexture(DiffuseParameterName, mainTexture);
            }
        }
    }
}