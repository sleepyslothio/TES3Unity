using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace TESUnity.Rendering
{
    /// <summary>
    /// An abstract class to describe a material.
    /// </summary>
    public abstract class BaseMaterial
    {
        private static Dictionary<Texture2D, Texture2D> m_GeneratedNormalMaps = new Dictionary<Texture2D, Texture2D>();
        protected static Dictionary<MWMaterialProps, Material> m_existingMaterials = new Dictionary<MWMaterialProps, Material>();
        protected TextureManager m_textureManager;
        protected bool m_GenerateNormalMap;
        protected Material m_Material = null;
        protected Material m_CutoutMaterial = null;
        protected Shader m_Shader = null;
        protected Shader m_CutoutShader = null;
        protected string m_SrcBlendParameter = "_SrcBlend";
        protected string m_DstBlendParameter = "_DstBlend";
        protected string m_CutoutParameter = "_Cutout";

        public BaseMaterial(TextureManager textureManager)
        {
            m_textureManager = textureManager;
            m_GenerateNormalMap = GameSettings.Get().GenerateNormalMaps;
        }

        public virtual Material BuildMaterialFromProperties(MWMaterialProps mp)
        {
            Material material = null;

            if (m_existingMaterials.TryGetValue(mp, out material))
                return material;

            if (mp.alphaBlended)
                material = BuildMaterialBlended(mp.srcBlendMode, mp.dstBlendMode);
            else if (mp.alphaTest)
                material = BuildMaterialTested(mp.alphaCutoff);
            else
                material = BuildMaterial();

            SetupMaterial(material, mp);

            m_existingMaterials.Add(mp, material);

            return material;
        }

        protected abstract void SetupMaterial(Material material, MWMaterialProps mp);

        protected virtual Material BuildMaterial()
        {
            var material = new Material(m_Shader);

            if (m_Material != null)
                material.CopyPropertiesFromMaterial(m_Material);

            return material;
        }

        protected virtual Material BuildMaterialBlended(BlendMode sourceBlendMode, BlendMode destinationBlendMode)
        {
            var material = BuildMaterialTested();
            material.SetInt(m_SrcBlendParameter, (int)sourceBlendMode);
            material.SetInt(m_DstBlendParameter, (int)destinationBlendMode);
            return material;
        }

        protected virtual Material BuildMaterialTested(float cutoff = 0.5f)
        {
            var material = new Material(m_CutoutShader);

            if (m_CutoutMaterial != null)
                material.CopyPropertiesFromMaterial(m_CutoutMaterial);

            material.SetFloat(m_CutoutParameter, cutoff);
            return material;
        }

        protected void TryEnableTexture(Material material, string texturePath, string textureParameter, string shaderKeyword)
        {
            var enabled = texturePath != null;

            if (enabled)
                material.SetTexture(textureParameter, m_textureManager.LoadTexture(texturePath));

            if (shaderKeyword != null)
            {
                if (enabled)
                    material.EnableKeyword(shaderKeyword);
                else
                    material.DisableKeyword(shaderKeyword);
            }
        }

        protected void TryEnableTexture(Material material, Texture2D texture, string textureParameter, string shaderKeyword)
        {
            material.SetTexture(textureParameter, texture);

            if (shaderKeyword != null)
            {
                if (texture != null)
                    material.EnableKeyword(shaderKeyword);
                else
                    material.DisableKeyword(shaderKeyword);
            }
        }

        #region Normal Map Generator

        public static Texture2D GenerateNormalMap(Texture2D source)
        {
            return GenerateNormalMap(source, TESManager.NormalMapGeneratorIntensity);
        }

        // https://gamedev.stackexchange.com/questions/106703/create-a-normal-map-using-a-script-unity
        public static Texture2D GenerateNormalMap(Texture2D source, float strength)
        {
            if (m_GeneratedNormalMaps.ContainsKey(source))
                return m_GeneratedNormalMaps[source];

            strength = Mathf.Clamp(strength, 0.0F, 100.0f);

            Texture2D normalTexture;
            float xLeft;
            float xRight;
            float yUp;
            float yDown;
            float yDelta;
            float xDelta;

            normalTexture = new Texture2D(source.width, source.height, TextureFormat.RGB24, true);

            for (int y = 0; y < normalTexture.height; y++)
            {
                for (int x = 0; x < normalTexture.width; x++)
                {
                    xLeft = source.GetPixel(x - 1, y).grayscale * strength;
                    xRight = source.GetPixel(x + 1, y).grayscale * strength;
                    yUp = source.GetPixel(x, y - 1).grayscale * strength;
                    yDown = source.GetPixel(x, y + 1).grayscale * strength;
                    xDelta = ((xLeft - xRight) + 1) * 0.5f;
                    yDelta = ((yUp - yDown) + 1) * 0.5f;
                    normalTexture.SetPixel(x, y, new Color(xDelta, yDelta, 1.0f, yDelta));
                }
            }

            normalTexture.Apply();

            return normalTexture;
        }

        #endregion
    }
}
