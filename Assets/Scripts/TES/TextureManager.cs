using System.Collections.Generic;
using UnityEngine;

namespace TESUnity
{
    /// <summary>
    /// Manages loading and instantiation of Morrowind textures.
    /// </summary>
    public class TextureManager
    {
        private static Dictionary<string, Texture2D> TextureStore = new Dictionary<string, Texture2D>();
        private static Dictionary<Color, Texture2D> MaskTextureStore = new Dictionary<Color, Texture2D>();
        private MorrowindDataReader _dataReader;

        public TextureManager(MorrowindDataReader reader)
        {
            _dataReader = reader;
        }

        /// <summary>
        /// Create a mast texture used by HDRP material.
        /// </summary>
        /// <param name="r">Metallic</param>
        /// <param name="g">Occlusion</param>
        /// <param name="b">Detail Mask</param>
        /// <param name="a">Smoothness</param>
        /// <returns>A mask texture.</returns>
        public static Texture2D CreateMaskTexture(float r, float g, float b, float a)
        {
            var color = new Color(r, g, b, a);

            if (MaskTextureStore.ContainsKey(color))
            {
                return MaskTextureStore[color];
            }

            var texture = new Texture2D(1, 1);
            texture.SetPixels(new Color[1] { color });
            texture.Apply();

            MaskTextureStore.Add(color, texture);

            return texture;
        }

        public Texture2D LoadTexture(string texturePath, bool flipVertically = false)
        {
            if (TextureStore.ContainsKey(texturePath))
            {
                return TextureStore[texturePath];
            }

            var textureInfo = _dataReader.LoadTexture(texturePath);
            var texture = (textureInfo != null) ? textureInfo.ToTexture2D() : new Texture2D(1, 1);

            if (flipVertically)
            {
                TextureUtils.FlipTexture2DVertically(texture);
            }

            TextureStore.Add(texturePath, texture);

            return texture;
        }

        #region Async Texture Loading

        /// <summary>
        /// Deprecated: For compatibility reason, will be removed soon.
        /// </summary>
        /// <param name="texturePath"></param>
        public void PreloadTextureFileAsync(string texturePath)
        {
        }

        #endregion
    }
}