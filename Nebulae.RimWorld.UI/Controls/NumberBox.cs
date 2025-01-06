using Nebulae.RimWorld.UI.Data;
using Nebulae.RimWorld.Utilities;
using System;
using System.Text.RegularExpressions;
using UnityEngine;
using Verse;

namespace Nebulae.RimWorld.UI.Controls
{
    /// <summary>
    /// 专用于输入数字的文本框控件
    /// </summary>
    public class NumberBox : FrameworkControl
    {
        private string _buffer;
        private int _decimalPartDigit;
        private bool _displayAsPercent;
        private Regex _inputValidator;
        private bool _isReadOnly;
        private float _maximun;
        private float _minimun;


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

                    _inputValidator = DisplayAsPercent
                        ? new Regex($@"^-?[0-9]{{0,41}}(\.[0-9]{{0,{Math.Max(value - 2, 0)}}})?%$")
                        : new Regex($@"^-?[0-9]{{0,39}}(\.[0-9]{{0,{value}}})?$");
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
                    _inputValidator = value
                        ? new Regex($@"^-?[0-9]{{0,41}}(\.[0-9]{{0,{Math.Max(DecimalPartDigit - 2, 0)}}})?%$")
                        : new Regex($@"^-?[0-9]{{0,39}}(\.[0-9]{{0,{DecimalPartDigit}}})?$");
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
                new PropertyMetadata(GameFont.Small));
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
                _maximun = value;
                SetValue(ValueProperty, value.Clamp(_minimun, value));
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
                _minimun = value;
                SetValue(ValueProperty, value.Clamp(value, _maximun));
            }
        }

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
                new PropertyMetadata(0f));
        #endregion

        #endregion


        /// <summary>
        /// 初始化 <see cref="NumberBox"/> 的新实例
        /// </summary>
        public NumberBox()
        {
            _buffer = "0";
            _decimalPartDigit = 2;
            _displayAsPercent = false;
            _inputValidator = new Regex(@"^-?[0-9]{0,39}(\.[0-9]{0,2})?$");
            _isReadOnly = false;
            _maximun = float.MaxValue;
            _minimun = float.MinValue;
        }


        /// <inheritdoc/>
        protected override Rect DrawCore(Rect renderRect)
        {
            GameFont currentFont = Text.Font;

            Text.Font = FontSize;
            if (_isReadOnly)
            {
                GUI.TextField(renderRect, _buffer, Text.CurTextFieldStyle);
            }
            else
            {
                string text = GUI.TextField(renderRect, _buffer, Text.CurTextFieldStyle);
                if (_buffer != text && _inputValidator.IsMatch(text))
                {
                    float value = float.Parse(text).Clamp(_minimun, _maximun);
                    _buffer = DisplayAsPercent
                        ? string.Format($"{{0:F{Math.Max(_decimalPartDigit - 2, 0)}}}%", value * 100f)
                        : value.ToString();
                    Value = value;
                }
            }
            Text.Font = currentFont;
            return renderRect;
        }
    }
}
