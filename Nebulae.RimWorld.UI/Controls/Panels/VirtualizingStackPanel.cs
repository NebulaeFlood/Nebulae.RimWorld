using Nebulae.RimWorld.UI.Controls.Basic;
using Nebulae.RimWorld.UI.Data;
using UnityEngine;

namespace Nebulae.RimWorld.UI.Controls.Panels
{
    /// <summary>
    /// 一个按照垂直或者水平方向排列子控件的支持虚拟化子控件的面板
    /// </summary>
    public class VirtualizingStackPanel : VirtualizingPanel
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
            DependencyProperty.Register(nameof(ChildMaxHeight), typeof(float), typeof(VirtualizingStackPanel),
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
            DependencyProperty.Register(nameof(ChildMaxWidth), typeof(float), typeof(VirtualizingStackPanel),
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
            DependencyProperty.Register(nameof(Orientation), typeof(Orientation), typeof(VirtualizingStackPanel),
                new ControlPropertyMetadata(Orientation.Vertical, ControlRelation.Measure));
        #endregion

        #endregion


        /// <summary>
        /// 初始化 <see cref="VirtualizingStackPanel"/> 的新实例
        /// </summary>
        public VirtualizingStackPanel()
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
        public VirtualizingStackPanel Append(Visual control)
        {
            Children.Add(control);
            return this;
        }

        /// <summary>
        /// 清除面板并重新设置面板中的控件
        /// </summary>
        /// <param name="controls">要添加的控件</param>
        /// <returns>该面板控件</returns>
        public VirtualizingStackPanel Set(params Visual[] controls)
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
            float currentX = availableRect.x;
            float currentY = availableRect.y;

            var children = FilteredChildren;

            Size childDesiredSize;
            if (Orientation is Orientation.Horizontal)
            {
                for (int i = 0; i < children.Length; i++)
                {
                    var child = children[i];
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
                }
            }
            else
            {
                for (int i = 0; i < children.Length; i++)
                {
                    var child = children[i];
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
            float childMaxHeight = ChildMaxHeight;
            float childMaxWidth = ChildMaxWidth;
            float childrenWidth = 0f;
            float childrenHeight = 0f;

            var children = FilteredChildren;

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

                for (int i = 0; i < children.Length; i++)
                {
                    var child = children[i];
                    childDesiredSize = child.Measure(childAvailableSize);
                    childrenWidth += childDesiredSize.Width;
                    childrenHeight = Mathf.Max(childrenHeight, childDesiredSize.Height);
                }
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

                for (int i = 0; i < children.Length; i++)
                {
                    var child = children[i];
                    childDesiredSize = child.Measure(childAvailableSize);
                    childrenHeight += childDesiredSize.Height;
                    childrenWidth = Mathf.Max(childrenWidth, childDesiredSize.Width);
                }
            }
            return new Size(childrenWidth, childrenHeight);
        }

        #endregion
    }
}
