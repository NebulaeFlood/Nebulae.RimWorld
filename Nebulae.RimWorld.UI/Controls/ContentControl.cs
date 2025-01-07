using UnityEngine;

namespace Nebulae.RimWorld.UI.Controls
{
    /// <summary>
    /// 包含一段任意类型内容的控件
    /// </summary>
    public abstract class ContentControl : FrameworkControl, IFrame
    {
        private bool _isSegmentValid = false;
        private Rect _cachedVisiableRect = Rect.zero;

        private object _content;

        /// <summary>
        /// 控件内容
        /// </summary>
        public object Content
        {
            get => _content;
            set
            {
                if (_content != value)
                {
                    if (value is Control control)
                    {
                        control.SetContainer(this);
                    }
                    else if (_content is Control currentControl)
                    {
                        currentControl.RemoveContainer();
                    }

                    _content = value;

                    OnContentChanged(_content);
                }
            }
        }


        /// <summary>
        /// 初始化 <see cref="ContentControl"/> 的新实例
        /// </summary>
        public ContentControl()
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

                OnSegmentInvalidated();

                if (_content is IFrame frame)
                {
                    frame.InvalidateSegment();
                }
            }
        }

        /// <summary>
        /// 计算 <see cref="ContentControl"/> 的内容控件的可显示区域
        /// </summary>
        /// <returns>内容控件的可显示区域。</returns>
        /// <remarks>默认返回 <see cref="Rect.zero"/>。</remarks>
        public Rect Segment()
        {
            if (_isSegmentValid)
            {
                return _cachedVisiableRect;
            }

            if (!IsHolded || this is ScrollViewer)
            {
                _cachedVisiableRect = SegmentCore();
            }
            else
            {
                _cachedVisiableRect = Container.Segment().IntersectWith(SegmentCore());
            }
            _isSegmentValid = IsArrangeValid;

            return _cachedVisiableRect;
        }

        #endregion


        //------------------------------------------------------
        //
        //  Procted Methods
        //
        //------------------------------------------------------

        #region Procted Methods

        /// <summary>
        /// 当 <see cref="Content"/> 发生变化时执行的方法
        /// </summary>
        /// <param name="content">新的控件内容</param>
        protected virtual void OnContentChanged(object content) { }

        /// <inheritdoc/>
        protected override void OnArrangeInvalidated()
        {
            base.OnArrangeInvalidated();

            // 控件度量无效，就说明已经调用过 InvalidateSegment 方法
            if (IsMeasureValid)
            {
                InvalidateSegment();

                // 控件度量无效，就说明已经调用过 _content 的 InvalidateMeasure 方法
                if (_content is Control control)
                {
                    control.InvalidateArrange();
                }
            }
        }

        /// <inheritdoc/>
        protected override void OnMeasureInvalidated()
        {
            base.OnMeasureInvalidated();
            InvalidateSegment();

            if (_content is Control control)
            {
                control.InvalidateMeasure();
            }
        }

        /// <summary>
        /// 当控件分割无效化时执行的方法
        /// </summary>
        protected virtual void OnSegmentInvalidated() { }

        /// <summary>
        /// 计算内容控件的可显示区域
        /// </summary>
        /// <returns>内容控件的可显示区域。</returns>
        protected virtual Rect SegmentCore() => RenderRect;

        #endregion

    }
}
