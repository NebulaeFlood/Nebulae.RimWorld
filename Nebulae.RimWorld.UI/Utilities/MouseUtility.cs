using Nebulae.RimWorld.UI.Controls.Basic;
using Nebulae.RimWorld.UI.Patches;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using Verse;

namespace Nebulae.RimWorld.UI.Utilities
{
    /// <summary>
    /// 鼠标帮助类
    /// </summary>
    public static class MouseUtility
    {
        /// <summary>
        /// 当前是否有控件被拖动
        /// </summary>
        public static bool AnyDragging;

        /// <summary>
        /// 当前是否有控件位于光标下方
        /// </summary>
        public static bool AnyHovered;

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

        /// <summary>
        /// 鼠标按钮开始按下的坐标
        /// </summary>
        public static Vector2 PressStartPos;


        internal static Visual CurrentHoveredControl;
        internal static Visual DraggingControl;
        internal static Visual PressingControl;

        internal static Window PressingWindow;

        internal static bool IsDraggable;
        internal static bool IsHitAvailable;

        internal static bool PressingWindowDraggable;

        internal static bool LeftButtonDown;
        internal static bool LeftButtonPressing;
        internal static bool MiddleButtonDown;
        internal static bool MiddleButtonPressing;
        internal static bool RightButtonDown;
        internal static bool RightButtonPressing;


        internal static void CheckState(List<Window> windowStack)
        {
            Vector2 cursorPos = Input.mousePosition / Prefs.UIScale;
            cursorPos.y = Verse.UI.screenHeight - cursorPos.y;

            CursorPosition = cursorPos;
            IsHitTesting = WindowStack_Patch.CurrentEventType is EventType.Repaint;

            if (!IsHitTesting)
            {
                return;
            }

            Window window;

            for (int i = windowStack.Count - 1; i >= 0; i--)
            {
                window = windowStack[i];

                if (window.ID >= 0
                    && (window.absorbInputAroundWindow
                        || window.windowRect.Contains(cursorPos)))
                {
                    HoveredWindow = window;
                    return;
                }
            }

            HoveredWindow = null;
        }

        internal static void Update()
        {
            if (!IsHitTesting)
            {
                return;
            }

            if (!ReferenceEquals(CurrentHoveredControl, HoveredControl))
            {
                if (IsHitAvailable)
                {
                    HoveredControl.OnCursorLeave();

                    if (AnyDragging)
                    {
                        HoveredControl.OnDragLeave(DraggingControl);
                    }
                }

                AnyHovered = CurrentHoveredControl != null;
                HoveredControl = CurrentHoveredControl;
                IsHitAvailable = AnyHovered && HoveredControl.IsEnabled;

                if (IsHitAvailable)
                {
                    HoveredControl.OnCursorEnter();

                    if (AnyDragging)
                    {
                        HoveredControl.OnDragEnter(DraggingControl);
                    }
                }
            }


            if (Input.GetMouseButton(0))
            {
                if (!LeftButtonPressing)
                {
                    IsPressing = true;
                    LeftButtonDown = true;
                    LeftButtonPressing = true;
                }

                if (IsHitAvailable)
                {
                    if (LeftButtonDown)
                    {
                        LeftButtonDown = false;
                        HoveredControl.OnMouseButtonDown(MouseButton.LeftMouse);

                        AnyPressing = true;
                        IsDraggable = HoveredControl.IsDraggable;
                        PressingControl = HoveredControl;
                        PressStartPos = CursorPosition;

                        if (HoveredWindow != null)
                        {
                            PressingWindow = HoveredWindow;
                            PressingWindowDraggable = HoveredWindow.draggable;
                            PressingWindow.draggable = false;
                        }
                    }

                    HoveredControl.OnMouseButtonPressing(MouseButton.LeftMouse);
                }
                else if (LeftButtonDown)
                {
                    LeftButtonDown = false;
                }
            }
            else if (LeftButtonPressing)
            {
                if (IsHitAvailable)
                {
                    HoveredControl.OnMouseButtonUp(MouseButton.LeftMouse);
                }

                if (AnyPressing
                    && ReferenceEquals(HoveredControl, PressingControl)
                    && (!AnyDragging || !ReferenceEquals(HoveredControl, DraggingControl))
                    && PressingControl.IsEnabled)
                {
                    PressingControl.OnClick();

                    if (PressingControl is Control control)
                    {
                        control._click.Invoke(control, EventArgs.Empty);
                    }
                }

                if (AnyDragging)
                {
                    DraggingControl.OnDragStop();

                    if (IsHitAvailable)
                    {
                        HoveredControl.OnDrop(DraggingControl);
                    }
                }

                DraggingControl = null;
                PressingControl = null;

                if (PressingWindowDraggable)
                {
                    PressingWindow.draggable = true;
                    PressingWindowDraggable = false;
                }

                PressingWindow = null;

                AnyDragging = false;
                AnyPressing = false;
                IsDraggable = false;
                LeftButtonDown = false;
                LeftButtonPressing = false;
            }

            if (Input.GetMouseButton(2))
            {
                if (!MiddleButtonPressing)
                {
                    IsPressing = true;
                    MiddleButtonDown = true;
                    MiddleButtonPressing = true;
                }

                if (IsHitAvailable)
                {
                    if (MiddleButtonDown)
                    {
                        MiddleButtonDown = false;
                        HoveredControl.OnMouseButtonDown(MouseButton.MiddleMouse);
                    }

                    HoveredControl.OnMouseButtonPressing(MouseButton.MiddleMouse);
                }
                else if (MiddleButtonDown)
                {
                    MiddleButtonDown = false;
                }
            }
            else if (MiddleButtonPressing)
            {
                if (IsHitAvailable)
                {
                    HoveredControl.OnMouseButtonUp(MouseButton.MiddleMouse);
                }

                MiddleButtonDown = false;
                MiddleButtonPressing = false;
            }

            if (Input.GetMouseButton(1))
            {
                if (!RightButtonPressing)
                {
                    IsPressing = true;
                    RightButtonDown = true;
                    RightButtonPressing = true;
                }

                if (IsHitAvailable)
                {
                    if (RightButtonDown)
                    {
                        RightButtonDown = false;
                        HoveredControl.OnMouseButtonDown(MouseButton.RightMouse);
                    }

                    HoveredControl.OnMouseButtonPressing(MouseButton.RightMouse);
                }
                else if (RightButtonDown)
                {
                    RightButtonDown = false;
                }
            }
            else if (RightButtonPressing)
            {
                if (IsHitAvailable)
                {
                    HoveredControl.OnMouseButtonUp(MouseButton.RightMouse);
                }

                RightButtonDown = false;
                RightButtonPressing = false;
            }

            if (!LeftButtonPressing && !MiddleButtonPressing && !RightButtonPressing)
            {
                IsPressing = false;
            }

            if (AnyDragging)
            {
                DraggingControl.OnDragging(CursorPosition);

                if (AnyHovered)
                {
                    HoveredControl.OnDragOver(DraggingControl);
                }
            }
            else if (IsDraggable && CanDrag())
            {
                AnyDragging = true;
                DraggingControl = PressingControl;
                DraggingControl.OnDragStart();

                if (IsHitAvailable
                    && !ReferenceEquals(DraggingControl, HoveredControl))
                {
                    HoveredControl.OnDragEnter(DraggingControl);
                }
            }

            CurrentHoveredControl = null;
        }

        private static bool CanDrag()
        {
            return !ReferenceEquals(HoveredControl, PressingControl)
                || (CursorPosition - PressStartPos).SqrMagnitude() > 2500f;
        }
    }
}
