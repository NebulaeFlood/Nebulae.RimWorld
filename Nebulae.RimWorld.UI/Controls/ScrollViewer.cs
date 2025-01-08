using Nebulae.RimWorld.UI.Data;
using Nebulae.RimWorld.Utilities;
using UnityEngine;
using Verse;
using Verse.Steam;

namespace Nebulae.RimWorld.UI.Controls
{
    /// <summary>
    /// 使自身内容可滚动的控件
    /// </summary>
    public class ScrollViewer : ContentControl
    {
        private static readonly GUIStyle _backgroundStyle = GUI.skin.scrollView;

        private static readonly GUIStyle _horizontalScrollBarStyle = GUI.skin.horizontalScrollbar;
        private static readonly GUIStyle _horizontalScrollBarThumb = GUI.skin.horizontalScrollbarThumb;

        private static readonly GUIStyle _verticalScrollBarStyle = GUI.skin.verticalScrollbar;
        private static readonly GUIStyle _verticalScrollBarThumb = GUI.skin.verticalScrollbarThumb;


        //------------------------------------------------------
        //
        //  Private Fields
        //
        //------------------------------------------------------

        #region Private Fields

        private Control _contentControl;
        private Size _contentSize;

        private bool _horizontalScroll = false;

        private bool _shouldDrawHorizontalScrollBar = false;
        private bool _shouldDrawVerticalScrollBar = false;
        private bool _shouldUpdateSegment = false;

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
            _contentControl?.Arrange(new Rect(
                0f,
                0f,
                Mathf.Max(_viewWidth, _contentSize.Width),
                Mathf.Max(_viewHeight, _contentSize.Height)));
            return base.ArrangeCore(availableRect);
        }

