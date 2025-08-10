using Nebulae.RimWorld.UI.Core.Data;
using Nebulae.RimWorld.UI.Utilities;
using System;
using UnityEngine;
using Verse;
using Verse.Steam;

namespace Nebulae.RimWorld.UI.Controls.Basic
{
    /// <summary>
    /// 一个可以滚动内容的控件
    /// </summary>
    public sealed class ScrollViewer : ContentControl
    {
        //------------------------------------------------------
        //
        //  Public Properties
        //
        //------------------------------------------------------

        #region Public Properties

        /// <summary>
        /// 获取或设置一个值，指示 <see cref="ScrollViewer"/> 是否启用拖动滚动
        /// </summary>
        public bool DragScroll
        {
            get => _dragScroll;
            set => _dragScroll = value;
        }

        /// <summary>
        /// 获取或设置一个值，指示 <see cref="ScrollViewer"/> 的内容的水平偏移量
        /// </summary>
        public float HorizontalOffset
        {
            get => _horizontalOffset;
            set
            {
                value = Mathf.Clamp(value, 0f, _horizontalMaxOffset);

                if (_horizontalOffset != value)
                {
                    _horizontalOffset = value;
                    _shouldUpdateSegment = true;
                }
            }
        }

        #region HorizontalScrollBarVisibility
        /// <summary>
        /// 获取或设置水平滚动条可见性
        /// </summary>
        public ScrollBarVisibility HorizontalScrollBarVisibility
        {
            get { return (ScrollBarVisibility)GetValue(HorizontalScrollBarVisibilityProperty); }
            set { SetValue(HorizontalScrollBarVisibilityProperty, value); }
        }

        /// <summary>
        /// 标识 <see cref="HorizontalScrollBarVisibility"/> 依赖属性
        /// </summary>
        public static readonly DependencyProperty HorizontalScrollBarVisibilityProperty =
            DependencyProperty.Register(nameof(HorizontalScrollBarVisibility), typeof(ScrollBarVisibility), typeof(ScrollViewer),
                new ControlPropertyMetadata(ScrollBarVisibility.Auto, ControlRelation.Measure));
        #endregion

        #region HorizontalScrollOrigin
        /// <summary>
        /// 获取或设置水平方向初始滚动位置
        /// </summary>
        public HorizontalScrollOrigin HorizontalScrollOrigin
        {
            get { return (HorizontalScrollOrigin)GetValue(HorizontalScrollOriginProperty); }
            set { SetValue(HorizontalScrollOriginProperty, value); }
        }

        /// <summary>
        /// 标识 <see cref="HorizontalScrollOrigin"/> 依赖属性
        /// </summary>
        public static readonly DependencyProperty HorizontalScrollOriginProperty =
            DependencyProperty.Register(nameof(HorizontalScrollOrigin), typeof(HorizontalScrollOrigin), typeof(ScrollViewer),
                new ControlPropertyMetadata(HorizontalScrollOrigin.Left, ControlRelation.Arrange));
        #endregion

        /// <summary>
        /// 获取或设置一个值，指示鼠标滚轮滚动时，<see cref="ScrollViewer"/> 使用水平滚动还是垂直滚动
        /// </summary>
        public bool HorizontalScroll
        {
            get => _horizontalScroll;
            set => _horizontalScroll = value;
        }

        /// <summary>
        /// 获取或设置一个值，指示 <see cref="ScrollViewer"/> 的内容的垂直偏移量
        /// </summary>
        public float VerticalOffset
        {
            get => _verticalOffset;
            set
            {
                value = Mathf.Clamp(value, 0f, _verticalMaxOffset);

                if (_verticalOffset != value)
                {
                    _verticalOffset = value;
                    _shouldUpdateSegment = true;
                }
            }
        }

        #region VerticalScrollBarVisibility
        /// <summary>
        /// 获取或设置垂直滚动条可见性
        /// </summary>
        public ScrollBarVisibility VerticalScrollBarVisibility
        {
            get { return (ScrollBarVisibility)GetValue(VerticalScrollBarVisibilityProperty); }
            set { SetValue(VerticalScrollBarVisibilityProperty, value); }
        }

