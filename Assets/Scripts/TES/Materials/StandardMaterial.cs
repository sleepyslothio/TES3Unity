using UnityEngine;
using ur = UnityEngine.Rendering;

namespace TESUnity
{
    /// <summary>
    /// A material that uses the new Standard Shader.
    /// </summary>
    public class StandardMaterial : BaseMaterial
    {
        private Material _standardMaterial;
        private Material _standardCutoutMaterial;

        public StandardMaterial(TextureManager textureManager)
            : base(textureManager)
        {
            _standardMaterial = Resources.Load<Material>("Materials/Standard/Standard");
            _standardCutoutMaterial = Resources.Load<Material>("Materials/Standard/Standard-Cutout");
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
                    material.mainTexture = m_textureManager.LoadTexture(mp.textures.mainFilePath);

                    if (TESManager.instance.generateNormalMap && mp.textures.bumpFilePath == null)
                    {
                        material.SetTexture("_BumpMap", GenerateNormalMap((Texture2D)material.mainTexture, TESManager.instance.normalGeneratorIntensity));
                        hasNormalMap = true;
                    }
                }

                if (mp.textures.bumpFilePath != null)
                {
                    material.SetTexture("_NORMALMAP", m_textureManager.LoadTexture(mp.textures.bumpFilePath));
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
            var material = new Material(Shader.Find("Standard"));
            material.CopyPropertiesFromMaterial(_standardMaterial);
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
            var material = new Material(Shader.Find("Standard"));
            material.CopyPropertiesFromMaterial(_standardCutoutMaterial);
            material.SetFloat("_Cutout", cutoff);
            material.EnableKeyword("_NORMALMAP");
            return material;
        }
    }
}