using Nebulae.RimWorld.UI.Controls.Basic;
using Nebulae.RimWorld.UI.Core.Data;
using Nebulae.RimWorld.UI.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;
using Verse;

namespace Nebulae.RimWorld.UI.Controls.Panels
{
    /// <summary>
    /// 按照等宽不等高或者等高不等宽排列子控件的面板
    /// </summary>
    public sealed class WaterfallPanel : Panel
    {
        /// <summary>
        /// 表示自动尺寸
        /// </summary>
        public const float Auto = Definition.Auto;


        //------------------------------------------------------
        //
        //  Dependency Properties
        //
        //------------------------------------------------------

        #region Dependency Properties

        #region Orientation
        /// <summary>
        /// 获取或设置 <see cref="WaterfallPanel"/> 排列子控件的方向
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
            DependencyProperty.Register(nameof(Orientation), typeof(Orientation), typeof(WaterfallPanel),
                new ControlPropertyMetadata(Orientation.Horizontal, ControlRelation.Measure));
        #endregion

        #region Spacing
        /// <summary>
        /// 获取或设置 <see cref="WaterfallPanel"/> 子控件的统一间距
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
            DependencyProperty.Register(nameof(Spacing), typeof(float), typeof(WaterfallPanel),
                new ControlPropertyMetadata(4f, CoerceSpacing, ControlRelation.Measure));

        private static object CoerceSpacing(DependencyObject d, object baseValue)
        {
            return UIUtility.Format((float)baseValue);
        }
        #endregion

        #region UniformSize
        /// <summary>
        /// 获取或设置 <see cref="WaterfallPanel"/> 分配给子控件的统一尺寸
        /// </summary>
        public float UniformSize
        {
            get { return (float)GetValue(UniformSizeProperty); }
            set { SetValue(UniformSizeProperty, value); }
        }

        /// <summary>
        /// 标识 <see cref="UniformSize"/> 依赖属性
        /// </summary>
        public static readonly DependencyProperty UniformSizeProperty =
            DependencyProperty.Register(nameof(UniformSize), typeof(float), typeof(WaterfallPanel),
                new ControlPropertyMetadata(30f, CoerceUniformSize, ControlRelation.Measure));

        private static object CoerceUniformSize(DependencyObject d, object baseValue)
        {
            return UIUtility.FormatProportion((float)baseValue);
        }
        #endregion

        #endregion


        /// <summary>
        /// 初始化 <see cref="WaterfallPanel"/> 的新实例
        /// </summary>
        public WaterfallPanel() { }


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
        public WaterfallPanel Append(Control control)
        {
            Children.Add(control);
            return this;
        }

        /// <summary>
        /// 清除面板并重新设置面板中的控件
        /// </summary>
        /// <param name="controls">要添加的控件</param>
        /// <returns>该面板控件。</returns>
        public WaterfallPanel Set(IEnumerable<Control> controls)
        {
            Children.OverrideCollection(controls);
            return this;
        }

        /// <summary>
        /// 清除面板并重新设置面板中的控件
        /// </summary>
        /// <param name="controls">要添加的控件</param>
        /// <returns>该面板控件。</returns>
        public WaterfallPanel Set(params Control[] controls)
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
        public WaterfallPanel Sort(Comparison<Control> comparison)
        {
            Children.Sort(comparison);
            return this;
        }

        /// <summary>
        /// 使用指定的比较强排序面板中的控件
        /// </summary>
        /// <param name="comparer">比较控件时使用的比较器</param>
        /// <returns>该面板控件。</returns>
        public WaterfallPanel Sort(IComparer<Control> comparer)
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
            float x = availableRect.x;
            float y = availableRect.y;

            if (GetValue(OrientationProperty) is Orientation.Horizontal)
            {
                ArrangeHorizontal(x, y, RenderSize.Width, children, _uniformSize, (float)GetValue(SpacingProperty));
            }
            else
            {
                ArrangeVertical(x, y, RenderSize.Height, children, _uniformSize, (float)GetValue(SpacingProperty));
            }

