using Nebulae.RimWorld.UI.Data;
using Nebulae.RimWorld.UI.Utilities;
using Nebulae.RimWorld.Utilities;
using System;
using System.Text.RegularExpressions;
using UnityEngine;
using Verse;

namespace Nebulae.RimWorld.UI.Controls.Basic
{
    /// <summary>
    /// 专用于输入数字的文本框控件
    /// </summary>
    public class NumberBox : FocusableControl
    {
        //------------------------------------------------------
        //
        //  Private Fields
        //
        //------------------------------------------------------

        #region Private Fields

        private Regex _inputValidator = new Regex(@"^-?[0-9]{0,39}(\.[0-9]{0,2})?$");

        private string _buffer = "0.00";

        private float _maximun = 99999f;
        private float _minimun = -99999f;

        private int _decimalPartDigit = 2;
        private bool _displayAsPercent;

        private bool _isReadOnly;

        private Thickness _textBoxExtension = Thickness.Empty;

        private Rect _innerRect;
        private Size _innerSize;

        #endregion


        //------------------------------------------------------
        //
        //  Public Properties
        //
        //------------------------------------------------------

        #region Public Properties

        /// <summary>
        /// 小数点后最大位数
        /// </summary>
        public int DecimalPartDigit
        {
            get => _decimalPartDigit;
            set
            {
                if (_decimalPartDigit != value)
                {
                    _decimalPartDigit = value;

                    if (_displayAsPercent)
                    {
                        _buffer = string.Format("{0:F" + Math.Max(_decimalPartDigit - 2, 0) + "}%", Value * 100f);
                        _inputValidator = new Regex($@"^-?[0-9]{{0,41}}(\.[0-9]{{0,{Math.Max(_decimalPartDigit - 2, 0)}}})?%$");
                    }
                    else
                    {
                        _buffer = string.Format("{0:F" + _decimalPartDigit + "}", Value);
                        _inputValidator = new Regex($@"^-?[0-9]{{0,39}}(\.[0-9]{{0,{_decimalPartDigit}}})?$");
                    }
                }
            }
        }

        /// <summary>
        /// 以百分比形式呈现数值
        /// </summary>
        public bool DisplayAsPercent
        {
            get => _displayAsPercent;
            set
            {
                if (_displayAsPercent != value)
                {
                    _displayAsPercent = value;

                    if (_displayAsPercent)
                    {
                        _buffer = string.Format("{0:F" + Math.Max(_decimalPartDigit - 2, 0) + "}%", Value * 100f);
                        _inputValidator = new Regex($@"^-?[0-9]{{0,41}}(\.[0-9]{{0,{Math.Max(_decimalPartDigit - 2, 0)}}})?%$");
                    }
                    else
                    {
                        _buffer = string.Format("{0:F" + _decimalPartDigit + "}", Value);
                        _inputValidator = new Regex($@"^-?[0-9]{{0,39}}(\.[0-9]{{0,{_decimalPartDigit}}})?$");
                    }
                }
            }
        }

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
            DependencyProperty.Register(nameof(FontSize), typeof(GameFont), typeof(NumberBox),
                new ControlPropertyMetadata(GameFont.Small, ControlRelation.Arrange));
        #endregion

        /// <summary>
        /// 用户是否可编辑文字内容
        /// </summary>
        public bool IsReadOnly
        {
            get => _isReadOnly;
            set => _isReadOnly = value;
        }

        /// <summary>
        /// 数字的最大值
        /// </summary>
        public float Maximum
        {
            get => _maximun;
            set
            {
                if (_maximun != value)
                {
                    _maximun = value;

                    float current = Value;
                    float result = current.Clamp(_minimun, _maximun);

                    if (current != result)
                    {
                        SetValue(ValueProperty, result);
                    }
                }
            }
        }

        /// <summary>
        /// 数字的最小值
        /// </summary>
        public float Minimum
        {
            get => _minimun;
            set
            {
                if (_minimun != value)
                {
                    _minimun = value;

                    float current = Value;
                    float result = current.Clamp(_minimun, _maximun);

                    if (current != result)
                    {
                        SetValue(ValueProperty, result);
                    }
                }
            }
        }

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
            DependencyProperty.Register(nameof(TextBoxVerticalAlignment), typeof(VerticalAlignment), typeof(NumberBox),
                new ControlPropertyMetadata(VerticalAlignment.Center, ControlRelation.Arrange));
        #endregion

        #region Value
        /// <summary>
        /// 获取或设置输入数字的当前值
        /// </summary>
        public float Value
        {
            get { return (float)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        /// <summary>
        /// 标识 <see cref="Value"/> 依赖属性。
        /// </summary>
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register(nameof(Value), typeof(float), typeof(NumberBox),
                new ControlPropertyMetadata(0f, OnValueChanged, ControlRelation.Arrange));

        private static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            NumberBox numberBox = (NumberBox)d;


            int decimalPartDigit;

            if (numberBox._displayAsPercent)
            {
                decimalPartDigit = Math.Max(numberBox._decimalPartDigit - 2, 0);
                numberBox._buffer = string.Format("{0:F" + decimalPartDigit + "}%", (float)e.NewValue * 100f);
                numberBox._inputValidator = new Regex($@"^-?[0-9]{{0,41}}(\.[0-9]{{0,{decimalPartDigit}}})?%$");
            }
            else
            {
                decimalPartDigit = numberBox._decimalPartDigit;
                numberBox._buffer = string.Format("{0:F" + decimalPartDigit + "}", e.NewValue);
                numberBox._inputValidator = new Regex($@"^-?[0-9]{{0,39}}(\.[0-9]{{0,{numberBox._decimalPartDigit}}})?$");
            }
        }
        #endregion

        #endregion


        static NumberBox()
        {
            HeightProperty.OverrideMetadata(typeof(NumberBox),
                new ControlPropertyMetadata(AutoLayoutUtility.StandardRowHeight, ControlRelation.Measure));
        }

        /// <summary>
        /// 初始化 <see cref="NumberBox"/> 的新实例
        /// </summary>
        public NumberBox()
        {
        }


        //------------------------------------------------------
        //
        //  Protected Methods
        //
        //------------------------------------------------------

        #region Protected Methods

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
            GameFont font = Text.Font;
            Text.Font = controlFont;

            bool isDisabled = !IsEnabled;

            if (isDisabled)
            {
                GUI.color *= Widgets.InactiveColor;
            }

            if (_isReadOnly || isDisabled)
            {
                _buffer.DrawTextBox(_innerRect, FontSize, false, false);
            }
            else
            {
                string text = _buffer.DrawTextBox(_innerRect, FontSize, isFocused || IsCursorOver || IsPressing, false);

                if (_buffer != text && _inputValidator.IsMatch(text))
                {
                    Value = _displayAsPercent
                        ? (text.Remove(text.Length - 1).Prase(_minimun) * 0.01f).Clamp(_minimun, _maximun)
                        : text.Prase(_minimun).Clamp(_minimun, _maximun);
                }
            }

            Text.Font = font;
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
                _textBoxExtension.Top + fontHeight + _textBoxExtension.Bottom);

            if (_innerSize.Height > renderSize.Height)
            {
                renderSize = new Size(renderSize.Width,
                    backgroundVerticalAlignment is VerticalAlignment.Center
                        ? _innerSize.Height + (AutoLayoutUtility.StandardRowHeight - (_textBoxExtension.Top + fontHeight + _textBoxExtension.Bottom))
                        : _innerSize.Height);
            }

            return renderSize;
        }

        #endregion
    }
}
