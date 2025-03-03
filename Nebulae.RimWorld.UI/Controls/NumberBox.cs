using Nebulae.RimWorld.UI.Data;
using Nebulae.RimWorld.UI.Utilities;
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
        private bool _displayAsPercent = false;

        private bool _isReadOnly = false;

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

                    _buffer = string.Format("{0:F" + value + "}", Value);
                    _inputValidator = _displayAsPercent
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

                    _buffer = value
                        ? string.Format("{0:F" + Math.Max(_decimalPartDigit - 2, 0) + "}%", Value * 100f)
                        : string.Format("{0:F" + _decimalPartDigit + "}", Value * 100f);

                    _inputValidator = value
                        ? new Regex($@"^-?[0-9]{{0,41}}(\.[0-9]{{0,{Math.Max(_decimalPartDigit - 2, 0)}}})?%$")
                        : new Regex($@"^-?[0-9]{{0,39}}(\.[0-9]{{0,{_decimalPartDigit}}})?$");
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
                new PropertyMetadata(0f, OnValueChanged));

        private static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            NumberBox numberBox = (NumberBox)d;

            int decimalPartDigit = numberBox.DecimalPartDigit;
            if (numberBox.DisplayAsPercent)
            {
                decimalPartDigit -= 2;
                if (decimalPartDigit < 1)
                {
                    numberBox._buffer = $"{numberBox.Value * 100f}%";
                }
                else
                {
                    numberBox._buffer = string.Format("{0:F" + decimalPartDigit + "}%", numberBox.Value * 100f);
                }
            }
            else
            {
                if (decimalPartDigit < 1)
                {
                    numberBox._buffer = numberBox.Value.ToString();
                }
                else
                {
                    numberBox._buffer = string.Format("{0:F" + decimalPartDigit + "}", numberBox.Value);
                }
            }
        }
        #endregion

        #endregion


        /// <summary>
        /// 初始化 <see cref="NumberBox"/> 的新实例
        /// </summary>
        public NumberBox()
        {
        }


        /// <inheritdoc/>
        protected override void DrawControl()
        {
            Color color = GUI.color;
            GameFont controlFont = FontSize;
            GameFont font = Text.Font;
            Text.Font = controlFont;

            bool isDisabled = !IsEnabled;

            if (isDisabled)
            {
                GUI.color = color * Widgets.InactiveColor;
            }

            if (_isReadOnly || isDisabled)
            {
                UIUtility.DrawInputBox(RenderRect, _buffer, controlFont, true, false);
            }
            else
            {
                string text = UIUtility.DrawInputBox(RenderRect, _buffer, controlFont, false, false);

                if (_buffer != text && _inputValidator.IsMatch(text))
                {
                    float value = text.Prase(0f).Clamp(_minimun, _maximun);
                    _buffer = DisplayAsPercent
                        ? string.Format($"{{0:F{Math.Max(_decimalPartDigit - 2, 0)}}}%", value * 100f)
                        : value.ToString();
                    Value = value;
                }
            }

            GUI.color = color;
            Text.Font = font;
        }
    }
}