            return new Rect(x, y, RenderSize.Width, RenderSize.Height);
        }

        /// <inheritdoc/>
        protected override Size MeasureOverride(Size availableSize, Control[] children)
        {
            _uniformSize = (float)GetValue(UniformSizeProperty);

            if (_uniformSize <= 1f)
            {
                return Size.Empty;
            }

            if (GetValue(OrientationProperty) is Orientation.Horizontal)
            {
                _uniformSize = _uniformSize > 1f
                    ? MathF.Min(_uniformSize, availableSize.Width)
                    : _uniformSize * availableSize.Width;

                return MeasureHorizontal(availableSize.Width, children, _uniformSize, (float)GetValue(SpacingProperty));
            }
            else
            {
                _uniformSize = _uniformSize > 1f
                    ? MathF.Min(_uniformSize, availableSize.Height)
                    : _uniformSize * availableSize.Height;

                return MeasureVertical(availableSize.Height, children, _uniformSize, (float)GetValue(SpacingProperty));
            }
        }

        #endregion


        //------------------------------------------------------
        //
        //  Private Static Methods
        //
        //------------------------------------------------------

        #region Private Static Methods

        private static bool AllowStack(float childSize, float availableSize, float spacing)
        {
            return childSize <= availableSize / 2f - spacing;
        }

        private static void ArrangeUniformHeightHorizontal(float x, float y, float availableWidth, Control[] children, float uniformHeight, float spacing)
        {
            int childrenCount = children.Length;

            float currentX = x;
            float currentY = y;

            float rightBound = x + availableWidth;

            if (spacing > 0f && childrenCount > 1)
            {
                for (int i = 0; i < childrenCount; i++)
                {
                    var child = children[i];

                    if (currentX > x)
                    {
                        if (currentX + spacing + child.DesiredSize.Width > rightBound)
                        {
                            currentX = x;
                            currentY += uniformHeight + spacing;
                        }
                        else
                        {
                            currentX += spacing;
                        }
                    }
                    else if (child.DesiredSize.Width > availableWidth)
                    {
                        rightBound = x + child.DesiredSize.Width;
                    }

                    child.Arrange(new Rect(currentX, currentY, child.DesiredSize.Width, uniformHeight));
                    currentX += child.DesiredSize.Width;
                }
            }
            else
            {
                for (int i = 0; i < childrenCount; i++)
                {
                    var child = children[i];

                    if (currentX > x)
                    {
                        if (currentX + child.DesiredSize.Width > rightBound)
                        {
                            currentX = x;
                            currentY += uniformHeight;
                        }
                    }
                    else if (child.DesiredSize.Width > availableWidth)
                    {
                        rightBound = x + child.DesiredSize.Width;
                    }

                    child.Arrange(new Rect(currentX, currentY, child.DesiredSize.Width, uniformHeight));
                    currentX += child.DesiredSize.Width;
                }
            }
        }

        private static void ArrangeUniformHeightVertical(float x, float y, Control[] children, float uniformHeight, float spacing, int rowCount)
        {
            int childrenCount = children.Length;

            float currentX = x;
            float currentY = y;

            if (spacing > 0f && childrenCount > 1)
            {
                if (rowCount > 1)
                {
                    int rowIndex = 0;

                    float maxColumnWidth = 0f;

                    for (int i = 0; i < childrenCount; i++)
                    {
                        if (rowIndex >= rowCount)
                        {
                            rowIndex = 0;

                            currentX += maxColumnWidth + spacing;
                            currentY = y;

                            maxColumnWidth = 0f;
                        }

                        var child = children[i];
                        child.Arrange(new Rect(currentX, currentY, child.DesiredSize.Width, uniformHeight));

                        rowIndex++;
                        currentY += uniformHeight + spacing;
                        maxColumnWidth = MathF.Max(maxColumnWidth, child.DesiredSize.Width);
                    }
                }
                else
                {
                    for (int i = 0; i < childrenCount; i++)
                    {
                        var child = children[i];
                        child.Arrange(new Rect(currentX, currentY, child.DesiredSize.Width, uniformHeight));

                        currentX += child.DesiredSize.Width + spacing;
                    }
                }
            }
            else
            {
                if (rowCount > 1)
                {
                    int rowIndex = 0;

                    float maxColumnWidth = 0f;

                    for (int i = 0; i < childrenCount; i++)
                    {
                        if (rowIndex >= rowCount)
                        {
                            currentX = x;

                            rowIndex = 0;
                            maxColumnWidth = 0f;
                        }

                        var child = children[i];
                        child.Arrange(new Rect(currentX, currentY, child.DesiredSize.Width, uniformHeight));

                        currentY += maxColumnWidth;
                    }
                }
                else
                {
                    for (int i = childrenCount - 1; i >= 0; i--)
                    {
                        var child = children[i];
                        child.Arrange(new Rect(currentX, currentY, child.DesiredSize.Width, uniformHeight));
                        currentX += child.DesiredSize.Width;
                    }
                }
            }
        }

