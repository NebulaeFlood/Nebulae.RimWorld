using Nebulae.RimWorld.UI.Controls.Basic;
using UnityEngine;
using Verse;

namespace Nebulae.RimWorld.UI.Controls.Resources
{
    /// <summary>
    /// <see cref="RadioButton"/> 的资源
    /// </summary>
    [StaticConstructorOnStartup]
    public static class RadioButtonResources
    {
        /// <summary>
        /// 默认的单选按钮处于 <see cref="ToggleState.On"/> 状态时显示的图标
        /// </summary>
        public static Texture2D OnIcon = ContentFinder<Texture2D>.Get("UI/Widgets/RadioButOn");

        /// <summary>
        /// 默认的单选按钮处于 <see cref="ToggleState.Indeterminate"/> 状态时显示的图标
        /// </summary>
        public static Texture2D IndeterminateIcon = ContentFinder<Texture2D>.Get("UI/Widgets/RadioButSemiOn");

        /// <summary>
        /// 默认的单选按钮处于 <see cref="ToggleState.Off"/> 状态时显示的图标
        /// </summary>
        public static Texture2D OffIcon = ContentFinder<Texture2D>.Get("UI/Widgets/RadioButOff");
    }
}
