using Nebulae.RimWorld.UI.Data;
using Nebulae.RimWorld.UI.Utilities;
using Nebulae.RimWorld.Utilities;
using System.Text.RegularExpressions;
using UnityEngine;
using Verse;
using GameText = Verse.Text;

namespace Nebulae.RimWorld.UI.Controls.Basic
{
    /// <summary>
    /// 文本框控件
    /// </summary>
    public class TextBox : FocusableControl
    {
        //------------------------------------------------------
        //
        //  Private Fields
        //
        //------------------------------------------------------

        #region Private Fields

        private bool _isReadOnly = false;
        private bool _wrapText = true;

        private Thickness _backgroundExtension = Thickness.Empty;

        private Rect _innerRect;

        #endregion


        //------------------------------------------------------
        //
        //  Public Properties
        //
        //------------------------------------------------------

        #region Public Properties

        /// <summary>
        /// 输入框背景拓展量
        /// </summary>
        /// <remarks>只有 <see cref="Thickness.Top"/> 和 <see cref="Thickness.Bottom"/> 有效。</remarks>
        public Thickness BackgroundExtension
        {
            get => _backgroundExtension;
            set
            {
                value = value.Normalize();
                value = new Thickness(0f, value.Top, 0f, value.Bottom);

                if (_backgroundExtension != value)
                {
                    _innerRect.yMax -= _backgroundExtension.Bottom;
                    _innerRect.yMin += _backgroundExtension.Top;
                    _backgroundExtension = value;
                    _innerRect.yMax += _backgroundExtension.Bottom;
                    _innerRect.yMin -= _backgroundExtension.Top;
                }
            }
        }

        #region BackgroundVerticalAlignment
        /// <summary>
        /// 获取或设置
        /// </summary>
        public VerticalAlignment BackgroundVerticalAlignment
        {
            get { return (VerticalAlignment)GetValue(BackgroundVerticalAlignmentProperty); }
            set { SetValue(BackgroundVerticalAlignmentProperty, value); }
        }

        /// <summary>
        /// 标识 <see cref="BackgroundVerticalAlignment"/> 依赖属性。
        /// </summary>
        public static readonly DependencyProperty BackgroundVerticalAlignmentProperty =
            DependencyProperty.Register(nameof(BackgroundVerticalAlignment), typeof(VerticalAlignment), typeof(TextBox),
                new ControlPropertyMetadata(VerticalAlignment.Center, ControlRelation.Arrange));
        #endregion

        #region FontSize
        /// <summary>
        /// 获取或设置字体大小
        /// </summary>
        public GameFont FontSize
        {
            get { return (GameFont)GetValue(FontSizeProperty); }
            set { SetValue(FontSizeProperty, value); }
        }

        /// <summary>
        /// 标识 <see cref="FontSize"/> 依赖属性。
        /// </summary>
        public static readonly DependencyProperty FontSizeProperty =
            DependencyProperty.Register(nameof(FontSize), typeof(GameFont), typeof(TextBox),
                new ControlPropertyMetadata(GameFont.Small, ControlRelation.Arrange));
        #endregion

        #region InputValidator
        /// <summary>
        /// 获取或设置验证输入文字的正则表达式
        /// </summary>
        public Regex InputValidator
        {
            get { return (Regex)GetValue(InputValidatorProperty); }
            set { SetValue(InputValidatorProperty, value); }
        }

        /// <summary>
        /// 标识 <see cref="InputValidator"/> 依赖属性。
        /// </summary>
        public static readonly DependencyProperty InputValidatorProperty =
            DependencyProperty.Register(nameof(InputValidator), typeof(Regex), typeof(TextBox),
                new ControlPropertyMetadata(null, OnInputValidatorChanged, ControlRelation.Arrange));

        private static void OnInputValidatorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((TextBox)d).Text = string.Empty;
        }
        #endregion

        /// <summary>
        /// 用户是否可编辑文字内容
        /// </summary>
        public bool IsReadOnly
        {
            get => _isReadOnly;
            set => _isReadOnly = value;
        }

        #region Text
        /// <summary>
        /// 获取或设置控件文本
        /// </summary>
        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        /// <summary>
        /// 标识 <see cref="Text"/> 依赖属性。
        /// </summary>
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register(nameof(Text), typeof(string), typeof(TextBox),
                new ControlPropertyMetadata(string.Empty, ControlRelation.Measure));
        #endregion

        /// <summary>
        /// 是否自动换行
        /// </summary>
        public bool WrapText
        {
            get => _wrapText;
            set
            {
                if (_wrapText != value)
                {
                    _wrapText = value;

                    InvalidateArrange();
                }
            }
        }

        #endregion


        static TextBox()
        {
            HeightProperty.OverrideMetadata(typeof(TextBox),
                new ControlPropertyMetadata(34f, ControlRelation.Measure));
        }

        /// <summary>
        /// 初始化 <see cref="TextBlock"/> 的新实例
        /// </summary>
        public TextBox()
        {
        }



        /// <inheritdoc/>
        protected override Rect AnalyseCore(Rect contentRect)
        {
            return _innerRect.IntersectWith(contentRect);
        }

        /// <inheritdoc/>
        protected override Rect ArrangeCore(Rect availableRect)
        {
            Rect renderRect = RenderSize.AlignToArea(availableRect,
                HorizontalAlignment, VerticalAlignment);

            if (_wrapText)
            {
                _innerRect = UIUtility.CalculateInputBoxSize(
                    Text,
                    renderRect.width,
                    FontSize,
                    _isReadOnly,
                    _wrapText).AlignToArea(renderRect,
                        HorizontalAlignment.Stretch, BackgroundVerticalAlignment);
            }
            else
            {
                _innerRect = new Size(renderRect.width, FontSize.GetHeight()).AlignToArea(renderRect,
                    HorizontalAlignment.Stretch, BackgroundVerticalAlignment);
            }

            _innerRect.yMax += _backgroundExtension.Bottom;
            _innerRect.xMin -= _backgroundExtension.Top;

            return renderRect;
        }


        /// <inheritdoc/>
        protected override void DrawControl()
        {
            Color color = GUI.color;
            GameFont controlFont = FontSize;
            GameFont font = GameText.Font;
            GameText.Font = controlFont;

            bool isDisabled = !IsEnabled;

            if (isDisabled)
            {
                GUI.color = color * Widgets.InactiveColor;
            }

            if (_isReadOnly || isDisabled)
            {
                UIUtility.DrawInputBox(_innerRect, Text, controlFont, true, _wrapText);
            }
            else
            {
                string text = Text;
                string buffer = UIUtility.DrawInputBox(_innerRect, text, controlFont, false, _wrapText);

                if (text != buffer && (InputValidator is null || InputValidator.IsMatch(buffer)))
                {
                    Text = buffer;
                }
            }

            GUI.color = color;
            GameText.Font = font;
        }

        /// <inheritdoc/>
        protected override Size MeasureCore(Size availableSize)
        {
            Size renderSize = base.MeasureCore(availableSize);

            if (_wrapText)
            {
                return UIUtility.CalculateInputBoxSize(
                    Text,
                    renderSize.Width,
                    FontSize,
                    _isReadOnly,
                    _wrapText);
            }

            return renderSize;
        }
    }
}
