using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Nebulae.RimWorld.UI.Controls.Basic;
using Verse;
using System.Diagnostics;

namespace Nebulae.RimWorld.UI.Controls
{
    /// <summary>
    /// <see cref="Control.SegmentCore(Rect)"/> 的返回结果
    /// </summary>
    [DebuggerStepThrough]
    public readonly struct SegmentResult
    {
        /// <summary>
        /// <see cref="Control"/> 的交互区域
        /// </summary>
        public readonly Rect ControlRect;

        /// <summary>
        /// <see cref="Control"/> 的提示区域
        /// </summary>
        public readonly Rect TooltipRect;

        /// <summary>
        /// <see cref="Control"/> 的可见区域
        /// </summary>
        public readonly Rect VisibleRect;


        /// <summary>
        /// 初始化 <see cref="SegmentResult"/> 的新实例
        /// </summary>
        /// <param name="rect"><see cref="Control"/> 的交互区域和可见区域</param>
        public SegmentResult(Rect rect)
        {
            rect = rect.Rounded();

            ControlRect = rect;
            TooltipRect = rect;
            VisibleRect = rect;
        }

        /// <summary>
        /// 初始化 <see cref="SegmentResult"/> 的新实例
        /// </summary>
        /// <param name="controlRect"><see cref="Control"/> 的交互区域</param>
        /// <param name="visibleRect"><see cref="Control"/> 的可见区域</param>
        public SegmentResult(Rect controlRect, Rect visibleRect)
        {
            ControlRect = controlRect.Rounded();
            TooltipRect = controlRect;
            VisibleRect = visibleRect.Rounded();
        }

        /// <summary>
        /// 初始化 <see cref="SegmentResult"/> 的新实例
        /// </summary>
        /// <param name="controlRect"><see cref="Control"/> 的交互区域</param>
        /// <param name="tooltipRect"><see cref="Control"/> 的提示区域</param>
        /// <param name="visibleRect"><see cref="Control"/> 的可见区域</param>
        public SegmentResult(Rect controlRect, Rect tooltipRect, Rect visibleRect)
        {
            ControlRect = controlRect.Rounded();
            TooltipRect = tooltipRect.Rounded();
            VisibleRect = visibleRect.Rounded();
        }


        /// <summary>
        /// 将 <see cref="Rect"/> 隐式转换为 <see cref="SegmentResult"/>
        /// </summary>
        /// <param name="rect"><see cref="Control"/> 的交互区域和可见区域</param>
        public static implicit operator SegmentResult(Rect rect) => new SegmentResult(rect);
    }
}