        /// <summary>
        /// 标识 <see cref="VerticalScrollBarVisibility"/> 依赖属性
        /// </summary>
        public static readonly DependencyProperty VerticalScrollBarVisibilityProperty =
            DependencyProperty.Register(nameof(VerticalScrollBarVisibility), typeof(ScrollBarVisibility), typeof(ScrollViewer),
                new ControlPropertyMetadata(ScrollBarVisibility.Auto, ControlRelation.Measure));
        #endregion

        #region VerticalScrollOrigin
        /// <summary>
        /// 获取或设置垂直方向初始滚动位置
        /// </summary>
        public VerticalScrollOrigin VerticalScrollOrigin
        {
            get { return (VerticalScrollOrigin)GetValue(VerticalScrollOriginProperty); }
            set { SetValue(VerticalScrollOriginProperty, value); }
        }

        /// <summary>
        /// 标识 <see cref="VerticalScrollOrigin"/> 依赖属性
        /// </summary>
        public static readonly DependencyProperty VerticalScrollOriginProperty =
            DependencyProperty.Register(nameof(VerticalScrollOrigin), typeof(VerticalScrollOrigin), typeof(ScrollViewer),
                new ControlPropertyMetadata(VerticalScrollOrigin.Top, ControlRelation.Arrange));
        #endregion

        #endregion


        static ScrollViewer()
        {
            HorizontalAlignmentProperty.OverrideMetadata(typeof(ScrollViewer),
                new PropertyMetadata(HorizontalAlignment.Stretch));

            VerticalAlignmentProperty.OverrideMetadata(typeof(ScrollViewer),
                new PropertyMetadata(VerticalAlignment.Stretch));
        }

        /// <summary>
        /// 初始化 <see cref="ScrollViewer"/> 的新实例
        /// </summary>
        public ScrollViewer()
        {
            IsHitTestVisible = true;
        }


        //------------------------------------------------------
        //
        //  Public Methods
        //
        //------------------------------------------------------

        #region Public Methods

        /// <summary>
        /// 滚动到底部
        /// </summary>
        public void ScrollToBottom()
        {
            if (_verticalOffset != _verticalMaxOffset)
            {
                _verticalOffset = _verticalMaxOffset;
                _shouldUpdateSegment = true;
            }
        }

        /// <summary>
        /// 滚动到最左处
        /// </summary>
        public void ScrollToLeft()
        {
            if (_horizontalOffset != 0f)
            {
                _horizontalOffset = 0f;
                _shouldUpdateSegment = true;
            }
        }

        /// <summary>
        /// 滚动到最右处
        /// </summary>
        public void ScrollToRight()
        {
            if (_horizontalOffset != _horizontalMaxOffset)
            {
                _horizontalOffset = _horizontalMaxOffset;
                _shouldUpdateSegment = true;
            }
        }

        /// <summary>
        /// 滚动到顶部
        /// </summary>
        public void ScrollToTop()
        {
            if (_verticalOffset != 0f)
            {
                _verticalOffset = 0f;
                _shouldUpdateSegment = true;
            }
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
            Content?.Arrange(new Rect(0f, 0f, MathF.Max(_viewWidth, _contentSize.Width), MathF.Max(_viewHeight, _contentSize.Height)));

            return availableRect;
        }

        /// <inheritdoc/>
        protected override Size MeasureOverride(Size availableSize)
        {
            if (IsEmpty)
            {
                return MeasureEmpty(availableSize);
            }

            bool autoWidth = float.IsNaN(availableSize.Width);
            bool autoHeight = float.IsNaN(availableSize.Height);

            if (autoWidth)
            {
                if (autoHeight)
                {
                    return MeasureAuto(availableSize, Content);
                }

                return MeasureAutoWidth(availableSize, Content);
            }
            else if (autoHeight)
            {
                return MeasureAutoHeight(availableSize, Content);
            }
            else
            {
                return Measure(availableSize, Content);
            }
        }

        /// <inheritdoc/>
        protected override SegmentResult SegmentCore(Rect visiableRect)
        {
            if (IsEmpty)
            {
                return visiableRect.IntersectWith(RenderRect);
            }

            visiableRect = visiableRect.IntersectWith(RenderRect);

            var visibleContentSize = visiableRect.IntersectSizeWith(new Rect(RenderRect.x, RenderRect.y, _viewWidth, _viewHeight));

            Content.Segment(new Rect(_horizontalOffset, _verticalOffset, visibleContentSize.Width, visibleContentSize.Height));

            return visiableRect;
        }

