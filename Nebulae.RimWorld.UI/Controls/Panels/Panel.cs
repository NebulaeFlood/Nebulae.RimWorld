using Nebulae.RimWorld.UI.Controls.Basic;
using Nebulae.RimWorld.UI.Core.Data;
using Nebulae.RimWorld.UI.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;

namespace Nebulae.RimWorld.UI.Controls.Panels
{
    /// <summary>
    /// 面板控件的基类，定义了其共同特性
    /// </summary>
    [DebuggerStepThrough]
    public abstract class Panel : FrameworkControl
    {
        /// <inheritdoc/>
        public override IEnumerable<Control> LogicalChildren => _children;


        #region Filter

        /// <summary>
        /// 获取或设置子控件过滤器
        /// </summary>
        public Predicate<Control> Filter
        {
            get { return (Predicate<Control>)GetValue(FilterProperty); }
            set { SetValue(FilterProperty, value); }
        }

        /// <summary>
        /// 标识 <see cref="Filter"/> 依赖属性
        /// </summary>
        public static readonly DependencyProperty FilterProperty =
            DependencyProperty.Register(nameof(Filter), typeof(Predicate<Control>), typeof(Panel),
                new ControlPropertyMetadata(null, OnFilterChanged, ControlRelation.Measure));

        private static void OnFilterChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((Panel)d).InvalidateFilter();
        }

        #endregion


        //------------------------------------------------------
        //
        //  Protected Properties
        //
        //------------------------------------------------------

        #region Protected Properties

        /// <summary>
        /// 获取 <see cref="Panel"/> 包含的子控件
        /// </summary>
        /// <remarks>对于 <see cref="Grid"/>，部分项可能为 <see langword="null"/>。</remarks>
        protected PanelChildrenCollection Children => _children;

        /// <summary>
        /// 获取 <see cref="Panel"/> 由 <see cref="Filter"/> 过滤后的子控件
        /// </summary>
        protected Control[] FilteredChildren
        {
            get
            {
                if (_isFilteredChildrenValid)
                {
                    return _filteredChildren;
                }

                _filteredChildren = FindFilteredChildren();
                _isFilteredChildrenValid = true;
                return _filteredChildren;
            }
        }

        #endregion


        static Panel()
        {
            HorizontalAlignmentProperty.OverrideMetadata(typeof(Panel),
                new ControlPropertyMetadata(HorizontalAlignment.Stretch, ControlRelation.Measure));

            VerticalAlignmentProperty.OverrideMetadata(typeof(Panel),
                new ControlPropertyMetadata(VerticalAlignment.Stretch, ControlRelation.Measure));
        }

        /// <summary>
        /// 为 <see cref="Panel"/> 派生类实现基本初始化
        /// </summary>
        protected Panel()
        {
            _children = new PanelChildrenCollection(this);
        }


        //------------------------------------------------------
        //
        //  Public Methods
        //
        //------------------------------------------------------

        #region Public Methods

        /// <summary>
        /// 要求 <see cref="Panel"/> 在下次绘制控件之前重新判断子控件是否可绘制
        /// </summary>
        public void InvalidateDrawableChildren()
        {
            _isDrawableChildrenValid = false;
        }

        /// <summary>
        /// 要求 <see cref="Panel"/> 在下次绘制控件之前重新过滤子控件
        /// </summary>
        public void InvalidateFilter()
        {
            if (_isFilteredChildrenValid)
            {
                _isDrawableChildrenValid = false;
                _isFilteredChildrenValid = false;

                InvalidateMeasure();
            }
        }

        #endregion


        internal void ClearInternal()
        {
            _drawableChildren = Array.Empty<Control>();
            _filteredChildren = Array.Empty<Control>();

            InvalidateFilter();
        }


        //------------------------------------------------------
        //
        //  Protected Methods
        //
        //------------------------------------------------------

        #region Protected Methods

        /// <inheritdoc/>
        protected override sealed Rect ArrangeOverride(Rect availableRect)
        {
            _isDrawableChildrenValid = false;
            return ArrangeOverride(availableRect, _filteredChildren);
        }

