﻿using Nebulae.RimWorld.UI.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Nebulae.RimWorld.UI.Controls.Panels
{
    /// <summary>
    /// 一个将子控件排列成数行或数列的可虚拟化子控件的面板
    /// </summary>
    public class VirtualizingWrapPanel : VirtualizingPanel
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
            DependencyProperty.Register(nameof(ChildMaxHeight), typeof(float), typeof(VirtualizingWrapPanel),
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
            DependencyProperty.Register(nameof(ChildMaxWidth), typeof(float), typeof(VirtualizingWrapPanel),
                new ControlPropertyMetadata(200f, ControlRelation.Measure));
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
            DependencyProperty.Register(nameof(Orientation), typeof(Orientation), typeof(VirtualizingWrapPanel),
                new ControlPropertyMetadata(Orientation.Vertical, ControlRelation.Measure));
        #endregion

        #endregion


        /// <summary>
        /// 初始化 <see cref="VirtualizingWrapPanel"/> 的新实例
        /// </summary>
        public VirtualizingWrapPanel()
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
        public void Append(Control control)
        {
            Children.Add(control);
        }

        /// <summary>
        /// 清除面板并重新设置面板中的控件
        /// </summary>
        /// <param name="controls">要添加的控件</param>
        /// <returns>该面板控件</returns>
        public VirtualizingWrapPanel Set(params Control[] controls)
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
            Size desiredSize = DesiredSize - Margin;
            float currentX = availableRect.x;
            float currentY = availableRect.y;

            Size childDesiredSize;
            if (Orientation is Orientation.Horizontal)
            {
                float maxRowHeight = 0f;

                Array.ForEach(FilteredChildren, child =>
                {
                    childDesiredSize = child.DesiredSize;
                    if (childDesiredSize > Size.Empty)
                    {

                    ArrangeStart:
                        if (currentX + childDesiredSize.Width > availableRect.x + availableRect.width)
                        {
                            currentX = availableRect.x;
                            currentY += maxRowHeight;   // 换行
                            maxRowHeight = 0f;

                            goto ArrangeStart;  // 重新排列该控件
                        }
                        else
                        {
                            currentX += child.Arrange(new Rect(
                                currentX,
                                currentY,
                                childDesiredSize.Width,
                                childDesiredSize.Height))
                                    .width;
                            maxRowHeight = Mathf.Max(maxRowHeight, childDesiredSize.Height);
                        }
                    }
                });
            }
            else
            {
                float maxColumnWidth = 0f;

                Array.ForEach(FilteredChildren, child =>
                {
                    childDesiredSize = child.DesiredSize;
                    if (childDesiredSize > Size.Empty)
                    {

                    ArrangeStart:
                        if (currentY + childDesiredSize.Height > availableRect.y + availableRect.height)
                        {
                            currentY = availableRect.y;
                            currentX += maxColumnWidth; // 换列
                            maxColumnWidth = 0f;

                            goto ArrangeStart;  // 重新排列该控件
                        }
                        else
                        {
                            currentY += child.Arrange(new Rect(
                                currentX,
                                currentY,
                                childDesiredSize.Width,
                                childDesiredSize.Height))
                                    .height;
                            maxColumnWidth = Mathf.Max(maxColumnWidth, childDesiredSize.Width);
                        }
                    }
                });
            }

            return new Rect(
                availableRect.x,
                availableRect.y,
                desiredSize.Width,
                desiredSize.Height);
        }

        /// <inheritdoc/>
        protected override Size MeasureOverride(Size availableSize)
        {
            float childMaxHeight = ChildMaxHeight;
            float childMaxWidth = ChildMaxWidth;
            float desiredWidth = 0f;
            float desiredHeight = 0f;

            Size childAvailableSize;
            Size childDesiredSize;
            if (Orientation is Orientation.Horizontal)
            {
                childAvailableSize = new Size(
                    childMaxWidth > 1f
                        ? Mathf.Min(childMaxWidth, availableSize.Width)
                        : childMaxWidth * availableSize.Width,
                    childMaxHeight > 1f
                        ? childMaxHeight
                        : childMaxHeight * availableSize.Height);

                float rowWidth = 0f;
                float maxRowHeight = 0f;
                Array.ForEach(FilteredChildren, child =>
                {
                    childDesiredSize = child.Measure(childAvailableSize);
                    if (childDesiredSize > Size.Empty)
                    {

                    MeasureStart:
                        if (rowWidth + childDesiredSize.Width > availableSize.Width)
                        {
                            desiredWidth = Mathf.Max(desiredWidth, rowWidth);   // 取所有行中最大的宽度
                            desiredHeight += maxRowHeight;  // 换行
                            rowWidth = 0f;
                            maxRowHeight = 0f;

                            goto MeasureStart;  // 重新计算该控件
                        }
                        else
                        {
                            rowWidth += childDesiredSize.Width;
                            maxRowHeight = Mathf.Max(maxRowHeight, childDesiredSize.Height);
                        }

                    }
                });
                desiredHeight += maxRowHeight;
            }
            else
            {
                childAvailableSize = new Size(
                    childMaxWidth > 1f
                        ? childMaxWidth
                        : childMaxWidth * availableSize.Width,
                    childMaxHeight > 1f
                        ? Mathf.Min(childMaxHeight, availableSize.Height)
                        : childMaxHeight * availableSize.Height);

                float columnHeight = 0f;
                float maxColumnWidth = 0f;
                Array.ForEach(FilteredChildren, child =>
                {
                    childDesiredSize = child.Measure(childAvailableSize);
                    if (childDesiredSize > Size.Empty)
                    {

                    MeasureStart:
                        if (columnHeight + childDesiredSize.Height > availableSize.Height)
                        {
                            desiredHeight = Mathf.Max(desiredHeight, columnHeight); // 取所有列中最大的高度
                            desiredWidth += maxColumnWidth;    // 换列
                            columnHeight = 0f;
                            maxColumnWidth = 0f;

                            goto MeasureStart;  // 重新计算该控件
                        }
                        else
                        {
                            columnHeight += childDesiredSize.Height;
                            maxColumnWidth = Mathf.Max(maxColumnWidth, childDesiredSize.Width);
                        }
                    }
                });
                desiredWidth += maxColumnWidth;
            }
            return new Size(desiredWidth, desiredHeight);
        }

        #endregion
    }
}