        private static void ArrangeUniformWidthHorizontal(float x, float y, Control[] children, float uniformWidth, float spacing, int columnCount)
        {
            int childrenCount = children.Length;

            float currentX = x;
            float currentY = y;

            if (spacing > 0f && childrenCount > 1)
            {
                if (columnCount > 1)
                {
                    int columnIndex = 0;

                    float maxRowHeight = 0f;

                    for (int i = 0; i < childrenCount; i++)
                    {
                        if (columnIndex >= columnCount)
                        {
                            columnIndex = 0;

                            currentX = x;
                            currentY += maxRowHeight + spacing;

                            maxRowHeight = 0f;
                        }

                        var child = children[i];
                        child.Arrange(new Rect(currentX, currentY, uniformWidth, child.DesiredSize.Height));

                        columnIndex++;
                        currentX += uniformWidth + spacing;
                        maxRowHeight = MathF.Max(maxRowHeight, child.DesiredSize.Height);
                    }
                }
                else
                {
                    for (int i = 0; i < childrenCount; i++)
                    {
                        var child = children[i];
                        child.Arrange(new Rect(currentX, currentY, uniformWidth, child.DesiredSize.Height));

                        currentY += child.DesiredSize.Height + spacing;
                    }
                }
            }
            else
            {
                if (columnCount > 1)
                {
                    int columnIndex = 0;

                    float maxRowHeight = 0f;

                    for (int i = 0; i < childrenCount; i++)
                    {
                        if (columnIndex >= columnCount)
                        {
                            currentX = x;

                            columnIndex = 0;
                            maxRowHeight = 0f;
                        }

                        var child = children[i];
                        child.Arrange(new Rect(currentX, currentY, uniformWidth, child.DesiredSize.Height));

                        currentY += maxRowHeight;
                    }
                }
                else
                {
                    for (int i = childrenCount - 1; i >= 0; i--)
                    {
                        var child = children[i];
                        child.Arrange(new Rect(currentX, currentY, uniformWidth, child.DesiredSize.Height));
                        currentY += child.DesiredSize.Height;
                    }
                }
            }
        }

        private static void ArrangeUniformWidthVertical(float x, float y, float availableHeight, Control[] children, float uniformWidth, float spacing)
        {
            int childrenCount = children.Length;

            float currentX = x;
            float currentY = y;

            float bottomBound = y + availableHeight;

            if (spacing > 0f && childrenCount > 1)
            {
                for (int i = 0; i < childrenCount; i++)
                {
                    var child = children[i];

                    if (currentY > y)
                    {
                        if (currentY + spacing + child.DesiredSize.Height > bottomBound)
                        {
                            currentX += uniformWidth + spacing;
                            currentY = y;
                        }
                        else
                        {
                            currentY += spacing;
                        }
                    }
                    else if (child.DesiredSize.Height > availableHeight)
                    {
                        bottomBound = y + child.DesiredSize.Height;
                    }

                    child.Arrange(new Rect(currentX, currentY, uniformWidth, child.DesiredSize.Height));
                    currentY += child.DesiredSize.Height;
                }
            }
            else
            {
                for (int i = 0; i < childrenCount; i++)
                {
                    var child = children[i];

                    if (currentY > y)
                    {
                        if (currentY + child.DesiredSize.Height > bottomBound)
                        {
                            currentX += uniformWidth;
                            currentY = y;
                        }
                    }
                    else if (child.DesiredSize.Height > availableHeight)
                    {
                        bottomBound = y + child.DesiredSize.Height;
                    }

                    child.Arrange(new Rect(currentX, currentY, uniformWidth, child.DesiredSize.Height));
                    currentY += child.DesiredSize.Height;
                }
            }
        }

