using Nebulae.RimWorld.UI.Data;
using System;
using UnityEngine;

namespace Nebulae.RimWorld.UI.Controls.Panels
{
    /// <summary>
    /// 一个按照垂直或者水平方向排列子控件的面板
    /// </summary>
    public class StackPanel : Panel
    {
        //------------------------------------------------------
        //
        //  Public Properties
        //
        //------------------------------------------------------

        #region Public Properties

        #region ChildMaxHeight
        /// <summary>
        /// 获取或设置子控件的最大高度
        /// </summary>
        public float ChildMaxHeight
        {
            get { return (float)GetValue(ChildMaxHeightProperty); }
            set { SetValue(ChildMaxHeightProperty, value); }
        }

        /// <summary>
        /// 标识 <see cref="ChildMaxHeight"/> 依赖属性。
        /// </summary>
        public static readonly DependencyProperty ChildMaxHeightProperty =
            DependencyProperty.Register(nameof(ChildMaxHeight), typeof(float), typeof(StackPanel),
                new ControlPropertyMetadata(40f, ControlRelation.Measure));
        #endregion

        #region ChildMaxWidth
        /// <summary>
        /// 获取或设置子控件的最大宽度
        /// </summary>
        public float ChildMaxWidth
        {
            get { return (float)GetValue(ChildMaxWidthProperty); }
            set { SetValue(ChildMaxWidthProperty, value); }
        }

        /// <summary>
        /// 标识 <see cref="ChildMaxWidth"/> 依赖属性。
        /// </summary>
        public static readonly DependencyProperty ChildMaxWidthProperty =
            DependencyProperty.Register(nameof(ChildMaxWidth), typeof(float), typeof(StackPanel),
                new ControlPropertyMetadata(1f, ControlRelation.Measure));
        #endregion

        #region Orientation
        /// <summary>
        /// 获取或设置子控件的排列方向
        /// </summary>
        public Orientation Orientation
        {
            get { return (Orientation)GetValue(OrientationProperty); }
            set { SetValue(OrientationProperty, value); }
        }

        /// <summary>
        /// 标识 <see cref="Orientation"/> 依赖属性。
        /// </summary>
        public static readonly DependencyProperty OrientationProperty =
            DependencyProperty.Register(nameof(Orientation), typeof(Orientation), typeof(StackPanel),
                new ControlPropertyMetadata(Orientation.Vertical, ControlRelation.Measure));
        #endregion

        #endregion


        /// <summary>
        /// 初始化 <see cref="StackPanel"/> 的新实例
        /// </summary>
        public StackPanel()
        {
        }


        //------------------------------------------------------
        //
        //  Public Method
        //
        //------------------------------------------------------

        #region Public Method

        /// <summary>
        /// 向面板末尾添加一个控件
        /// </summary>
        /// <param name="control">要添加的控件</param>
        /// <returns>该面板控件</returns>
        public StackPanel Append(Control control)
        {
            Children.Add(control);
            return this;
        }

        /// <summary>
        /// 清除面板并重新设置面板中的控件
        /// </summary>
        /// <param name="controls">要添加的控件</param>
        /// <returns>该面板控件</returns>
        public StackPanel Set(params Control[] controls)
        {
            Children.OverrideCollection(controls);
            return this;
        }

        /// <summary>
        /// 清除面板中的所有控件
        /// </summary>
        public void Clear()
        {
            Children.Clear();
        }

        /// <summary>
        /// 移除面板中的指定控件
        /// </summary>
        /// <param name="control">要移除的控件</param>
        /// <returns>若移除了指定控件，返回 <see langword="true"/>；反之则返回 <see langword="false"/>。</returns>
        public bool Remove(Control control)
        {
            return Children.Remove(control);
        }

        #endregion


        //------------------------------------------------------
        //
        //  Protected Methods
        //
        //------------------------------------------------------

        #region Protected Methods

        /// <inheritdoc/>
        protected override Rect ArrangeOverride(Rect availableRect)
        {
            Size childrenSize = RenderSize;
            float currentX = availableRect.x;
            float currentY = availableRect.y;

            Size childDesiredSize;
            if (Orientation is Orientation.Horizontal)
            {
                Array.ForEach(FilteredChildren, child =>
                {
                    childDesiredSize = child.DesiredSize;
                    if (childDesiredSize > Size.Empty)
                    {
                        currentX += child.Arrange(new Rect(
                            currentX,
                            currentY,
                            childDesiredSize.Width,
                            childrenSize.Height))
                                .width;
                    }
                });
            }
            else
            {
                Array.ForEach(FilteredChildren, child =>
                {
                    childDesiredSize = child.DesiredSize;
                    if (childDesiredSize > Size.Empty)
                    {
                        currentY += child.Arrange(new Rect(
                            currentX,
                            currentY,
                            childrenSize.Width,
                            childDesiredSize.Height))
                                .height;
                    }
                });
            }

            return new Rect(
                availableRect.x,
                availableRect.y,
                childrenSize.Width,
                childrenSize.Height);
        }

        /// <inheritdoc/>
        protected override Size MeasureOverride(Size availableSize)
        {
            float childMaxHeight = ChildMaxHeight;
            float childMaxWidth = ChildMaxWidth;
            float childrenWidth = 0f;
            float childrenHeight = 0f;

            Size childAvailableSize;
            Size childDesiredSize;
            if (Orientation is Orientation.Horizontal)
            {
                childAvailableSize = new Size(
                    childMaxWidth > 1f
                        ? childMaxWidth
                        : childMaxWidth * availableSize.Width,
                    childMaxHeight > 1f
                        ? childMaxHeight
                        : childMaxHeight * availableSize.Height);

                Array.ForEach(FilteredChildren, child =>
                {
                    childDesiredSize = child.Measure(childAvailableSize);
                    childrenWidth += childDesiredSize.Width;
                    childrenHeight = Mathf.Max(childrenHeight, childDesiredSize.Height);
                });
            }
            else
            {
                childAvailableSize = new Size(
                    childMaxWidth > 1f
                        ? childMaxWidth
                        : childMaxWidth * availableSize.Width,
                    childMaxHeight > 1f
                        ? childMaxHeight
                        : childMaxHeight * availableSize.Height);

                Array.ForEach(FilteredChildren, child =>
                {
                    childDesiredSize = child.Measure(childAvailableSize);
                    childrenHeight += childDesiredSize.Height;
                    childrenWidth = Mathf.Max(childrenWidth, childDesiredSize.Width);
                });
            }
            return new Size(childrenWidth, childrenHeight);
        }

        #endregion
    }
}
