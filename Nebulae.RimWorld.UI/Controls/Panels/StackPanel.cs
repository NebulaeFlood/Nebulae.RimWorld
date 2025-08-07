using Nebulae.RimWorld.UI.Controls.Basic;
using Nebulae.RimWorld.UI.Core.Data;
using Nebulae.RimWorld.UI.Utilities;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Nebulae.RimWorld.UI.Controls.Panels
{
    /// <summary>
    /// 按照垂直或者水平方向排列子控件的面板
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
                new ControlPropertyMetadata(1f, CoerceItemSize, ControlRelation.Measure));
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
                new ControlPropertyMetadata(34f, CoerceItemSize, ControlRelation.Measure));
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

        #region Spacing
        /// <summary>
        /// 获取或设置 <see cref="StackPanel"/> 子控件的统一间距
        /// </summary>
        public float Spacing
        {
            get { return (float)GetValue(SpacingProperty); }
            set { SetValue(SpacingProperty, value); }
        }

        /// <summary>
        /// 标识 <see cref="Spacing"/> 依赖属性
        /// </summary>
        public static readonly DependencyProperty SpacingProperty =
            DependencyProperty.Register(nameof(Spacing), typeof(float), typeof(StackPanel),
                new ControlPropertyMetadata(0f, CoerceSpacing, ControlRelation.Measure));

        private static object CoerceSpacing(DependencyObject d, object baseValue)
        {
            return UIUtility.Format((float)baseValue);
        }
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

            float spacing = (float)GetValue(SpacingProperty);

            if (GetValue(OrientationProperty) is Orientation.Horizontal)
            {
                for (int i = 0; i < children.Length; i++)
                {
                    var child = children[i];

                    child.Arrange(new Rect(
                        currentX,
                        currentY,
                        child.DesiredSize.Width,
                        RenderSize.Height));

                    currentX += child.DesiredSize.Width + spacing;
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

                    currentY += child.DesiredSize.Height + spacing;
                }
            }

            return new Rect(availableRect.x, availableRect.y, RenderSize.Width, RenderSize.Height);
        }

        /// <inheritdoc/>
        protected override Size MeasureOverride(Size availableSize, Control[] children)
        {
            int childrenCount = children.Length;

            float renderWidth = 0f;
            float renderHeight = 0f;

            float childWidth = (float)GetValue(ItemWidthProperty);
            float childHeight = (float)GetValue(ItemHeightProperty);

            if (childWidth is 0f || childHeight is 0f)
            {
                return Size.Empty;
            }

            childWidth = childWidth > 1f ? childWidth : childWidth * availableSize.Width;
            childHeight = childHeight > 1f ? childHeight : childHeight * availableSize.Height;

            Size childSize = new Size(childWidth, childHeight);
            Size childDesiredSize;

            if (GetValue(OrientationProperty) is Orientation.Horizontal)
            {
                for (int i = 0; i < childrenCount; i++)
                {
                    childDesiredSize = children[i].Measure(childSize);

                    renderWidth += childDesiredSize.Width;
                    renderHeight = Mathf.Max(renderHeight, childDesiredSize.Height);
                }
            }
            else
            {
                for (int i = 0; i < childrenCount; i++)
                {
                    childDesiredSize = children[i].Measure(childSize);

                    renderHeight += childDesiredSize.Height;
                    renderWidth = Mathf.Max(renderWidth, childDesiredSize.Width);
                }
            }

            float spacing = (float)GetValue(SpacingProperty);

            if (spacing > 0f && childrenCount > 1)
            {
                renderHeight += spacing * (childrenCount - 1);
            }

            return new Size(renderWidth, renderHeight);
        }

        #endregion


        private static object CoerceItemSize(DependencyObject d, object baseValue)
        {
            return UIUtility.FormatProportion((float)baseValue);
        }
    }
}
