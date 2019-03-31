using UnityEngine;

namespace TESUnity.Rendering
{
    /// <summary>
    /// A material that uses the new Standard Shader.
    /// </summary>
    public class StandardMaterial : BaseMaterial
    {
        public StandardMaterial(TextureManager textureManager)
            : base(textureManager)
        {
            m_Shader = Shader.Find("Standard");
            m_CutoutShader = m_Shader;
            m_Material = Resources.Load<Material>($"Rendering/Legacy/Materials/Standard");
            m_CutoutMaterial = Resources.Load<Material>($"Rendering/Legacy/Materials/Standard-Cutout");
        }

        protected override void SetupMaterial(Material material, MWMaterialProps mp)
        {
            if (mp.textures.mainFilePath != null)
            {
                var mainTexture = (Texture2D)m_textureManager.LoadTexture(mp.textures.mainFilePath);
                material.mainTexture = mainTexture;

                if (m_GenerateNormalMap && mp.textures.bumpFilePath == null)
                    TryEnableTexture(material, GenerateNormalMap(mainTexture), "_BumpMap", "_NORMALMAP");
            }

            if (mp.textures.bumpFilePath != null)
                TryEnableTexture(material, mp.textures.bumpFilePath, "_BumpMap", "_NORMALMAP");

            TryEnableTexture(material, mp.textures.bumpFilePath, "_MetallicGlossMap", "_METALLICGLOSSMAP");
            TryEnableTexture(material, mp.textures.bumpFilePath, "_EmissionMap", "_EMISSION");
            TryEnableTexture(material, mp.textures.bumpFilePath, "_DetailMask", "_DETAIL_MULX2");

            material.SetColor("_Color", mp.diffuseColor);
            material.SetColor("_Glossiness", mp.specularColor);

            if (mp.emissiveColor != Color.white)
            {
                material.SetColor("_EmissionColor", mp.emissiveColor);
                material.EnableKeyword("_EMISSION");
            }

            if (mp.alphaCutoff > 0)
                material.SetFloat("_Cutoff", mp.alphaCutoff);
        }
    }
}