using Nebulae.RimWorld.UI.Controls.Basic;
using Nebulae.RimWorld.UI.Core.Data;
using System.Collections.Generic;
using UnityEngine;

namespace Nebulae.RimWorld.UI.Controls.Panels
{
    /// <summary>
    /// 一个将子控件排列成数行或数列的面板
    /// </summary>
    public class WrapPanel : Panel
    {
        //------------------------------------------------------
        //
        //  Public Properties
        //
        //------------------------------------------------------

        #region Public Properties

        #region ItemHeight
        /// <summary>
        /// 获取或设置分配给子控件的高度
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
            DependencyProperty.Register(nameof(ItemHeight), typeof(float), typeof(WrapPanel),
                new ControlPropertyMetadata(40f, ControlRelation.Measure));
        #endregion

        #region ItemWidth
        /// <summary>
        /// 获取或设置分配给子控件的宽度
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
            DependencyProperty.Register(nameof(ItemWidth), typeof(float), typeof(WrapPanel),
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
        /// 标识 <see cref="Orientation"/> 依赖属性
        /// </summary>
        public static readonly DependencyProperty OrientationProperty =
            DependencyProperty.Register(nameof(Orientation), typeof(Orientation), typeof(WrapPanel),
                new ControlPropertyMetadata(Orientation.Vertical, ControlRelation.Measure));
        #endregion

        #endregion


        /// <summary>
        /// 初始化 <see cref="WrapPanel"/> 的新实例
        /// </summary>
        public WrapPanel() { }

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
        public WrapPanel Append(Control control)
        {
            Children.Add(control);
            return this;
        }

        /// <summary>
        /// 清除面板并重新设置面板中的控件
        /// </summary>
        /// <param name="controls">要添加的控件</param>
        /// <returns>该面板控件</returns>
        public WrapPanel Set(IEnumerable<Control> controls)
        {
            Children.OverrideCollection(controls);
            return this;
        }

        /// <summary>
        /// 清除面板并重新设置面板中的控件
        /// </summary>
        /// <param name="controls">要添加的控件</param>
        /// <returns>该面板控件</returns>
        public WrapPanel Set(params Control[] controls)
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
        /// 将控件插入或移动到面板中的指定控件之前
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
            Size childrenSize = RenderSize;

            float currentX = availableRect.x;
            float currentY = availableRect.y;

            float limitation;

            if (Orientation is Orientation.Horizontal)
            {
                limitation = currentX + childrenSize.Width;

                for (int i = 0; i < children.Length; i++)
                {
                    var child = children[i];

                ArrangeStart:
                    if (currentX + _childMaxSize.Width > limitation)
                    {
                        currentX = availableRect.x;
                        currentY += _childMaxSize.Height;   // 换行

                        goto ArrangeStart;  // 重新排列该控件
                    }
                    else
                    {
                        child.Arrange(new Rect(currentX, currentY, _childMaxSize.Width, _childMaxSize.Height));

                        currentX += _childMaxSize.Width;
                    }
                }
            }
            else
            {
                limitation = currentY + childrenSize.Height;

                for (int i = 0; i < children.Length; i++)
                {
                    var child = children[i];

                ArrangeStart:
                    if (currentY + _childMaxSize.Height > limitation)
                    {
                        currentY = availableRect.y;
                        currentX += _childMaxSize.Width; // 换列

                        goto ArrangeStart;  // 重新排列该控件
                    }
                    else
                    {
                        child.Arrange(new Rect(currentX, currentY, _childMaxSize.Width, _childMaxSize.Height));

                        currentY += _childMaxSize.Height;
                    }
                }
            }

            return new Rect(availableRect.x, availableRect.y, childrenSize.Width, childrenSize.Height);
        }

        /// <inheritdoc/>
        protected override Size MeasureOverride(Size availableSize, Control[] children)
        {
            float childMaxHeight = ItemHeight;
            float childMaxWidth = ItemWidth;

            int childrenColumnCount;
            int childrenRowCount;

            if (Orientation is Orientation.Horizontal)
            {
                childMaxWidth = childMaxWidth > 1f
                        ? Mathf.Min(childMaxWidth, availableSize.Width)
                        : childMaxWidth * availableSize.Width;

                // 限制子控件最大宽度不超过面板宽度
                childMaxHeight = childMaxHeight > 1f
                        ? childMaxHeight
                        : childMaxHeight * availableSize.Height;

                childrenColumnCount = (int)Mathf.Max(1f, availableSize.Width / childMaxWidth);
                childrenRowCount = children.Length / childrenColumnCount;

                if (children.Length % childrenColumnCount > 0)
                {
                    childrenRowCount++;
                }
            }
            else
            {
                childMaxWidth = childMaxWidth > 1f
                    ? childMaxWidth
                    : childMaxWidth * availableSize.Width;

                // 限制子控件最大高度不超过面板高度
                childMaxHeight = childMaxHeight > 1f
                    ? Mathf.Min(childMaxHeight, availableSize.Height)
                    : childMaxHeight * availableSize.Height;

                childrenRowCount = (int)Mathf.Max(1f, availableSize.Height / childMaxHeight);
                childrenColumnCount = children.Length / childrenRowCount;

                if (children.Length % childrenRowCount > 0)
                {
                    childrenColumnCount++;
                }
            }

            _childMaxSize = new Size(childMaxWidth, childMaxHeight);

            for (int i = 0; i < children.Length; i++)
            {
                children[i].Measure(_childMaxSize);
            }

            return new Size(
                childMaxWidth * childrenColumnCount,
                childMaxHeight * childrenRowCount);
        }

        #endregion


        private Size _childMaxSize = Size.Empty;
    }
}
