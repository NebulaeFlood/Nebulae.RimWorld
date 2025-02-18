using Nebulae.RimWorld.UI.Utilities;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Nebulae.RimWorld.UI.Controls
{
    /// <summary>
    /// 页面控件
    /// </summary>
    public abstract class Page : Control
    {
        private Control _content;


        /// <summary>
        /// 初始化 <see cref="Page"/> 的新实例
        /// </summary>
        public Page()
        {
            _content = CreateContent();

            if (_content is null)
            {
                throw new InvalidOperationException("The content control of a page control cannot be null.");
            }

            _content.SetParent(this);
        }


        /// <summary>
        /// 重置页面内容
        /// </summary>
        /// <remarks>将会移除内容控件及其子控件的所有绑定关系。</remarks>
        public void ResetContent()
        {
            if (CreateContent() is Control control)
            {
                _content.Unbind();
                _content.SetParent(null);
                control.SetParent(this);

                _content = control;
            }
            else
            {
                throw new InvalidOperationException("The content control of a page control cannot be null.");
            }
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
            _content.Arrange(availableRect);
            return availableRect;
        }

        /// <summary>
        /// 创建内容控件
        /// </summary>
        /// <returns>页面的内容。</returns>
        protected abstract Control CreateContent();

        /// <inheritdoc/>
        protected override void DrawCore()
        {
            _content.Draw();
        }

        /// <inheritdoc/>
        protected internal override IEnumerable<Control> EnumerateLogicalChildren()
        {
            yield return _content;
        }

        /// <inheritdoc/>
        protected override Size MeasureCore(Size availableSize)
        {
            _content.Measure(availableSize);
            return availableSize;
        }

        /// <inheritdoc/>
        protected override Rect SegmentCore(Rect visiableRect)
        {
            Rect contentRect = visiableRect.IntersectWith(RenderRect);
            _content.Segment(contentRect);
            return contentRect;
        }

        #endregion
    }
}
