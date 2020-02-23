using System.Collections.Generic;
using UnityEngine;

namespace TESUnity
{
    /// <summary>
    /// Manages loading and instantiation of Morrowind textures.
    /// </summary>
    public class TextureManager
    {
        private MorrowindDataReader _dataReader;
        private Dictionary<string, Texture2D> _cachedTextures = new Dictionary<string, Texture2D>();

        public TextureManager(MorrowindDataReader reader)
        {
            _dataReader = reader;
        }

        public Texture2D LoadTexture(string texturePath, bool flipVertically = false)
        {
            if (_cachedTextures.ContainsKey(texturePath))
            {
                return _cachedTextures[texturePath];
            }

            var textureInfo = _dataReader.LoadTexture(texturePath);
            var texture = (textureInfo != null) ? textureInfo.ToTexture2D() : new Texture2D(1, 1);

            if (flipVertically)
            {
                TextureUtils.FlipTexture2DVertically(texture);
            }

            _cachedTextures.Add(texturePath, texture);

            return texture;
        }

        #region Async Texture Loading

        public void PreloadTextureFileAsync(string texturePath)
        {
        }

        #endregion
    }
}