using Nebulae.RimWorld.UI.Controls.Basic;
using System;

namespace Nebulae.RimWorld.UI.Controls
{
    /// <summary>
    /// 调试内容
    /// </summary>
    [Flags]
    public enum DebugContent : int
    {
        /// <summary>
        /// 绘制所有预设内容
        /// </summary>
        All = 0b1111,

        /// <summary>
        /// 不绘制内容
        /// </summary>
        Empty = 0b0000,

        /// <summary>
        /// 绘制 <see cref="Control.ControlRect"/> 区域
        /// </summary>
        ControlRect = 0b0001,

        /// <summary>
        /// 绘制 <see cref="Control.DesiredRect"/> 区域
        /// </summary>
        DesiredRect = 0b0010,

        /// <summary>
        /// 绘制 <see cref="FrameworkControl.RenderRect"/> 区域
        /// </summary>
        RenderRect = 0b0100,

        /// <summary>
        /// 绘制 <see cref="Control.VisibleRect"/> 区域
        /// </summary>
        VisibleRect = 0b1000
    }
}
