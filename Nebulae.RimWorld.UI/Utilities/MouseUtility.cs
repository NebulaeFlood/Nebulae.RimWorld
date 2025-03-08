using Nebulae.RimWorld.UI.Controls;
using Nebulae.RimWorld.UI.Controls.Basic;
using UnityEngine;
using Verse;

namespace Nebulae.RimWorld.UI.Utilities
{
    /// <summary>
    /// 鼠标帮助类
    /// </summary>
    public static class MouseUtility
    {
        /// <summary>
        /// 当前是否有控件被按下
        /// </summary>
        public static bool AnyPressing;

        /// <summary>
        /// 光标在 UI 系统中的坐标（和窗口在同一坐标系）
        /// </summary>
        public static Vector2 CursorPosition;

        /// <summary>
        /// 正位于光标下方的控件
        /// </summary>
        public static Visual HoveredControl;

        /// <summary>
        /// 正位于光标下方的窗口
        /// </summary>
        public static Window HoveredWindow;

        /// <summary>
        /// 正在进行点击测试
        /// </summary>
        public static bool IsHitTesting;

        /// <summary>
        /// 正在按下鼠标按钮
        /// </summary>
        public static bool IsPressing;


        internal static Visual CurrentHoveredControl;
        internal static Visual DraggingControl;
        internal static Visual PressingControl;

        internal static Window PressingWindow;

        internal static bool AnyDragging;
        internal static bool AnyHovered;

        internal static bool IsDraggable;
        internal static bool IsHitAvailable;

        internal static bool PressingWindowDraggable;

        internal static bool LeftButtonDown;
        internal static bool LeftButtonPressing;
        internal static bool MiddleButtonDown;
        internal static bool MiddleButtonPressing;
        internal static bool RightButtonDown;
        internal static bool RightButtonPressing;

        internal static EventType CurrentEvent;

        internal static Vector2 PressStartPos;
    }
}
