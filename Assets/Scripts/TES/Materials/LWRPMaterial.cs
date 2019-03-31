using UnityEngine;

namespace TESUnity.Rendering
{
    /// <summary>
    /// A material that uses the legacy Bumped Diffuse Shader.
    /// </summary>
    public class LWRPMaterial : BaseMaterial
    {
        public const string LitPath = "Lightweight Render Pipeline/Lit";
        public const string SimpleLitPath = "Lightweight Render Pipeline/Simple Lit";
        private const string DiffuseParameterName = "_BaseMap";
        private const string BumpMapParameterName = "_BumpMap";
        private const string BumpMapKeyword = "_NORMALMAP";

        public LWRPMaterial(TextureManager textureManager)
            : base(textureManager)
        {
            Initialize();
            m_CutoutParameter = "_Cutoff";
        }

        protected virtual void Initialize()
        {
            m_Shader = Shader.Find("Lightweight Render Pipeline/Lit");
            m_CutoutShader = m_Shader;
            m_Material = Resources.Load<Material>($"Rendering/LWRP/Materials/Lit");
            m_CutoutMaterial = Resources.Load<Material>($"Rendering/LWRP/Materials/Lit-Cutout");
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