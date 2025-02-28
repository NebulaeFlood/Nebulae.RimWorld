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


        private static float _pressingStartTime;
        private static Vector2 _pressingPos;


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
            else if (AnyPressing
                && PressingControl.IsDraggable
                && Event.current.type is EventType.Repaint)
            {
                if (!ReferenceEquals(HoveredControl, PressingControl)
                || (_pressingStartTime + 0.45f < Time.realtimeSinceStartup
                        && _pressingPos != CursorPosition))
                {
                    AnyDragging = true;
                    DraggingControl = PressingControl;
                    DraggingControl.OnDragStart();
                }

                _pressingPos = CursorPosition;
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

                _pressingStartTime = Time.realtimeSinceStartup;
            }
        }
    }
}
