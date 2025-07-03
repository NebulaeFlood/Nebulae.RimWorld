using Nebulae.RimWorld.UI.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Nebulae.RimWorld.UI.Controls.Basic
{
    /// <summary>
    /// 以其它控件作为内容的控件
    /// </summary>
    [DebuggerStepThrough]
    public abstract class ContentControl : FrameworkControl
    {
        //------------------------------------------------------
        //
        //  Public Properties
        //
        //------------------------------------------------------

        #region Public Properties

        /// <summary>
        /// 获取或设置 <see cref="ContentControl"/> 的内容控件
        /// </summary>
        public Control Content
        {
            get => _content;
            set
            {
                if (!ReferenceEquals(_content, value))
                {
                    if (!_isEmpty)
                    {
                        _content.Parent = null;
                    }

                    _content = value;
                    _isEmpty = value is null;

                    if (!_isEmpty)
                    {
                        _content.Parent = this;
                    }

                    OnContentChanged();
                }
            }
        }

        /// <summary>
        /// 获取一个值，指示 <see cref="ContentControl"/> 是否没有内容控件
        /// </summary>
        public bool IsEmpty => _isEmpty;

        /// <inheritdoc/>
        public override IEnumerable<Control> LogicalChildren
        {
            get
            {
                if (!_isEmpty)
                {
                    yield return _content;
                }
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
        protected override Rect ArrangeOverride(Rect availableRect) => _isEmpty ? availableRect : _content.Arrange(availableRect);

        /// <inheritdoc/>
        protected override Size MeasureOverride(Size availableSize) => _isEmpty ? availableSize : _content.Measure(availableSize);

        /// <inheritdoc/>
        protected override SegmentResult SegmentCore(Rect visiableRect) => _isEmpty ? visiableRect.IntersectWith(RenderRect) : _content.Segment(visiableRect);

        /// <inheritdoc/>
        protected override HitTestResult HitTestCore(Vector2 hitPoint)
        {
            if (!ControlRect.Contains(hitPoint))
            {
                return HitTestResult.Empty;
            }

            if (!_isEmpty)
            {
                var result = _content.HitTest(hitPoint);

                if (result.IsHit)
                {
                    return result;
                }
            }

            return HitTestResult.HitTest(this, true);
        }

        /// <inheritdoc/>
        protected override void DrawCore(ControlState states)
        {
            if (!_isEmpty)
            {
                _content.Draw();
            }
        }

        /// <summary>
        /// 当 <see cref="Content"/> 被更改时调用
        /// </summary>
        protected virtual void OnContentChanged() { }

        #endregion


        //------------------------------------------------------
        //
        //  Private Fields
        //
        //------------------------------------------------------

        #region Private Fields

        private Control _content;

        // Cache

        private bool _isEmpty = true;

        #endregion
    }
}
