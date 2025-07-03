using NAudio.Utils;
using Nebulae.RimWorld.UI.Controls.Resources;
using Nebulae.RimWorld.UI.Core.Data;
using Nebulae.RimWorld.UI.Utilities;
using Nebulae.RimWorld.Utilities;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using Verse.Noise;
using Verse.Sound;

namespace Nebulae.RimWorld.UI.Controls.Basic
{
    /// <summary>
    /// 滑块控件
    /// </summary>
    public sealed class Slider : FrameworkControl
    {
        //------------------------------------------------------
        //
        //  Public Consts
        //
        //------------------------------------------------------

        #region Public Consts

        /// <summary>
        /// 滑块的尺寸
        /// </summary>
        public const float HandleSize = 12f;

        /// <summary>
        /// 滑轨的高度
        /// </summary>
        public const float RailHeight = 8f;

        #endregion


        //------------------------------------------------------
        //
        //  Dependency Properties
        //
        //------------------------------------------------------

        #region Dependency Properties

        #region Decimals
        /// <summary>
        /// 获取或设置 <see cref="Slider"/> 的数字的小数位数
        /// </summary>
        public ushort Decimals
        {
            get { return (ushort)GetValue(DecimalsProperty); }
            set { SetValue(DecimalsProperty, value); }
        }

        /// <summary>
        /// 标识 <see cref="Decimals"/> 依赖属性
        /// </summary>
        public static readonly DependencyProperty DecimalsProperty =
            DependencyProperty.Register(nameof(Decimals), typeof(ushort), typeof(Slider),
                new PropertyMetadata(0, CoerceDecimals, UpdateDrawer));

        private static object CoerceDecimals(DependencyObject d, object baseValue)
        {
            return (ushort)baseValue > 5 ? 5 : baseValue;
        }
        #endregion

        #region IsPercentage
        /// <summary>
        /// 获取或设置一个值，该值指示 <see cref="Slider"/> 的数字是否以百分数的形式显示
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
            DependencyProperty.Register(nameof(IsPercentage), typeof(bool), typeof(Slider),
                new ControlPropertyMetadata(false, UpdateDrawer, ControlRelation.Measure));
        #endregion

        #region MaxValue
        /// <summary>
        /// 获取或设置 <see cref="Slider"/> 的最大值
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
            DependencyProperty.Register(nameof(MaxValue), typeof(float), typeof(Slider),
                new ControlPropertyMetadata(32767f, UpdateDrawer, ControlRelation.Arrange));
        #endregion

        #region MinValue
        /// <summary>
        /// 获取或设置 <see cref="Slider"/> 的最小值
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
            DependencyProperty.Register(nameof(MinValue), typeof(float), typeof(Slider),
                new ControlPropertyMetadata(-32768f, UpdateDrawer, ControlRelation.Arrange));
        #endregion

        #region ShowLabels
        /// <summary>
        /// 获取或设置一个值，该值指示 <see cref="Slider"/> 是否显示最大值和最小值的标签
        /// </summary>
        public bool ShowLabels
        {
            get { return (bool)GetValue(ShowLabelsProperty); }
            set { SetValue(ShowLabelsProperty, value); }
        }

        /// <summary>
        /// 标识 <see cref="ShowLabels"/> 依赖属性
        /// </summary>
        public static readonly DependencyProperty ShowLabelsProperty =
            DependencyProperty.Register(nameof(ShowLabels), typeof(bool), typeof(Slider),
                new ControlPropertyMetadata(true, UpdateDrawer, ControlRelation.Arrange));
        #endregion

        #region Value
        /// <summary>
        /// 获取或设置 <see cref="Slider"/> 的数字
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
            DependencyProperty.Register(nameof(Value), typeof(float), typeof(Slider),
                new PropertyMetadata(0f, OnValueChanged));

        private static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((Slider)d)._cache = (float)e.NewValue;
        }
        #endregion

        #endregion


        static Slider()
        {
            MarginProperty.OverrideMetadata(typeof(Slider),
                new ControlPropertyMetadata(new Thickness(6f, 0f, 6f, 0f), ControlRelation.Measure));
        }

        /// <summary>
        /// 初始化 <see cref="Slider"/> 的新实例
        /// </summary>
        public Slider()
        {
            IsHitTestVisible = true;
        }


        //------------------------------------------------------
        //
        //  Protected Methods
        //
        //------------------------------------------------------

        #region Protected Methods

        /// <inheritdoc/>
        protected override Rect ArrangeOverride(Rect availableRect)
        {
            _sliderRect = new Size(RenderSize.Width, HandleSize)
                .AlignToArea(availableRect, HorizontalAlignment.Stretch, VerticalAlignment.Center);

            if ((bool)GetValue(ShowLabelsProperty))
            {
                FormatLimit(
                    (ushort)GetValue(DecimalsProperty),
                    (float)GetValue(MinValueProperty),
                    (float)GetValue(MaxValueProperty),
                    (bool)GetValue(IsPercentageProperty),
                    out var minStr, out var maxStr);

                _sliderRect.x += minStr.CalculateLength(GameFont.Tiny);
                _sliderRect.xMax -= 10f + maxStr.CalculateLength(GameFont.Tiny);
            }

            _railRect = new Rect(_sliderRect.x + 6f, _sliderRect.y + 2f, _sliderRect.width - HandleSize, RailHeight);

            return availableRect;
        }

        /// <inheritdoc/>
        protected override Size MeasureOverride(Size availableSize)
        {
            return new Size(availableSize.Width, ShowLabels ? HandleSize + 12f : HandleSize);
        }

