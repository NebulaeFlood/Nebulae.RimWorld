using Nebulae.RimWorld.UI.Data;
using Nebulae.RimWorld.Utilities;
using UnityEngine;

namespace Nebulae.RimWorld.UI.Controls
{
    /// <summary>
    /// 可自由设置宽高的控件的基类，定义了其共同特性
    /// </summary>
    public abstract class FrameworkControl : Control
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
            DependencyProperty.Register(nameof(Height), typeof(float), typeof(FrameworkControl),
                new ControlPropertyMetadata(1f, ControlRelation.Measure),
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
            DependencyProperty.Register(nameof(Width), typeof(float), typeof(FrameworkControl),
                new ControlPropertyMetadata(1f, ControlRelation.Measure),
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


        /// <summary>
        /// 为 <see cref="FrameworkControl"/> 派生类实现基本初始化
        /// </summary>
        protected FrameworkControl()
        {
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
                float width = Width.ReplaceInfinityWith(UIUltility.ScreenWidth);

                desiredWidth = width > 1f
                    ? Mathf.Min(width, availableSize.Width)
                    : (availableSize.Width.ReplaceInfinityWith(UIUltility.ScreenWidth) * width);
            }

            if (VerticalAlignment is VerticalAlignment.Stretch)
            {
                desiredHeight = availableSize.Height.IsInfinity()
                    ? UIUltility.ScreenHeight
                    : availableSize.Height;
            }
            else
            {
                float height = Height.ReplaceInfinityWith(UIUltility.ScreenHeight);

                desiredHeight = height > 1f
                    ? Mathf.Min(height, availableSize.Height)
                    : (availableSize.Height.ReplaceInfinityWith(UIUltility.ScreenHeight) * height);
            }

            return new Size(desiredWidth, desiredHeight);
        }
    }
}
