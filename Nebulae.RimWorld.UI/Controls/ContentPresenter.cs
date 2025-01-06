using Nebulae.RimWorld.UI.Controls.Panels;
using Nebulae.RimWorld.UI.Data;
using Nebulae.RimWorld.UI.Windows;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace Nebulae.RimWorld.UI.Controls
{
    /// <summary>
    /// 用于显示控件内容的控件
    /// </summary>
    public sealed class ContentPresenter : Control, IFrame
    {
        //------------------------------------------------------
        //
        //  Private Fields
        //
        //------------------------------------------------------

        #region Private Fields

        private Window _assosiatedWindow;
        private Control _content;

        private float _horizontalOffset = 0f;
        private float _verticalOffset = 0f;
        private float _viewHeight = 1f;
        private float _viewWidth = 1f;

        private bool _isSegmentValid = false;
        private Rect _cachedVisiableRect = Rect.zero;

        #endregion


        //------------------------------------------------------
        //
        //  Public Properties
        //
        //------------------------------------------------------

        #region Public Properties

        /// <summary>
        /// 关联的窗口
        /// </summary>
        /// <remarks>当窗口不派生自 <see cref="ControlWindow"/> 时，设置此属性为显示该控件的窗口。</remarks>
        public Window AssosiatedWindow
        {
            get => _assosiatedWindow;
            set => _assosiatedWindow = value;
        }

        /// <summary>
        /// 内容控件
        /// </summary>
        public Control Content
        {
            get => _content;
            set
            {
                if (_content != value)
                {
                    if (value is null)
                    {
                        _content.RemoveContainer();
                    }
                    else
                    {
                        value.SetContainer(this);
                    }

                    _content = value;

                    InvalidateMeasure();
                }
            }
        }

        /// <summary>
        /// 内容控件的可显示区域的水平偏移量
        /// </summary>
        public float HorizontalOffset
        {
            get => _horizontalOffset;
            set => _horizontalOffset = value;
        }

        /// <summary>
        /// 内容控件的可显示区域的垂直偏移量
        /// </summary>
        public float VerticalOffset
        {
            get => _verticalOffset;
            set => _verticalOffset = value;
        }

        /// <summary>
        /// 内容控件的可显示区域的高度
        /// </summary>
        public float ViewHeight
        {
            get => _viewHeight;
            set => _viewHeight = value;
        }

        #endregion


        /// <summary>
        /// 初始化 <see cref="ContentPresenter"/> 的新实例
        /// </summary>
        public ContentPresenter()
        {
        }


        //------------------------------------------------------
        //
        //  Public Methods
        //
        //------------------------------------------------------

        #region Public Methods

        /// <inheritdoc/>
        public void InvalidateSegment()
        {
            if (_isSegmentValid)
            {
                _isSegmentValid = false;

                if (_content is IFrame frame)
                {
                    frame.InvalidateSegment();
                }
            }
        }

        /// <summary>
        /// 计算 <see cref="ContentPresenter"/> 的内容控件的可显示区域
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
                _cachedVisiableRect = Container.Segment().IntersectWith(DesiredRect);
            }
            else if (_assosiatedWindow != null)
            {
                _cachedVisiableRect = _assosiatedWindow.windowRect.IntersectWith(DesiredRect);
            }
            else
            {
                _cachedVisiableRect = DesiredRect;
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
        protected override Rect DrawCore(Rect renderRect)
        {
            return _content?.Draw() ?? Rect.zero;
        }

        /// <inheritdoc/>
        protected override Rect ArrangeCore(Rect availableRect)
        {
            return _content?.Arrange(new Rect(
                _horizontalOffset + availableRect.x,
                _verticalOffset + availableRect.y,
                availableRect.width,
                availableRect.height))
                ?? Rect.zero;
        }

        /// <inheritdoc/>
        protected override Size MeasureCore(Size availableSize)
        {
            return _content?.Measure(new Size(
                _viewWidth > 1f ? _viewWidth : _viewWidth * availableSize.Width,
                _viewHeight > 1f ? _viewHeight : _viewHeight * availableSize.Height))
                ?? Size.Empty;
        }

        /// <inheritdoc/>
        protected override void OnArrangeInvalidated()
        {
            base.OnArrangeInvalidated();

            // 控件度量无效，说明已经调用过 InvalidateSegment 方法
            if (IsMeasureValid)
            {
                InvalidateSegment();

                _content?.InvalidateArrange();
            }
        }

        /// <inheritdoc/>
        protected override void OnMeasureInvalidated()
        {
            base.OnMeasureInvalidated();
            InvalidateSegment();

            _content?.InvalidateMeasure();
        }

        #endregion
    }
}
