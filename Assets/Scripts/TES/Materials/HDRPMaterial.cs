using UnityEngine;
using ur = UnityEngine.Rendering;

namespace TESUnity
{
    /// <summary>
    /// A material that uses the legacy Bumped Diffuse Shader.
    /// </summary>
    public class HDRPMaterial : BaseMaterial
    {
        public const string LitName = "HDRenderPipeline/Lit";
        private const string DiffuseParameterName = "_BaseColorMap";
        private const string BumpMapParameterName = "_NormalMap";
        private const string BumpMapKeyword = "_NORMALMAP";

        private Material m_LitMaterial;
        private Material m_LitCutout;

        public HDRPMaterial(TextureManager textureManager)
            : base(textureManager)
        {
            m_LitMaterial = Resources.Load<Material>("Rendering/HDRP/Materials/Lit");
            m_LitCutout = Resources.Load<Material>("Rendering/HDRP/Materials/Lit-Cutout");
        }

        public override Material BuildMaterialFromProperties(MWMaterialProps mp)
        {
            Material material;

            //check if the material is already cached
            if (!m_existingMaterials.TryGetValue(mp, out material))
            {
                //otherwise create a new material and cache it
                if (mp.alphaBlended)
                    material = BuildMaterialBlended(mp.srcBlendMode, mp.dstBlendMode);
                else if (mp.alphaTest)
                    material = BuildMaterialTested(mp.alphaCutoff);
                else
                    material = BuildMaterial();

                var hasNormalMap = false;

                if (mp.textures.mainFilePath != null)
                {
                    var albedoMap = m_textureManager.LoadTexture(mp.textures.mainFilePath);
                    material.SetTexture(DiffuseParameterName, albedoMap);

                    if (m_GenerateNormalMap && mp.textures.bumpFilePath == null)
                    {
                        material.SetTexture(BumpMapParameterName, GenerateNormalMap(albedoMap));
                        hasNormalMap = true;
                    }
                }

                if (mp.textures.bumpFilePath != null)
                {
                    material.SetTexture(BumpMapParameterName, m_textureManager.LoadTexture(mp.textures.bumpFilePath));
                    hasNormalMap = true;
                }

                if (hasNormalMap)
                    material.EnableKeyword(BumpMapKeyword);
                else
                    material.DisableKeyword(BumpMapKeyword);

                m_existingMaterials[mp] = material;
            }
            return material;
        }

        public override Material BuildMaterial()
        {
            var material = new Material(Shader.Find(LitName));
            material.CopyPropertiesFromMaterial(m_LitMaterial);
            return material;
        }

        public override Material BuildMaterialBlended(ur.BlendMode sourceBlendMode, ur.BlendMode destinationBlendMode)
        {
            var material = BuildMaterialTested();
            material.SetInt("_SrcBlend", (int)sourceBlendMode);
            material.SetInt("_DstBlend", (int)destinationBlendMode);
            return material;
        }

        public override Material BuildMaterialTested(float cutoff = 0.5f)
        {
            var material = BuildMaterial();
            material.CopyPropertiesFromMaterial(m_LitCutout);
            material.SetFloat("_Cutout", cutoff);
            return material;
        }
    }
}