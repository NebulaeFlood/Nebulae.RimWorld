using Nebulae.RimWorld.UI.Controls.Basic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        All = 0b11111,

        /// <summary>
        /// 不绘制内容
        /// </summary>
        Empty = 0b00000,

        /// <summary>
        /// 绘制调试按钮
        /// </summary>
        Buttons = 0b00001,

        /// <summary>
        /// 绘制 <see cref="Visual.ContentRect"/>
        /// </summary>
        ContentRect = 0b00010,

        /// <summary>
        /// 绘制 <see cref="Visual.ControlRect"/>
        /// </summary>
        ControlRect = 0b00100,

        /// <summary>
        /// 绘制 <see cref="Visual.DesiredRect"/>
        /// </summary>
        DesiredRect = 0b01000,

        /// <summary>
        /// 绘制 <see cref="Visual.RenderRect"/>
        /// </summary>
        RenderRect = 0b10000
    }
}
