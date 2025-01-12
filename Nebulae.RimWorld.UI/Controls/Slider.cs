using Nebulae.RimWorld.UI.Data;
using Nebulae.RimWorld.Utilities;
using UnityEngine;
using Verse;

namespace Nebulae.RimWorld.UI.Controls
{
    /// <summary>
    /// 滑块控件，用于选取数字
    /// </summary>
    public class Slider : FrameworkControl
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

        #endregion


        /// <summary>
        /// 初始化 <see cref="Slider"/> 的新实例
        /// </summary>
        public Slider()
        {
        }


        /// <inheritdoc/>
        protected override Rect DrawCore(Rect renderRect)
        {
            if (renderRect.Contains(Event.current.mousePosition))
            {
                EventType eventType = Event.current.type;

                if (eventType is EventType.MouseDown)
                {
                    FocusableControl.FocusingControl?.LostFocus();
                }
            }

            Value = Widgets.HorizontalSlider(
                renderRect,
                Value,
                _minimun,
                _maximun,
                middleAlignment: true,
                roundTo: _step);

            return renderRect;
        }
    }
}
