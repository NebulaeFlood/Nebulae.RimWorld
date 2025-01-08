using Nebulae.RimWorld.UI.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Nebulae.RimWorld.UI.Controls.Panels
{
    /// <summary>
    /// 面板控件的基类，定义了其共同特性
    /// </summary>
    public abstract class Panel : FrameworkControl, IFrame
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

        private bool _isCachedRenderedRectValid = false;
        private bool _isDrawableChildrenValid = false;
        private bool _isFilteredChildrenValid = false;
        private bool _isSegmentValid = false;

        private Rect _cachedRenderedRect = Rect.zero;
        private Rect _cachedVisiableRect = Rect.zero;

        private int _childrenVersion = 0;
        private IFrame[] _cachedFrameChildren;

        #endregion


        #region Filter
        /// <summary>
        /// 获取或设置子控件过滤器
        /// </summary>
        public Func<Control, bool> Filter
        {
            get { return (Func<Control, bool>)GetValue(FilterProperty); }
            set { SetValue(FilterProperty, value); }
        }

        /// <summary>
        /// 标识 <see cref="Filter"/> 依赖属性。
        /// </summary>
        public static readonly DependencyProperty FilterProperty =
            DependencyProperty.Register(nameof(Filter), typeof(Func<Control, bool>), typeof(Panel),
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
        /// <see cref="Panel"/> 包含的子控件
        /// </summary>
        /// <remarks>对于 <see cref="Grid"/>，部分项可能为 <see langword="null"/>。</remarks>
        protected PanelChildrenCollection Children => _children;


        /// <summary>
        /// 当前条件下可以被绘制的子控件
        /// </summary>
        protected Control[] DrawableChildren
        {
            get
            {
                if (_isDrawableChildrenValid)
                {
                    return _drawableChildren;
                }

                _drawableChildren = FindDrawableChildren().ToArray();
                _isDrawableChildrenValid = IsArrangeValid;
                return _drawableChildren;
            }
        }

        /// <summary>
        /// <see cref="Filter"/> 过滤后的子控件
        /// </summary>
        protected Control[] FilteredChildren
        {
            get
            {
                if (_isFilteredChildrenValid)
                {
                    return _filteredChildren;
                }

                _filteredChildren = FindFilteredChildren().ToArray();
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
        /// 要求面板在下次绘制控件之前重新过滤控件
        /// </summary>
        public void InvalidateFilter()
        {
            if (_isFilteredChildrenValid)
            {
                _isFilteredChildrenValid = false;
                InvalidateMeasure();
            }
        }

        /// <inheritdoc/>
        public void InvalidateSegment()
        {
            if (_isSegmentValid)
            {
                _isDrawableChildrenValid = false;
                _isSegmentValid = false;

                if (_childrenVersion != _children.Version)
                {
                    _childrenVersion = _children.Version;
                    _cachedFrameChildren = _children.Where(x => x is IFrame).Cast<IFrame>().ToArray();
                }

                Array.ForEach(_cachedFrameChildren, x => x.InvalidateSegment());
            }
        }

        /// <summary>
        /// 计算面板控件的内容控件的可显示区域
        /// </summary>
        /// <returns>内容控件的可显示区域。</returns>
        public Rect Segment()
        {
            if (_isSegmentValid)
            {
                return _cachedVisiableRect;
            }

            if (IsHolded)
            {
                _cachedVisiableRect = Container.Segment().IntersectWith(RenderRect);
            }
            else
            {
                _cachedVisiableRect = RenderRect;
            }
            _isSegmentValid = IsArrangeValid;

            return _cachedVisiableRect;
        }

        #endregion


        //------------------------------------------------------
        //
        //  Protected Methods
        //
        //------------------------------------------------------

        #region Protected Methods

        /// <inheritdoc/>
        protected sealed override Rect ArrangeCore(Rect availableRect)
        {
            return ArrangeOverride(base.ArrangeCore(availableRect));
        }

        /// <summary>
        /// 计算排布将被绘制的子控件需要的区域
        /// </summary>
        /// <param name="availableRect">子控件允许排布的区域</param>
        /// <returns>排布子控件占用的区域。</returns>
        protected abstract Rect ArrangeOverride(Rect availableRect);

        /// <inheritdoc/>
        protected sealed override Rect DrawCore(Rect renderRect)
        {
            if (!_isCachedRenderedRectValid)
            {
                for (int i = 0; i < DrawableChildren.Length; i++)
                {
                    if (i > 0)
                    {
                        _cachedRenderedRect = _cachedRenderedRect.CombineWith(
                            _drawableChildren[i].Draw());
                    }
                    else
                    {
                        _cachedRenderedRect = _drawableChildren[i].Draw();
                    }
                }
                _isCachedRenderedRectValid = true;
            }
            else
            {
                for (int i = 0; i < DrawableChildren.Length; i++)
                {
                    _drawableChildren[i].Draw();
                }
            }

            return _cachedRenderedRect;
        }

        /// <summary>
        /// 判断子控件是否应该被绘制
        /// </summary>
        /// <param name="child">要判断的子控件</param>
        /// <returns>子控件是否应该被绘制</returns>
        protected virtual bool IsDrawable(Control child)
        {
            if (child.Visibility is Visibility.Collapsed)
            {
                return false;
            }

            Size renderSize = child.RenderRect;
            if (renderSize == Size.Empty || renderSize < Size.Empty)
            {
                return false;
            }

            return true;
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
        protected override void OnArrangeInvalidated()
        {
            base.OnArrangeInvalidated();

            // 控件度量无效，说明已经调用过 InvalidateSegment 方法
            if (IsMeasureValid)
            {
                InvalidateSegment();

                _children.ForEach(x => x.InvalidateArrange());
                _isCachedRenderedRectValid = false;
                _isDrawableChildrenValid = false;
            }
        }

        /// <inheritdoc/>
        protected override void OnMeasureInvalidated()
        {
            base.OnMeasureInvalidated();
            InvalidateSegment();

            _children.ForEach(x => x.InvalidateMeasure());
            _isCachedRenderedRectValid = false;
            _isDrawableChildrenValid = false;
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
            Control[] filteredChildren = FilteredChildren;
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
        private IEnumerable<Control> FindFilteredChildren()
        {
            Func<Control, bool> filter = Filter;

            if (filter is null)
            {
                foreach (var child in _children)
                {
                    yield return child;
                }
            }
            else
            {
                foreach (var child in _children)
                {
                    if (filter.Invoke(child))
                    {
                        yield return child;
                    }
                }
            }
        }

        #endregion
    }
}
