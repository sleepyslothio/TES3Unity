using UnityEngine;

namespace TESUnity.Rendering
{
    /// <summary>
    /// A material that uses the legacy Bumped Diffuse Shader.
    /// </summary>
    public class DiffuseMaterial : BaseMaterial
    {
        public DiffuseMaterial(TextureManager textureManager) : base(textureManager)
        {
            m_Shader = Shader.Find("Legacy Shaders/Bumped Diffuse");
            m_CutoutShader = Shader.Find("Legacy Shaders/Transparent/Cutout/Bumped Diffuse");
            m_CutoutParameter = "_AlphaCutoff";
        }

        protected override void SetupMaterial(Material material, MWMaterialProps mp)
        {
            if (mp.textures.mainFilePath != null)
            {
                material.mainTexture = m_textureManager.LoadTexture(mp.textures.mainFilePath);

                if (m_GenerateNormalMap)
                    material.SetTexture("_BumpMap", GenerateNormalMap((Texture2D)material.mainTexture));
            }

            if (mp.textures.bumpFilePath != null)
                material.SetTexture("_BumpMap", m_textureManager.LoadTexture(mp.textures.bumpFilePath));
        }
    }
}