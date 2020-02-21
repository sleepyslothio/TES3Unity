using UnityEngine;

namespace TESUnity.Rendering
{
    /// <summary>
    /// A material that uses the legacy Bumped Diffuse Shader.
    /// </summary>
    public class HDRPMaterial : BaseMaterial
    {
        public const string LitPath = "HDRP/Lit";
        private const string DiffuseParameterName = "_BaseColorMap";
        private const string BumpMapParameterName = "_NormalMap";
        private const string BumpMapKeyword = "_NORMALMAP";

        public HDRPMaterial(TextureManager textureManager)
            : base(textureManager)
        {
            m_Shader = Shader.Find(LitPath);
            m_CutoutShader = Shader.Find(LitPath); ;
            m_CutoutParameter = "_Cutout";
        }

        protected override void SetupMaterial(Material material, MWMaterialProps mp)
        {
            if (mp.textures.mainFilePath != null)
            {
                var mainTexture = m_textureManager.LoadTexture(mp.textures.mainFilePath);
                material.SetTexture(DiffuseParameterName, mainTexture);

                if (m_GenerateNormalMap && mp.textures.bumpFilePath == null)
                {
                    material.SetTexture(BumpMapParameterName, GenerateNormalMap(mainTexture));
                    TryEnableTexture(material, GenerateNormalMap(mainTexture), BumpMapParameterName, BumpMapKeyword);
                }
            }

            if (mp.textures.bumpFilePath != null)
                TryEnableTexture(material, mp.textures.bumpFilePath, BumpMapParameterName, BumpMapKeyword);
        }
    }
}