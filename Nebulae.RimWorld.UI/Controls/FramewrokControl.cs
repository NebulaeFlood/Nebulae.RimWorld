using Nebulae.RimWorld.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nebulae.RimWorld.UI.Controls
{
    /// <summary>
    /// 可自由设置宽高的控件的基类，定义了其共同行为
    /// </summary>
    public abstract class FramewrokControl : Control
    {
        //------------------------------------------------------
        //
        //  Public Properties
        //
        //------------------------------------------------------

        #region Public Properties

        #region Height
        /// <summary>
        /// 获取或设置控件高度
        /// </summary>
        /// <remarks>当值小于等于 1 时代表占用可用空间的高度的百分比。</remarks>
        public float Height
        {
            get { return (float)GetValue(HeightProperty); }
            set { SetValue(HeightProperty, value); }
        }

        /// <summary>
        /// 标识 <see cref="Height"/> 依赖属性。
        /// </summary>
        public static readonly DependencyProperty HeightProperty =
            DependencyProperty.Register(nameof(Height), typeof(float), typeof(FramewrokControl),
                new ControlPropertyMetadata(1f, ControlPropertyMetadataFlag.Measure),
                ValidateSize);
        #endregion

        #region Width
        /// <summary>
        /// 获取或设置
        /// </summary>
        public float Width
        {
            get { return (float)GetValue(WidthProperty); }
            set { SetValue(WidthProperty, value); }
        }

        /// <summary>
        /// 标识 <see cref="Width"/> 依赖属性。
        /// </summary>
        public static readonly DependencyProperty WidthProperty =
            DependencyProperty.Register(nameof(Width), typeof(float), typeof(FramewrokControl),
                new ControlPropertyMetadata(1f, ControlPropertyMetadataFlag.Measure),
                ValidateSize);
        #endregion

        private static bool ValidateSize(object value)
        {
            if (value is float size)
            {
                return !size.IsInfinity() && size >= 0f;
            }
            else
            {
                return false;
            }
        }

        #endregion


        static FramewrokControl()
        {
            HorizontalAlignmentProperty.OverrideMetadata(typeof(FramewrokControl),
                new ControlPropertyMetadata(HorizontalAlignment.Stretch, ControlPropertyMetadataFlag.Arrange));
            VerticalAlignmentProperty.OverrideMetadata(typeof(FramewrokControl),
                new ControlPropertyMetadata(VerticalAlignment.Stretch, ControlPropertyMetadataFlag.Arrange));
        }


        /// <inheritdoc/>
        protected override Size MeasureCore(Size availableSize)
        {
            float desiredWidth, desiredHeight;

            if (HorizontalAlignment is HorizontalAlignment.Stretch)
            {
                desiredWidth = availableSize.Width.IsInfinity() 
                    ? UIUltility.ScreenWidth
                    : availableSize.Width;
            }
            else
            {
                desiredWidth = Width > 1f
                    ? Width
                    : (availableSize.Width.IsInfinity()
                        ? UIUltility.ScreenWidth
                        : availableSize.Width) * Width;
            }

            if (VerticalAlignment is VerticalAlignment.Stretch)
            {
                desiredHeight = availableSize.Height.IsInfinity()
                    ? UIUltility.ScreenHeight
                    : availableSize.Height;
            }
            else
            {
                desiredHeight = Height > 1f
                    ? Height
                    : (availableSize.Height.IsInfinity()
                        ? UIUltility.ScreenHeight
                        : availableSize.Height) * Height;
            }

            return new Size(desiredWidth, desiredHeight);
        }
    }
}
