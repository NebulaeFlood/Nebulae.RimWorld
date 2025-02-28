using Nebulae.RimWorld.UI.Data;
using Nebulae.RimWorld.UI.Utilities;
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
        /// 关闭按钮的混合色
        /// </summary>
        public static Color CloseButtonCompositionColor = new Color(1f, 0.3f, 0.35f);


        //------------------------------------------------------
        //
        //  Private Fields
        //
        //------------------------------------------------------

        #region Private Fields

        private Size _iconRenderSize = Size.Empty;
        private Rect _iconRenderRect;

        private Size _textRenderSize = Size.Empty;
        private Rect _textRenderRect;

        private Color _compositionColor = Color.white;

        private Texture2D _icon;

        private bool _iconHitOnly = false;
        private bool _iconHighlightable = true;

        private bool _reverseContent = false;
        private bool _separateContent = false;

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

        /// <summary>
        /// 按钮的图标是否可高亮
        /// </summary>
        public bool IconHighlightable
        {
            get => _iconHighlightable;
            set => _iconHighlightable = value;
        }

        /// <summary>
        /// 是否翻转图标和文字的位置
        /// </summary>
        public bool ReverseContent
        {
            get => _reverseContent;
            set => _reverseContent = value;
        }

        /// <summary>
        /// 是否拆分图标为文字
        /// </summary>
        public bool SeparateContent
        {
            get => _separateContent;
            set => _separateContent = value;
        }

        #endregion


        /// <summary>
        /// 指示按钮是否只有图标可交互
        /// </summary>
        protected bool IconHitOnly
        {
            get => _iconHitOnly;
            set => _iconHitOnly = value;
        }


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
            Rect renderRect = base.ArrangeCore(availableRect);

            if (_status is ContentStatus.None)
            {
                return renderRect;
            }

            Rect contentRenderRect = renderRect;

            if (!_separateContent)
            {
                contentRenderRect = new Size(
                    _iconRenderSize.Width + _textRenderSize.Width,
                    Mathf.Max(_iconRenderSize.Height, _textRenderSize.Height))
                    .AlignToArea(
                        renderRect,
                        HorizontalAlignment.Center,
                        VerticalAlignment.Center);
            }

            if ((_status & ContentStatus.IconSetted) != 0)
            {
                _iconRenderRect = _iconRenderSize
                    .AlignToArea(
                        contentRenderRect,
                        HorizontalAlignment.Left.ReverseIf(_reverseContent),
                        VerticalAlignment.Center);
            }
            else
            {
                _iconRenderRect = Rect.zero;
            }

            if ((_status & ContentStatus.TextSetted) != 0)
            {
                _textRenderRect = _textRenderSize.AlignToArea(
                    contentRenderRect,
                    HorizontalAlignment.Right.ReverseIf(_reverseContent),
                    VerticalAlignment.Center);
            }
            else
            {
                _textRenderRect = Rect.zero;
            }

            return renderRect;
        }

        /// <summary>
        /// 绘制按钮背景
        /// </summary>
        /// <param name="status">按钮状态</param>
        protected virtual void DrawBackground(ButtonStatus status)
        {
        }

        /// <inheritdoc/>
        protected override sealed void DrawButton(ButtonStatus status)
        {
            Color color = GUI.color;
            Color contentColor = GUI.contentColor;
            bool isDisabled = status.HasFlag(ButtonStatus.Disabled);

            if (isDisabled)
            {
                GUI.color = _compositionColor * Widgets.InactiveColor * color;
                GUI.contentColor = _compositionColor * Widgets.InactiveColor * contentColor;
            }

            DrawBackground(status);

            if ((_status & ContentStatus.TextSetted) != 0)
            {
                TextAnchor anchor = GameText.Anchor;
                GameFont font = GameText.Font;
                GameText.Anchor = TextAnchor.MiddleLeft;
                GameText.Font = FontSize;

                Widgets.Label(_textRenderRect, Text);

                GameText.Font = font;
                GameText.Anchor = anchor;
            }

            if ((_status & ContentStatus.IconSetted) != 0)
            {
                if (_iconHighlightable
                    && status.HasFlag(ButtonStatus.Hovered))
                {
                    color = GUI.color;
                    GUI.color = _compositionColor * GenUI.MouseoverColor * color;
                }

                GUI.DrawTexture(_iconRenderRect, _icon, ScaleMode.ScaleToFit);
            }

            GUI.color = color;
            GUI.contentColor = color;
        }

        /// <inheritdoc/>
        protected override Rect HitTestCore(Rect contentRect)
        {
            if (_iconHitOnly)
            {
                return _iconRenderRect;
            }
            else
            {
                return contentRect;
            }
        }

        /// <inheritdoc/>
        protected override Size MeasureCore(Size availableSize)
        {
            _status = ContentStatus.None;

            if (_icon is null)
            {
                _iconRenderSize = Size.Empty;
            }
            else
            {
                _iconRenderSize = new Size(IconSize);
                _status |= ContentStatus.IconSetted;
            }

            string text = Text;

            if (string.IsNullOrEmpty(text))
            {
                _textRenderSize = Size.Empty;
            }
            else
            {
                _textRenderSize = text.CalculateLineSize(FontSize);
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
            None = 0b00,
            /// <summary>
            /// 按钮设置了图标
            /// </summary>
            IconSetted = 0b01,
            /// <summary>
            /// 按钮设置了文字
            /// </summary>
            TextSetted = 0b10,
            /// <summary>
            /// 按钮设置了图标和文字
            /// </summary>
            FullSetted = 0b11
        }
    }
}
