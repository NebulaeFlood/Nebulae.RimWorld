using Nebulae.RimWorld.UI.Core.Data;
using Nebulae.RimWorld.UI.Utilities;
using System;
using System.Text.RegularExpressions;
using UnityEngine;
using Verse;

namespace Nebulae.RimWorld.UI.Controls.Basic
{
    /// <summary>
    /// 专用于输入数字的文本框控件
    /// </summary>
    public sealed class NumberBox : InputControl
    {
        //------------------------------------------------------
        //
        //  Dependency Properties
        //
        //------------------------------------------------------

        #region Dependency Properties

        #region AutoHeight
        /// <summary>
        /// 获取或设置一个值，该值指示 <see cref="NumberBox"/> 是否根据内容调整高度
        /// </summary>
        public bool AutoHeight
        {
            get { return (bool)GetValue(AutoHeightProperty); }
            set { SetValue(AutoHeightProperty, value); }
        }

        /// <summary>
        /// 标识 <see cref="AutoHeight"/> 依赖属性
        /// </summary>
        public static readonly DependencyProperty AutoHeightProperty =
            DependencyProperty.Register(nameof(AutoHeight), typeof(bool), typeof(NumberBox),
                new ControlPropertyMetadata(true, ControlRelation.Measure));
        #endregion

        #region Decimals
        /// <summary>
        /// 获取或设置 <see cref="NumberBox"/> 的数字的小数位数
        /// </summary>
        /// <remarks>只影响显示的值。</remarks>
        public ushort Decimals
        {
            get { return (ushort)GetValue(DecimalsProperty); }
            set { SetValue(DecimalsProperty, value); }
        }

        /// <summary>
        /// 标识 <see cref="Decimals"/> 依赖属性
        /// </summary>
        public static readonly DependencyProperty DecimalsProperty =
            DependencyProperty.Register(nameof(Decimals), typeof(ushort), typeof(NumberBox),
                new PropertyMetadata(new ushort(), CoerceDecimals, OnDecimalsChanged));

        private static object CoerceDecimals(DependencyObject d, object baseValue)
        {
            return (ushort)baseValue > 5 ? 5 : baseValue;
        }

        private static void OnDecimalsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((NumberBox)d).UpdateValueProcesser((ushort)e.NewValue, (bool)d.GetValue(IsPercentageProperty));
        }
        #endregion

        #region FontSize
        /// <summary>
        /// 获取或设置 <see cref="NumberBox"/> 的字体大小
        /// </summary>
        public GameFont FontSize
        {
            get { return (GameFont)GetValue(FontSizeProperty); }
            set { SetValue(FontSizeProperty, value); }
        }

        /// <summary>
        /// 标识 <see cref="FontSize"/> 依赖属性
        /// </summary>
        public static readonly DependencyProperty FontSizeProperty =
            DependencyProperty.Register(nameof(FontSize), typeof(GameFont), typeof(NumberBox),
                new ControlPropertyMetadata(GameFont.Small, TextUtility.CoerceFontSize, UpdateDrawer, ControlRelation.Measure));
        #endregion

        #region IsPercentage
        /// <summary>
        /// 获取或设置一个值，该值指示 <see cref="NumberBox"/> 的数字是否以百分数的形式显示
        /// </summary>
        public bool IsPercentage
        {
            get { return (bool)GetValue(IsPercentageProperty); }
            set { SetValue(IsPercentageProperty, value); }
        }

        /// <summary>
        /// 标识 <see cref="IsPercentage"/> 依赖属性
        /// </summary>
        public static readonly DependencyProperty IsPercentageProperty =
            DependencyProperty.Register(nameof(IsPercentage), typeof(bool), typeof(NumberBox),
                new PropertyMetadata(false, OnIsPercentageChanged));

        private static void OnIsPercentageChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var numberBox = (NumberBox)d;
            numberBox._drawer = CreateDrawer(
                (GameFont)d.GetValue(FontSizeProperty),
                (bool)d.GetValue(IsPercentageProperty),
                (bool)d.GetValue(IsReadOnlyProperty));

