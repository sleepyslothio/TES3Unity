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
        public const string MaterialPath = "Rendering/LWRP/Materials";
        private const string DiffuseParameterName = "_BaseMap";
        private const string BumpMapParameterName = "_BumpMap";
        private const string BumpMapKeyword = "_NORMALMAP";

        private Material m_StandardPBR;
        private Material m_StandardSimple;
        private Material m_CutoutPBRMaterial;
        private Material m_CutoutSimpleMaterial;
        private bool m_PBR;

        public LightweightMaterial(TextureManager textureManager)
            : base(textureManager)
        {
            m_StandardPBR = Resources.Load<Material>($"{MaterialPath}Lit");
            m_StandardSimple = Resources.Load<Material>($"{MaterialPath}/SimpleLit");
            m_CutoutPBRMaterial = Resources.Load<Material>($"{MaterialPath}/Lit-Cutout");
            m_CutoutSimpleMaterial = Resources.Load<Material>($"{MaterialPath}/SimpleLit-Cutout");
            m_PBR = GameSettings.Get().MaterialType == MWMaterialType.PBR;
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

                    material.SetTexture(DiffuseParameterName, mainTexture);

                    if (m_GenerateNormalMap && mp.textures.bumpFilePath == null)
                    {
                        material.SetTexture(BumpMapParameterName, GenerateNormalMap(mainTexture));
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
            var material = new Material(Shader.Find(m_PBR ? LitName : SimpleLitName));
            material.CopyPropertiesFromMaterial(m_PBR ? m_StandardPBR : m_StandardSimple);
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
            material.CopyPropertiesFromMaterial(m_PBR ? m_CutoutPBRMaterial : m_CutoutSimpleMaterial);
            material.SetFloat("_Cutoff", cutoff);
            return material;
        }
    }
}