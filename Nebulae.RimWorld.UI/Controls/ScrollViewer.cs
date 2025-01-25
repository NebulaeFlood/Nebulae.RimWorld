using Nebulae.RimWorld.UI.Data;
using Nebulae.RimWorld.UI.Utilities;
using Nebulae.RimWorld.Utilities;
using UnityEngine;
using Verse;
using Verse.Steam;

namespace Nebulae.RimWorld.UI.Controls
{
    /// <summary>
    /// 滚动视图控件
    /// </summary>
    public class ScrollViewer : FrameworkControl
    {
        //------------------------------------------------------
        //
        //  Private Fields
        //
        //------------------------------------------------------

        #region Private Fields

        private Control _content;

        private bool _horizontalScroll = false;

        private bool _shouldDrawHorizontalScrollBar = false;
        private bool _shouldDrawVerticalScrollBar = false;
        private bool _shouldUpdateSegment = true;

        private float _horizontalOffset = 0f;
        private float _verticalOffset = 0f;
        private float _viewHeight = 0f;
        private float _viewWidth = 0f;

        #endregion


        //------------------------------------------------------
        //
        //  Public Properties
        //
        //------------------------------------------------------

        #region Public Properties

        /// <summary>
        /// 内容控件
        /// </summary>
        public Control Content
        {
            get => _content;
            set
            {
                if (!ReferenceEquals(_content, value))
                {
                    _content?.RemoveParent();
                    _content = value;
                    _content?.SetParent(this);
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
        /// 标识 <see cref="HorizontalScrollBarVisibility"/> 依赖属性。
        /// </summary>
        public static readonly DependencyProperty HorizontalScrollBarVisibilityProperty =
            DependencyProperty.Register(nameof(HorizontalScrollBarVisibility), typeof(ScrollBarVisibility), typeof(ScrollViewer),
                new ControlPropertyMetadata(ScrollBarVisibility.Auto, ControlRelation.Measure));
        #endregion

        /// <summary>
        /// 默认使用水平滚动
        /// </summary>
        public bool HorizontalScroll
        {
            get => _horizontalScroll;
            set => _horizontalScroll = value;
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
        /// 标识 <see cref="VerticalScrollBarVisibility"/> 依赖属性。
        /// </summary>
        public static readonly DependencyProperty VerticalScrollBarVisibilityProperty =
            DependencyProperty.Register(nameof(VerticalScrollBarVisibility), typeof(ScrollBarVisibility), typeof(ScrollViewer),
                new ControlPropertyMetadata(ScrollBarVisibility.Auto, ControlRelation.Measure));
        #endregion

        #endregion


        static ScrollViewer()
        {
            HorizontalAlignmentProperty.OverrideMetadata(typeof(ScrollViewer),
                new ControlPropertyMetadata(HorizontalAlignment.Stretch, ControlRelation.Measure));

            VerticalAlignmentProperty.OverrideMetadata(typeof(ScrollViewer),
                new ControlPropertyMetadata(VerticalAlignment.Stretch, ControlRelation.Measure));
        }

        /// <summary>
        /// 初始化 <see cref="ScrollViewer"/> 的新实例
        /// </summary>
        public ScrollViewer()
        {
        }


        //------------------------------------------------------
        //
        //  Protected Methods
        //
        //------------------------------------------------------

        #region Protected Methods

        /// <inheritdoc/>
        protected override Rect ArrangeCore(Rect availableRect)
        {
            _shouldUpdateSegment = true;

            _content?.Arrange(new Rect(
                0f,
                0f,
                Mathf.Min(_viewWidth, _content.DesiredSize.Width),
                Mathf.Min(_viewHeight, _content.DesiredSize.Height)));

            return base.ArrangeCore(availableRect);
        }

        /// <inheritdoc/>
        protected override void DrawCore()
        {
            HandleSteamDeckTouchScreen(RenderRect);

            EventType eventType = Event.current.type;

            if (_content is null)
            {
                if (eventType is EventType.Layout || eventType is EventType.Used)
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

            #region BeginScrollViewer

            if (eventType != EventType.Layout
                && eventType != EventType.Used)
            {
                if (eventType is EventType.Repaint)
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
                        Mathf.Max(_content.DesiredSize.Width, _viewWidth),
                        isHorizontal: true)
                            .Clamp(0f, Mathf.Max(0f, _content.DesiredSize.Width - _viewWidth));

                    _shouldUpdateSegment = _shouldUpdateSegment || _horizontalOffset != horizontalOffset;
                    _horizontalOffset = horizontalOffset;
                }
                else
                {
                    _horizontalOffset = 0f;
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
                        Mathf.Max(_content.DesiredSize.Height, _viewHeight),
                        isHorizontal: false)
                            .Clamp(0f, Mathf.Max(0f, _content.DesiredSize.Height - _viewHeight));

                    _shouldUpdateSegment = _shouldUpdateSegment || _verticalOffset != verticalOffset;
                    _verticalOffset = verticalOffset;
                }
                else
                {
                    _verticalOffset = 0f;
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
                _content.InvalidateSegment();
                _shouldUpdateSegment = false;
            }
            _content.Draw();


            #region EndScrollViewer

            Widgets.EndGroup();
            GUI.EndClip();
            Widgets.mouseOverScrollViewStack.Pop();

            if (Event.current.type == EventType.ScrollWheel
                && RenderRect.Contains(Event.current.mousePosition))
            {
                if (_horizontalScroll)
                {
                    _horizontalOffset = (_horizontalOffset + Event.current.delta.y * 20f)
                        .Clamp(0f, Mathf.Max(0f, _content.DesiredSize.Width - _viewWidth));

                    _verticalOffset = (_verticalOffset + Event.current.delta.x * 20f)
                        .Clamp(0f, Mathf.Max(0f, _content.DesiredSize.Height - _viewHeight));
                }
                else
                {
                    _horizontalOffset = (_horizontalOffset + Event.current.delta.x * 20f)
                        .Clamp(0f, Mathf.Max(0f, _content.DesiredSize.Width - _viewWidth));

                    _verticalOffset = (_verticalOffset + Event.current.delta.y * 20f)
                        .Clamp(0f, Mathf.Max(0f, _content.DesiredSize.Height - _viewHeight));
                }

                Event.current.Use();
                _shouldUpdateSegment = true;
            }
            #endregion
        }

        /// <inheritdoc/>
        protected override Size MeasureCore(Size availableSize)
        {
            Size renderSize = base.MeasureCore(availableSize);

            _shouldDrawHorizontalScrollBar = HorizontalScrollBarVisibility is ScrollBarVisibility.Visible;
            _shouldDrawVerticalScrollBar = VerticalScrollBarVisibility is ScrollBarVisibility.Visible;

            _horizontalOffset = 0f;
            _verticalOffset = 0f;

            if (_content is null)
            {
                _viewHeight = _shouldDrawHorizontalScrollBar
                    ? renderSize.Height - GUI.skin.horizontalScrollbar.margin.bottom - GUI.skin.horizontalScrollbar.fixedHeight - GUI.skin.horizontalScrollbar.margin.top
                    : renderSize.Height;

                _viewWidth = _shouldDrawVerticalScrollBar
                    ? renderSize.Width - GUI.skin.verticalScrollbar.margin.left - GUI.skin.verticalScrollbar.fixedWidth - GUI.skin.verticalScrollbar.margin.right
                    : renderSize.Width;
            }
            else
            {
                float contentAvailableHeight = renderSize.Height;
                float contentAvailableWidth = renderSize.Width;

                if (_shouldDrawHorizontalScrollBar)
                {
                    contentAvailableHeight = contentAvailableHeight - GUI.skin.horizontalScrollbar.margin.bottom - GUI.skin.horizontalScrollbar.fixedHeight - GUI.skin.horizontalScrollbar.margin.top;
                }

                if (_shouldDrawVerticalScrollBar)
                {
                    contentAvailableWidth = contentAvailableWidth - GUI.skin.verticalScrollbar.margin.left - GUI.skin.verticalScrollbar.fixedWidth - GUI.skin.verticalScrollbar.margin.right;
                }

                Size contentSize = _content.Measure(new Size(contentAvailableWidth, contentAvailableHeight));

                bool shouldMeasureAgain = false;
                if (!_shouldDrawHorizontalScrollBar
                    && HorizontalScrollBarVisibility != ScrollBarVisibility.Hidden
                    && contentSize.Width > contentAvailableWidth)
                {
                    contentAvailableHeight = contentAvailableHeight - GUI.skin.horizontalScrollbar.margin.bottom - GUI.skin.horizontalScrollbar.fixedHeight - GUI.skin.horizontalScrollbar.margin.top;
                    shouldMeasureAgain = true;
                    _shouldDrawHorizontalScrollBar = true;
                }
                if (!_shouldDrawVerticalScrollBar
                    && VerticalScrollBarVisibility != ScrollBarVisibility.Hidden
                    && contentSize.Height > contentAvailableHeight)
                {
                    contentAvailableWidth = contentAvailableWidth - GUI.skin.verticalScrollbar.margin.left - GUI.skin.verticalScrollbar.fixedWidth - GUI.skin.verticalScrollbar.margin.right;
                    shouldMeasureAgain = true;
                    _shouldDrawVerticalScrollBar = true;
                }

                if (shouldMeasureAgain)
                {
                    contentSize = _content.Measure(new Size(contentAvailableWidth, contentAvailableHeight));
                }

                _viewHeight = contentAvailableHeight;
                _viewWidth = contentAvailableWidth;
            }

            return renderSize;
        }

        /// <inheritdoc/>
        protected override Rect SegmentCore(Rect visiableRect)
        {
            visiableRect = base.SegmentCore(visiableRect);

            if (IsChild)
            {
                Rect contentVisiableRect = visiableRect
                    .IntersectWith(new Rect(
                        RenderRect.x,
                        RenderRect.y,
                        _viewWidth,
                        _viewHeight));

                _content?.Segment(new Rect(
                    _horizontalOffset,
                    _verticalOffset,
                    contentVisiableRect.width,
                    contentVisiableRect.height));
            }
            else
            {
                _content?.Segment(new Rect(
                    _horizontalOffset,
                    _verticalOffset,
                    _viewWidth,
                    _viewHeight));
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

        private static float DrawScrollBar(
            Rect renderRect,
            float value,
            float size,
            float minValue,
            float maxValue,
            bool isHorizontal)
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

        #endregion
    }
}
