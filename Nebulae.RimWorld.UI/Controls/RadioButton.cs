using RimWorld;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace Nebulae.RimWorld.UI.Controls
{
    /// <summary>
    /// 单选按钮控件
    /// </summary>
    [StaticConstructorOnStartup]
    public class RadioButton : ToggleButton
    {
        /// <summary>
        /// 默认的单选按钮处于 <see cref="ToggleStatus.Checked"/> 状态时显示的图标
        /// </summary>
        public static Texture2D DefaultCheckedIcon = ContentFinder<Texture2D>.Get("UI/Widgets/RadioButOn");
        /// <summary>
        /// 默认的单选按钮处于 <see cref="ToggleStatus.Indeterminate"/> 状态时显示的图标
        /// </summary>
        public static Texture2D DefaultIndeterminateIcon = ContentFinder<Texture2D>.Get("UI/Widgets/RadioButSemiOn");
        /// <summary>
        /// 默认的单选按钮处于 <see cref="ToggleStatus.Unchecked"/> 状态时显示的图标
        /// </summary>
        public static Texture2D DefaultUncheckedIcon = ContentFinder<Texture2D>.Get("UI/Widgets/RadioButOff");


        private RadioButtonGroup _group;

        /// <summary>
        /// 单选按钮所在的组
        /// </summary>
        public RadioButtonGroup Group
        {
            get => _group;
            set
            {
                if (_group != value)
                {
                    _group?.Remove(this);
                    _group = value;
                    _group?.Add(this);
                }
            }
        }


        /// <summary>
        /// 初始化 <see cref="RadioButton"/> 的新实例
        /// </summary>
        public RadioButton() : base(DefaultCheckedIcon, DefaultIndeterminateIcon, DefaultUncheckedIcon)
        {
            ClickSound = SoundDefOf.Tick_Tiny;
            IconHitOnly = false;
            IconHighlightable = false;
        }


        /// <inheritdoc/>
        protected override void OnClick()
        {
            if (Status is ToggleStatus.Checked) { return; }
            else
            {
                if (_group != null)
                {
                    foreach (var button in _group)
                    {
                        button.Status = ToggleStatus.Unchecked;
                    }
                }
                Status = ToggleStatus.Checked;
                base.OnClick();
            }
        }
    }
}