        /// <inheritdoc/>
        protected override HitTestResult HitTestCore(Vector2 hitPoint)
        {
            var result = HitTestResult.HitTest(this, hitPoint);

            if (!result.IsHit || IsEmpty)
            {
                return result;
            }

            var childResult = Content.HitTest(new Vector2(hitPoint.x - RenderRect.x + _horizontalOffset, hitPoint.y - RenderRect.y + _verticalOffset));

            if (childResult.IsHit)
            {
                return childResult;
            }

            return result;
        }

        /// <inheritdoc/>
        protected override void DrawCore(ControlState states)
        {
            HandleSteamDeckTouchScreen(RenderRect);

            var currentEvent = Event.current;
            var currentEventType = currentEvent.type;

            if (IsEmpty)
            {
                if (currentEventType is EventType.Layout || currentEventType is EventType.Used)
                {
                    Widgets.mouseOverScrollViewStack.Pop();
                    return;
                }

                if (_shouldDrawHorizontalScrollBar)
                {
                    DrawScrollBar(
                        new Rect(
                            RenderRect.x,
                            RenderRect.y + _viewHeight + GUI.skin.horizontalScrollbar.margin.top,
                            _viewWidth,
                            GUI.skin.horizontalScrollbar.fixedHeight),
                        0f,
                        _viewWidth,
                        0f,
                        _viewWidth,
                        isHorizontal: true);
                }

                if (_shouldDrawVerticalScrollBar)
                {
                    DrawScrollBar(
                        new Rect(
                            RenderRect.x + _viewWidth + GUI.skin.verticalScrollbar.margin.right,
                            RenderRect.y,
                            GUI.skin.verticalScrollbar.fixedWidth,
                            _viewHeight),
                        0f,
                        _viewHeight,
                        0f,
                        _viewHeight,
                        isHorizontal: false);
                }

                Widgets.mouseOverScrollViewStack.Pop();
                return;
            }

            var content = Content;

            #region BeginScrollViewer

            if (currentEventType != EventType.Layout && currentEventType != EventType.Used)
            {
                if (currentEventType is EventType.Repaint)
                {
                    GUI.skin.scrollView.Draw(
                        RenderRect,
                        RenderRect.Contains(Event.current.mousePosition),
                        false,
                        _shouldDrawHorizontalScrollBar && _shouldDrawVerticalScrollBar,
                        false);
                }

                if (_shouldDrawHorizontalScrollBar)
                {
                    float horizontalOffset = DrawScrollBar(
                        new Rect(
                            RenderRect.x,
                            RenderRect.y + _viewHeight + GUI.skin.horizontalScrollbar.margin.top,
                            _viewWidth,
                            GUI.skin.horizontalScrollbar.fixedHeight),
                        _horizontalOffset,
                        _viewWidth,
                        0f,
                        MathF.Max(content.DesiredSize.Width, _viewWidth),
                        isHorizontal: true);

                    _shouldUpdateSegment = _shouldUpdateSegment || _horizontalOffset != horizontalOffset;
                    _horizontalOffset = horizontalOffset;
                }

                if (_shouldDrawVerticalScrollBar)
                {
                    float verticalOffset = DrawScrollBar(
                        new Rect(
                            RenderRect.x + _viewWidth + GUI.skin.verticalScrollbar.margin.left,
                            RenderRect.y,
                            GUI.skin.verticalScrollbar.fixedWidth,
                            _viewHeight),
                        _verticalOffset,
                        _viewHeight,
                        0f,
                        MathF.Max(content.DesiredSize.Height, _viewHeight),
                        isHorizontal: false);

                    _shouldUpdateSegment = _shouldUpdateSegment || _verticalOffset != verticalOffset;
                    _verticalOffset = verticalOffset;
                }
            }

            UnityGUIBugsFixer.Notify_BeginScrollView();
            GUI.BeginClip(new Rect(
                RenderRect.x,
                RenderRect.y,
                _viewWidth,
                _viewHeight));
            Widgets.BeginGroup(new Rect(
                -_horizontalOffset,
                -_verticalOffset,
                _viewWidth + _horizontalOffset,
                _viewHeight + _verticalOffset));

            #endregion

            if (_shouldUpdateSegment)
            {
                content.InvalidateSegment();
                _shouldUpdateSegment = false;
            }

            content.Draw();

            #region EndScrollViewer

            Widgets.EndGroup();
            GUI.EndClip();
            Widgets.mouseOverScrollViewStack.Pop();

            if (!_canScroll)
            {
                return;
            }

            if (_dragScroll && currentEventType is EventType.MouseDrag && states.HasState(ControlState.Dragging))
            {
                var delta = currentEvent.delta;

                _horizontalOffset = Mathf.Clamp(_horizontalOffset - delta.x, 0f, _horizontalMaxOffset);
                _verticalOffset = Mathf.Clamp(_verticalOffset - delta.y, 0f, _verticalMaxOffset);

                Event.current.Use();

                _shouldUpdateSegment = true;
            }
            else if (currentEventType is EventType.ScrollWheel && states.HasState(ControlState.CursorOver))
            {
                var delta = currentEvent.delta;

                if (_horizontalScroll)
                {
                    _horizontalOffset = Mathf.Clamp(_horizontalOffset + delta.y * 20f, 0f, _horizontalMaxOffset);
                    _verticalOffset = Mathf.Clamp(_verticalOffset + delta.x * 20f, 0f, _verticalMaxOffset);
                }
                else
                {
                    _horizontalOffset = Mathf.Clamp(_horizontalOffset + delta.x * 20f, 0f, _horizontalMaxOffset);
                    _verticalOffset = Mathf.Clamp(_verticalOffset + delta.y * 20f, 0f, _verticalMaxOffset);
                }

                Event.current.Use();

                _shouldUpdateSegment = true;
            }

            #endregion
        }

