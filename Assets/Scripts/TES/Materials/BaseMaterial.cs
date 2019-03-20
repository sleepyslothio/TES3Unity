using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace TESUnity
{
    /// <summary>
    /// An abstract class to describe a material.
    /// </summary>
    public abstract class BaseMaterial
    {
        public struct MatProxy
        {
            public MWMaterialProps Props;
            public Material Material;
        }

        protected Dictionary<MWMaterialProps, Material> m_existingMaterials;
        protected List<MatProxy> m_Materials = new List<MatProxy>();
        protected TextureManager m_textureManager;
        protected bool m_GenerateNormalMap;

        public BaseMaterial(TextureManager textureManager)
        {
            m_textureManager = textureManager;
            m_existingMaterials = new Dictionary<MWMaterialProps, Material>();
            m_GenerateNormalMap = GameSettings.Get().GenerateNormalMaps;
        }

        public Material CompareMaterial(MWMaterialProps material)
        {
            foreach (var mat in m_Materials)
            {
                if (CompareMaterialProps(mat.Props, material))
                    return mat.Material;
            }

            return null;
        }

        public bool CompareMaterialProps(MWMaterialProps first, MWMaterialProps second)
        {
            return first.alphaBlended == second.alphaBlended && first.textures.mainFilePath == second.textures.mainFilePath;
        }

        public abstract Material BuildMaterialFromProperties(MWMaterialProps mp);
        public abstract Material BuildMaterial();
        public abstract Material BuildMaterialBlended(BlendMode sourceBlendMode, BlendMode destinationBlendMode);
        public abstract Material BuildMaterialTested(float cutoff = 0.5f);

        public static Texture2D GenerateNormalMap(Texture2D source)
        {
            return GenerateNormalMap(source, TESManager.NormalMapGeneratorIntensity);
        }

        // https://gamedev.stackexchange.com/questions/106703/create-a-normal-map-using-a-script-unity
        public static Texture2D GenerateNormalMap(Texture2D source, float strength)
        {
            strength = Mathf.Clamp(strength, 0.0F, 100.0f);

            Texture2D normalTexture;
            float xLeft;
            float xRight;
            float yUp;
            float yDown;
            float yDelta;
            float xDelta;

            normalTexture = new Texture2D(source.width, source.height, TextureFormat.ARGB32, true);

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
    }
}
