using Nebulae.RimWorld.UI.Data;
using UnityEngine;
using Verse;
using GameText = Verse.Text;

namespace Nebulae.RimWorld.UI.Controls
{
    /// <summary>
    /// 可切换状态的按钮的基类，定义了其共同特性
    /// </summary>
    public abstract class ToggleButton : ButtonBase
    {
        #region StatusChanged
        private readonly WeakEvent<ToggleButton, ToggleStatus> statusChanged = new WeakEvent<ToggleButton, ToggleStatus>();

        /// <summary>
        /// 按钮的 <see cref="Status"/> 变化时发生的弱事件
        /// </summary>
        public event WeakEventHandler<ToggleButton, ToggleStatus> OnStatusChanged
        {
            add => statusChanged.Add(value);
            remove => statusChanged.Remove(value);
        }
        #endregion

        private Size _cachedContentSize;
        private Rect _cachedIconRect;
        private Rect _cachedTextRect;
        private Texture2D _checkedIcon;
        private Texture2D _indeterminateIcon;
        private Texture2D _uncheckedIcon;


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

        /// <summary>
        /// 按钮处于 <see cref="ToggleStatus.Checked"/> 状态时显示的图标
        /// </summary>
        public Texture2D CheckedIcon
        {
            get => _checkedIcon;
            set => _checkedIcon = value;
        }

        /// <summary>
        /// 按钮处于 <see cref="ToggleStatus.Indeterminate"/> 状态时显示的图标
        /// </summary>
        public Texture2D IndeterminateIcon
        {
            get => _indeterminateIcon;
            set => _indeterminateIcon = value;
        }

        /// <summary>
        /// 按钮处于 <see cref="ToggleStatus.Unchecked"/> 状态时显示的图标
        /// </summary>
        public Texture2D UncheckedIcon
        {
            get => _uncheckedIcon;
            set => _uncheckedIcon = value;
        }

        #endregion


        /// <summary>
        /// 为 <see cref="ToggleButton"/> 派生类实现基本初始化
        /// </summary>
        protected ToggleButton()
        {
        }


        /// <inheritdoc/>
        protected override Rect ArrangeCore(Rect availableRect)
        {
            Rect desiredRect = base.ArrangeCore(availableRect);
            Rect contentRect = _cachedContentSize.AlignRectToArea(desiredRect, HorizontalAlignment.Center, VerticalAlignment.Center);
            _cachedIconRect = UIUltility.AlignRectToArea(new Size(24f), contentRect, HorizontalAlignment.Left, VerticalAlignment.Center);
            _cachedTextRect = new Rect(contentRect.x + 24f, contentRect.y, _cachedContentSize.Width - 24f, _cachedContentSize.Height);
            return desiredRect;
        }

        /// <inheritdoc/>
        protected sealed override Rect DrawButton(Rect renderRect)
        {
            switch (Status)
            {
                case ToggleStatus.Unchecked:
                    GUI.DrawTexture(_cachedIconRect, _uncheckedIcon);
                    break;
                case ToggleStatus.Checked:
                    GUI.DrawTexture(_cachedIconRect, _checkedIcon);
                    break;
                default:    // Indeterminate
                    GUI.DrawTexture(_cachedIconRect, _indeterminateIcon);
                    break;
            }

            GameFont currentFont = GameText.Font;
            GameText.Font = FontSize;
            Widgets.Label(_cachedTextRect, Text);
            GameText.Font = currentFont;
            return renderRect;
        }

        /// <inheritdoc/>
        protected override Size MeasureCore(Size availableSize)
        {
            Size textSize = Text.CalculateLineSize(FontSize);
            _cachedContentSize = new Size(24f + textSize.Width, textSize.Height);
            return base.MeasureCore(availableSize);
        }
    }
}