        #endregion


        //------------------------------------------------------
        //
        //  Private Methods
        //
        //------------------------------------------------------

        #region Private Methods

        private static float DrawScrollBar(Rect renderRect, float value, float size, float minValue, float maxValue, bool isHorizontal)
        {
            GUIStyle sliderStyle;
            GUIStyle thumbStyle;

            if (isHorizontal)
            {
                sliderStyle = GUI.skin.horizontalScrollbar;
                thumbStyle = GUI.skin.horizontalScrollbarThumb;
            }
            else
            {
                sliderStyle = GUI.skin.verticalScrollbar;
                thumbStyle = GUI.skin.verticalScrollbarThumb;
            }

            return GUI.Slider(renderRect,
                value,
                size,
                minValue,
                maxValue,
                sliderStyle,
                thumbStyle,
                isHorizontal,
                renderRect.GetHashCode());
        }

        private void HandleSteamDeckTouchScreen(Rect renderRect)
        {
            Vector2 scrollPosition = new Vector2(_horizontalOffset, _verticalOffset);

            if (Widgets.mouseOverScrollViewStack.Count > 0)
            {
                Widgets.mouseOverScrollViewStack.Push(
                    Widgets.mouseOverScrollViewStack.Peek()
                    && renderRect.Contains(Event.current.mousePosition));
            }
            else
            {
                Widgets.mouseOverScrollViewStack.Push(renderRect.Contains(Event.current.mousePosition));
            }

            SteamDeck.HandleTouchScreenScrollViewScroll(renderRect, ref scrollPosition);

            _horizontalOffset = scrollPosition.x;
            _verticalOffset = scrollPosition.y;
        }

