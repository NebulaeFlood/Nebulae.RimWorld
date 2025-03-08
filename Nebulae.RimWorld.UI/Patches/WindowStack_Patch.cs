using HarmonyLib;
using Nebulae.RimWorld.UI.Controls.Basic;
using Nebulae.RimWorld.UI.Utilities;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using Verse;
using static Nebulae.RimWorld.UI.Utilities.MouseUtility;

namespace Nebulae.RimWorld.UI.Patches
{
    [HarmonyPatch(typeof(WindowStack), nameof(WindowStack.WindowStackOnGUI))]
    internal static class WindowStack_Patch
    {
        internal static Event CurrentEvent;
        internal static EventType CurrentEventType;

        [HarmonyPrefix]
        internal static void WindowStackOnGUIPrefix(List<Window> ___windows)
        {
            Vector2 cursorPos = Input.mousePosition / Prefs.UIScale;
            cursorPos.y = Verse.UI.screenHeight - cursorPos.y;

            CursorPosition = cursorPos;
            CurrentEvent = Event.current;
            CurrentEventType = CurrentEvent.type;
            IsHitTesting = CurrentEventType is EventType.Repaint;

            GameStateEventUtility.CheckState(Current.ProgramState);
            KeyboardEventUtility.CheckEvent(CurrentEvent, CurrentEventType);

            if (!IsHitTesting)
            {
                return;
            }

            Window window;

            for (int i = ___windows.Count - 1; i >= 0; i--)
            {
                window = ___windows[i];

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

        [HarmonyPostfix]
        internal static void WindowStackOnGUIPostfix()
        {
            if (!IsHitTesting)
            {
                return;
            }

            if (!ReferenceEquals(CurrentHoveredControl, HoveredControl))
            {
                if (AnyHovered)
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

                if (AnyHovered)
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

                        if (HoveredWindow != null)
                        {
                            AnyPressing = true;
                            IsDraggable = HoveredControl.IsDraggable;
                            PressingControl = HoveredControl;
                            PressStartPos = CursorPosition;
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
