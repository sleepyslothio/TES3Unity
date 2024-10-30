using UnityEngine;

namespace Demonixis.ToolboxV2
{
    public static class Texture2DExtensions
    {
        public static Sprite ToSprite(this Texture2D texture)
        {
            return Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector3.zero);
        }
    }
}
