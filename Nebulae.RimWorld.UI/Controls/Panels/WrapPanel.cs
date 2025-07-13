using Nebulae.RimWorld.UI.Controls.Basic;
using Nebulae.RimWorld.UI.Core.Data;
using System;
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
                new ControlPropertyMetadata(40f, ControlRelation.Measure));
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
                new ControlPropertyMetadata(200f, ControlRelation.Measure));
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
            float currentX = availableRect.x;
            float currentY = availableRect.y;

            if (Orientation is Orientation.Horizontal)
            {
                int columnIndex = 0;

                for (int i = 0; i < children.Length; i++)
                {
                    var child = children[i];

                ArrangeStart:
                    if (columnIndex < _columnCount)
                    {
                        child.Arrange(new Rect(currentX, currentY, _childWidth, _childHeight));
                        columnIndex++;
                        currentX += _childWidth;
                    }
                    else
                    {
                        currentX = availableRect.x;
                        currentY += _childHeight;   // 换行
                        columnIndex = 0;

                        goto ArrangeStart;  // 重新排列该控件
                    }
                }
            }
            else
            {
                int rowIndex = 0;

                for (int i = 0; i < children.Length; i++)
                {
                    var child = children[i];

                ArrangeStart:
                    if (rowIndex < _rowCount)
                    {
                        child.Arrange(new Rect(currentX, currentY, _childWidth, _childHeight));
                        currentY += _childHeight;
                        rowIndex++;
                    }
                    else
                    {
                        currentY = availableRect.y;
                        currentX += _childWidth;    // 换列
                        rowIndex = 0;

                        goto ArrangeStart;  // 重新排列该控件
                    }
                }
            }

            return new Rect(availableRect.x, availableRect.y, RenderSize.Width, RenderSize.Height);
        }

        /// <inheritdoc/>
        protected override Size MeasureOverride(Size availableSize, Control[] children)
        {
            _childWidth = (float)GetValue(ItemWidthProperty);
            _childHeight = (float)GetValue(ItemHeightProperty);

            if (_childWidth <= 0 || _childHeight <= 0)
            {
                return Size.Empty;
            }

            int count = children.Length;

            if (Orientation is Orientation.Horizontal)
            {
                // 限制子控件最大宽度不超过面板宽度
                _childWidth = _childWidth > 1f
                    ? Mathf.Min(_childWidth, availableSize.Width)
                    : _childWidth * availableSize.Width;

                _childHeight = _childHeight > 1f
                        ? _childHeight
                        : _childHeight * availableSize.Height;

                _columnCount = Mathf.FloorToInt(availableSize.Width / _childWidth);
                _rowCount = count / _columnCount;

                if (count % _columnCount > 0)
                {
                    _rowCount++;
                }
            }
            else
            {
                _childWidth = _childWidth > 1f
                    ? _childWidth
                    : _childWidth * availableSize.Width;

                // 限制子控件最大高度不超过面板高度
                _childHeight = _childHeight > 1f
                    ? Mathf.Min(_childHeight, availableSize.Height)
                    : _childHeight * availableSize.Height;

                _rowCount = Mathf.FloorToInt(availableSize.Height / _childHeight);
                _columnCount = count / _rowCount;

                if (count % _rowCount > 0)
                {
                    _columnCount++;
                }
            }

            var childSize = new Size(_childWidth, _childHeight);

            for (int i = 0; i < children.Length; i++)
            {
                children[i].Measure(childSize);
            }

            return new Size(_childWidth * _columnCount, _childHeight * _rowCount);
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
