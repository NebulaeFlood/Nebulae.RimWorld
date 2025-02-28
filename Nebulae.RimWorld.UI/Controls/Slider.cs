using Nebulae.RimWorld.UI.Data;
using Nebulae.RimWorld.UI.Utilities;
using Nebulae.RimWorld.Utilities;
using UnityEngine;
using Verse;

namespace Nebulae.RimWorld.UI.Controls
{
    /// <summary>
    /// 滑块控件，用于选取数字
    /// </summary>
    public class Slider : Control
    {
        /// <summary>
        /// 滑块的高度
        /// </summary>
        public const float SliderHeight = 12f;

        //------------------------------------------------------
        //
        //  Private Fields
        //
        //------------------------------------------------------

        #region Private Fields

        private bool _drawExtremeValues = false;
        private Rect _leftValueRect;
        private Rect _rightValueRect;

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

        /// <summary>
        /// 是否显示最大值和最小值
        /// </summary>
        public bool DrawExtremeValues
        {
            get => _drawExtremeValues;
            set
            {
                if (_drawExtremeValues != value)
                {
                    _drawExtremeValues = value;
                    InvalidateMeasure();
                }
            }
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
            if (!_drawExtremeValues)
            {
                _sliderRect = new Size(availableRect.width, SliderHeight)
                    .AlignToArea(availableRect, HorizontalAlignment.Stretch, VerticalAlignment.Center);

                return availableRect;
            }

            float leftMargin = _minimun.ToString().CalculateLength(GameFont.Tiny) + 6f;
            float rightMargin = _maximun.ToString().CalculateLength(GameFont.Tiny) + 6f;

            Size sliderSize = new Size(availableRect.width - leftMargin - rightMargin, SliderHeight);

            _sliderRect = sliderSize.AlignToArea(new Rect(availableRect.x + leftMargin, availableRect.y, sliderSize.Width, availableRect.height),
                HorizontalAlignment.Stretch, VerticalAlignment.Bottom);

            _leftValueRect = availableRect;
            _rightValueRect = new Rect(_sliderRect.xMax, availableRect.y, availableRect.xMax - _sliderRect.xMax, availableRect.height);

            return availableRect;
        }

        /// <inheritdoc/>
        protected override void DrawCore()
        {
            if (RenderRect.Contains(Event.current.mousePosition))
            {
                EventType eventType = Event.current.type;

                if (eventType is EventType.MouseDown)
                {
                    FocusableControl.FocusingControl?.LostFocus();
                }
            }

            if (_drawExtremeValues)
            {
                TextAnchor anchor = Text.Anchor;
                GameFont font = Text.Font;
                Text.Font = GameFont.Tiny;
                Text.Anchor = TextAnchor.UpperLeft;

                GUI.Label(_leftValueRect, _minimun.ToString());
                GUI.Label(_rightValueRect, _maximun.ToString());

                Text.Anchor = anchor;
                Text.Font = font;
            }

            Value = Widgets.HorizontalSlider(
                _sliderRect,
                Value,
                _minimun,
                _maximun,
                middleAlignment: true,
                roundTo: _step);
        }

        /// <inheritdoc/>
        override protected Size MeasureCore(Size availableSize)
        {
            return new Size(availableSize.Width, GameFont.Tiny.GetHeight());
        }


        /// <inheritdoc/>
        protected override void OnDebugDraw(DebugContent content)
        {
            if (content.HasFlag(DebugContent.ControlRect))
            {
                UIUtility.DrawBorder(_sliderRect, UIUtility.HitBoxRectBorderColor);
            }
        }

        #endregion
    }
}
