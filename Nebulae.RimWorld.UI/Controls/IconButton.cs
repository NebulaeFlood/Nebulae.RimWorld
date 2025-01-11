using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        private Size _cachedContentSize = Size.Empty;
        private Rect _cachedIconRect;
        private Rect _cachedTextRect;

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
        /// 按钮的图标
        /// </summary>
        public Texture2D Icon
        {
            get => _icon;
            set => _icon = value;
        }

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
            Rect contentRect = _cachedContentSize.AlignRectToArea(
                desiredRect,
                HorizontalAlignment.Center,
                VerticalAlignment.Center);

            switch (_status)
            {
                case ContentStatus.TextOnly:
                    _cachedIconRect = Rect.zero;
                    _cachedTextRect = contentRect;
                    break;
                case ContentStatus.IconOnly:
                    _cachedIconRect = contentRect;
                    _cachedTextRect = Rect.zero;
                    break;
                case ContentStatus.TextWithIcon:
                    _cachedIconRect = new Rect(
                        contentRect.x,
                        contentRect.y,
                        _cachedContentSize.Height,
                        _cachedContentSize.Height);

                    _cachedTextRect = new Rect(
                        contentRect.x + _cachedContentSize.Height,
                        contentRect.y,
                        _cachedContentSize.Width - _cachedContentSize.Height,
                        _cachedContentSize.Height);
                    break;
                default:    // NoContent
                    _cachedIconRect = Rect.zero;
                    _cachedTextRect = Rect.zero;
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

                        Widgets.Label(_cachedTextRect, Text);

                        GameText.Font = currentFont;
                        break;
                    }
                case ContentStatus.IconOnly:
                    GUI.DrawTexture(_cachedIconRect, _icon, ScaleMode.ScaleToFit);
                    break;
                case ContentStatus.TextWithIcon:
                    {
                        GUI.DrawTexture(_cachedIconRect, _icon, ScaleMode.ScaleToFit);
                        GameFont currentFont = GameText.Font;
                        GameText.Font = FontSize;

                        Widgets.Label(_cachedTextRect, Text);

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
        protected override  Size MeasureCore(Size availableSize)
        {
            string text = Text;

            bool noText = string.IsNullOrEmpty(text);
            bool noIcon = Icon is null;

            if (noText && noIcon)
            {
                _cachedContentSize = Size.Empty;
                _status = ContentStatus.NoContent;
            }
            else if (!noText && noIcon)
            {
                _cachedContentSize = text.CalculateLineSize(FontSize);
                _status = ContentStatus.TextOnly;
            }
            else if (noText && !noIcon)
            {
                _cachedContentSize = new Size(Mathf.Min(availableSize.Width, availableSize.Height) * 0.8f).Round();
                _status = ContentStatus.IconOnly;
            }
            else
            {
                Size textSize = text.CalculateLineSize(FontSize);

                _cachedContentSize = new Size(textSize.Width + textSize.Height, textSize.Height);
                _status = ContentStatus.TextWithIcon;
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