        /// <inheritdoc/>
        protected override Rect DrawCore(Rect renderRect)
        {
            float x = renderRect.x;
            float y = renderRect.y;
            float width = _contentSize.Width;
            float height = _contentSize.Height;

            #region BeginScrollViewer

            HandleSteamDeckTouchScreen(renderRect);

            EventType eventType = Event.current.type;
            if (eventType != EventType.Layout && eventType != EventType.Used)
            {
                if (_contentControl is null)
                {
                    if (_shouldDrawHorizontalScrollBar)
                    {
                        DrawScrollBar(
                            new Rect(
                                x,
                                y + _viewHeight + _horizontalScrollBarStyle.margin.top,
                                width,
                                _horizontalScrollBarStyle.fixedHeight),
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
                                x + _viewWidth + _verticalScrollBarStyle.margin.right,
                                y,
                                _verticalScrollBarStyle.fixedWidth,
                                height),
                            0f,
                            _viewHeight,
                            0f,
                            _viewHeight,
                            isHorizontal: false);
                    }
                }
                else
                {
                    if (eventType is EventType.Repaint)
                    {
                        _backgroundStyle.Draw(
                            renderRect,
                            renderRect.Contains(Event.current.mousePosition),
                            false,
                            _shouldDrawHorizontalScrollBar && _shouldDrawVerticalScrollBar,
                            false);
                    }

                    if (_shouldDrawHorizontalScrollBar)
                    {
                        float horizontalOffset = DrawScrollBar(
                            new Rect(
                                x,
                                y + _viewHeight + _horizontalScrollBarStyle.margin.top,
                                _viewWidth,
                                _horizontalScrollBarStyle.fixedHeight),
                            _horizontalOffset,
                            _viewWidth,
                            0f,
                            Mathf.Max(_contentSize.Width, _viewWidth),
                            isHorizontal: true);

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
                                x + _viewWidth + _verticalScrollBarStyle.margin.left,
                                y,
                                _verticalScrollBarStyle.fixedWidth,
                                _viewHeight),
                            _verticalOffset,
                            _viewHeight,
                            0f,
                            Mathf.Max(_contentSize.Height, _viewHeight),
                            isHorizontal: false);

                        _shouldUpdateSegment = _shouldUpdateSegment || _verticalOffset != verticalOffset;
                        _verticalOffset = verticalOffset;
                    }
                    else
                    {
                        _verticalOffset = 0f;
                    }
                }
            }

            UnityGUIBugsFixer.Notify_BeginScrollView();
            GUI.BeginClip(new Rect(
                x,
                y,
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
                InvalidateSegment();
                _shouldUpdateSegment = false;
            }
            _contentControl.Draw();

            #region EndScrollViewer

            Widgets.EndGroup();
            GUI.EndClip();
            Widgets.mouseOverScrollViewStack.Pop();

            if (Event.current.type == EventType.ScrollWheel
                && renderRect.Contains(Event.current.mousePosition))
            {
                if (_horizontalScroll)
                {
                    _horizontalOffset = (_horizontalOffset + Event.current.delta.y * 20f).Clamp(0f, Mathf.Max(0f, _contentSize.Width - _viewWidth));
                    _verticalOffset = (_verticalOffset + Event.current.delta.x * 20f).Clamp(0f, Mathf.Max(0f, _contentSize.Height - _viewHeight));
                }
                else
                {
                    _horizontalOffset = (_horizontalOffset + Event.current.delta.x * 20f).Clamp(0f, Mathf.Max(0f, _contentSize.Width - _viewWidth));
                    _verticalOffset = (_verticalOffset + Event.current.delta.y * 20f).Clamp(0f, Mathf.Max(0f, _contentSize.Height - _viewHeight));
                }
                Event.current.Use();
                _shouldUpdateSegment = true;
            }
            #endregion

            return renderRect;
        }

        /// <inheritdoc/>
        protected override Size MeasureCore(Size availableSize)
        {
            Size desiredSize = base.MeasureCore(availableSize);

            _shouldDrawHorizontalScrollBar = HorizontalScrollBarVisibility is ScrollBarVisibility.Visible;
            _shouldDrawVerticalScrollBar = VerticalScrollBarVisibility is ScrollBarVisibility.Visible;

            if (_contentControl is null)
            {
                _viewHeight = _shouldDrawHorizontalScrollBar
                    ? desiredSize.Height - _horizontalScrollBarStyle.margin.bottom - _horizontalScrollBarStyle.fixedHeight - _horizontalScrollBarStyle.margin.top
                    : desiredSize.Height;
                _viewWidth = _shouldDrawVerticalScrollBar
                    ? desiredSize.Width - _verticalScrollBarStyle.margin.left - _verticalScrollBarStyle.fixedWidth - _verticalScrollBarStyle.margin.right
                    : desiredSize.Width;

                _contentSize = Size.Empty;
            }
            else
            {
                float contentAvailableHeight = desiredSize.Height;
                float contentAvailableWidth = desiredSize.Width;

                if (_shouldDrawHorizontalScrollBar)
                {
                    contentAvailableHeight = contentAvailableHeight - _horizontalScrollBarStyle.margin.bottom - _horizontalScrollBarStyle.fixedHeight - _horizontalScrollBarStyle.margin.top;
                }

                if (_shouldDrawVerticalScrollBar)
                {
                    contentAvailableWidth = contentAvailableWidth - _verticalScrollBarStyle.margin.left - _verticalScrollBarStyle.fixedWidth - _verticalScrollBarStyle.margin.right;
                }

                _contentSize = _contentControl.Measure(new Size(contentAvailableWidth, contentAvailableHeight));

                bool shouldMeasureAgain = false;
                if (!_shouldDrawHorizontalScrollBar
                    && HorizontalScrollBarVisibility != ScrollBarVisibility.Hidden
                    && _contentSize.Width > contentAvailableWidth)
                {
                    contentAvailableHeight = contentAvailableHeight - _horizontalScrollBarStyle.margin.bottom - _horizontalScrollBarStyle.fixedHeight - _horizontalScrollBarStyle.margin.top;
                    shouldMeasureAgain = true;
                    _shouldDrawHorizontalScrollBar = true;
                }
                if (!_shouldDrawVerticalScrollBar
                    && VerticalScrollBarVisibility != ScrollBarVisibility.Hidden
                    && _contentSize.Height > contentAvailableHeight)
                {
                    contentAvailableWidth = contentAvailableWidth - _verticalScrollBarStyle.margin.left - _verticalScrollBarStyle.fixedWidth - _verticalScrollBarStyle.margin.right;
                    shouldMeasureAgain = true;
                    _shouldDrawVerticalScrollBar = true;
                }

                if (shouldMeasureAgain)
                {
                    _contentControl.InvalidateMeasure();
                    _contentSize = _contentControl.Measure(new Size(contentAvailableWidth, contentAvailableHeight));
                }

                _viewHeight = contentAvailableHeight;
                _viewWidth = contentAvailableWidth;
            }

            return desiredSize;
        }

        /// <inheritdoc/>
        protected override void OnContentChanged(object content)
        {
            if (content is Control control)
            {
                _contentControl = control;
            }
            else
            {
                _contentControl = null;
            }
        }

        /// <inheritdoc/>
        protected override Rect SegmentCore()
        {
            if (IsHolded)
            {
                Rect renderRect = RenderRect;
                Rect contentVisiableRect = Container.Segment()
                    .IntersectWith(new Rect(
                        renderRect.x,
                        renderRect.y,
                        _viewWidth,
                        _viewHeight));
                return new Rect(
                    _horizontalOffset,
                    _verticalOffset,
                    contentVisiableRect.width,
                    contentVisiableRect.height);
            }
            else
            {
                return new Rect(
                    _horizontalOffset,
                    _verticalOffset,
                    _viewWidth,
                    _viewHeight);
            }
        }

        #endregion


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
                sliderStyle = _horizontalScrollBarStyle;
                thumbStyle = _horizontalScrollBarThumb;
            }
            else
            {
                sliderStyle = _verticalScrollBarStyle;
                thumbStyle = _verticalScrollBarThumb;
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
    }
}