        private static void ArrangeHorizontal(float x, float y, float availableWidth, Control[] children, float uniformHeight, float spacing)
        {
            int childrenCount = children.Length;

            float currentX = x;
            float currentY = y;

            float rightBound = x + availableWidth;

            if (spacing > 0f && childrenCount > 1)
            {
                for (int i = 0; i < childrenCount; i++)
                {
                    var child = children[i];
                    var childWidth = MathF.Min(availableWidth, child.DesiredSize.Width);

                    if (currentX > x)
                    {
                        if (currentX + spacing + childWidth > rightBound)
                        {
                            currentX = x;
                            currentY += uniformHeight + spacing;
                        }
                        else
                        {
                            currentX += spacing;
                        }
                    }

                    child.Arrange(new Rect(currentX, currentY, child.DesiredSize.Width, uniformHeight));
                    currentX += childWidth;
                }
            }
            else
            {
                for (int i = 0; i < childrenCount; i++)
                {
                    var child = children[i];
                    var childWidth = MathF.Min(availableWidth, child.DesiredSize.Width);

                    if (currentX > x && currentX + childWidth > rightBound)
                    {
                        currentX = x;
                        currentY += uniformHeight;
                    }

                    child.Arrange(new Rect(currentX, currentY, child.DesiredSize.Width, uniformHeight));
                    currentX += childWidth;
                }
            }
        }

        private static void ArrangeVertical(float x, float y, float availableHeight, Control[] children, float uniformWidth, float spacing)
        {
            int childrenCount = children.Length;

            float currentX = x;
            float currentY = y;

            float bottomBound = y + availableHeight;

            if (spacing > 0f && childrenCount > 1)
            {
                for (int i = 0; i < childrenCount; i++)
                {
                    var child = children[i];
                    var childHeight = MathF.Min(availableHeight, child.DesiredSize.Height);

                    if (currentY > y)
                    {
                        if (currentY + spacing + childHeight > bottomBound)
                        {
                            currentX += uniformWidth + spacing;
                            currentY = y;
                        }
                        else
                        {
                            currentY += spacing;
                        }
                    }

                    child.Arrange(new Rect(currentX, currentY, uniformWidth, child.DesiredSize.Height));
                    currentY += childHeight;
                }
            }
            else
            {
                for (int i = 0; i < childrenCount; i++)
                {
                    var child = children[i];
                    var childHeight = MathF.Min(availableHeight, child.DesiredSize.Height);

                    if (currentY > y && currentY + childHeight > bottomBound)
                    {
                        currentX += uniformWidth;
                        currentY = y;
                    }

                    child.Arrange(new Rect(currentX, currentY, uniformWidth, child.DesiredSize.Height));
                    currentY += childHeight;
                }
            }
        }

