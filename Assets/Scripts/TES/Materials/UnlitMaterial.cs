using UnityEngine;

namespace TESUnity.Rendering
{
    /// <summary>
    /// A material that uses the Unlit Shader.
    /// </summary>
    public class UnliteMaterial : BaseMaterial
    {
        public UnliteMaterial(TextureManager textureManager) : base(textureManager)
        {
            m_Shader = Shader.Find("Unlit/Texture");
            m_CutoutShader = Shader.Find("Unlit/Transparent Cutout");
            m_CutoutParameter = "_AlphaCutoff";
        }

        protected override void SetupMaterial(Material material, MWMaterialProps mp)
        {
            if (mp.textures.mainFilePath != null)
                material.mainTexture = m_textureManager.LoadTexture(mp.textures.mainFilePath);
        }
    }
}