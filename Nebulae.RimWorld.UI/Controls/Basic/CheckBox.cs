using Nebulae.RimWorld.UI.Core.Events;
using RimWorld;
using UnityEngine;
using static Verse.Widgets;

namespace Nebulae.RimWorld.UI.Controls.Basic
{
    /// <summary>
    /// 复选框控件
    /// </summary>
    public class CheckBox : ToggleButton
    {
        //------------------------------------------------------
        //
        //  Constructors
        //
        //------------------------------------------------------

        #region Constructors

        /// <summary>
        /// 初始化 <see cref="CheckBox"/> 的新实例
        /// </summary>
        public CheckBox() : base(CheckboxOnTex, CheckboxPartialTex, CheckboxOffTex) { }

        /// <summary>
        /// 初始化 <see cref="CheckBox"/> 的新实例
        /// </summary>
        /// <param name="onIcon">按钮开启时的图标</param>
        /// <param name="indeterminateIcon">按钮状态未确定时的图标</param>
        /// <param name="offIcon">按钮关闭时的图标</param>
        public CheckBox(Texture2D onIcon, Texture2D indeterminateIcon, Texture2D offIcon)
             : base(onIcon, indeterminateIcon, offIcon) { }

        #endregion


        /// <inheritdoc/>
        protected override void OnClick(RoutedEventArgs e)
        {
            if (State is ToggleState.On)
            {
                ClickSound = SoundDefOf.Checkbox_TurnedOff;
                State = ToggleState.Off;
            }
            else
            {
                ClickSound = SoundDefOf.Checkbox_TurnedOn;
                State = ToggleState.On;
            }

            base.OnClick(e);
        }
    }
}
