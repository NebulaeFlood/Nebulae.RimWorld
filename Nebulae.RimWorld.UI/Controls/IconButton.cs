using Nebulae.RimWorld.UI.Data;
using System;
using UnityEngine;
using Verse;
using GameText = Verse.Text;

namespace Nebulae.RimWorld.UI.Controls
{
    /// <summary>
    /// 图标按钮
    /// </summary>
    public class IconButton : ButtonBase
    {
        /// <summary>
        /// 默认的图标尺寸
        /// </summary>
        public const float DefaultIconSize = 24f;

        /// <summary>
        /// 按钮的默认内边距
        /// </summary>
        public const float DefaultPadding = 5f;


        /// <summary>
        /// 关闭按钮的混合色
        /// </summary>
        public static Color CloseButtonCompositionColor = new Color(1f, 0.3f, 0.35f);


        //------------------------------------------------------
        //
        //  Private Fields
        //
        //------------------------------------------------------

        #region Private Fields

        private Size _iconDesiredSize = Size.Empty;
        private Rect _iconDesiredRect;

        private Size _textDesiredSize = Size.Empty;
        private Rect _textDesiredRect;

        private Color _compositionColor = Color.white;

        private Texture2D _icon;

        private ContentStatus _status = ContentStatus.None;

        #endregion


        //------------------------------------------------------
        //
        //  Public Properties
        //
        //------------------------------------------------------

        #region Public Properties

        /// <summary>
        /// 按钮激活时，按钮材质的混合色
        /// </summary>
        public Color CompositionColor
        {
            get => _compositionColor;
            set => _compositionColor = value;
        }

        /// <summary>
        /// 按钮当前显示的图标
        /// </summary>
        public Texture2D Icon
        {
            get => _icon;
            set => _icon = value;
        }

        #region IconSize
        /// <summary>
        /// 获取或设置图标的尺寸
        /// </summary>
        public float IconSize
        {
            get { return (float)GetValue(IconSizeProperty); }
            set { SetValue(IconSizeProperty, value); }
        }

        /// <summary>
        /// 标识 <see cref="IconSize"/> 依赖属性。
        /// </summary>
        public static readonly DependencyProperty IconSizeProperty =
            DependencyProperty.Register(nameof(IconSize), typeof(float), typeof(IconButton),
                new ControlPropertyMetadata(DefaultIconSize, ControlRelation.Measure));
        #endregion

        #region Padding
        /// <summary>
        /// 获取或设置按钮内容的统一边距
        /// </summary>
        public float Padding
        {
            get { return (float)GetValue(PaddingProperty); }
            set { SetValue(PaddingProperty, value); }
        }

        /// <summary>
        /// 标识 <see cref="Padding"/> 依赖属性。
        /// </summary>
        public static readonly DependencyProperty PaddingProperty =
            DependencyProperty.Register(nameof(Padding), typeof(float), typeof(IconButton),
                new ControlPropertyMetadata(DefaultPadding, ControlRelation.Measure));
        #endregion

        #endregion


        /// <summary>
        /// 初始化 <see cref="IconButton"/> 的新实例
        /// </summary>
        public IconButton()
        {
        }


        //------------------------------------------------------
        //
        //  Protected Methods
        //
        //------------------------------------------------------

        #region Protected Methods

        /// <inheritdoc/>
        protected override Rect ArrangeCore(Rect availableRect)
        {
            if (_status is ContentStatus.None)
            {
                return base.ArrangeCore(availableRect);
            }

            Rect desiredRect = base.ArrangeCore(availableRect);
            Rect contentAvailableRect = desiredRect - new Thickness(Padding);

            if ((_status & ContentStatus.IconSetted) != 0)
            {
                _iconDesiredRect = _iconDesiredSize.AlignRectToArea(
                    contentAvailableRect, 
                    ContentHorizontalAlignment, 
                    ContentVerticalAlignment);
            }
            else
            {
                _iconDesiredRect = Rect.zero;
            }

            if ((_status & ContentStatus.TextSetted) != 0)
            {
                contentAvailableRect.x += _iconDesiredRect.x + Padding;
                _textDesiredRect = _textDesiredSize.AlignRectToArea(
                    contentAvailableRect,
                    ContentHorizontalAlignment,
                    ContentVerticalAlignment);
            }
            else
            {
                _textDesiredRect = Rect.zero;
            }

            return desiredRect;
        }

        /// <summary>
        /// 绘制按钮背景
        /// </summary>
        /// <param name="renderRect">允许绘制背景的区域</param>
        /// <param name="isEnabled">按钮是否被启用，用于判断应该绘制的状态背景</param>
        /// <param name="isCursorOver">光标是否位于控件上方，用于判断应该绘制的状态背景</param>
        protected virtual void DrawBackground(Rect renderRect, bool isEnabled, bool isCursorOver) { }


        /// <inheritdoc/>
        protected override sealed Rect DrawButton(Rect renderRect, bool isEnabled, bool isCursorOver)
        {
            Color currentColor = GUI.color;
            GUI.color = isEnabled
                ? _compositionColor * currentColor
                : _compositionColor * Widgets.InactiveColor;

            DrawBackground(renderRect, isEnabled, isCursorOver);

            if ((_status & ContentStatus.IconSetted) != 0)
            {
                GUI.DrawTexture(_iconDesiredRect, _icon, ScaleMode.ScaleToFit);
            }

            if ((_status & ContentStatus.TextSetted) != 0)
            {
                TextAnchor anchor = GameText.Anchor;
                GameFont font = GameText.Font;
                GameText.Anchor = TextAnchor.MiddleLeft;
                GameText.Font = FontSize;

                Widgets.Label(_textDesiredRect, Text);

                GameText.Font = font;
                GameText.Anchor = anchor;
            }

            GUI.color = currentColor;
            return renderRect;
        }

        /// <inheritdoc/>
        protected override Size MeasureCore(Size availableSize)
        {
            _status = ContentStatus.None;

            if (_icon is null)
            {
                _iconDesiredSize = Size.Empty;
            }
            else
            {
                _iconDesiredSize = new Size(IconSize);
                _status |= ContentStatus.IconSetted;
            }

            string text = Text;

            if (string.IsNullOrEmpty(text))
            {
                _textDesiredSize = Size.Empty;
            }
            else
            {
                _textDesiredSize = text.CalculateLineSize(FontSize);
                _status |= ContentStatus.TextSetted;
            }

            return base.MeasureCore(availableSize);
        }

        #endregion


        /// <summary>
        /// 按钮内容的状态
        /// </summary>
        [Flags]
        protected enum ContentStatus : int
        {
            /// <summary>
            /// 按钮没有设置内容
            /// </summary>
            None = 0x00000000,

            /// <summary>
            /// 按钮设置了图标
            /// </summary>
            IconSetted,

            /// <summary>
            /// 按钮设置了文字
            /// </summary>
            TextSetted,
        }
    }
}
