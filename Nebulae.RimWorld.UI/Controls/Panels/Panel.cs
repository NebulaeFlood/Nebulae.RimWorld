using Nebulae.RimWorld.UI.Data;
using Nebulae.RimWorld.UI.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Nebulae.RimWorld.UI.Controls.Panels
{
    /// <summary>
    /// 面板控件的基类，定义了其共同特性
    /// </summary>
    public abstract class Panel : FrameworkControl
    {
        //------------------------------------------------------
        //
        //  Private Fields
        //
        //------------------------------------------------------

        #region Private Fields

        private readonly PanelChildrenCollection _children;

        private Control[] _drawableChildren;
        private Control[] _filteredChildren;

        private bool _isDrawableChildrenValid = false;
        private bool _isFilteredChildrenValid = false;

        #endregion


        //------------------------------------------------------
        //
        //  Protected Properties
        //
        //------------------------------------------------------

        #region Protected Properties

        /// <summary>
        /// <see cref="Panel"/> 包含的子控件
        /// </summary>
        /// <remarks>对于 <see cref="Grid"/>，部分项可能为 <see langword="null"/>。</remarks>
        protected internal PanelChildrenCollection Children => _children;

        /// <summary>
        /// <see cref="Filter"/> 过滤后的子控件
        /// </summary>
        protected internal Control[] FilteredChildren
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
        /// 标识 <see cref="Filter"/> 依赖属性。
        /// </summary>
        public static readonly DependencyProperty FilterProperty =
            DependencyProperty.Register(nameof(Filter), typeof(Predicate<Control>), typeof(Panel),
                new ControlPropertyMetadata(null, OnFilterChanged, ControlRelation.Measure));

        private static void OnFilterChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((Panel)d).InvalidateFilter();
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
        /// 要求面板在下次绘制控件之前重新判断子控件是否可绘制
        /// </summary>
        public void InvalidateDrawableChildren()
        {
            _isDrawableChildrenValid = false;
        }

        /// <summary>
        /// 要求面板在下次绘制控件之前重新过滤子控件
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
        protected sealed override Rect ArrangeCore(Rect availableRect)
        {
            _isDrawableChildrenValid = false;

            return ArrangeOverride(base.ArrangeCore(availableRect));
        }

        /// <summary>
        /// 计算排布将被绘制的子控件需要的区域
        /// </summary>
        /// <param name="availableRect">子控件允许排布的区域</param>
        /// <returns>排布子控件占用的区域。</returns>
        protected abstract Rect ArrangeOverride(Rect availableRect);

        /// <inheritdoc/>
        protected sealed override void DrawCore()
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

        /// <inheritdoc/>
        protected internal override IEnumerable<Control> EnumerateLogicalChildren()
        {
            return _children;
        }

        /// <summary>
        /// 判断子控件是否应该被绘制
        /// </summary>
        /// <param name="child">要判断的子控件</param>
        /// <returns>子控件是否应该被绘制</returns>
        protected virtual bool IsDrawable(Control child)
        {
            return child.RenderSize.Height > float.Epsilon
                && child.RenderSize.Width > float.Epsilon;
        }

        /// <inheritdoc/>
        protected sealed override Size MeasureCore(Size availableSize)
        {
            return MeasureOverride(base.MeasureCore(availableSize));
        }

        /// <summary>
        /// 计算排布将被绘制的子控件需要的尺寸
        /// </summary>
        /// <param name="availableSize">子控件允许排布的尺寸</param>
        /// <returns>排布子控件需要的尺寸。</returns>
        protected abstract Size MeasureOverride(Size availableSize);

        /// <inheritdoc/>
        protected override Rect SegmentCore(Rect visiableRect)
        {
            _isDrawableChildrenValid = false;

            var children = FilteredChildren;

            for (int i = 0; i < children.Length; i++)
            {
                children[i].Segment(visiableRect);
            }

            return visiableRect;
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
    }
}
