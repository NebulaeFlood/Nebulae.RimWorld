using Nebulae.RimWorld.UI.Controls;
using UnityEngine;
using Verse;

namespace Nebulae.RimWorld.UI.Utilities
{
    /// <summary>
    /// 光标帮助类
    /// </summary>
    public static class CursorUtility
    {
        /// <summary>
        /// 光标在 UI 系统中的坐标（和窗口在同一坐标系）
        /// </summary>
        public static Vector2 CursorPosition;

        internal static Control DraggingControl;
        internal static Control HoveredControl;
        internal static Control PressingControl;

        internal static Window HoveredWindow;
        internal static Window PressingWindow;

        internal static bool AnyDragging;
        internal static bool AnyPressing;
        internal static bool IsPressing;

        internal static bool PressingWindowDraggable;


        private static Vector2 _pressStartPos;


        internal static void Click()
        {
            if (ReferenceEquals(HoveredControl, PressingControl)
                && PressingControl is ButtonBase button)
            {
                button.Click();
            }
        }


        internal static void Drag()
        {
            if (AnyDragging)
            {
                DraggingControl.OnDragging(CursorPosition);
                HoveredControl?.OnDragOver(DraggingControl);
            }
            else if (CanDrag())
            {
                AnyDragging = true;
                DraggingControl = PressingControl;
                DraggingControl.OnDragStart();
            }
        }

        internal static void Drop()
        {
            if (!AnyDragging)
            {
                return;
            }

            DraggingControl.OnDragStop();
            HoveredControl?.OnDrop(DraggingControl);
        }

        internal static void Press()
        {
            if (HoveredControl is null
                || HoveredWindow is null)
            {
                return;
            }

            if (!AnyPressing
                && IsPressing
                && !AnyDragging)
            {
                AnyPressing = true;
                PressingControl = HoveredControl;
                PressingWindow = HoveredWindow;
                PressingWindowDraggable = HoveredWindow.draggable;
                PressingWindow.draggable = false;

                _pressStartPos = CursorPosition;
            }
        }


        private static bool CanDrag()
        {
            if (AnyPressing
                && PressingControl.IsDraggable)
            {
                if (!ReferenceEquals(HoveredControl, PressingControl))
                {
                    return true;
                }

                float x = CursorPosition.x - _pressStartPos.x;
                float y = CursorPosition.y - _pressStartPos.y;

                return x * x + y * y > 2500f;
            }

            return false;
        }
    }
}
