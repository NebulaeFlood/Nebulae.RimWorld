using Nebulae.RimWorld.UI.Controls.Basic;
using Nebulae.RimWorld.UI.Core.Data;
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
        //  Public Properties
        //
        //------------------------------------------------------

        #region Public Properties

        #region ReferenceSize
        /// <summary>
        /// 获取或设置子控件参照尺寸
        /// </summary>
        /// <remarks>代表了在对子控件进行测量时，子控件能够获取到的尺寸。</remarks>
        public Size ReferenceSize
        {
            get { return (Size)GetValue(ReferenceSizeProperty); }
            set { SetValue(ReferenceSizeProperty, value.Normalize()); }
        }

        /// <summary>
        /// 标识 <see cref="ReferenceSize"/> 依赖属性
        /// </summary>
        public static readonly DependencyProperty ReferenceSizeProperty =
            DependencyProperty.Register(nameof(ReferenceSize), typeof(Size), typeof(StackPanel),
                new ControlPropertyMetadata(new Size(1f, 34f), ControlRelation.Measure));
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
        public StackPanel Set(IEnumerable<Control> controls)
        {
            Children.OverrideCollection(controls);
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

            if (Orientation is Orientation.Horizontal)
            {
                for (int i = 0; i < children.Length; i++)
                {
                    var child = children[i];

                    child.Arrange(new Rect(
                        currentX,
                        currentY,
                        child.DesiredSize.Width,
                        childrenSize.Height));

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
                        childrenSize.Width,
                        child.DesiredSize.Height));

                    currentY += child.DesiredSize.Height;
                }
            }

            return new Rect(
                availableRect.x,
                availableRect.y,
                childrenSize.Width,
                childrenSize.Height);
        }

        /// <inheritdoc/>
        protected override Size MeasureOverride(Size availableSize, Control[] children)
        {
            float childrenWidth = 0f;
            float childrenHeight = 0f;

            Size childReferenceSize = ReferenceSize.Resolve(availableSize);
            Size childDesiredSize;

            if (Orientation is Orientation.Horizontal)
            {
                for (int i = 0; i < children.Length; i++)
                {
                    childDesiredSize = children[i].Measure(childReferenceSize);
                    childrenWidth += childDesiredSize.Width;
                    childrenHeight = Mathf.Max(childrenHeight, childDesiredSize.Height);
                }
            }
            else
            {
                for (int i = 0; i < children.Length; i++)
                {
                    childDesiredSize = children[i].Measure(childReferenceSize);
                    childrenHeight += childDesiredSize.Height;
                    childrenWidth = Mathf.Max(childrenWidth, childDesiredSize.Width);
                }
            }

            return new Size(childrenWidth, childrenHeight);
        }

        #endregion
    }
}
