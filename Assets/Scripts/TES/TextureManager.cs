using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace TESUnity
{
	/// <summary>
	/// Manages loading and instantiation of Morrowind textures.
	/// </summary>
	public class TextureManager
	{
        private MorrowindDataReader _dataReader;
        private Dictionary<string, Task<Texture2DInfo>> _textureFilePreloadTasks = new Dictionary<string, Task<Texture2DInfo>>();
        private Dictionary<string, Texture2D> _cachedTextures = new Dictionary<string, Texture2D>();

        public TextureManager(MorrowindDataReader reader)
		{
			_dataReader = reader;
		}

        /// <summary>
        /// Loads a texture.
        /// </summary>
        /// <param name="texturePath">The texture's path</param>
        /// <param name="flipVertically">Indicates if the texture must be vertically flipped. Default is False.</param>
        /// <returns></returns>
        public Texture2D LoadTexture(string texturePath, bool flipVertically = false)
        {
            Texture2D texture;

            if (!_cachedTextures.TryGetValue(texturePath, out texture))
            {
                // Load & cache the texture.
                var textureInfo = LoadTextureInfo(texturePath);

                texture = (textureInfo != null) ? textureInfo.ToTexture2D() : new Texture2D(1, 1);
                if(flipVertically) { TextureUtils.FlipTexture2DVertically(texture); }

                _cachedTextures[texturePath] = texture;
            }
            
			return texture;
		}

        public void PreloadTextureFileAsync(string texturePath)
        {
            // If the texture has already been created we don't have to load the file again.
            if(_cachedTextures.ContainsKey(texturePath)) { return; }

            Task<Texture2DInfo> textureFileLoadingTask;

            // Start loading the texture file asynchronously if we haven't already started.
            if(!_textureFilePreloadTasks.TryGetValue(texturePath, out textureFileLoadingTask))
            {
                textureFileLoadingTask = _dataReader.LoadTextureAsync(texturePath);
                _textureFilePreloadTasks[texturePath] = textureFileLoadingTask;
            }
        }

        private Texture2DInfo LoadTextureInfo(string texturePath)
        {
            Debug.Assert(!_cachedTextures.ContainsKey(texturePath));

            PreloadTextureFileAsync(texturePath);
            var textureInfo = _textureFilePreloadTasks[texturePath].Result;
            _textureFilePreloadTasks.Remove(texturePath);

            return textureInfo;
        }
	}
}