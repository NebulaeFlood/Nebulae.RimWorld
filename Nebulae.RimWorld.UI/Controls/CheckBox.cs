﻿using RimWorld;
using Verse;
using Verse.Sound;

namespace Nebulae.RimWorld.UI.Controls
{
    /// <summary>
    /// 复选框控件
    /// </summary>
    public class CheckBox : ToggleButton
    {
        /// <summary>
        /// 初始化 <see cref="CheckBox"/> 的新实例
        /// </summary>
        public CheckBox()
        {
            CheckedIcon = Widgets.CheckboxOnTex;
            IndeterminateIcon = Widgets.CheckboxPartialTex;
            UncheckedIcon = Widgets.CheckboxOffTex;
        }


        /// <inheritdoc/>
        protected override void OnClick()
        {
            if (Status is ToggleStatus.Checked)
            {
                Status = ToggleStatus.Unchecked;
                SoundDefOf.Checkbox_TurnedOff.PlayOneShotOnCamera();
            }
            else
            {
                Status = ToggleStatus.Checked;
                SoundDefOf.Checkbox_TurnedOn.PlayOneShotOnCamera();
            }
        }
    }
}
