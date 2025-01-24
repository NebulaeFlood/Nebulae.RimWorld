using Nebulae.RimWorld.UI.Data;
using System;
using UnityEngine;

namespace Nebulae.RimWorld.UI.Controls.Panels
{
    /// <summary>
    /// 选项卡面板
    /// </summary>
    public class TabPanel : Panel
    {
        /// <summary>
        /// 选项卡之间重叠的宽度
        /// </summary>
        public const float IntersectedWidth = 8f;

        /// <summary>
        /// 每行选项卡之间的间隔
        /// </summary>
        public const float RowMargin = 4f;


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
            DependencyProperty.Register(nameof(ItemHeight), typeof(float), typeof(TabPanel),
                new ControlPropertyMetadata(30f, ControlRelation.Measure));
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
            DependencyProperty.Register(nameof(ItemWidth), typeof(float), typeof(TabPanel),
                new ControlPropertyMetadata(190f, ControlRelation.Measure));
        #endregion

        #endregion


        /// <summary>
        /// 初始化 <see cref="TabPanel"/> 的新实例
        /// </summary>
        public TabPanel()
        {
        }


        //------------------------------------------------------
        //
        //  Public Method
        //
        //------------------------------------------------------

        #region Public Method

        /// <summary>
        /// 向面板末尾添加一个选项卡
        /// </summary>
        /// <param name="tabItem">要添加的选项卡</param>
        /// <returns>该面板控件</returns>
        public TabPanel Append(TabItem tabItem)
        {
            Children.Add(tabItem);
            return this;
        }

        /// <summary>
        /// 清除面板并重新设置面板中的选项卡
        /// </summary>
        /// <param name="tabItems">要添加的选项卡</param>
        /// <returns>该面板控件</returns>
        public TabPanel Set(params Control[] tabItems)
        {
            Children.OverrideCollection(tabItems);
            return this;
        }

        /// <summary>
        /// 清除面板中的所有选项卡
        /// </summary>
        public void Clear()
        {
            Children.Clear();
        }

        /// <summary>
        /// 移除面板中的指定选项卡
        /// </summary>
        /// <param name="tabItem">要移除的选项卡</param>
        /// <returns>若移除了指定选项卡，返回 <see langword="true"/>；反之则返回 <see langword="false"/>。</returns>
        public bool Remove(Control tabItem)
        {
            return Children.Remove(tabItem);
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

            Size childAvailableSize = new Size(
                childMaxWidth > 1f
                    ? Mathf.Min(childMaxWidth, availableRect.width)
                    : childMaxWidth * availableRect.width,
                childMaxHeight > 1f
                    ? childMaxHeight
                    : childMaxHeight * availableRect.width);

            Array.ForEach(FilteredChildren, child =>
            {
                if (child.RenderSize > Size.Empty)
                {

                ArrangeStart:
                    if (currentX + childAvailableSize.Width > childrenSize.Width)
                    {
                        currentX = 0f;
                        currentY += childAvailableSize.Height + RowMargin;   // 换行

                        goto ArrangeStart;  // 重新排列该控件
                    }
                    else
                    {
                        if (currentX > 0f)
                        {
                            currentX -= IntersectedWidth;
                        }

                        child.Arrange(new Rect(
                            currentX + availableRect.x,
                            currentY + availableRect.y,
                            childAvailableSize.Width,
                            childAvailableSize.Height));

                        currentX += childAvailableSize.Width;
                    }
                }
            });

            return new Rect(
                availableRect.x,
                availableRect.y,
                childrenSize.Width,
                childrenSize.Height);
        }

        /// <inheritdoc/>
        protected override bool IsDrawable(Control child)
        {
            return base.IsDrawable(child)
                && child is TabItem tabItem
                && !tabItem.Selected;
        }

        /// <inheritdoc/>
        protected override Size MeasureOverride(Size availableSize)
        {
            Control[] filteredChildren = FilteredChildren;

            float childMaxHeight = ItemHeight;
            float childMaxWidth = ItemWidth;

            // 限制子控件最大宽度不超过面板宽度
            childMaxWidth = childMaxWidth > 1f
                    ? Mathf.Min(childMaxWidth, availableSize.Width)
                    : childMaxWidth * availableSize.Width;

            childMaxHeight = childMaxHeight > 1f
                    ? childMaxHeight
                    : childMaxHeight * availableSize.Height;

            int childrenColumnCount;
            int childrenRowCount;

            float avaialbeWidth = availableSize.Width - childMaxWidth;

            if (avaialbeWidth > childMaxWidth - IntersectedWidth)
            {
                childrenColumnCount = 1 + (int)(avaialbeWidth / (childMaxWidth - IntersectedWidth));
            }
            else
            {
                childrenColumnCount = 1;
            }

            childrenRowCount = filteredChildren.Length / childrenColumnCount;

            if (filteredChildren.Length % childrenColumnCount > 0)
            {
                childrenRowCount++;
            }

            Size childMaxSize = new Size(childMaxWidth, childMaxHeight);

            Array.ForEach(filteredChildren, child => child.Measure(childMaxSize));

            return new Size(
                childMaxWidth * childrenColumnCount,
                (childMaxHeight + 2f) * childrenRowCount);
        }

        #endregion
    }
}
