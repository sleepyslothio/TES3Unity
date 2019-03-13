using UnityEngine;
using ur = UnityEngine.Rendering;

namespace TESUnity
{
    /// <summary>
    /// A material that uses the legacy Bumped Diffuse Shader.
    /// </summary>
    public class LightweightMaterial : BaseMaterial
    {
        public const string LitName = "Lightweight Render Pipeline/Lit";
        public const string SimpleLitName = "Lightweight Render Pipeline/Simple Lit";

        private Material m_StandardPBR;
        private Material m_StandardSimple;
        private Material m_CutoutPBRMaterial;
        private Material m_CutoutSimpleMaterial;

        public LightweightMaterial(TextureManager textureManager)
            : base(textureManager)
        {
            m_StandardPBR = Resources.Load<Material>("Materials/LWRP/Lit");
            m_StandardSimple = Resources.Load<Material>("Materials/LWRP/SimpleLit");
            m_CutoutPBRMaterial = Resources.Load<Material>("Materials/LWRP/Lit-Cutout");
            m_CutoutSimpleMaterial = Resources.Load<Material>("Materials/LWRP/SimpleLit-Cutout");
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
                    var mainTexture = m_textureManager.LoadTexture(mp.textures.mainFilePath);

                    material.SetTexture("_BaseMap", mainTexture);

                    if (TESManager.instance.generateNormalMap && mp.textures.bumpFilePath == null)
                    {
                        material.SetTexture("_BumpMap", GenerateNormalMap(mainTexture, TESManager.instance.normalGeneratorIntensity));
                        hasNormalMap = true;
                    }
                }

                if (mp.textures.bumpFilePath != null)
                {
                    material.SetTexture("_BumpMap", m_textureManager.LoadTexture(mp.textures.bumpFilePath));
                    hasNormalMap = true;
                }

                if (hasNormalMap)
                    material.EnableKeyword("_NORMALMAP");
                else
                    material.DisableKeyword("_NORMALMAP");

                m_existingMaterials[mp] = material;
            }
            return material;
        }

        public override Material BuildMaterial()
        {
            var pbr = TESManager.instance.materialType == TESManager.MWMaterialType.PBR;
            var material = new Material(Shader.Find(pbr ? LitName : SimpleLitName));
            material.CopyPropertiesFromMaterial(pbr ? m_StandardPBR : m_StandardSimple);
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
            var pbr = TESManager.instance.materialType == TESManager.MWMaterialType.PBR;
            material.CopyPropertiesFromMaterial(pbr ? m_CutoutPBRMaterial : m_CutoutSimpleMaterial);
            material.SetFloat("_Cutoff", cutoff);
            return material;
        }
    }
}