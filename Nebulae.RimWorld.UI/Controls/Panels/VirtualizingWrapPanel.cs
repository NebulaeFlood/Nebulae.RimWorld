using Nebulae.RimWorld.UI.Controls.Basic;
using Nebulae.RimWorld.UI.Data;
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
        /// 标识 <see cref="ItemHeight"/> 依赖属性。
        /// </summary>
        public static readonly DependencyProperty ItemHeightProperty =
            DependencyProperty.Register(nameof(ItemHeight), typeof(float), typeof(VirtualizingWrapPanel),
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
        /// 标识 <see cref="ItemWidth"/> 依赖属性。
        /// </summary>
        public static readonly DependencyProperty ItemWidthProperty =
            DependencyProperty.Register(nameof(ItemWidth), typeof(float), typeof(VirtualizingWrapPanel),
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
        /// <returns>该面板控件</returns>
        public VirtualizingWrapPanel Append(Visual control)
        {
            Children.Add(control);
            return this;
        }

        /// <summary>
        /// 清除面板并重新设置面板中的控件
        /// </summary>
        /// <param name="controls">要添加的控件</param>
        /// <returns>该面板控件</returns>
        public VirtualizingWrapPanel Set(params Visual[] controls)
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
        public bool Insert(Visual control, Visual index)
        {
            return Children.Insert(index, control);
        }

        /// <summary>
        /// 移除面板中的指定控件
        /// </summary>
        /// <param name="control">要移除的控件</param>
        /// <returns>若移除了指定控件，返回 <see langword="true"/>；反之则返回 <see langword="false"/>。</returns>
        public bool Remove(Visual control)
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

            float childMaxHeight = ItemHeight;
            float childMaxWidth = ItemWidth;

            float currentX = 0f;
            float currentY = 0f;

            var children = FilteredChildren;

            Size childAvailableSize;

            if (Orientation is Orientation.Horizontal)
            {
                childAvailableSize = new Size(
                    childMaxWidth > 1f
                        ? Mathf.Min(childMaxWidth, availableRect.width)
                        : childMaxWidth * availableRect.width,
                    childMaxHeight > 1f
                        ? childMaxHeight
                        : childMaxHeight * availableRect.width);

                for (int i = 0; i < children.Length; i++)
                {
                    var child = children[i];
                    if (child.RenderSize > Size.Empty)
                    {

                    ArrangeStart:
                        if (currentX + childAvailableSize.Width > childrenSize.Width)
                        {
                            currentX = 0f;
                            currentY += childAvailableSize.Height;   // 换行

                            goto ArrangeStart;  // 重新排列该控件
                        }
                        else
                        {
                            child.Arrange(new Rect(
                                currentX + availableRect.x,
                                currentY + availableRect.y,
                                childAvailableSize.Width,
                                childAvailableSize.Height));

                            currentX += childAvailableSize.Width;
                        }
                    }
                }
            }
            else
            {
                childAvailableSize = new Size(
                    childMaxWidth > 1f
                        ? childMaxWidth
                        : childMaxWidth * availableRect.width,
                    childMaxHeight > 1f
                        ? Mathf.Min(childMaxHeight, availableRect.height)
                        : childMaxHeight * availableRect.height);

                for (int i = 0; i < children.Length; i++)
                {
                    var child = children[i];
                    if (child.RenderSize > Size.Empty)
                    {

                    ArrangeStart:
                        if (currentY + childAvailableSize.Height > childrenSize.Height)
                        {
                            currentY = 0f;
                            currentX += childAvailableSize.Width; // 换列

                            goto ArrangeStart;  // 重新排列该控件
                        }
                        else
                        {
                            child.Arrange(new Rect(
                                currentX + availableRect.x,
                                currentY + availableRect.y,
                                childAvailableSize.Width,
                                childAvailableSize.Height));

                            currentY += childAvailableSize.Height;
                        }
                    }
                }
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
            Visual[] filteredChildren = FilteredChildren;

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
                childrenRowCount = filteredChildren.Length / childrenColumnCount;

                if (filteredChildren.Length % childrenColumnCount > 0)
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
                childrenColumnCount = filteredChildren.Length / childrenRowCount;

                if (filteredChildren.Length % childrenRowCount > 0)
                {
                    childrenColumnCount++;
                }
            }

            Size childMaxSize = new Size(childMaxWidth, childMaxHeight);

            var children = FilteredChildren;

            for (int i = 0; i < children.Length; i++)
            {
                children[i].Measure(childMaxSize);
            }

            return new Size(
                childMaxWidth * childrenColumnCount,
                childMaxHeight * childrenRowCount);
        }

        #endregion
    }
}