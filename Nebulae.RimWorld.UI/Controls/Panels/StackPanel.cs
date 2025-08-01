using Nebulae.RimWorld.UI.Controls.Basic;
using Nebulae.RimWorld.UI.Core.Data;
using System;
using System.Collections.Generic;
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
        //  Dependency Properties
        //
        //------------------------------------------------------

        #region Dependency Properties

        #region ItemWidth
        /// <summary>
        /// 获取或设置 <see cref="StackPanel"/> 分配给子控件的宽度
        /// </summary>
        public float ItemWidth
        {
            get { return (float)GetValue(ItemWidthProperty); }
            set { SetValue(ItemWidthProperty, value); }
        }

        /// <summary>
        /// 标识 <see cref="ItemWidth"/> 依赖属性
        /// </summary>
        public static readonly DependencyProperty ItemWidthProperty =
            DependencyProperty.Register(nameof(ItemWidth), typeof(float), typeof(StackPanel),
                new ControlPropertyMetadata(1f, ControlRelation.Measure));
        #endregion

        #region ItemHeight
        /// <summary>
        /// 获取或设置 <see cref="StackPanel"/> 分配给子控件的高度
        /// </summary>
        public float ItemHeight
        {
            get { return (float)GetValue(ItemHeightProperty); }
            set { SetValue(ItemHeightProperty, value); }
        }

        /// <summary>
        /// 标识 <see cref="ItemHeight"/> 依赖属性
        /// </summary>
        public static readonly DependencyProperty ItemHeightProperty =
            DependencyProperty.Register(nameof(ItemHeight), typeof(float), typeof(StackPanel),
                new ControlPropertyMetadata(34f, ControlRelation.Measure));
        #endregion

        #region Orientation
        /// <summary>
        /// 获取或设置 <see cref="StackPanel"/> 排列子控件的方向
        /// </summary>
        public Orientation Orientation
        {
            get { return (Orientation)GetValue(OrientationProperty); }
            set { SetValue(OrientationProperty, value); }
        }

        /// <summary>
        /// 标识 <see cref="Orientation"/> 依赖属性
        /// </summary>
        public static readonly DependencyProperty OrientationProperty =
            DependencyProperty.Register(nameof(Orientation), typeof(Orientation), typeof(StackPanel),
                new ControlPropertyMetadata(Orientation.Vertical, ControlRelation.Measure));
        #endregion

        #endregion


        /// <summary>
        /// 初始化 <see cref="StackPanel"/> 的新实例
        /// </summary>
        public StackPanel() { }


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
        /// <returns>该面板控件。</returns>
        public StackPanel Append(Control control)
        {
            Children.Add(control);
            return this;
        }

        /// <summary>
        /// 清除面板并重新设置面板中的控件
        /// </summary>
        /// <param name="controls">要添加的控件</param>
        /// <returns>该面板控件。</returns>
        public StackPanel Set(IEnumerable<Control> controls)
        {
            Children.OverrideCollection(controls);
            return this;
        }

        /// <summary>
        /// 清除面板并重新设置面板中的控件
        /// </summary>
        /// <param name="controls">要添加的控件</param>
        /// <returns>该面板控件。</returns>
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
        /// 将控件按照视觉效果插入或移动到面板中的指定控件之前或之后
        /// </summary>
        /// <param name="control">要插入的控件</param>
        /// <param name="index">被挤开的控件</param>
        /// <returns>若插入了指定控件，返回 <see langword="true"/>；反之则返回 <see langword="false"/>。</returns>
        public bool Insert(Control control, Control index)
        {
            return Children.Insert(index, control);
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

        /// <summary>
        /// 使用指定的比较强排序面板中的控件
        /// </summary>
        /// <param name="comparison">比较控件时使用的比较器</param>
        /// <returns>该面板控件。</returns>
        public StackPanel Sort(Comparison<Control> comparison)
        {
            Children.Sort(comparison);
            return this;
        }

        /// <summary>
        /// 使用指定的比较强排序面板中的控件
        /// </summary>
        /// <param name="comparer">比较控件时使用的比较器</param>
        /// <returns>该面板控件。</returns>
        public StackPanel Sort(IComparer<Control> comparer)
        {
            Children.Sort(comparer);
            return this;
        }

        #endregion


        //------------------------------------------------------
        //
        //  Protected Methods
        //
        //------------------------------------------------------

        #region Protected Methods

        /// <inheritdoc/>
        protected override Rect ArrangeOverride(Rect availableRect, Control[] children)
        {
            float currentX = availableRect.x;
            float currentY = availableRect.y;

            if (Orientation is Orientation.Horizontal)
            {
                for (int i = 0; i < children.Length; i++)
                {
                    var child = children[i];

                    child.Arrange(new Rect(
                        currentX,
                        currentY,
                        child.DesiredSize.Width,
                        RenderSize.Height));

                    currentX += child.DesiredSize.Width;
                }
            }
            else
            {
                for (int i = 0; i < children.Length; i++)
                {
                    var child = children[i];

                    child.Arrange(new Rect(
                        currentX,
                        currentY,
                        RenderSize.Width,
                        child.DesiredSize.Height));

                    currentY += child.DesiredSize.Height;
                }
            }

            return new Rect(
                availableRect.x,
                availableRect.y,
                RenderSize.Width,
                RenderSize.Height);
        }

        /// <inheritdoc/>
        protected override Size MeasureOverride(Size availableSize, Control[] children)
        {
            float childrenWidth = 0f;
            float childrenHeight = 0f;

            float childWidth = (float)GetValue(ItemWidthProperty);
            float childHeight = (float)GetValue(ItemHeightProperty);

            if (childWidth <= 0f || childHeight <= 0f)
            {
                return Size.Empty;
            }

            childWidth = childWidth > 1f ? childWidth : childWidth * availableSize.Width;
            childHeight = childHeight > 1f ? childHeight : childHeight * availableSize.Height;

            Size childSize = new Size(childWidth, childHeight);
            Size childDesiredSize;

            if (Orientation is Orientation.Horizontal)
            {
                for (int i = 0; i < children.Length; i++)
                {
                    childDesiredSize = children[i].Measure(childSize);
                    childrenWidth += childDesiredSize.Width;
                    childrenHeight = Mathf.Max(childrenHeight, childDesiredSize.Height);
                }
            }
            else
            {
                for (int i = 0; i < children.Length; i++)
                {
                    childDesiredSize = children[i].Measure(childSize);
                    childrenHeight += childDesiredSize.Height;
                    childrenWidth = Mathf.Max(childrenWidth, childDesiredSize.Width);
                }
            }

            return new Size(childrenWidth, childrenHeight);
        }

        #endregion
    }
}
