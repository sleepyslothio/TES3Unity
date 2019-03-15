#if HDRP_ENABLED
using UnityEngine;
using ur = UnityEngine.Rendering;

namespace TESUnity
{
    /// <summary>
    /// A material that uses the legacy Bumped Diffuse Shader.
    /// </summary>
    public class HDRPMaterial : BaseMaterial
    {
		public const string LitName = "HDRP/Lit";
		
        private Material m_LitMaterial;
        private Material m_LitCutout;

        public HDRPMaterial(TextureManager textureManager)
            : base(textureManager)
        {
            m_LitMaterial = Resources.Load<Material>("Materials/HDRP/Lit");
            m_LitCutout = Resources.Load<Material>("Materials/HDRP/Lit-Cutout");
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

                if (mp.textures.mainFilePath != null)
                {
                    var albedoMap = m_textureManager.LoadTexture(mp.textures.mainFilePath);
                    material.SetTexture("_BaseColorMap", albedoMap);

                    if (TESManager.instance.generateNormalMap)
                        material.SetTexture("_NormalMap", GenerateNormalMap(albedoMap, TESManager.instance.normalGeneratorIntensity));
                }

                if (mp.textures.bumpFilePath != null)
                    material.SetTexture("_NormalMap", m_textureManager.LoadTexture(mp.textures.bumpFilePath));

                m_existingMaterials[mp] = material;
            }
            return material;
        }

        public override Material BuildMaterial()
        {
            var material = new Material(Shader.Find("HDRenderPipeline/Lit"));
            material.CopyPropertiesFromMaterial(m_LitMaterial);
            material.EnableKeyword("_NORMALMAP");
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
            material.EnableKeyword("_NORMALMAP");
            return material;
        }
    }
}
#endif