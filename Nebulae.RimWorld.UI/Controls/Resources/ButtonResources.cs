using Nebulae.RimWorld.UI.Controls.Basic;
using UnityEngine;
using Verse;

namespace Nebulae.RimWorld.UI.Controls.Resources
{
    /// <summary>
    /// <see cref="Button"/> 的资源
    /// </summary>
    [StaticConstructorOnStartup]
    public static class ButtonResources
    {
        /// <summary>
        /// 默认的光标位于按钮上方时的按钮背景
        /// </summary>
        public static readonly Texture2D CursorOverBackground = ContentFinder<Texture2D>.Get("UI/Widgets/ButtonBGMouseover");

        /// <summary>
        /// 默认的一般状态下的按钮背景
        /// </summary>
        public static readonly Texture2D NormalBackground = ContentFinder<Texture2D>.Get("UI/Widgets/ButtonBG");

        /// <summary>
        /// 默认的按钮被按下时的按钮背景
        /// </summary>
        public static readonly Texture2D PressedBackground = ContentFinder<Texture2D>.Get("UI/Widgets/ButtonBGClick");

        /// <summary>
        /// 破坏性按钮的默认混合色
        /// </summary>
        public static readonly Color DistructiveComposition = new Color(1f, 0.3f, 0.35f);
    }
}
