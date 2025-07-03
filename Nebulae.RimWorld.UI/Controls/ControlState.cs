using System;

namespace Nebulae.RimWorld.UI.Controls
{
    /// <summary>
    /// 控件状态
    /// </summary>
    [Flags]
    public enum ControlState
    {
        /// <summary>
        /// 正常状态
        /// </summary>
        Normal = 0b0000000000,

        /// <summary>
        /// 光标直接位于控件上方
        /// </summary>
        CursorDirectlyOver = 0b0000000010,

        /// <summary>
        /// 光标直接或间接位于控件上方
        /// </summary>
        CursorOver = 0b0000000001,

        /// <summary>
        /// 正在被拖动
        /// </summary>
        Dragging = 0b0000000100,

        /// <summary>
        /// 正在被按下
        /// </summary>
        Pressing = 0b0000001000,

        /// <summary>
        /// 拥有焦点
        /// </summary>
        Focused = 0b0000010000,

        /// <summary>
        /// 强制拥有焦点
        /// </summary>
        ForceFocus = 0b0000100000,

        /// <summary>
        /// 即将拥有焦点
        /// </summary>
        WillFocus = 0b0001000000,

        /// <summary>
        /// 即将失去焦点
        /// </summary>
        WillLossFocus = 0b0010000000,

        /// <summary>
        /// 被禁用
        /// </summary>
        Disabled = 0b0100000000,
    }
}
