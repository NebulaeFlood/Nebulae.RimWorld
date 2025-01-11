using Nebulae.RimWorld.UI.Data;
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
        public const float DefaultIconSize = 27f;

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

        private Size _contentDesiredSize = Size.Empty;
        private Rect _iconDesiredRect;
        private Rect _textDesiredRect;

        private Color _compositionColor = Color.white;

        private Texture2D _icon;

        private ContentStatus _status = ContentStatus.NoContent;

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
            Rect desiredRect = base.ArrangeCore(availableRect);
            Rect contentRect = _contentDesiredSize.AlignRectToArea(
                desiredRect,
                ContentHorizontalAlignment,
                ContentVerticalAlignment);

            switch (_status)
            {
                case ContentStatus.TextOnly:
                    _iconDesiredRect = Rect.zero;
                    _textDesiredRect = contentRect;
                    break;
                case ContentStatus.IconOnly:
                    _iconDesiredRect = contentRect;
                    _textDesiredRect = Rect.zero;
                    break;
                case ContentStatus.TextWithIcon:
                    _iconDesiredRect = new Rect(
                        contentRect.x,
                        contentRect.y,
                        _contentDesiredSize.Height,
                        _contentDesiredSize.Height);

                    _textDesiredRect = new Rect(
                        contentRect.x + _contentDesiredSize.Height,
                        contentRect.y,
                        _contentDesiredSize.Width - _contentDesiredSize.Height,
                        _contentDesiredSize.Height);
                    break;
                default:    // NoContent
                    _iconDesiredRect = Rect.zero;
                    _textDesiredRect = Rect.zero;
                    break;
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

            switch (_status)
            {
                case ContentStatus.TextOnly:
                    {
                        GameFont currentFont = GameText.Font;
                        GameText.Font = FontSize;

                        Widgets.Label(_textDesiredRect, Text);

                        GameText.Font = currentFont;
                        break;
                    }
                case ContentStatus.IconOnly:
                    GUI.DrawTexture(_iconDesiredRect, _icon, ScaleMode.ScaleToFit);
                    break;
                case ContentStatus.TextWithIcon:
                    {
                        GUI.DrawTexture(_iconDesiredRect, _icon, ScaleMode.ScaleToFit);
                        GameFont currentFont = GameText.Font;
                        GameText.Font = FontSize;

                        Widgets.Label(_textDesiredRect, Text);

                        GameText.Font = currentFont;
                        break;
                    }
                default:    // NoContent
                    break;
            }

            GUI.color = currentColor;
            return renderRect;
        }

        /// <inheritdoc/>
        protected override Size MeasureCore(Size availableSize)
        {
            string text = Text;

            bool hasText = string.IsNullOrEmpty(text);
            bool hasIcon = Icon is null;

            if (hasIcon)
            {
                float iconSize = IconSize;

                if (hasText)
                {
                    Size textSize = text.CalculateLineSize(FontSize);

                    _contentDesiredSize = new Size(
                        textSize.Width + iconSize,
                        Mathf.Max(textSize.Height, iconSize));

                    _status = ContentStatus.TextWithIcon;
                }
                else
                {
                    _contentDesiredSize = new Size(iconSize);
                    _status = ContentStatus.IconOnly;
                }
            }
            else
            {
                if (hasText)
                {
                    _contentDesiredSize = text.CalculateLineSize(FontSize);
                    _status = ContentStatus.TextOnly;
                }
                else
                {
                    _contentDesiredSize = Size.Empty;
                    _status = ContentStatus.NoContent;
                }
            }

            return base.MeasureCore(availableSize);
        }

        #endregion


        /// <summary>
        /// 按钮内容的状态
        /// </summary>
        protected enum ContentStatus
        {
            /// <summary>
            /// 按钮没有设置内容
            /// </summary>
            NoContent,

            /// <summary>
            /// 按钮只设置的文字内容
            /// </summary>
            TextOnly,

            /// <summary>
            /// 按钮只设置了图标内容
            /// </summary>
            IconOnly,

            /// <summary>
            /// 按钮设置了图标和文字
            /// </summary>
            TextWithIcon
        }
    }
}
