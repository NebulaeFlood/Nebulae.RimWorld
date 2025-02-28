using Nebulae.RimWorld.UI.Data;
using UnityEngine;

namespace Nebulae.RimWorld.UI.Controls
{
    /// <summary>
    /// 可切换状态的按钮的基类，定义了其共同特性
    /// </summary>
    public abstract class ToggleButton : IconButton
    {
        //------------------------------------------------------
        //
        //  Private Fields
        //
        //------------------------------------------------------

        #region Private Fields

        private readonly Texture2D _checkedIcon;
        private readonly Texture2D _indeterminateIcon;
        private readonly Texture2D _uncheckedIcon;

        #endregion


        //------------------------------------------------------
        //
        //  Public Properties
        //
        //------------------------------------------------------

        #region Public Properties

        #region Status
        /// <summary>
        /// 获取或设置按钮的状态
        /// </summary>
        public ToggleStatus Status
        {
            get { return (ToggleStatus)GetValue(StatusProperty); }
            set { SetValue(StatusProperty, value); }
        }

        /// <summary>
        /// 标识 <see cref="Status"/> 依赖属性。
        /// </summary>
        public static readonly DependencyProperty StatusProperty =
            DependencyProperty.Register(nameof(Status), typeof(ToggleStatus), typeof(ToggleButton),
                new PropertyMetadata(ToggleStatus.Indeterminate));
        #endregion

        #endregion


        /// <summary>
        /// 初始化 <see cref="ToggleButton"/> 的新实例
        /// </summary>
        /// <param name="checkedIcon"></param>
        /// <param name="indeterminateIcon"></param>
        /// <param name="uncheckedIcon"></param>
        protected ToggleButton(
            Texture2D checkedIcon,
            Texture2D indeterminateIcon,
            Texture2D uncheckedIcon)
        {
            IconHitOnly = true;
            SeparateContent = true;

            _checkedIcon = checkedIcon;
            _indeterminateIcon = indeterminateIcon;
            _uncheckedIcon = uncheckedIcon;

            switch (Status)
            {
                case ToggleStatus.Unchecked:
                    Icon = uncheckedIcon;
                    break;
                case ToggleStatus.Checked:
                    Icon = checkedIcon;
                    break;
                default:    // Indeterminate
                    Icon = indeterminateIcon;
                    break;
            }
        }


        /// <inheritdoc/>
        protected override void DrawBackground(ButtonStatus status)
        {
            switch (Status)
            {
                case ToggleStatus.Unchecked:
                    Icon = _uncheckedIcon;
                    break;
                case ToggleStatus.Checked:
                    Icon = _checkedIcon;
                    break;
                default:    // Indeterminate
                    Icon = _indeterminateIcon;
                    break;
            }
        }
    }
}
