using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;
using Verse;
using Verse.Steam;

namespace Nebulae.RimWorld.UI.Controls
{
    /// <summary>
    /// 使自身内容可滚动的控件
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

        private float _horizontalOffset;
        private float _verticalOffset;
        private float _viewHeight;
        private float _viewWidth;

        private ScrollBarVisibility _horizontalScrollBarVisibility;
        private ScrollBarVisibility _verticalScrollBarVisibility;

        private bool _horizontalScroll;

        private GUIStyle _backgroundStyle;
        private GUIStyle _horizontalScrollBarStyle;
        private GUIStyle _verticalScrollBarStyle;

        private bool _shouldDrawHorizontalScrollBar;
        private bool _shouldDrawVerticalScrollBar;

        #endregion


        //------------------------------------------------------
        //
        //  Public Properties
        //
        //------------------------------------------------------

        #region Public Properties

        /// <summary>
        /// 背景样式
        /// </summary>
        public GUIStyle BackgroundStyle
        {
            get => _backgroundStyle;
            set => _backgroundStyle = value;
        }

        /// <summary>
        /// 控件内容
        /// </summary>
        public Control Content
        {
            get => _content;
            set => _content = value;
        }

        /// <summary>
        /// 水平滚动条样式
        /// </summary>
        public GUIStyle HorizontalScrollBarStyle
        {
            get => _horizontalScrollBarStyle;
            set
            {
                if (value is null || value == GUIStyle.none)
                {
                    throw new InvalidOperationException($"{typeof(ScrollViewer)}.{nameof(HorizontalScrollBarStyle)} can not set to be null or {typeof(GUIStyle)}.none.");
                }
                _horizontalScrollBarStyle = value;
            }
        }

        /// <summary>
        /// 水平滚动条可见性
        /// </summary>
        public ScrollBarVisibility HorizontalScrollBarVisibility
        {
            get => _horizontalScrollBarVisibility;
            set => _horizontalScrollBarVisibility = value;
        }

        /// <summary>
        /// 默认使用水平滚动
        /// </summary>
        public bool HorizontalScroll
        {
            get => _horizontalScroll;
            set => _horizontalScroll = value;
        }

        /// <summary>
        /// 垂直滚动条样式
        /// </summary>
        public GUIStyle VerticalScrollBarStyle
        {
            get => _verticalScrollBarStyle;
            set
            {
                if (value is null || value == GUIStyle.none)
                {
                    throw new InvalidOperationException($"{typeof(ScrollViewer)}.{nameof(VerticalScrollBarStyle)} can not set to be null or {typeof(GUIStyle)}.none.");
                }
                _verticalScrollBarStyle = value;
            }
        }

        /// <summary>
        /// 垂直滚动条可见性
        /// </summary>
        public ScrollBarVisibility VerticalScrollBarVisibility
        {
            get => _verticalScrollBarVisibility;
            set => _verticalScrollBarVisibility = value;
        }

        #endregion


        /// <summary>
        /// 初始化 <see cref="ScrollViewer"/> 的新实例
        /// </summary>
        public ScrollViewer()
        {
            _horizontalOffset = 0f;
            _verticalOffset = 0f;
            _viewHeight = 0f;
            _viewWidth = 0f;

            _horizontalScroll = false;

            _horizontalScrollBarVisibility = ScrollBarVisibility.Auto;
            _verticalScrollBarVisibility = ScrollBarVisibility.Auto;

            _backgroundStyle = GUI.skin.scrollView;
            _horizontalScrollBarStyle = GUI.skin.verticalScrollbar;
            _verticalScrollBarStyle = GUI.skin.horizontalScrollbar;

            _shouldDrawHorizontalScrollBar = false;
            _shouldDrawVerticalScrollBar = false;
        }


        /// <inheritdoc/>
        protected override Rect ArrangeCore(Rect availableRect)
        {
            _content?.Arrange(new Rect(0f, 0f, _viewWidth, _viewHeight));
            return base.ArrangeCore(availableRect);
        }

