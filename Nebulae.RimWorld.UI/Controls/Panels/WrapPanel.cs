using Nebulae.RimWorld.UI.Controls.Basic;
using Nebulae.RimWorld.UI.Core.Data;
using Nebulae.RimWorld.UI.Utilities;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Nebulae.RimWorld.UI.Controls.Panels
{
    /// <summary>
    /// 将子控件排列成数行或数列的面板
    /// </summary>
    public class WrapPanel : Panel
    {
        //------------------------------------------------------
        //
        //  Dependency Properties
        //
        //------------------------------------------------------

        #region Dependency Properties

        #region ItemHeight
        /// <summary>
        /// 获取或设置 <see cref="WrapPanel"/> 分配给子控件的高度
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
                new ControlPropertyMetadata(40f, CoerceItemSize, ControlRelation.Measure));
        #endregion

        #region ItemWidth
        /// <summary>
        /// 获取或设置 <see cref="WrapPanel"/> 分配给子控件的宽度
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
                new ControlPropertyMetadata(200f, CoerceItemSize, ControlRelation.Measure));
        #endregion

        #region Orientation
        /// <summary>
        /// 获取或设置 <see cref="WrapPanel"/> 排列子控件的方向
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

        #region Spacing
        /// <summary>
        /// 获取或设置 <see cref="WrapPanel"/> 子控件的统一间距
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
            DependencyProperty.Register(nameof(Spacing), typeof(float), typeof(WrapPanel),
                new ControlPropertyMetadata(4f, CoerceSpacing, ControlRelation.Measure));

        private static object CoerceSpacing(DependencyObject d, object baseValue)
        {
            return UIUtility.Format((float)baseValue);
        }
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
        /// <returns>该面板控件。</returns>
        public WrapPanel Append(Control control)
        {
            Children.Add(control);
            return this;
        }

        /// <summary>
        /// 清除面板并重新设置面板中的控件
        /// </summary>
        /// <param name="controls">要添加的控件</param>
        /// <returns>该面板控件。</returns>
        public WrapPanel Set(IEnumerable<Control> controls)
        {
            Children.OverrideCollection(controls);
            return this;
        }

        /// <summary>
        /// 清除面板并重新设置面板中的控件
        /// </summary>
        /// <param name="controls">要添加的控件</param>
        /// <returns>该面板控件。</returns>
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
        public WrapPanel Sort(Comparison<Control> comparison)
        {
            Children.Sort(comparison);
            return this;
        }

        /// <summary>
        /// 使用指定的比较强排序面板中的控件
        /// </summary>
        /// <param name="comparer">比较控件时使用的比较器</param>
        /// <returns>该面板控件。</returns>
        public WrapPanel Sort(IComparer<Control> comparer)
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
                ArrangeHorizontal(x, y, children, (float)GetValue(SpacingProperty));
            }
            else
            {
                ArrangeVertical(x, y, children, (float)GetValue(SpacingProperty));
            }

            return new Rect(x, y, RenderSize.Width, RenderSize.Height);
        }

        /// <inheritdoc/>
        protected override Size MeasureOverride(Size availableSize, Control[] children)
        {
            _childWidth = (float)GetValue(ItemWidthProperty);
            _childHeight = (float)GetValue(ItemHeightProperty);

            if (_childWidth is 0f || _childHeight is 0f)
            {
                return Size.Empty;
            }

            return GetValue(OrientationProperty) is Orientation.Horizontal
                ? MeasureHorizontal(availableSize, children, (float)GetValue(SpacingProperty))
                : MeasureVertical(availableSize, children, (float)GetValue(SpacingProperty));
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

        private static object CoerceItemSize(DependencyObject d, object baseValue)
        {
            return UIUtility.FormatProportion((float)baseValue);
        }

        #endregion


        //------------------------------------------------------
        //
        //  Private Methods
        //
        //------------------------------------------------------

        #region Private Methods

        private void ArrangeHorizontal(float x, float y, Control[] children, float spacing)
        {
            int childrenCount = children.Length;

            float currentX = x;
            float currentY = y;

            if (spacing > 0f)
            {
                if (_columnCount > 1)
                {
                    int columnIndex = 0;

                    for (int i = 0; i < childrenCount; i++)
                    {
                        if (columnIndex >= _columnCount)
                        {
                            currentX = x;
                            currentY += _childHeight + spacing; // 换行
                            columnIndex = 0;
                        }

                        children[i].Arrange(new Rect(currentX, currentY, _childWidth, _childHeight));

                        columnIndex++;
                        currentX += _childWidth + spacing;
                    }
                }
                else
                {
                    for (int i = 0; i < childrenCount; i++)
                    {
                        children[i].Arrange(new Rect(currentX, currentY, _childWidth, _childHeight));
                        currentY += _childHeight + spacing;
                    }
                }
            }
            else
            {
                if (_columnCount > 1)
                {
                    int columnIndex = 0;

                    for (int i = 0; i < childrenCount; i++)
                    {
                        if (columnIndex >= _columnCount)
                        {
                            currentX = x;
                            currentY += _childHeight; // 换行
                            columnIndex = 0;
                        }

                        children[i].Arrange(new Rect(currentX, currentY, _childWidth, _childHeight));

                        columnIndex++;
                        currentX += _childWidth;
                    }
                }
                else
                {
                    for (int i = 0; i < childrenCount; i++)
                    {
                        children[i].Arrange(new Rect(currentX, currentY, _childWidth, _childHeight));
                        currentY += _childHeight;
                    }
                }
            }
        }

        private void ArrangeVertical(float x, float y, Control[] children, float spacing)
        {
            int childrenCount = children.Length;

            float currentX = x;
            float currentY = y;

            if (spacing > 0f)
            {
                if (_rowCount > 1)
                {
                    int rowIndex = 0;

                    for (int i = 0; i < childrenCount; i++)
                    {
                        if (rowIndex >= _rowCount)
                        {
                            currentY = y;
                            currentX += _childWidth + spacing;  // 换列
                            rowIndex = 0;
                        }

                        children[i].Arrange(new Rect(currentX, currentY, _childWidth, _childHeight));

                        currentY += _childHeight + spacing;
                        rowIndex++;
                    }
                }
                else
                {
                    for (int i = 0; i < childrenCount; i++)
                    {
                        children[i].Arrange(new Rect(currentX, currentY, _childWidth, _childHeight));
                        currentX += _childWidth + spacing;
                    }
                }
            }
            else
            {
                if (_rowCount > 1)
                {
                    int rowIndex = 0;

                    for (int i = 0; i < children.Length; i++)
                    {
                        if (rowIndex >= _rowCount)
                        {
                            currentY = y;
                            currentX += _childWidth;  // 换列
                            rowIndex = 0;
                        }

                        children[i].Arrange(new Rect(currentX, currentY, _childWidth, _childHeight));

                        currentY += _childHeight;
                        rowIndex++;
                    }
                }
                else
                {
                    for (int i = 0; i < children.Length; i++)
                    {
                        children[i].Arrange(new Rect(currentX, currentY, _childWidth, _childHeight));
                        currentX += _childWidth;
                    }
                }
            }
        }

        private Size MeasureHorizontal(Size availableSize, Control[] children, float spacing)
        {
            int childrenCount = children.Length;
            bool allowSpace = spacing > 0f && childrenCount > 1;

            // 限制子控件最大宽度不超过面板宽度
            _childWidth = _childWidth > 1f
                ? MathF.Min(_childWidth, availableSize.Width)
                : _childWidth * availableSize.Width;

            _childHeight = _childHeight > 1f
                ? _childHeight
                : _childHeight * availableSize.Height;

            float renderWidth, renderHeight;

            if (allowSpace)
            {
                if (AllowStack(_childWidth, availableSize.Width, spacing))
                {
                    _columnCount = 1 + (int)MathF.Floor((availableSize.Width - _childWidth) / (_childWidth + spacing));
                    _rowCount = childrenCount / _columnCount;

                    if (childrenCount % _columnCount > 0)
                    {
                        _rowCount++;
                    }

                    renderWidth = (_childWidth + spacing) * _columnCount - spacing;
                    renderHeight = (_childHeight + spacing) * _rowCount - spacing;
                }
                else
                {
                    _columnCount = 1;
                    _rowCount = childrenCount;

                    renderWidth = _childWidth;
                    renderHeight = (_childHeight + spacing) * _rowCount - spacing;
                }
            }
            else
            {
                _columnCount = (int)MathF.Floor(availableSize.Width / _childWidth);
                _rowCount = childrenCount / _columnCount;

                if (childrenCount % _columnCount > 0)
                {
                    _rowCount++;
                }

                renderWidth = _childWidth * _columnCount;
                renderHeight = _childHeight * _rowCount;
            }

            var childAvailableSize = new Size(_childWidth, _childHeight);

            for (int i = childrenCount - 1; i >= 0; i--)
            {
                children[i].Measure(childAvailableSize);
            }

            return new Size(renderWidth, renderHeight);
        }

        private Size MeasureVertical(Size availableSize, Control[] children, float spacing)
        {
            int childrenCount = children.Length;
            bool allowSpace = spacing > 0f && childrenCount > 1;

            _childWidth = _childWidth > 1f
                ? _childWidth
                : _childWidth * availableSize.Width;

            // 限制子控件最大高度不超过面板高度
            _childHeight = _childHeight > 1f
                ? MathF.Min(_childHeight, availableSize.Height)
                : _childHeight * availableSize.Height;

            float renderWidth, renderHeight;

            if (allowSpace)
            {
                if (AllowStack(_childHeight, availableSize.Height, spacing))
                {
                    _rowCount = 1 + (int)MathF.Floor((availableSize.Height - _childHeight) / (_childHeight + spacing));
                    _columnCount = childrenCount / _rowCount;

                    if (childrenCount % _rowCount > 0)
                    {
                        _columnCount++;
                    }

                    renderWidth = (_childWidth + spacing) * _columnCount - spacing;
                    renderHeight = (_childHeight + spacing) * _rowCount - spacing;
                }
                else
                {
                    _rowCount = 1;
                    _columnCount = childrenCount;

                    renderWidth = (_childWidth + spacing) * _columnCount - spacing;
                    renderHeight = _childHeight;
                }
            }
            else
            {
                _rowCount = (int)MathF.Floor(availableSize.Height / _childHeight);
                _columnCount = childrenCount / _rowCount;

                if (childrenCount % _rowCount > 0)
                {
                    _columnCount++;
                }

                renderWidth = _childWidth * _columnCount;
                renderHeight = _childHeight * _rowCount;
            }

            var childAvailableSize = new Size(_childWidth, _childHeight);

            for (int i = childrenCount - 1; i >= 0; i--)
            {
                children[i].Measure(childAvailableSize);
            }

            return new Size(renderWidth, renderHeight);
        }

        #endregion


        //------------------------------------------------------
        //
        //  Private Fields
        //
        //------------------------------------------------------

        #region Private Fields

        private float _childWidth;
        private float _childHeight;

        private int _columnCount;
        private int _rowCount;

        #endregion
    }
}