        private Size Measure(Size availableSize, Control content)
        {
            var horizontalScrollBarVisibility = (ScrollBarVisibility)GetValue(HorizontalScrollBarVisibilityProperty);
            var verticalScrollBarVisibility = (ScrollBarVisibility)GetValue(VerticalScrollBarVisibilityProperty);

            _shouldDrawHorizontalScrollBar = horizontalScrollBarVisibility is ScrollBarVisibility.Visible;
            _shouldDrawVerticalScrollBar = verticalScrollBarVisibility is ScrollBarVisibility.Visible;

            float contentAvailableWidth = _shouldDrawVerticalScrollBar ? availableSize.Width - VerticalScrollBarWidth : availableSize.Width;
            float contentAvailableHeight = _shouldDrawHorizontalScrollBar ? availableSize.Height - HorizontalScrollBarHeight : availableSize.Height;

            _contentSize = content.Measure(new Size(contentAvailableWidth, contentAvailableHeight));

            bool shouldMeasureAgain = false;

            if (!_shouldDrawHorizontalScrollBar && horizontalScrollBarVisibility is ScrollBarVisibility.Auto && _contentSize.Width > contentAvailableWidth)
            {
                contentAvailableHeight -= HorizontalScrollBarHeight;
                shouldMeasureAgain = true;
                _shouldDrawHorizontalScrollBar = true;
            }

            if (!_shouldDrawVerticalScrollBar && verticalScrollBarVisibility is ScrollBarVisibility.Auto && _contentSize.Height > contentAvailableHeight)
            {
                contentAvailableWidth -= VerticalScrollBarWidth;
                shouldMeasureAgain = true;
                _shouldDrawVerticalScrollBar = true;
            }

            _viewHeight = contentAvailableHeight;
            _viewWidth = contentAvailableWidth;

            if (shouldMeasureAgain)
            {
                _contentSize = content.Measure(new Size(contentAvailableWidth, contentAvailableHeight));
            }

            if (_horizontalMaxOffset > 0f)
            {
                _horizontalMaxOffset = MathF.Max(0f, _contentSize.Width - _viewWidth);
                _horizontalOffset = Mathf.Clamp(_horizontalOffset, 0f, _horizontalMaxOffset);
            }
            else
            {
                _horizontalMaxOffset = MathF.Max(0f, _contentSize.Width - _viewWidth);
                _horizontalOffset = HorizontalScrollOrigin is HorizontalScrollOrigin.Left ? 0f : _horizontalMaxOffset;
            }

            if (_verticalMaxOffset > 0f)
            {
                _verticalMaxOffset = MathF.Max(0f, _contentSize.Height - _viewHeight);
                _verticalOffset = Mathf.Clamp(_verticalOffset, 0f, _verticalMaxOffset);
            }
            else
            {
                _verticalMaxOffset = MathF.Max(0f, _contentSize.Height - _viewHeight);
                _verticalOffset = VerticalScrollOrigin is VerticalScrollOrigin.Top ? 0f : _verticalMaxOffset;
            }

            _canScroll = _horizontalMaxOffset > 0f || _verticalMaxOffset > 0f;

            return availableSize;
        }

        private Size MeasureEmpty(Size availableSize)
        {
            _shouldDrawHorizontalScrollBar = GetValue(HorizontalScrollBarVisibilityProperty) is ScrollBarVisibility.Visible;
            _shouldDrawVerticalScrollBar = GetValue(VerticalScrollBarVisibilityProperty) is ScrollBarVisibility.Visible;

            _horizontalOffset = 0f;
            _horizontalMaxOffset = 0f;

            _verticalOffset = 0f;
            _verticalMaxOffset = 0f;

            _viewHeight = _shouldDrawHorizontalScrollBar
                ? availableSize.Height - HorizontalScrollBarHeight
                : availableSize.Height;

            _viewWidth = _shouldDrawVerticalScrollBar
                ? availableSize.Width - VerticalScrollBarWidth
                : availableSize.Width;

            // 测量完成后尺寸会被格式化，所以这里不判断是否为负数
            return availableSize;
        }

        private Size MeasureAuto(Size availableSize, Control content)
        {
            _contentSize = content.Measure(availableSize);

            _shouldDrawHorizontalScrollBar = GetValue(HorizontalScrollBarVisibilityProperty) is ScrollBarVisibility.Visible;
            _shouldDrawVerticalScrollBar = GetValue(VerticalScrollBarVisibilityProperty) is ScrollBarVisibility.Visible;

            _viewWidth = _contentSize.Width;
            _viewHeight = _contentSize.Height;

            _horizontalOffset = 0f;
            _horizontalMaxOffset = 0f;

            _verticalOffset = 0f;
            _verticalMaxOffset = 0f;

            _canScroll = false;

            return new Size(
                _shouldDrawVerticalScrollBar ? _contentSize.Width + VerticalScrollBarWidth : _contentSize.Width,
                _shouldDrawHorizontalScrollBar ? _contentSize.Height + HorizontalScrollBarHeight : _contentSize.Height);
        }