            numberBox.UpdateValueProcesser(numberBox.Decimals, (bool)e.NewValue);
        }
        #endregion

        #region IsReadOnly
        /// <summary>
        /// 获取或设置一个值，该值指示 <see cref="NumberBox"/> 的文本是否为只读
        /// </summary>
        public bool IsReadOnly
        {
            get { return (bool)GetValue(IsReadOnlyProperty); }
            set { SetValue(IsReadOnlyProperty, value); }
        }

        /// <summary>
        /// 标识 <see cref="IsReadOnly"/> 依赖属性
        /// </summary>
        public static readonly DependencyProperty IsReadOnlyProperty =
            DependencyProperty.Register(nameof(IsReadOnly), typeof(bool), typeof(NumberBox),
                new PropertyMetadata(false, UpdateDrawer));
        #endregion

        #region MaxValue
        /// <summary>
        /// 获取或设置 <see cref="NumberBox"/> 的最大值
        /// </summary>
        public float MaxValue
        {
            get { return (float)GetValue(MaxValueProperty); }
            set { SetValue(MaxValueProperty, value); }
        }

        /// <summary>
        /// 标识 <see cref="MaxValue"/> 依赖属性
        /// </summary>
        public static readonly DependencyProperty MaxValueProperty =
            DependencyProperty.Register(nameof(MaxValue), typeof(float), typeof(NumberBox),
                new PropertyMetadata(32767f, OnLimitChanged));
        #endregion

        #region MinValue
        /// <summary>
        /// 获取或设置 <see cref="NumberBox"/> 的最小值
        /// </summary>
        public float MinValue
        {
            get { return (float)GetValue(MinValueProperty); }
            set { SetValue(MinValueProperty, value); }
        }

        /// <summary>
        /// 标识 <see cref="MinValue"/> 依赖属性
        /// </summary>
        public static readonly DependencyProperty MinValueProperty =
            DependencyProperty.Register(nameof(MinValue), typeof(float), typeof(NumberBox),
                new PropertyMetadata(-32768f, OnLimitChanged));
        #endregion

        #region Value
        /// <summary>
        /// 获取或设置 <see cref="NumberBox"/> 的数字
        /// </summary>
        public float Value
        {
            get { return (float)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        /// <summary>
        /// 标识 <see cref="Value"/> 依赖属性
        /// </summary>
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register(nameof(Value), typeof(float), typeof(NumberBox),
                new PropertyMetadata(0f, CoerceValue, OnValueChanged));

        private static object CoerceValue(DependencyObject d, object baseValue)
        {
            return Mathf.Clamp((float)baseValue, (float)d.GetValue(MinValueProperty), (float)d.GetValue(MaxValueProperty));
        }

        private static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var numberBox = (NumberBox)d;
            numberBox._cache = numberBox._valueProcesser((float)e.NewValue);
        }
        #endregion

        #endregion


        /// <summary>
        /// 初始化 <see cref="NumberBox"/> 的新实例
        /// </summary>
        public NumberBox() { }


        //------------------------------------------------------
        //
        //  Protected Methods
        //
        //------------------------------------------------------

        #region Protected Methods

        /// <inheritdoc/>
        protected override Size MeasureOverride(Size availableSize)
        {
            if ((bool)GetValue(AutoHeightProperty))
            {
                var height = _cache.CalculateTextBoxHeight(
                    availableSize.Width,
                    (GameFont)GetValue(FontSizeProperty),
                    false);

                return new Size(availableSize.Width, height);
            }

            return availableSize;
        }

        /// <inheritdoc/>
        protected override void DrawControl(ControlState states)
        {
            _drawer(this, states);
        }

        #endregion


        //------------------------------------------------------
        //
        //  Private Static Methods
        //
        //------------------------------------------------------

        #region Private Static Methods

        private static Action<NumberBox, ControlState> CreateDrawer(GameFont fontSize, bool isPercentage, bool isReadOnly)
        {
            void Draw(NumberBox numberBox, ControlState states)
            {
                var isDisabled = states.HasState(ControlState.Disabled);

                if (isDisabled)
                {
                    GUI.color *= Widgets.InactiveColor;
                }

                if (isReadOnly || isDisabled)
                {
                    numberBox._cache.DrawTextBox(numberBox.RenderRect, fontSize, false, false);
                }
                else
                {
                    var buffer = numberBox._cache.DrawTextBox(numberBox.RenderRect, fontSize, states.HasState(ControlState.Focused | ControlState.CursorDirectlyOver | ControlState.Pressing), false);

                    if (numberBox._cache != buffer && numberBox._inputValidator.IsMatch(buffer))
                    {
                        numberBox.SetValue(ValueProperty, string.IsNullOrEmpty(buffer)
                            ? 0f
                            : (isPercentage
                                ? float.Parse(buffer.Remove(buffer.Length - 1)) * 0.01f
                                : float.Parse(buffer)));
                    }
                }
            }

            return Draw;
        }

        private static void OnLimitChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            d.SetValue(ValueProperty, Mathf.Clamp((float)d.GetValue(ValueProperty), (float)d.GetValue(MinValueProperty), (float)d.GetValue(MaxValueProperty)));
        }

        private static void UpdateDrawer(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((NumberBox)d)._drawer = CreateDrawer(
                (GameFont)d.GetValue(FontSizeProperty),
                (bool)d.GetValue(IsPercentageProperty),
                (bool)d.GetValue(IsReadOnlyProperty));
        }

        private static Func<float, string> CreateFormatter(string formatter, bool isPercentage)
        {
            string Format(float value)
            {
                if (isPercentage)
                {
                    value *= 100f;
                }

                if (float.IsPositiveInfinity(value))
                {
                    return "∞";
                }
                else if (float.IsNegativeInfinity(value))
                {
                    return "-∞";
                }
                else
                {
                    return string.Format(formatter, value);
                }
            }

            return Format;
        }

        #endregion


        private void UpdateValueProcesser(ushort decimals, bool isPercentage)
        {
            if (isPercentage)
            {
                decimals -= 2;

                if (decimals > 0)
                {
                    _inputValidator = new Regex($@"^-?[0-9]{{0,41}}(\.[0-9]{{0,{decimals}}})?%$");
                    _valueProcesser = CreateFormatter($"{{0:F{decimals}}}%", isPercentage: true);
                }
                else
                {
                    _inputValidator = DefaultPercentageValidator;
                    _valueProcesser = CreateFormatter("{0:F0}%", isPercentage: true);
                }
            }
            else
            {
                if (decimals > 0)
                {
                    _inputValidator = new Regex($@"^-?[0-9]{{0,39}}(\.[0-9]{{0,{decimals}}})?$");
                    _valueProcesser = CreateFormatter($"{{0:F{decimals}}}", isPercentage: false);
                }
                else
                {
                    _inputValidator = DefaultValidator;
                    _valueProcesser = DefaultFormatter;
                }
            }

            _cache = _valueProcesser(Value);
        }


        //------------------------------------------------------
        //
        //  Private Fields
        //
        //------------------------------------------------------

        #region Private Fields

        private static readonly Action<NumberBox, ControlState> DefaultDrawer = CreateDrawer(GameFont.Small, false, false);
        private static readonly Func<float, string> DefaultFormatter = CreateFormatter("{0:F0}", isPercentage: false);
        private static readonly Regex DefaultPercentageValidator = new Regex(@"^-?[0-9]{0,41}?$", RegexOptions.Compiled);
        private static readonly Regex DefaultValidator = new Regex(@"^-?[0-9]{0,39}?$", RegexOptions.Compiled);

        private string _cache = "0";
        private Func<float, string> _valueProcesser = DefaultFormatter;

        private Regex _inputValidator = DefaultValidator;

        private Action<NumberBox, ControlState> _drawer = DefaultDrawer;

        #endregion
    }
}
