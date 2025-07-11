using Nebulae.RimWorld.UI.Controls.Basic;
using UnityEngine;
using Verse;

namespace Nebulae.RimWorld.UI.Controls.Resources
{
    /// <summary>
    /// <see cref="Slider"/> 的资源
    /// </summary>
    [StaticConstructorOnStartup]
    public static class SliderResources
    {
        /// <summary>
        /// 滑轨的混合色
        /// </summary>
        public static readonly Color RailColor = new Color(0.6f, 0.6f, 0.6f, 1f);

        /// <summary>
        /// 滑轨的图像
        /// </summary>
        public static readonly Texture2D Handle = ContentFinder<Texture2D>.Get("UI/Buttons/SliderHandle");

        /// <summary>
        /// 滑块的图像
        /// </summary>
        public static readonly Texture2D RailAtlas = ContentFinder<Texture2D>.Get("UI/Buttons/SliderRail");
    }
}
