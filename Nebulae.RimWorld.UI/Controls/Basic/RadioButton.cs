using Nebulae.RimWorld.UI.Core.Data;
using Nebulae.RimWorld.UI.Core.Events;
using RimWorld;
using System;
using static Nebulae.RimWorld.UI.Controls.Resources.RadioButtonResources;

namespace Nebulae.RimWorld.UI.Controls.Basic
{
    /// <summary>
    /// 单选按钮
    /// </summary>
    public sealed class RadioButton : ToggleButton
    {
        /// <summary>
        /// 获取或设置 <see cref="RadioButton"/> 所在的组
        /// </summary>
        public RadioButtonGroup Group
        {
            get => _group;
            set
            {
                if (value is null)
                {
                    throw new InvalidOperationException($"Cannot assign a null value to {Type}.{nameof(Group)}");
                }

                if (!ReferenceEquals(value, _group))
                {
                    _group.Remove(this);
                    _group = value;
                    _group.Add(this);
                }
            }
        }


        static RadioButton()
        {
            ClickSoundProperty.OverrideMetadata(typeof(RadioButton),
                new PropertyMetadata(SoundDefOf.Tick_Tiny));
            StateProperty.OverrideMetadata(typeof(RadioButton),
                new PropertyMetadata(ToggleState.Indeterminate, OnStateChanged));
        }

        /// <summary>
        /// 初始化 <see cref="RadioButton"/> 的新实例
        /// </summary>
        /// <param name="group"><see cref="RadioButton"/> 所在的组</param>
        public RadioButton(RadioButtonGroup group) : base(OnIcon, IndeterminateIcon, OffIcon)
        {
            if (group is null)
            {
                throw new ArgumentNullException("group");
            }

            _group = group;
            _group.Add(this);
        }


        /// <inheritdoc/>
        protected override void OnClick(RoutedEventArgs e)
        {
            State = (State is ToggleState.On) ? ToggleState.Off : ToggleState.On;

            base.OnClick(e);
        }


        private static void OnStateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var button = (RadioButton)d;

            if (e.NewValue is ToggleState.On)
            {
                button._group.Select(button);
            }
        }


        private RadioButtonGroup _group;
    }
}