        /// <summary>
        /// 计算 <see cref="Panel"/> 的内容需要占用的区域
        /// </summary>
        /// <param name="availableRect">由 <see cref="FrameworkControl"/> 计算后分配的区域</param>
        /// <param name="children"><see cref="Filter"/> 过滤后的子控件</param>
        /// <returns>内容需要占用的区域。</returns>
        protected abstract Rect ArrangeOverride(Rect availableRect, Control[] children);

        /// <inheritdoc/>
        protected override sealed Size MeasureOverride(Size availableSize)
        {
            return MeasureOverride(availableSize, FilteredChildren);
        }

        /// <summary>
        /// 计算 <see cref="Panel"/> 的内容需要占用的尺寸
        /// </summary>
        /// <param name="availableSize">由 <see cref="FrameworkControl"/> 计算后可用的尺寸</param>
        /// <param name="children"><see cref="Filter"/> 过滤后的子控件</param>
        /// <returns>内容需要占用的尺寸。</returns>
        protected abstract Size MeasureOverride(Size availableSize, Control[] children);

        /// <inheritdoc/>
        protected override sealed SegmentResult SegmentCore(Rect visiableRect)
        {
            _isDrawableChildrenValid = false;

            visiableRect = visiableRect.IntersectWith(RenderRect);

            for (int i = _filteredChildren.Length - 1; i >= 0; i--)
            {
                _filteredChildren[i].Segment(visiableRect);
            }

            return visiableRect;
        }

        /// <inheritdoc/>
        protected override HitTestResult HitTestCore(Vector2 hitPoint)
        {
            if (!ControlRect.Contains(hitPoint))
            {
                return HitTestResult.Empty;
            }

            for (int i = _filteredChildren.Length - 1; i >= 0; i--)
            {
                var result = _filteredChildren[i].HitTest(hitPoint);

                if (result.IsHit)
                {
                    return result;
                }
            }

            return HitTestResult.HitTest(this, true);
        }

        /// <inheritdoc/>
        protected override sealed void DrawCore(ControlState states)
        {
            if (!_isDrawableChildrenValid)
            {
                _drawableChildren = FindDrawableChildren().ToArray();
                _isDrawableChildrenValid = true;
            }

            for (int i = 0; i < _drawableChildren.Length; i++)
            {
                _drawableChildren[i].Draw();
            }
        }

        #endregion


        //------------------------------------------------------
        //
        //  Private Methods
        //
        //------------------------------------------------------

        #region Private Methods

        /// <summary>
        /// 查找当前条件下可以被绘制的子控件
        /// </summary>
        /// <returns>当前条件下可以被绘制的子控件</returns>
        private IEnumerable<Control> FindDrawableChildren()
        {
            var filteredChildren = FilteredChildren;

            bool IsDrawable(Control child)
            {
                return !(child.DesiredSize < Size.Epsilon) && VisibleRect.Overlaps(child.VisibleRect);
            }

            for (int i = 0; i < filteredChildren.Length; i++)
            {
                if (IsDrawable(filteredChildren[i]))
                {
                    yield return filteredChildren[i];
                }
            }
        }

        /// <summary>
        /// 查找 <see cref="Filter"/> 过滤后的子控件
        /// </summary>
        /// <returns><see cref="Filter"/> 过滤后的子控件</returns>
        private Control[] FindFilteredChildren()
        {
            var filter = Filter;

            if (filter is null)
            {
                return _children.ToArray();
            }
            else
            {
                return _children.FindAll(filter).ToArray();
            }
        }

        #endregion


        //------------------------------------------------------
        //
        //  Private Fields
        //
        //------------------------------------------------------

        #region Private Fields

        private readonly PanelChildrenCollection _children;

        private Control[] _drawableChildren = Array.Empty<Control>();
        private Control[] _filteredChildren = Array.Empty<Control>();

        private bool _isDrawableChildrenValid = false;
        private bool _isFilteredChildrenValid = false;

        #endregion
    }
}
