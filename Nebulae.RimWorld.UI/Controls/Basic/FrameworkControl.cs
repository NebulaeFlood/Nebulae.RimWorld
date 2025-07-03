using Nebulae.RimWorld.UI.Core.Data;
using Nebulae.RimWorld.UI.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace Nebulae.RimWorld.UI.Controls.Basic
{
    /// <summary>
    /// 可控制布局属性的控件
    /// </summary>
    [DebuggerStepThrough]
    public abstract class FrameworkControl : Control
    {
        //------------------------------------------------------
        //
        //  Public Fields
        //
        //------------------------------------------------------

        #region Public Fields

        /// <summary>
        /// 控件将要绘制的区域
        /// </summary>
        public Rect RenderRect;

        /// <summary>
        /// 控件将要绘制的尺寸
        /// </summary>
        public Size RenderSize = Size.Empty;

        #endregion


        //------------------------------------------------------
        //
        //  Dependecy Properties
        //
        //------------------------------------------------------

        #region Dependecy Properties

        #region HorizontalAlignment
        /// <summary>
        /// 获取或设置 <see cref="FrameworkControl"/> 的水平对齐方式
        /// </summary>
        public HorizontalAlignment HorizontalAlignment
        {
            get { return (HorizontalAlignment)GetValue(HorizontalAlignmentProperty); }
            set { SetValue(HorizontalAlignmentProperty, value); }
        }

        /// <summary>
        /// 标识 <see cref="HorizontalAlignment"/> 依赖属性
        /// </summary>
        public static readonly DependencyProperty HorizontalAlignmentProperty =
            DependencyProperty.Register(nameof(HorizontalAlignment), typeof(HorizontalAlignment), typeof(FrameworkControl),
                new PropertyMetadata(HorizontalAlignment.Center, OnHorizontalAlignmentChanged));

        private static void OnHorizontalAlignmentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue is HorizontalAlignment.Stretch || e.NewValue is HorizontalAlignment.Stretch)
            {
                ((Control)d).InvalidateMeasure();
            }
            else
            {
                ((Control)d).InvalidateArrange();
            }
        }
        #endregion

        #region VerticalAlignment
        /// <summary>
        /// 获取或设置 <see cref="FrameworkControl"/> 的垂直对齐方式
        /// </summary>
        public VerticalAlignment VerticalAlignment
        {
            get { return (VerticalAlignment)GetValue(VerticalAlignmentProperty); }
            set { SetValue(VerticalAlignmentProperty, value); }
        }

        /// <summary>
        /// 标识 <see cref="VerticalAlignment"/> 依赖属性
        /// </summary>
        public static readonly DependencyProperty VerticalAlignmentProperty =
            DependencyProperty.Register(nameof(VerticalAlignment), typeof(VerticalAlignment), typeof(FrameworkControl),
                new PropertyMetadata(VerticalAlignment.Center, OnVerticalAlignmentChanged));

        private static void OnVerticalAlignmentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue is VerticalAlignment.Stretch || e.NewValue is VerticalAlignment.Stretch)
            {
                ((Control)d).InvalidateMeasure();
            }
            else
            {
                ((Control)d).InvalidateArrange();
            }
        }
        #endregion

        #region Margin
        /// <summary>
        /// 获取或设置 <see cref="FrameworkControl"/> 的外边距
        /// </summary>
        public Thickness Margin
        {
            get { return (Thickness)GetValue(MarginProperty); }
            set { SetValue(MarginProperty, value); }
        }

        /// <summary>
        /// 标识 <see cref="Margin"/> 依赖属性
        /// </summary>
        public static readonly DependencyProperty MarginProperty =
            DependencyProperty.Register(nameof(Margin), typeof(Thickness), typeof(FrameworkControl),
                new ControlPropertyMetadata(Thickness.Empty, CoerceThickness, ControlRelation.Measure));
        #endregion

        #region Padding
        /// <summary>
        /// 获取或设置 <see cref="FrameworkControl"/> 的内边距
        /// </summary>
        public Thickness Padding
        {
            get { return (Thickness)GetValue(PaddingProperty); }
            set { SetValue(PaddingProperty, value); }
        }

        /// <summary>
        /// 标识 <see cref="Padding"/> 依赖属性
        /// </summary>
        public static readonly DependencyProperty PaddingProperty =
            DependencyProperty.Register(nameof(Padding), typeof(Thickness), typeof(FrameworkControl),
                new ControlPropertyMetadata(Thickness.Empty, CoerceThickness, ControlRelation.Measure));
        #endregion

        #region Width
        /// <summary>
        /// 获取或设置 <see cref="FrameworkControl"/> 的宽度
        /// </summary>
        /// <remarks>当值为小于等于 1 的正数时，则表示所占可用空间的百分比。</remarks>
        public float Width
        {
            get { return (float)GetValue(WidthProperty); }
            set { SetValue(WidthProperty, value); }
        }

        /// <summary>
        /// 标识 <see cref="Width"/> 依赖属性
        /// </summary>
        public static readonly DependencyProperty WidthProperty =
            DependencyProperty.Register(nameof(Width), typeof(float), typeof(FrameworkControl),
                new ControlPropertyMetadata(1f, CoerceSize, ControlRelation.Measure));
        #endregion

        #region Height
        /// <summary>
        /// 获取或设置 <see cref="FrameworkControl"/> 的高度
        /// </summary>
        /// <remarks>当值为小于等于 1 的正数时，则表示所占可用空间的百分比。</remarks>
        public float Height
        {
            get { return (float)GetValue(HeightProperty); }
            set { SetValue(HeightProperty, value); }
        }

        /// <summary>
        /// 标识 <see cref="Height"/> 依赖属性
        /// </summary>
        public static readonly DependencyProperty HeightProperty =
            DependencyProperty.Register(nameof(Height), typeof(float), typeof(FrameworkControl),
                new ControlPropertyMetadata(1f, CoerceSize, ControlRelation.Measure));
        #endregion

        #endregion


        //------------------------------------------------------
        //
        //  Protected Methods
        //
        //------------------------------------------------------

        #region Protected Methods

        /// <inheritdoc/>
        protected override sealed Rect ArrangeCore(Rect availableRect)
        {
            var margin = Margin;
            var padding = Padding;

            RenderRect = ArrangeOverride(RenderSize.AlignToArea(availableRect - (margin + padding), HorizontalAlignment, VerticalAlignment))
                .Rounded() + padding;

            return RenderRect + margin;
        }

        /// <summary>
        /// 计算 <see cref="FrameworkControl"/> 的内容需要占用的区域
        /// </summary>
        /// <param name="availableRect">由 <see cref="FrameworkControl"/> 计算后分配的区域</param>
        /// <returns>内容需要占用的区域。</returns>
        protected virtual Rect ArrangeOverride(Rect availableRect) => availableRect;

        /// <inheritdoc/>
        protected override sealed Size MeasureCore(Size availableSize)
        {
            var margin = Margin;
            var padding = Padding;

            availableSize -= (margin + padding);

            float renderWidth, renderHeight;

            if (HorizontalAlignment is HorizontalAlignment.Stretch)
            {
                renderWidth = availableSize.Width;
            }
            else
            {
                float logicalWidth = Width;

                renderWidth = logicalWidth > 1f
                    ? logicalWidth
                    : availableSize.Width * logicalWidth;
            }

            if (VerticalAlignment is VerticalAlignment.Stretch)
            {
                renderHeight = availableSize.Height;
            }
            else
            {
                float logicalHeight = Height;

                renderHeight = logicalHeight > 1f
                    ? logicalHeight
                    : availableSize.Height * logicalHeight;
            }

            RenderSize = MeasureOverride(new Size(renderWidth, renderHeight)).Round() + padding;

            return RenderSize + margin;
        }

        /// <summary>
        /// 计算 <see cref="FrameworkControl"/> 的内容需要占用的尺寸
        /// </summary>
        /// <param name="availableSize">由 <see cref="FrameworkControl"/> 计算后可用的尺寸</param>
        /// <returns>内容需要占用的尺寸。</returns>
        protected virtual Size MeasureOverride(Size availableSize) => availableSize;

        /// <inheritdoc/>
        protected override SegmentResult SegmentCore(Rect visiableRect) => visiableRect.IntersectWith(RenderRect);

        #endregion


        internal override sealed void DrawDebugContent()
        {
            if (DebugContent.HasFlag(DebugContent.RenderRect) && RenderSize != Size.Empty)
            {
                UIUtility.DrawBorder(RenderRect, BrushUtility.RederRectBorderBrush);
            }

            base.DrawDebugContent();
        }


        //------------------------------------------------------
        //
        //  Private Static Methods
        //
        //------------------------------------------------------

        #region Private Static Methods

        private static object CoerceSize(DependencyObject d, object baseValue)
        {
            return Mathf.Max((float)baseValue, 0f);
        }

        private static object CoerceThickness(DependencyObject d, object baseValue)
        {
            return ((Thickness)baseValue).Normalize();
        }

        #endregion
    }
}