        private static Size MeasureHorizontal(float availableWidth, Control[] children, float uniformHeight, float spacing)
        {
            int childrenCount = children.Length;
            var childAvailableSize = new Size(availableWidth, uniformHeight);
            var childReferenceSize = new Size(Auto, uniformHeight);

            float currentWidth = 0f;
            float renderHeight = 0f;

            if (spacing > 0f && childrenCount > 1)
            {
                int rowCount = 0;

                for (int i = 0; i < childrenCount; i++)
                {
                    var child = children[i];
                    var childWidth = child.Measure(childReferenceSize).Width;

                    if (childWidth > availableWidth)
                    {
                        // 限制子控件宽度
                        childWidth = MathF.Min(availableWidth, child.Measure(childAvailableSize).Width);
                    }

                    if (currentWidth > 0f)
                    {
                        var finalWidth = currentWidth + spacing + childWidth;

                        if (finalWidth > availableWidth)
                        {
                            currentWidth = childWidth;
                            renderHeight += uniformHeight;
                            rowCount++;
                        }
                        else
                        {
                            currentWidth = finalWidth;
                        }
                    }
                    else
                    {
                        currentWidth = childWidth;
                    }
                }

                if (currentWidth > 0f)
                {
                    renderHeight += uniformHeight; // 计算最后一行
                    rowCount++;
                }

                renderHeight += spacing * (rowCount - 1);
            }
            else
            {
                for (int i = 0; i < childrenCount; i++)
                {
                    var child = children[i];
                    var childWidth = child.Measure(childReferenceSize).Width;

                    if (childWidth > availableWidth)
                    {
                        // 限制子控件宽度
                        childWidth = MathF.Min(availableWidth, child.Measure(childAvailableSize).Width);
                    }

                    if (currentWidth > 0f)
                    {
                        var finalWidth = currentWidth + childWidth;

                        if (finalWidth > availableWidth)
                        {
                            currentWidth = childWidth;
                            renderHeight += uniformHeight;
                        }
                        else
                        {
                            currentWidth = finalWidth;
                        }
                    }
                    else
                    {
                        currentWidth = childWidth;
                    }
                }

                if (currentWidth > 0f)
                {
                    renderHeight += uniformHeight; // 计算最后一行
                }
            }

            return new Size(availableWidth, renderHeight);
        }

        private static Size MeasureVertical(float availableHeight, Control[] children, float uniformWidth, float spacing)
        {
            int childrenCount = children.Length;
            var childAvailableSize = new Size(uniformWidth, availableHeight);
            var childReferenceSize = new Size(uniformWidth, Auto);

            float currentHeight = 0f;
            float renderWidth = 0f;

            if (spacing > 0f && childrenCount > 1)
            {
                int columnCount = 0;

                for (int i = 0; i < childrenCount; i++)
                {
                    var child = children[i];
                    var childHeight = child.Measure(childReferenceSize).Height;

                    if (childHeight > availableHeight)
                    {
                        // 限制子控件宽度
                        childHeight = MathF.Min(availableHeight, child.Measure(childAvailableSize).Height);
                    }

                    if (currentHeight > 0f)
                    {
                        var finalHeight = currentHeight + spacing + childHeight;

                        if (finalHeight > availableHeight)
                        {
                            currentHeight = childHeight;
                            renderWidth += uniformWidth;
                            columnCount++;
                        }
                        else
                        {
                            currentHeight = finalHeight;
                        }
                    }
                    else
                    {
                        currentHeight = childHeight;
                    }
                }

                if (currentHeight > 0f)
                {
                    renderWidth += uniformWidth; // 计算最后一行
                    columnCount++;
                }

                renderWidth += spacing * (columnCount - 1);
            }
            else
            {
                for (int i = 0; i < childrenCount; i++)
                {
                    var child = children[i];
                    var childHeight = child.Measure(childReferenceSize).Height;

                    if (childHeight > availableHeight)
                    {
                        // 限制子控件宽度
                        childHeight = MathF.Min(availableHeight, child.Measure(childAvailableSize).Height);
                    }

                    if (currentHeight > 0f)
                    {
                        var finalHeight = currentHeight + childHeight;

                        if (finalHeight > availableHeight)
                        {
                            currentHeight = childHeight;
                            renderWidth += uniformWidth;
                        }
                        else
                        {
                            currentHeight = finalHeight;
                        }
                    }
                    else
                    {
                        currentHeight = childHeight;
                    }
                }

                if (currentHeight > 0f)
                {
                    renderWidth += uniformWidth; // 计算最后一行
                }
            }

            return new Size(renderWidth, availableHeight);
        }

        #endregion


        private float _uniformSize;
    }
}