        private Size MeasureAutoHeight(Size availableSize, Control content)
        {
            _contentSize = content.Measure(availableSize);

            var horizontalScrollBarVisibility = (ScrollBarVisibility)GetValue(HorizontalScrollBarVisibilityProperty);
            _shouldDrawHorizontalScrollBar = horizontalScrollBarVisibility is ScrollBarVisibility.Visible
                || (horizontalScrollBarVisibility is ScrollBarVisibility.Auto && _contentSize.Width > availableSize.Width);

            _shouldDrawVerticalScrollBar = GetValue(VerticalScrollBarVisibilityProperty) is ScrollBarVisibility.Visible;

            _viewWidth = availableSize.Width;
            _viewHeight = _contentSize.Height;

            if (_horizontalMaxOffset > 0f)
            {
                _horizontalMaxOffset = MathF.Max(0f, _contentSize.Width - _viewWidth);
                _horizontalOffset = Mathf.Clamp(_horizontalOffset, 0f, _horizontalMaxOffset);
            }
            else
            {
                _horizontalMaxOffset = MathF.Max(0f, _contentSize.Width - _viewWidth);
                _horizontalOffset = HorizontalScrollOrigin is HorizontalScrollOrigin.Left ? 0f : _horizontalMaxOffset;
            }

            _verticalOffset = 0f;
            _verticalMaxOffset = 0f;

            _canScroll = _horizontalMaxOffset > 0f;

            return new Size(
                _shouldDrawVerticalScrollBar ? _contentSize.Width + VerticalScrollBarWidth : _contentSize.Width,
                _shouldDrawHorizontalScrollBar ? _contentSize.Height + HorizontalScrollBarHeight : _contentSize.Height);
        }

        private Size MeasureAutoWidth(Size availableSize, Control content)
        {
            _contentSize = content.Measure(availableSize);

            _shouldDrawHorizontalScrollBar = GetValue(HorizontalScrollBarVisibilityProperty) is ScrollBarVisibility.Visible;

            var verticalScrollBarVisibility = (ScrollBarVisibility)GetValue(VerticalScrollBarVisibilityProperty);
            _shouldDrawVerticalScrollBar = verticalScrollBarVisibility is ScrollBarVisibility.Visible
                || (verticalScrollBarVisibility is ScrollBarVisibility.Auto && _contentSize.Height > availableSize.Height);

            _viewWidth = _contentSize.Width;
            _viewHeight = availableSize.Height;

            _horizontalOffset = 0f;
            _horizontalMaxOffset = 0f;

            if (_verticalMaxOffset > 0f)
            {
                _verticalMaxOffset = MathF.Max(0f, _contentSize.Height - _viewHeight);
                _verticalOffset = Mathf.Clamp(_verticalOffset, 0f, _verticalMaxOffset);
            }
            else
            {
                _verticalMaxOffset = MathF.Max(0f, _contentSize.Height - _viewHeight);
                _verticalOffset = VerticalScrollOrigin is VerticalScrollOrigin.Top ? 0f : _verticalMaxOffset;
            }

            _canScroll = _verticalMaxOffset > 0f;

            return new Size(
                _shouldDrawVerticalScrollBar ? _contentSize.Width + VerticalScrollBarWidth : _contentSize.Width,
                _shouldDrawHorizontalScrollBar ? _contentSize.Height + HorizontalScrollBarHeight : _contentSize.Height);
        }

        #endregion


        //------------------------------------------------------
        //
        //  Private Properties
        //
        //------------------------------------------------------

        #region Private Properties

        private float HorizontalScrollBarHeight
        {
            get
            {
                var skin = GUI.skin.horizontalScrollbar;
                return skin.margin.top + skin.fixedHeight + skin.margin.bottom;
            }
        }

        private float VerticalScrollBarWidth
        {
            get
            {
                var skin = GUI.skin.verticalScrollbar;
                return skin.margin.left + skin.fixedWidth + skin.margin.right;
            }
        }

        #endregion


        //------------------------------------------------------
        //
        //  Private Fields
        //
        //------------------------------------------------------

        #region Private Fields

        private bool _canScroll;

        private bool _dragScroll;

        private bool _horizontalScroll;

        private bool _shouldDrawHorizontalScrollBar;
        private bool _shouldDrawVerticalScrollBar;
        private bool _shouldUpdateSegment;

        private Size _contentSize;

        private float _horizontalOffset = 0f;
        private float _horizontalMaxOffset = 0f;

        private float _verticalOffset = 0f;
        private float _verticalMaxOffset = 0f;

        private float _viewHeight = 0f;
        private float _viewWidth = 0f;

        #endregion
    }
}
