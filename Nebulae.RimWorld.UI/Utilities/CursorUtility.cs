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

        internal static bool IsDraggable;
        internal static bool IsPressing;

        internal static bool PressStart;
        internal static bool PressingWindowDraggable;


        private static Vector2 _pressStartPos;


        internal static void Drag()
        {
            if (AnyDragging)
            {
                DraggingControl.OnDragging(CursorPosition);
                HoveredControl?.OnDragOver(DraggingControl);
            }
            else if (IsDraggable && CanDrag())
            {
                AnyDragging = true;
                DraggingControl = PressingControl;
                DraggingControl.OnDragStart();
            }
        }

        internal static void Press()
        {
            if (PressStart)
            {
                PressStart = false;
            }
            else
            {
                return;
            }

            if (HoveredControl is null
                || HoveredWindow is null)
            {
                return;
            }

            AnyPressing = true;
            IsDraggable = HoveredControl.IsDraggable;
            PressingControl = HoveredControl;
            PressingWindow = HoveredWindow;
            PressingWindowDraggable = HoveredWindow.draggable;
            PressingWindow.draggable = false;

            _pressStartPos = CursorPosition;
        }

        internal static void ReleaseButton()
        {
            Click();
            Drop();

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
            IsPressing = false;
            PressStart = false;
        }


        private static bool CanDrag()
        {
            float x = CursorPosition.x - _pressStartPos.x;
            float y = CursorPosition.y - _pressStartPos.y;

            return x * x + y * y > 2500f
                || (Event.current.type is EventType.Repaint
                    && !ReferenceEquals(HoveredControl, PressingControl));
        }

        private static void Click()
        {
            if (ReferenceEquals(HoveredControl, PressingControl)
                && PressingControl is ButtonBase button)
            {
                button.Click();
            }
        }

        private static void Drop()
        {
            if (!AnyDragging)
            {
                return;
            }

            DraggingControl.OnDragStop();
            HoveredControl?.OnDrop(DraggingControl);
        }
    }
}
