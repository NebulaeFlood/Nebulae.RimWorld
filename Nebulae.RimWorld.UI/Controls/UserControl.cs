using Nebulae.RimWorld.UI.Utilities;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Nebulae.RimWorld.UI.Controls
{
    /// <summary>
    /// 用户控件
    /// </summary>
    /// <remarks>方便使用控件组成新控件。需要在构造函数中手动调用 <see cref="Initialize"/> 方法。</remarks>
    public abstract class UserControl : Control
    {
        private Control _content;


        /// <summary>
        /// 初始化 <see cref="UserControl"/> 的新实例
        /// </summary>
        public UserControl()
        {
        }


        /// <summary>
        /// 重置页面内容
        /// </summary>
        /// <remarks>将会移除内容控件及其子控件的所有绑定关系。</remarks>
        public void ResetContent()
        {
            if (_content is null)
            {
                throw new InvalidOperationException($"{Type} is subclass of {typeof(UserControl)} but {typeof(UserControl)}.Initialize method has not been called.");
            }

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
            if (_content is null)
            {
                throw new InvalidOperationException($"{Type} is subclass of {typeof(UserControl)} but {typeof(UserControl)}.Initialize method has not been called.");
            }

            yield return _content;
        }

        /// <summary>
        /// 初始化控件
        /// </summary>
        protected void Initialize()
        {
            _content = CreateContent();

            if (_content is null)
            {
                throw new InvalidOperationException("The content control of a page control cannot be null.");
            }

            _content.SetParent(this);
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
            _content.Segment(visiableRect);
            return visiableRect;
        }

        #endregion
    }
}
