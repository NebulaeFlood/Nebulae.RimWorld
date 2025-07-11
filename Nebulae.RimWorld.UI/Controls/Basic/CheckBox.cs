using Nebulae.RimWorld.UI.Core.Events;
using RimWorld;
using Verse;

namespace Nebulae.RimWorld.UI.Controls.Basic
{
    /// <summary>
    /// 复选框控件
    /// </summary>
    public sealed class CheckBox : ToggleButton
    {
        /// <summary>
        /// 初始化 <see cref="CheckBox"/> 的新实例
        /// </summary>
        public CheckBox() : base(Widgets.CheckboxOnTex, Widgets.CheckboxPartialTex, Widgets.CheckboxOffTex) { }


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
