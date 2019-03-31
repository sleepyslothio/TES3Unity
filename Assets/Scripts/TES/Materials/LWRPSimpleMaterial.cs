using UnityEngine;

namespace TESUnity.Rendering
{
    /// <summary>
    /// A material that uses the legacy Bumped Diffuse Shader.
    /// </summary>
    public class LWRPSimpleMaterial : LWRPMaterial
    {
        public LWRPSimpleMaterial(TextureManager textureManager) : base(textureManager)
        {
        }

        protected override void Initialize()
        {
            m_Shader = Shader.Find(SimpleLitPath);
            m_CutoutShader = m_Shader;
            m_Material = Resources.Load<Material>($"Rendering/LWRP/Materials/SimpleLit");
            m_CutoutMaterial = Resources.Load<Material>($"Rendering/LWRP/Materials/SimpleLit-Cutout");
        }
    }
}