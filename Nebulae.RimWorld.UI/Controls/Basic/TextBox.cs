using Nebulae.RimWorld.UI.Data;
using Nebulae.RimWorld.UI.Utilities;
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

        private bool _isReadOnly;
        private bool _wrapText = true;

        private Thickness _textBoxExtension = Thickness.Empty;

        private Rect _innerRect;
        private Size _innerSize = Size.Empty;

        #endregion


        //------------------------------------------------------
        //
        //  Public Properties
        //
        //------------------------------------------------------

        #region Public Properties

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
        /// 文本输入框背景拓展量
        /// </summary>
        /// <remarks>只有 <see cref="Thickness.Top"/> 和 <see cref="Thickness.Bottom"/> 有效。</remarks>
        public Thickness TextBoxExtension
        {
            get => _textBoxExtension;
            set
            {
                value = value.Normalize();
                value = new Thickness(0f, value.Top, 0f, value.Bottom);

                if (_textBoxExtension != value)
                {
                    _textBoxExtension = value;

                    InvalidateMeasure();
                }
            }
        }

        #region TextBoxVerticalAlignment
        /// <summary>
        /// 获取或设置文本输入框的垂直对齐方式
        /// </summary>
        public VerticalAlignment TextBoxVerticalAlignment
        {
            get { return (VerticalAlignment)GetValue(BackgroundVerticalAlignmentProperty); }
            set { SetValue(BackgroundVerticalAlignmentProperty, value); }
        }

        /// <summary>
        /// 标识 <see cref="TextBoxVerticalAlignment"/> 依赖属性。
        /// </summary>
        public static readonly DependencyProperty BackgroundVerticalAlignmentProperty =
            DependencyProperty.Register(nameof(TextBoxVerticalAlignment), typeof(VerticalAlignment), typeof(TextBox),
                new ControlPropertyMetadata(VerticalAlignment.Center, ControlRelation.Arrange));
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
                new ControlPropertyMetadata(AutoLayoutUtility.StandardRowHeight, ControlRelation.Measure));
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

            _innerRect = _innerSize.AlignToArea(renderRect,
                HorizontalAlignment.Stretch, TextBoxVerticalAlignment);

            return renderRect;
        }


        /// <inheritdoc/>
        protected override void DrawControl(bool isFocused)
        {
            GameFont controlFont = FontSize;
            GameFont font = GameText.Font;
            GameText.Font = controlFont;

            bool isDisabled = !IsEnabled;

            if (isDisabled)
            {
                GUI.color *= Widgets.InactiveColor;
            }

            if (_isReadOnly || isDisabled)
            {
                Text.DrawTextBox(_innerRect, FontSize, false, _wrapText);
            }
            else
            {
                string text = Text;
                string buffer = text.DrawTextBox(_innerRect, FontSize, isFocused || IsCursorOver || IsPressing, _wrapText);

                if (text != buffer && (InputValidator is null || InputValidator.IsMatch(buffer)))
                {
                    Text = buffer;
                }
            }

            GameText.Font = font;
        }

        /// <inheritdoc/>
        protected override Size MeasureCore(Size availableSize)
        {
            var backgroundVerticalAlignment = TextBoxVerticalAlignment;
            Size renderSize = base.MeasureCore(availableSize);

            if (backgroundVerticalAlignment is VerticalAlignment.Stretch)
            {
                _innerSize = renderSize;
                return renderSize;
            }

            GameFont fontSize = FontSize;
            float fontHeight = fontSize.GetHeight();

            _innerSize = new Size(renderSize.Width,
                (_wrapText
                    ? Text.CalculateTextBoxHeight(renderSize.Width, FontSize, true)
                    : fontHeight)
                        + _textBoxExtension.Top + _textBoxExtension.Bottom);

            if (_innerSize.Height > renderSize.Height)
            {
                renderSize = new Size(renderSize.Width,
                    backgroundVerticalAlignment is VerticalAlignment.Center
                        ? _innerSize.Height + (AutoLayoutUtility.StandardRowHeight - (_textBoxExtension.Top + fontHeight + _textBoxExtension.Bottom))
                        : _innerSize.Height);
            }

            return renderSize;
        }
    }
}