        /// <inheritdoc/>
        protected override SegmentResult SegmentCore(Rect visiableRect)
        {
            return new SegmentResult(_sliderRect.IntersectWith(visiableRect), RenderRect.IntersectWith(visiableRect));
        }

        /// <inheritdoc/>
        protected override void DrawCore(ControlState states)
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

        private static void UpdateDrawer(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((Slider)d)._drawer = CreateDrawer(
                (ushort)d.GetValue(DecimalsProperty),
                (float)d.GetValue(MinValueProperty),
                (float)d.GetValue(MaxValueProperty),
                (bool)d.GetValue(IsPercentageProperty),
                (bool)d.GetValue(ShowLabelsProperty));
        }

        private static Action<Slider, ControlState> CreateDrawer(ushort decimals, float minValue, float maxValue, bool isPercentage, bool showLabels)
        {
            float range = maxValue - minValue;

            if (showLabels)
            {
                FormatLimit(decimals, minValue, maxValue, isPercentage, out var minStr, out var maxStr);

                void Draw(Slider slider, ControlState states)
                {
                    if (states.HasState(ControlState.Disabled))
                    {
                        GUI.color *= Widgets.InactiveColor;
                    }

                    var railWidth = slider._railRect.width;

                    float handleX = Mathf.Round(
                        Mathf.Clamp(
                            railWidth * slider._cache / range,
                            0f,
                            railWidth));

                    var handleRect = new Rect(slider._sliderRect.x + handleX, slider._sliderRect.y, HandleSize, HandleSize);

                    var color = GUI.color;
                    GUI.color = color * SliderResources.RailColor;
                    Widgets.DrawAtlas(slider._railRect, SliderResources.RailAtlas);
                    GUI.color = color;

                    GUI.DrawTexture(handleRect, SliderResources.Handle, ScaleMode.ScaleToFit);

                    if (states.HasState(ControlState.Pressing))
                    {
                        float currentValue = Mathf.Clamp((minValue + (Event.current.mousePosition.x - slider._railRect.x) * range / railWidth),
                            minValue, maxValue).Round(decimals);

                        if (currentValue != slider._cache)
                        {
                            slider.SetValue(ValueProperty, currentValue);

                            if (Time.realtimeSinceStartup > _lastSoundPlayedTime + 0.075f)
                            {
                                _lastSoundPlayedTime = Time.realtimeSinceStartup;
                                SoundDefOf.DragSlider.PlayOneShotOnCamera();
                            }
                        }
                    }

                    var anchor = Text.Anchor;
                    var font = Text.Font;

                    Text.Font = GameFont.Tiny;
                    Text.Anchor = TextAnchor.LowerLeft;

                    Widgets.Label(slider.RenderRect, minStr);

                    Text.Anchor = TextAnchor.LowerRight;

                    Widgets.Label(slider.RenderRect, maxStr);

                    Text.Anchor = anchor;
                    Text.Font = font;
                }

                return Draw;
            }
            else
            {
                void Draw(Slider slider, ControlState states)
                {
                    if (states.HasState(ControlState.Disabled))
                    {
                        GUI.color *= Widgets.InactiveColor;
                    }

                    var railWidth = slider._railRect.width;

                    float handleX = Mathf.Round(
                        Mathf.Clamp(
                            railWidth * slider._cache / range,
                            0f,
                            railWidth));

                    var handleRect = new Rect(slider._sliderRect.x + handleX, slider._sliderRect.y, HandleSize, HandleSize);

                    var color = GUI.color;
                    GUI.color = color * SliderResources.RailColor;
                    Widgets.DrawAtlas(slider._railRect, SliderResources.RailAtlas);
                    GUI.color = color;

                    GUI.DrawTexture(handleRect, SliderResources.Handle, ScaleMode.ScaleToFit);

                    if (states.HasState(ControlState.Pressing))
                    {
                        float currentValue = Mathf.Clamp((minValue + (Event.current.mousePosition.x - slider._railRect.x) * range / railWidth),
                            minValue, maxValue).Round(decimals);

                        if (currentValue != slider._cache)
                        {
                            slider.SetValue(ValueProperty, currentValue);

                            if (Time.realtimeSinceStartup > _lastSoundPlayedTime + 0.075f)
                            {
                                _lastSoundPlayedTime = Time.realtimeSinceStartup;
                                SoundDefOf.DragSlider.PlayOneShotOnCamera();
                            }
                        }
                    }
                }

                return Draw;
            }
        }

        private static void FormatLimit(ushort decimals, float minValue, float maxValue, bool isPercentage, out string minStr, out string maxStr)
        {
            string Format(string format, float value)
            {
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
                    return string.Format(format, value);
                }
            }

            if (isPercentage)
            {
                var format = $"{{0:F{Math.Max(decimals - 2, 0)}}}%";

                maxStr = Format(format, maxValue * 100f);
                minStr = Format(format, minValue * 100f);
            }
            else
            {
                var format = $"{{0:F{decimals}}}";

                maxStr = Format(format, maxValue);
                minStr = Format(format, minValue);
            }
        }

        #endregion


        //------------------------------------------------------
        //
        //  Private Fields
        //
        //------------------------------------------------------

        #region Private Fields

        private static readonly Action<Slider, ControlState> DefaultDrawer = CreateDrawer(0, -32768f, 32767f, false, true);
        private static float _lastSoundPlayedTime = -1f;

        private float _cache;
        private Action<Slider, ControlState> _drawer = DefaultDrawer;

        private Rect _railRect;
        private Rect _sliderRect;

        #endregion
    }
}
