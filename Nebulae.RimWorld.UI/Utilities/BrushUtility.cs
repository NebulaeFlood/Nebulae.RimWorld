using UnityEngine;
using Verse;

namespace Nebulae.RimWorld.UI.Utilities
{
    /// <summary>
    /// 画刷工具类
    /// </summary>
    public static class BrushUtility
    {
        /// <summary>
        /// 灰色画刷
        /// </summary>
        public static readonly Texture2D Grey = SolidColorMaterials.NewSolidColorTexture(ColorLibrary.Grey);

        /// <summary>
        /// 透明画刷
        /// </summary>
        public static readonly Texture2D Transparent = SolidColorMaterials.NewSolidColorTexture(new Color(1f, 1f, 1f, 0f));


        /// <summary>
        /// 将 <see cref="Color"/> 转化为纯色画刷
        /// </summary>
        /// <param name="color">要转化的颜色</param>
        /// <returns><paramref name="color"/> 对应颜色的纯色画刷。</returns>
        public static Texture2D ToBrush(this Color color)
        {
            return SolidColorMaterials.NewSolidColorTexture(color);
        }
    }
}
