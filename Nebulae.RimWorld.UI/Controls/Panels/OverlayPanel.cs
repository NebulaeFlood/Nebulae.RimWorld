using Nebulae.RimWorld.UI.Controls.Basic;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Nebulae.RimWorld.UI.Controls.Panels
{
    /// <summary>
    /// 子控件在区域内叠加显示的面板
    /// </summary>
    public sealed class OverlayPanel : Panel
    {
        /// <summary>
        /// 初始化 <see cref="OverlayPanel"/> 的新实例
        /// </summary>
        public OverlayPanel()
        {
            IsSolid = true;
        }


        //------------------------------------------------------
        //
        //  Public Method
        //
        //------------------------------------------------------

        #region Public Method

        /// <summary>
        /// 向面板顶部添加一个控件
        /// </summary>
        /// <param name="control">要添加的控件</param>
        /// <returns>该面板控件</returns>
        public OverlayPanel Over(Control control)
        {
            Children.Add(control);
            return this;
        }

        /// <summary>
        /// 清除面板并重新设置面板中的控件
        /// </summary>
        /// <param name="controls">要添加的控件</param>
        /// <returns>该面板控件</returns>
        public OverlayPanel Set(IEnumerable<Control> controls)
        {
            Children.OverrideCollection(controls);
            return this;
        }

        /// <summary>
        /// 清除面板并重新设置面板中的控件
        /// </summary>
        /// <param name="controls">要添加的控件</param>
        /// <returns>该面板控件</returns>
        public OverlayPanel Set(params Control[] controls)
        {
            Children.OverrideCollection(controls);
            return this;
        }

        /// <summary>
        /// 清除面板中的所有控件
        /// </summary>
        public void Clear()
        {
            Children.Clear();
        }

        /// <summary>
        /// 将控件插入或移动到面板中的指定控件下方
        /// </summary>
        /// <param name="control">要插入的控件</param>
        /// <param name="index">被挤开的控件</param>
        /// <returns>若插入了指定控件，返回 <see langword="true"/>；反之则返回 <see langword="false"/>。</returns>
        public bool Insert(Control control, Control index)
        {
            return Children.Insert(index, control);
        }

        /// <summary>
        /// 移除面板中的指定控件
        /// </summary>
        /// <param name="control">要移除的控件</param>
        /// <returns>若移除了指定控件，返回 <see langword="true"/>；反之则返回 <see langword="false"/>。</returns>
        public bool Remove(Control control)
        {
            return Children.Remove(control);
        }

        #endregion


        //------------------------------------------------------
        //
        //  Protected Methods
        //
        //------------------------------------------------------

        #region Protected Methods

        /// <inheritdoc/>
        protected override Rect ArrangeOverride(Rect availableRect, Control[] children)
        {
            for (int i = children.Length - 1; i >= 0; i--)
            {
                children[i].Arrange(availableRect);
            }

            return availableRect;
        }

        /// <inheritdoc/>
        protected override Size MeasureOverride(Size availableSize, Control[] children)
        {
            bool autoWidth = float.IsNaN(availableSize.Width);
            bool autoHeight = float.IsNaN(availableSize.Height);

            float renderWidth, renderHeight;

            if (autoWidth)
            {
                if (autoHeight)
                {
                    renderWidth = 0f;
                    renderHeight = 0f;

                    var autoSize = new Size(float.NaN);

                    for (int i = children.Length - 1; i >= 0; i--)
                    {
                        var childDesiredSize = children[i].Measure(autoSize);

                        renderWidth = MathF.Max(renderWidth, childDesiredSize.Width);
                        renderHeight = MathF.Max(renderHeight, childDesiredSize.Height);
                    }
                }
                else
                {
                    renderWidth = 0f;
                    renderHeight = availableSize.Height;

                    var childSize = new Size(float.NaN, renderHeight);

                    for (int i = children.Length - 1; i >= 0; i--)
                    {
                        var childDesiredSize = children[i].Measure(childSize);

                        renderWidth = MathF.Max(renderWidth, childDesiredSize.Width);
                    }
                }
            }
            else if (autoHeight)
            {
                renderWidth = availableSize.Width;
                renderHeight = 0f;

                var childSize = new Size(renderWidth, float.NaN);

                for (int i = children.Length - 1; i >= 0; i--)
                {
                    var childDesiredSize = children[i].Measure(childSize);

                    renderHeight = MathF.Max(renderHeight, childDesiredSize.Height);
                }
            }
            else
            {
                renderWidth = availableSize.Width;
                renderHeight = availableSize.Height;
            }

            availableSize = new Size(renderWidth, renderHeight);

            for (int i = children.Length - 1; i >= 0; i--)
            {
                children[i].Measure(availableSize);
            }

            return availableSize;
        }

        /// <inheritdoc/>
        protected override HitTestResult HitTestCore(Vector2 hitPoint)
        {
            var result = HitTestResult.HitTest(this, hitPoint);

            if (!result.IsHit)
            {
                return HitTestResult.Empty;
            }

            var children = DrawableChildren;
            var count = children.Length;

            for (int i = 0; i < count - 1; i++)
            {
                var childResult = children[i].HitTest(hitPoint);

                if (childResult.IsHit)
                {
                    result = childResult;
                }
            }

            return result;
        }

        #endregion
    }
}
