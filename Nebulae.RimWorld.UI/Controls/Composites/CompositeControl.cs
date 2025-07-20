using Nebulae.RimWorld.UI.Controls.Basic;
using Nebulae.RimWorld.UI.Core.Data;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace Nebulae.RimWorld.UI.Controls.Composites
{
    /// <summary>
    /// 组合控件的基类
    /// </summary>
    /// <remarks>需要在构造函数中手动调用 <see cref="Initialize"/> 方法，对其没有进行任何检查。</remarks>
    [DebuggerStepThrough]
    public abstract class CompositeControl : FrameworkControl
    {
        /// <inheritdoc/>
        public override sealed IEnumerable<Control> LogicalChildren
        {
            get
            {
                if (_content != null)
                {
                    yield return _content;
                }
            }
        }


        static CompositeControl()
        {
            HorizontalAlignmentProperty.OverrideMetadata(typeof(CompositeControl),
                new PropertyMetadata(HorizontalAlignment.Stretch));
            VerticalAlignmentProperty.OverrideMetadata(typeof(CompositeControl),
                new PropertyMetadata(VerticalAlignment.Stretch));
        }

        /// <summary>
        /// 为 <see cref="CompositeControl"/> 实现基本初始化
        /// </summary>
        protected CompositeControl() { }


        //------------------------------------------------------
        //
        //  Public Methods
        //
        //------------------------------------------------------

        #region Public Methods

        /// <summary>
        /// 初始化该 <see cref="CompositeControl"/>
        /// </summary>
        public void Initialize()
        {
            if (_content != null)
            {
                throw new InvalidOperationException($"Cannot initialize {Type} because {Type} has been initialized.");
            }

            _content = CreateContent();

            if (_content is null)
            {
                throw new InvalidOperationException($"Cannot initialize {Type} because {Type}.CreateContent() return null.");
            }

            _content.Parent = this;
        }

        /// <summary>
        /// 重置该 <see cref="CompositeControl"/> 的内容控件及其所有绑定关系
        /// </summary>
        public void Reset()
        {
            if (_content is null)
            {
                throw new InvalidOperationException($"Cannot reset {Type} because it has not been initialized yet. Call {Type}.Initialize() first.");
            }

            Initialize();
        }

        #endregion


        //------------------------------------------------------
        //
        //  Protected Methods
        //
        //------------------------------------------------------

        #region Protected Methods

        /// <inheritdoc/>
        protected override sealed Rect ArrangeOverride(Rect availableRect) => _content.Arrange(availableRect);

        /// <inheritdoc/>
        protected override sealed Size MeasureOverride(Size availableSize) => _content.Measure(availableSize);

        /// <inheritdoc/>
        protected override sealed SegmentResult SegmentCore(Rect visiableRect) => _content.Segment(visiableRect);

        /// <inheritdoc/>
        protected override sealed HitTestResult HitTestCore(Vector2 hitPoint)
        {
            var result = HitTestResult.HitTest(this, hitPoint);

            if (!result.IsHit)
            {
                return result;
            }

            if (_content is null)
            {
                throw new LogicalTreeException($"Cannot perform hit test on {Type} because it has not been initialized. Remember to call {Type}.Initialize().", this);
            }

            var childResult = _content.HitTest(hitPoint);

            if (childResult.IsHit)
            {
                return childResult;
            }

            return result;
        }

        /// <inheritdoc/>
        protected override sealed void DrawCore(ControlState states) => _content.Draw();

        /// <summary>
        /// 创建 <see cref="CompositeControl"/> 的内容控件
        /// </summary>
        /// <returns>该 <see cref="CompositeControl"/> 的内容控件。</returns>
        protected abstract Control CreateContent();

        #endregion



        private Control _content;
    }
}
