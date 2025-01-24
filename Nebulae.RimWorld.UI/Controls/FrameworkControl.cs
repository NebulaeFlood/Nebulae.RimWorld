using Nebulae.RimWorld.UI.Data;
using Nebulae.RimWorld.UI.Utilities;
using Nebulae.RimWorld.Utilities;
using UnityEngine;

namespace Nebulae.RimWorld.UI.Controls
{
    /// <summary>
    /// 可设置更多布局属性的控件的基类，定义了其共同特性
    /// </summary>
    public abstract class FrameworkControl : Control
    {
        //------------------------------------------------------
        //
        //  Pubicl Properties
        //
        //------------------------------------------------------

        #region Pubicl Properties

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

        #region HorizontalAlignment
        /// <summary>
        /// 获取或设置控件水平对齐方式
        /// </summary>
        public HorizontalAlignment HorizontalAlignment
        {
            get { return (HorizontalAlignment)GetValue(HorizontalAlignmentProperty); }
            set { SetValue(HorizontalAlignmentProperty, value); }
        }

        /// <summary>
        /// 标识 <see cref="HorizontalAlignment"/> 依赖属性。
        /// </summary>
        public static readonly DependencyProperty HorizontalAlignmentProperty =
            DependencyProperty.Register(nameof(HorizontalAlignment), typeof(HorizontalAlignment), typeof(FrameworkControl),
                new ControlPropertyMetadata(HorizontalAlignment.Center, ControlRelation.Measure));
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
            DependencyProperty.Register(nameof(VerticalAlignment), typeof(VerticalAlignment), typeof(FrameworkControl),
                new ControlPropertyMetadata(VerticalAlignment.Center, ControlRelation.Measure));
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

        #endregion


        //------------------------------------------------------
        //
        //  Protected Methods
        //
        //------------------------------------------------------

        #region Protected Methods

        /// <inheritdoc/>
        protected override Rect ArrangeCore(Rect availableRect)
        {
            return RenderSize.AlignToArea(availableRect,
                HorizontalAlignment, VerticalAlignment);
        }

        /// <inheritdoc/>
        protected override Size MeasureCore(Size availableSize)
        {
            float desiredWidth, desiredHeight;

            if (HorizontalAlignment is HorizontalAlignment.Stretch)
            {
                desiredWidth = availableSize.Width.IsInfinity()
                    ? Verse.UI.screenWidth
                    : availableSize.Width;
            }
            else
            {
                float width = Width.ReplaceInfinityWith(Verse.UI.screenWidth);

                desiredWidth = width > 1f
                    ? Mathf.Min(width, availableSize.Width)
                    : (availableSize.Width.ReplaceInfinityWith(Verse.UI.screenWidth) * width);
            }

            if (VerticalAlignment is VerticalAlignment.Stretch)
            {
                desiredHeight = availableSize.Height.IsInfinity()
                    ? Verse.UI.screenHeight
                    : availableSize.Height;
            }
            else
            {
                float height = Height.ReplaceInfinityWith(Verse.UI.screenHeight);

                desiredHeight = height > 1f
                    ? Mathf.Min(height, availableSize.Height)
                    : (availableSize.Height.ReplaceInfinityWith(Verse.UI.screenHeight) * height);
            }

            return new Size(desiredWidth, desiredHeight);
        }

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
    }
}
