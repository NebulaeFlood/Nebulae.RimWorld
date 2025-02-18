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
        private float _maximun = 99999f;
        private float _minimun = -99999f;
        private float _step = 0.1f;


        //------------------------------------------------------
        //
        //  Public Properties
        //
        //------------------------------------------------------

        #region Public Properties

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

        #region VerticalAlignment
        /// <summary>
        /// 获取或设置控件垂直对齐方式
        /// </summary>
        public VerticalAlignment VerticalAlignment
        {
            get { return (VerticalAlignment)GetValue(VerticalAlignmentProperty); }
            set { SetValue(VerticalAlignmentProperty, value); }
        }

        /// <summary>
        /// 标识 <see cref="VerticalAlignment"/> 依赖属性。
        /// </summary>
        public static readonly DependencyProperty VerticalAlignmentProperty =
            DependencyProperty.Register(nameof(VerticalAlignment), typeof(VerticalAlignment), typeof(Slider),
                new ControlPropertyMetadata(VerticalAlignment.Center, ControlRelation.Measure));
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
            return RenderSize.AlignToArea(availableRect, HorizontalAlignment.Stretch, VerticalAlignment);
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

            Value = Widgets.HorizontalSlider(
                RenderRect,
                Value,
                _minimun,
                _maximun,
                middleAlignment: true,
                roundTo: _step);
        }

        /// <inheritdoc/>
        protected override Size MeasureCore(Size availableSize)
        {
            return new Size(availableSize.Width, 12f);
        }

        #endregion
    }
}
