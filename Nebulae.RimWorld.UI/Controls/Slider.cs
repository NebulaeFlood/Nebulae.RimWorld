using Nebulae.RimWorld.UI.Data;
using Nebulae.RimWorld.UI.Utilities;
using Nebulae.RimWorld.Utilities;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace Nebulae.RimWorld.UI.Controls
{
    /// <summary>
    /// 滑块控件，用于选取数字
    /// </summary>
    [StaticConstructorOnStartup]
    public class Slider : Control
    {
        /// <summary>
        /// 滑轨的混合色
        /// </summary>
        public static readonly Color SliderRailColor = new Color(0.6f, 0.6f, 0.6f, 1f);

        /// <summary>
        /// 滑轨的图像
        /// </summary>
        public static readonly Texture2D SliderHandle = ContentFinder<Texture2D>.Get("UI/Buttons/SliderHandle");

        /// <summary>
        /// 滑块的图像
        /// </summary>
        public static readonly Texture2D SliderRailAtlas = ContentFinder<Texture2D>.Get("UI/Buttons/SliderRail");

        /// <summary>
        /// 滑块的高度
        /// </summary>
        public const float HandleSize = 12f;

        /// <summary>
        /// 滑轨的高度
        /// </summary>
        public const float RailHeight = 8f;


        private static float _lastSoundPlayedTime = -1f;


        //------------------------------------------------------
        //
        //  Private Fields
        //
        //------------------------------------------------------

        #region Private Fields

        private Rect _sliderRect;

        private float _maximun = 99999f;
        private float _minimun = -99999f;
        private float _step = 0.1f;

        #endregion


        //------------------------------------------------------
        //
        //  Public Properties
        //
        //------------------------------------------------------

        #region Public Properties

        #region DrawExtremeValues
        /// <summary>
        /// 获取或设置指示绘制最值提示的值
        /// </summary>
        public bool DrawExtremeValues
        {
            get { return (bool)GetValue(DrawExtremeValuesProperty); }
            set { SetValue(DrawExtremeValuesProperty, value); }
        }

        /// <summary>
        /// 标识 <see cref="DrawExtremeValues"/> 依赖属性。
        /// </summary>
        public static readonly DependencyProperty DrawExtremeValuesProperty =
            DependencyProperty.Register(nameof(DrawExtremeValues), typeof(bool), typeof(Slider),
                new ControlPropertyMetadata(false, ControlRelation.Measure));
        #endregion

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

        /// <summary>
        /// 范围中每个可取值的最小间隔
        /// </summary>
        public float Step
        {
            get => _step;
            set => _step = value;
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
            DependencyProperty.Register(nameof(Value), typeof(float), typeof(Slider),
                new PropertyMetadata(0f));
        #endregion

        #endregion


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
        protected override Rect ArrangeCore(Rect availableRect)
        {
            Rect renderRect = RenderSize.AlignToArea(availableRect,
                HorizontalAlignment.Stretch, VerticalAlignment.Center);

            Size sliderSize = new Size(RenderSize.Width - 12f, HandleSize);

            if (!DrawExtremeValues)
            {
                _sliderRect = sliderSize
                    .AlignToArea(renderRect, HorizontalAlignment.Center, VerticalAlignment.Center);

                return renderRect;
            }

            _sliderRect = sliderSize.AlignToArea(renderRect,
                HorizontalAlignment.Center, VerticalAlignment.Bottom);

            return renderRect;
        }

        /// <inheritdoc/>
        protected override void DrawCore()
        {
            Color color = GUI.color;

            if (!IsEnabled)
            {
                GUI.color = color * SliderRailColor;
            }

            float currentValue = Value;

            DrawSlider(
                _sliderRect,
                currentValue,
                _minimun,
                _maximun);

            if (MouseUtility.IsHitTesting
                && IsPressing)
            {
                float value = Mathf.Round(Mathf.Clamp(
                    (Event.current.mousePosition.x - _sliderRect.x)
                        / _sliderRect.width * (_maximun - _minimun)
                    + _minimun,
                    _minimun,
                    _maximun) / _step) * _step;

                if (currentValue != value)
                {
                    Value = value;

                    if (Time.realtimeSinceStartup > _lastSoundPlayedTime + 0.075f)
                    {
                        _lastSoundPlayedTime = Time.realtimeSinceStartup;
                        SoundDefOf.DragSlider.PlayOneShotOnCamera();
                    }
                }
            }

            if (DrawExtremeValues)
            {
                TextAnchor anchor = Text.Anchor;
                GameFont font = Text.Font;
                Text.Font = GameFont.Tiny;
                Text.Anchor = TextAnchor.UpperLeft;

                Widgets.Label(RenderRect, _minimun.ToString());

                Text.Anchor = TextAnchor.UpperRight;

                Widgets.Label(RenderRect, _maximun.ToString());

                Text.Anchor = anchor;
                Text.Font = font;
            }

            GUI.color = color;
        }

        /// <inheritdoc/>
        protected override Rect HitTestCore(Rect contentRect)
        {
            return contentRect.IntersectWith(new Rect(
                _sliderRect.x - 6f,
                _sliderRect.y,
                _sliderRect.width + 12f,
                HandleSize));
        }

        /// <inheritdoc/>
        override protected Size MeasureCore(Size availableSize)
        {
            return new Size(availableSize.Width, DrawExtremeValues ? HandleSize + 12f : HandleSize);
        }

        #endregion


        private static void DrawSlider(
            Rect sliderRect,
            float value,
            float min,
            float max)
        {
            float handlePos = Mathf.Clamp(
                sliderRect.x + sliderRect.width * Mathf.InverseLerp(min, max, value) - 6f,
                sliderRect.x - 6f,
                sliderRect.xMax - 6f);

            Rect handleRect = new Rect(
                handlePos,
                sliderRect.y,
                HandleSize,
                HandleSize);

            Rect trackRect = new Rect(
                sliderRect.x,
                sliderRect.y + 2f,
                sliderRect.width,
                sliderRect.height - 4f);

            Color color = GUI.color;
            GUI.color = color * SliderRailColor;
            Widgets.DrawAtlas(trackRect, SliderRailAtlas);
            GUI.color = color;

            GUI.DrawTexture(handleRect, SliderHandle);
        }
    }
}