        /// <inheritdoc/>
        protected override Rect DrawCore(Rect renderRect)
        {
            HandleSteamDeckTouchScreen(renderRect);

            EventType eventType = Event.current.type;
            if (eventType != EventType.Layout && eventType != EventType.Used)
            {
                if (_content is null)
                {
                    if (_shouldDrawHorizontalScrollBar)
                    {
                        GUI.HorizontalScrollbar(
                            new Rect(renderRect.x, renderRect.yMax - (float)_horizontalScrollBarStyle.margin.bottom - _horizontalScrollBarStyle.fixedHeight, renderRect.width, _horizontalScrollBarStyle.fixedHeight),
                            0f,
                            0f,
                            0f,
                            renderRect.width);
                    }
                    if (_shouldDrawVerticalScrollBar)
                    {
                        GUI.VerticalScrollbar(
                            new Rect(renderRect.xMax - (float)_verticalScrollBarStyle.margin.left - _verticalScrollBarStyle.fixedWidth, renderRect.y, _verticalScrollBarStyle.fixedWidth, renderRect.height),
                            0f,
                            0f,
                            0f,
                            0f);
                    }
                }
                else
                {
                    if (eventType is EventType.Repaint)
                    {
                        _backgroundStyle?.Draw(
                            renderRect,
                            renderRect.Contains(Event.current.mousePosition),
                            false,
                            _shouldDrawHorizontalScrollBar && _shouldDrawVerticalScrollBar,
                            false);
                    }

                    GUI.BeginClip(new Rect(
                        _horizontalOffset + renderRect.x,
                        _verticalOffset + renderRect.y,
                        _viewWidth,
                        _viewHeight));
                    _content.Draw();
                    GUI.EndClip();

                    if (_shouldDrawHorizontalScrollBar)
                    {
                        _horizontalOffset = GUI.HorizontalScrollbar(
                            new Rect(renderRect.x, renderRect.yMax - (float)_horizontalScrollBarStyle.margin.bottom - _horizontalScrollBarStyle.fixedHeight, renderRect.width, _horizontalScrollBarStyle.fixedHeight),
                            _horizontalOffset,
                            _viewWidth,
                            0f,
                            renderRect.width);
                    }
                    else
                    {
                        _horizontalOffset = 0f;
                    }

                    if (_shouldDrawVerticalScrollBar)
                    {
                        _verticalOffset = GUI.VerticalScrollbar(
                            new Rect(renderRect.xMax - (float)_verticalScrollBarStyle.margin.right - _verticalScrollBarStyle.fixedWidth, renderRect.y, _verticalScrollBarStyle.fixedWidth, renderRect.height),
                            _verticalOffset,
                            _viewHeight,
                            0f,
                            renderRect.height);
                    }
                    else
                    {
                        _verticalOffset = 0f;
                    }
                }
            }

            UnityGUIBugsFixer.Notify_BeginScrollView();
            return renderRect;
        }


        /// <inheritdoc/>
        protected override Size MeasureCore(Size availableSize)
        {
            Size desiredSize = base.MeasureCore(availableSize);

            if (_content is null)
            {
                _shouldDrawHorizontalScrollBar = _horizontalScrollBarVisibility is ScrollBarVisibility.Visible;
                _shouldDrawVerticalScrollBar = _verticalScrollBarVisibility is ScrollBarVisibility.Visible;

                _viewHeight = _shouldDrawHorizontalScrollBar
                    ? desiredSize.Height - _horizontalScrollBarStyle.margin.bottom - _horizontalScrollBarStyle.fixedHeight - _horizontalScrollBarStyle.margin.top
                    : desiredSize.Height;
                _viewWidth = _shouldDrawVerticalScrollBar
                    ? desiredSize.Width - _verticalScrollBarStyle.margin.left - _verticalScrollBarStyle.fixedWidth - _verticalScrollBarStyle.margin.right
                    : desiredSize.Width;
            }
            else
            {
                float contentAvailableHeight = desiredSize.Height;
                float contentAvailableWidth = desiredSize.Width;
                if (_horizontalScrollBarVisibility is ScrollBarVisibility.Visible)
                {
                    contentAvailableHeight = contentAvailableHeight - _horizontalScrollBarStyle.margin.bottom - _horizontalScrollBarStyle.fixedHeight - _horizontalScrollBarStyle.margin.top;
                    _shouldDrawHorizontalScrollBar = true;
                }
                if (_verticalScrollBarVisibility is ScrollBarVisibility.Visible)
                {
                    contentAvailableWidth = contentAvailableWidth - _verticalScrollBarStyle.margin.left - _verticalScrollBarStyle.fixedWidth - _verticalScrollBarStyle.margin.right;
                    _shouldDrawVerticalScrollBar = true;
                }

                Size contentDesiredSize = _content.Measure(new Size(contentAvailableWidth, contentAvailableHeight));

                bool shouldMeasureAgain = false;
                if (!_shouldDrawHorizontalScrollBar 
                    && _horizontalScrollBarVisibility != ScrollBarVisibility.Hidden 
                    && contentDesiredSize.Width > contentAvailableWidth)
                {
                    contentAvailableHeight = contentAvailableHeight - _horizontalScrollBarStyle.margin.bottom - _horizontalScrollBarStyle.fixedHeight - _horizontalScrollBarStyle.margin.top;
                    shouldMeasureAgain = true;
                    _shouldDrawHorizontalScrollBar = true;
                }
                if (!_shouldDrawVerticalScrollBar 
                    && _verticalScrollBarVisibility != ScrollBarVisibility.Hidden
                    && contentDesiredSize.Height > contentAvailableHeight)
                {
                    contentAvailableWidth = contentAvailableWidth - _verticalScrollBarStyle.margin.left - _verticalScrollBarStyle.fixedWidth - _verticalScrollBarStyle.margin.right;
                    shouldMeasureAgain = true;
                    _shouldDrawVerticalScrollBar = true;
                }

                if (shouldMeasureAgain)
                {
                    _content.Measure(new Size(contentAvailableWidth, contentAvailableHeight));
                }

                _viewHeight = contentAvailableWidth;
                _viewWidth = contentAvailableHeight;
            }

            return desiredSize;
        }


        private void HandleSteamDeckTouchScreen(Rect renderRect)
        {
            Vector2 scrollPosition = new Vector2(_horizontalOffset, _verticalOffset);
            if (Widgets.mouseOverScrollViewStack.Count > 0)
            {
                Widgets.mouseOverScrollViewStack.Push(Widgets.mouseOverScrollViewStack.Peek() && renderRect.Contains(Event.current.mousePosition));
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
